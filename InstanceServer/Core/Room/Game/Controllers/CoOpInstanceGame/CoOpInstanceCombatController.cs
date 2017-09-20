using Common.Combat;
using Common.Combat.Actions;
using Common.Inventory.Abilities;
using Common.NumberUtils;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.LockStep;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Controllers.CoOpInstanceGame
{
    public class CoOpInstanceCombatController
    {
        private const int TURN_UPDATE_INTERVAL_MS = 200;//2000;
        private const float CRITICAL_STRIKE_MULTIPLIER = 1.5f;

        public Action<CombatActionsCollectionModel> CombatTurnCompleted;

        public bool IsRunning { get; private set; }

        public  TurnDispatcher TurnDispatcher = new TurnDispatcher(TURN_UPDATE_INTERVAL_MS);

        private List<CombatantModel> allCombatants = new List<CombatantModel>();
        private List<CombatantModel> targets = new List<CombatantModel>();
        private Dictionary<int, List<CombatantModel>> teamIDToCombatantModel = new Dictionary<int, List<CombatantModel>>();
        private CombatActionsCollectionModel combatActionsCollection = new CombatActionsCollectionModel();
        private Dictionary<CombatantModel, CombatBehaviourBase> combatantModelToBehaviour = new Dictionary<CombatantModel, CombatBehaviourBase>();
        private AbilityDurationData[] tickedRegenAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] expiredRegenAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] tickedDamageAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] expiredDamageAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] tickedStatAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] expiredStatAbilities = new AbilityDurationData[AbilityDurationDataCollection.MAX_APPLIED_ABILITIES];
        private int delayStartTurns = 0;

        public CoOpInstanceCombatController()
        {
        }

        public void Destroy()
        {
            StopAndClear();
            CombatTurnCompleted = null;
        }

        public bool ContainsCombatant(CombatantModel combatantModel)
        {
            return combatantModelToBehaviour.ContainsKey(combatantModel);
        }

        public void AddCombatantWithBehaviour(CombatantModel combatantModel, CombatBehaviourBase combatBehaviour)
        {
            combatBehaviour.Init(combatantModel);
            // Update existing combatants to consider the newly added one.
            // And update new one with existing combatant
            for (int i = 0; i < allCombatants.Count; i++)
            {
                combatantModelToBehaviour[allCombatants[i]].AddOtherCombatant(combatantModel);
                combatBehaviour.AddOtherCombatant(allCombatants[i]);
            }

            List<CombatantModel> teamCombatantModels = null;
            if (!teamIDToCombatantModel.TryGetValue(combatantModel.TeamID, out teamCombatantModels))
            {
                teamCombatantModels = new List<CombatantModel>();
                teamIDToCombatantModel.Add(combatantModel.TeamID, teamCombatantModels);
            }
            teamCombatantModels.Add(combatantModel);
            allCombatants.Add(combatantModel);
            combatActionsCollection.Add(combatantModel.ID, new List<CombatActionModel>());
            combatantModelToBehaviour.Add(combatantModel, combatBehaviour);
        }

        public void Start(int delayStartTurns)
        {
            this.delayStartTurns = delayStartTurns;
            TurnDispatcher.Updated += onTurnElapsed;
            TurnDispatcher.Start();
            IsRunning = true;
        }

        public void StopAndClear()
        {
            IsRunning = false;
            for (int i = 0; i < allCombatants.Count; i++)
            {
                combatantModelToBehaviour[allCombatants[i]].Clear();
            }

            teamIDToCombatantModel.Clear();
            allCombatants.Clear();
            combatActionsCollection.Clear();
            combatantModelToBehaviour.Clear();
            TurnDispatcher.Stop();
            TurnDispatcher.Updated -= onTurnElapsed;
        }

        private void onTurnElapsed()
        {
            // Update combatActionsCollection turn number
            combatActionsCollection.TurnNumber = TurnDispatcher.TurnNumber;
            Logging.Log.Info(">>>>>>> CoOpInstanceCombatController START TURN " + combatActionsCollection.TurnNumber);

            // If we're delaying, decrement delay turns and return
            if (delayStartTurns > 0)
            {
                Logging.Log.Info("CoOpInstanceCombatController delaying start for " + delayStartTurns + " turns...");
                delayStartTurns--;
                return;
            }

            // Clear all CombatActionModels for all combatants
            for (int c = 0; c < allCombatants.Count; c++)
            {
                combatActionsCollection[allCombatants[c].ID].Clear();
            }

            foreach(var kvp in teamIDToCombatantModel)
            {
                List<CombatantModel> teamCombatants = kvp.Value;
                for (int i = 0; i < teamCombatants.Count; i++)
                {
                    CombatantModel combatantModel = teamCombatants[i];
                    if (!combatantModel.IsAlive()) { continue; }

                    // Advance and apply regen duration abilities and keep track of CombatActionModels
                    combatantModel.RegenOverTimeAbilityDurations.AdvanceAbilityDurations(tickedRegenAbilities);
                    for (int r = 0; r < tickedRegenAbilities.Length && tickedRegenAbilities[r] != null; r++)
                    {
                        AbilityDurationData tickedRegenAbilityDuration = tickedRegenAbilities[r];
                        combatActionsCollection[combatantModel.ID].Add(
                            applyRegenAbilityEffectToCombatant(
                                tickedRegenAbilityDuration.AbilityEffectType, tickedRegenAbilityDuration.OriginAbilityID,
                                tickedRegenAbilityDuration.OwnerCombatantID, tickedRegenAbilityDuration.PerTickAmount, 
                                combatantModel, tickedRegenAbilityDuration.NumTurnsRemaining, tickedRegenAbilityDuration.IsCrit));
                    }

                    // Clean up any expired regen abilities and keep track of CombatActionModels
                    combatantModel.RegenOverTimeAbilityDurations.RemoveAndResetExpired(expiredRegenAbilities);
                    for(int rx = 0; rx < expiredRegenAbilities.Length && expiredRegenAbilities[rx] != null; rx++)
                    {
                        combatActionsCollection[combatantModel.ID].Add(new CombatActionModel(expiredRegenAbilities[rx].OwnerCombatantID,
                            CombatActionType.ExpireAbility, expiredRegenAbilities[rx].OriginAbilityID, combatantModel.ID, 0, 0, false));
                    }

                    // Advance and apply damage duration abilities and keep track of CombatActionModels
                    combatantModel.DamageOverTimeAbilityDurations.AdvanceAbilityDurations(tickedDamageAbilities);
                    for (int d = 0; d < tickedDamageAbilities.Length && tickedDamageAbilities[d] != null; d++)
                    {
                        AbilityDurationData tickedDamageAbilityDuration = tickedDamageAbilities[d];
                        combatActionsCollection[combatantModel.ID].Add(
                            applyDamageAbilityEffectToCombatant(
                                tickedDamageAbilityDuration.AbilityEffectType, tickedDamageAbilityDuration.OriginAbilityID, 
                                tickedDamageAbilityDuration.OwnerCombatantID, tickedDamageAbilityDuration.PerTickAmount, 
                                combatantModel, tickedDamageAbilityDuration.NumTurnsRemaining, tickedDamageAbilityDuration.IsCrit));
                        
                        // If target has died then append CombatActionModel and break;
                        if (!combatantModel.IsAlive())
                        {
                            combatActionsCollection[combatantModel.ID].Add(new CombatActionModel(tickedDamageAbilities[d].OwnerCombatantID,
                                CombatActionType.Death, tickedDamageAbilities[d].OriginAbilityID, combatantModel.ID, 0, 0, false));
                            break;
                        }
                    }
                    if (!combatantModel.IsAlive()) continue;

                    // Clean up any expired damage abilities and keep track of CombatActionModels
                    combatantModel.DamageOverTimeAbilityDurations.RemoveAndResetExpired(expiredDamageAbilities);
                    for (int dx = 0; dx < expiredDamageAbilities.Length && expiredDamageAbilities[dx] != null; dx++)
                    {
                        combatActionsCollection[combatantModel.ID].Add(new CombatActionModel(expiredDamageAbilities[dx].OwnerCombatantID,
                            CombatActionType.ExpireAbility, expiredDamageAbilities[dx].OriginAbilityID, combatantModel.ID, 0, 0, false));
                    }

                    // Advance stat modifier duration abilities 
                    // NOTE: no need to output per tick amount as stat modifiers only report their delta on application.
                    combatantModel.StatModifierAbilityDurations.AdvanceAbilityDurations(tickedStatAbilities);
                    // Expire and remove any stat modifier abilities and keep track of CombatActionModels
                    combatantModel.StatModifierAbilityDurations.RemoveAndResetExpired(expiredStatAbilities);
                    for (int sx = 0; sx < expiredStatAbilities.Length && expiredStatAbilities[sx] != null; sx++)
                    {
                        combatantModel.StatModifiersDeltas.RemoveModifier(
                            expiredStatAbilities[sx].PerTickAmount, expiredStatAbilities[sx].AbilityEffectType);
                        combatActionsCollection[combatantModel.ID].Add(
                            new CombatActionModel(expiredStatAbilities[sx].OwnerCombatantID, CombatActionType.ExpireAbility, 
                            expiredStatAbilities[sx].OriginAbilityID, combatantModel.ID, 0, 0, false));
                    }

                    // Process Combatant's behaviour
                    processCombatantBehaviour(combatantModel);
                }
            }

            if (combatActionsCollection.HasEntries())
            {
                CombatTurnCompleted(combatActionsCollection);
            }
        }

        private CombatActionModel applyRegenAbilityEffectToCombatant(AbilityEffectType abilityEffectType, int abilityID,
            int abilityCombatantOwnerID, int abilityAmount, CombatantModel combatantModel, int turnsRemaining, bool isCrit)
        {
            // Don't consider stat modifiers for regen abilities (if an enemy casts increased spell damage on you,
            // we don't want it to boost the healing you receive)
            CombatActionType combatActionType = CombatActionType.None;
            switch (abilityEffectType)
            {
                case AbilityEffectType.HealthRegen:
                    combatantModel.SetCurrentHealth(combatantModel.CurrentHealth + abilityAmount);
                    combatActionType = CombatActionType.RegenHealth;
                    break;
                case AbilityEffectType.ManaRegen:
                    combatantModel.SetCurrentMana(combatantModel.CurrentMana + abilityAmount);
                    combatActionType = CombatActionType.RegenMana;
                    break;
                default:
                    throw new Exception("Invalid AbilityEffectType");
            }
            return new CombatActionModel(abilityCombatantOwnerID, combatActionType, abilityID, 
                combatantModel.ID, abilityAmount, turnsRemaining, isCrit);
        }

        private CombatActionModel applyDamageAbilityEffectToCombatant(AbilityEffectType abilityEffectType, int abilityID, 
            int abilityCombatantOwnerID, int abilityAmount, CombatantModel combatantModel, int turnsRemaining, bool isCrit)
        {
            StatModifiersDeltas statModifiersDeltas = combatantModel.StatModifiersDeltas;
            CombatActionType combatActionType = CombatActionType.None;
            int damageDeltaWithStatModifiers = 0;
            switch (abilityEffectType)
            {
                case AbilityEffectType.MeleeDamage:
                    damageDeltaWithStatModifiers = getDamageDeltaWithStatMod(statModifiersDeltas.ArmorStatModifiersDelta, abilityAmount);
                    combatActionType = CombatActionType.ReceiveMeleeDamage;
                    break;
                case AbilityEffectType.SpellDamage:
                    damageDeltaWithStatModifiers = getDamageDeltaWithStatMod(statModifiersDeltas.SpellResistStatModifiersDelta, abilityAmount);
                    combatActionType = CombatActionType.ReceiveSpellDamage;
                    break;
                default:
                    throw new Exception("Invalid AbilityEffectType");
            }
            combatantModel.SetCurrentHealth(combatantModel.CurrentHealth + damageDeltaWithStatModifiers);
            Logging.Log.Info("Combatant ID : " + combatantModel.ID + " on team " + combatantModel.TeamID + 
                " receives damage: " + damageDeltaWithStatModifiers + ".  New health = " + combatantModel.CurrentHealth);
            return new CombatActionModel(
                abilityCombatantOwnerID, combatActionType, abilityID, combatantModel.ID, damageDeltaWithStatModifiers, 
                turnsRemaining, isCrit);
        }

        private int getDamageDeltaWithStatMod(int statModifierDelta, int abilityAmount)
        {
            int delta = -(abilityAmount - statModifierDelta); // Resistance can be -ve with debuffs
            // damage should be -ve
            return delta > 0 ? 0 : delta;
        }

        private void processCombatantBehaviour(CombatantModel combatantModel)
        {
            // Advance all ability cooldowns
            for (int i = 0; i < combatantModel.AbilityItemModels.Length; i++)
            {
                combatantModel.AbilityItemModels[i].CoolDownTurnsElapsed++;
            }

            AbilityItemModel combatAbility = null;
            targets.Clear();
            CombatActionModel combatActionModel = combatantModelToBehaviour[combatantModel].GetCombatAction(out combatAbility, ref targets);

            // If waiting, return
            if (combatActionModel.CombatActionType == CombatActionType.Wait)
            {
                return;
            }

            // if starting or canceling cast, append CombatActionModel and return
            if (combatActionModel.CombatActionType == CombatActionType.StartCastingAbility ||
                combatActionModel.CombatActionType == CombatActionType.CancelCastingAbility)
            {
                combatActionsCollection[combatantModel.ID].Add(combatActionModel);
                return;
            }

            if (combatActionModel.CombatActionType != CombatActionType.ApplyAbility)
            {
                throw new Exception("Unexpected CombatActionType: " + combatActionModel.CombatActionType.ToString());
            }

            // Append cast completion CombatActionModel
            combatActionsCollection[combatantModel.ID].Add(combatActionModel);

            // Restart ability cooldown
            combatAbility.CoolDownTurnsElapsed = 0;

            // Apply ability to targets and keep track of CombatActionModels
            bool isAbilityFriendly = combatAbility.IsFriendly();
            for (int t = 0; t < targets.Count; t++)
            {
                CombatActionModel combatActionOnTarget;

                if (combatAbility.AbilityDurationType == AbilityDurationType.Immediate) // Handle ability with IMMEDIATE effect.
                {
                    bool isCrit;
                    int immediateAmountWithStatMods = getAbilityAmountWithStatMods(combatantModel, combatAbility.ImmediateAmout,
                        combatAbility.IsSpellBased, out isCrit);

                    if (isAbilityFriendly)
                    {
                        combatActionOnTarget = applyRegenAbilityEffectToCombatant(combatAbility.AbilityEffectType, combatAbility.ID, combatantModel.ID,
                            immediateAmountWithStatMods, targets[t], 0, isCrit);
                    }
                    else
                    {
                        combatActionOnTarget = applyDamageAbilityEffectToCombatant(combatAbility.AbilityEffectType, combatAbility.ID, combatantModel.ID,
                            immediateAmountWithStatMods, targets[t], 0, isCrit);
                    }
                }
                else // Handle ability with DURATION effect
                {
                    AbilityDurationData abilityDurationDataClone = combatAbility.AbilityDurationData.Clone(true);

                    if (combatAbility.IsStatModifier())
                    {
                        // Stat modifiers don't consider other stat modifiers. Ex: (if target has spell effect/damage debuff from
                        // another modifier, we dont want another spell dmg debuff to cause exponential damage).
                        targets[t].StatModifiersDeltas.AddModifier(abilityDurationDataClone.PerTickAmount, abilityDurationDataClone.AbilityEffectType);
                        targets[t].StatModifierAbilityDurations.Add(abilityDurationDataClone);
                        CombatActionType combatActionType = isAbilityFriendly ? CombatActionType.StatIncreasedByDurationAbility : CombatActionType.StatDecreasedByDurationAbility;
                        combatActionOnTarget = new CombatActionModel(
                            combatantModel.ID, combatActionType, combatAbility.ID, targets[t].ID, 
                            abilityDurationDataClone.PerTickAmount, abilityDurationDataClone.NumTurnsRemaining, false);
                    }
                    else
                    {
                        // Update clone's per tick value to consider the caster's spell/melee buffs/debuffs
                        bool isCrit;
                        int perTickAmountWithStatMods = getAbilityAmountWithStatMods(
                            combatantModel, abilityDurationDataClone.PerTickAmount, combatAbility.IsSpellBased, out isCrit);
                        abilityDurationDataClone.PerTickAmount = perTickAmountWithStatMods;
                        abilityDurationDataClone.IsCrit = isCrit;

                        AbilityDurationDataCollection abilityDurationCollection = isAbilityFriendly ? 
                            targets[t].RegenOverTimeAbilityDurations : targets[t].DamageOverTimeAbilityDurations;
                        abilityDurationCollection.Add(abilityDurationDataClone);
                        combatActionOnTarget = new CombatActionModel(
                            combatantModel.ID, CombatActionType.AffectedByDurationAbility, combatAbility.ID, targets[t].ID, 
                            0, abilityDurationDataClone.NumTurnsRemaining, isCrit);
                    }
                }

                combatActionsCollection[targets[t].ID].Add(combatActionOnTarget);

                // If target has died then append death CombatActionModel
                if (!targets[t].IsAlive())
                {
                    combatActionsCollection[targets[t].ID].Add(
                        new CombatActionModel(combatantModel.ID, CombatActionType.Death, combatAbility.ID, targets[t].ID, 0, 0, false));
                }
            }
        }

        private int getAbilityAmountWithStatMods(CombatantModel combatantModel, int abilityAmount, bool isAbilitySpellBased, out bool isCrit)
        {
            StatModifiersDeltas statModifiersDeltas = combatantModel.StatModifiersDeltas;
            int moddedAmount = abilityAmount;
            isCrit = false;
            if (isAbilitySpellBased)
            {
                moddedAmount += statModifiersDeltas.SpellDamageStatModifiersDelta;
                isCrit = RandomGen.NextNormalizedFloat() < (combatantModel.SpellCritChanceNormalized + statModifiersDeltas.SpellCritStatModifiersDelta);
            }
            else
            {
                moddedAmount += statModifiersDeltas.MeleeDamageStatModifiersDelta;
                isCrit = RandomGen.NextNormalizedFloat() < (combatantModel.MeleeCritChanceNormalized + statModifiersDeltas.MeleeDamageStatModifiersDelta);
            }

            if (isCrit)
            {
                moddedAmount = (int)((float)moddedAmount * CRITICAL_STRIKE_MULTIPLIER);
            }

            return moddedAmount;
        }

    }
}
