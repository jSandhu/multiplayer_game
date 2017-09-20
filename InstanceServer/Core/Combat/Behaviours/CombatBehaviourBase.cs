using Common.Combat;
using Common.Combat.Actions;
using Common.Inventory.Abilities;
using InstanceServer.Core.Logging;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Combat.Behaviours
{
    /// <summary>
    /// Base class for combatant behaviours.  Used to determine actions during combat.
    /// Both Player and NPC CombatantModels must have a CombatBehaviour.  
    /// </summary>
    public abstract class CombatBehaviourBase
    {
        private AbilityItemModel[] availableAbilities;
        protected CombatantModel ownerCombatantModel;
        protected List<CombatantModel> allies = new List<CombatantModel>();
        protected List<CombatantModel> enemies = new List<CombatantModel>();
        protected Dictionary<int, CombatantModel> combatantIDsToModels = new Dictionary<int, CombatantModel>();
        protected AbilityItemModel castingAbilityItemModel;
        protected int castingAbilityIndex;

        private List<CombatantModel> targetCombatantModels = new List<CombatantModel>();

        public void Init(CombatantModel ownerCombatantModel)
        {
            this.ownerCombatantModel = ownerCombatantModel;
            allies.Add(ownerCombatantModel);
            availableAbilities = new AbilityItemModel[ownerCombatantModel.AbilityItemModels.Length];
        }

        public void Clear()
        {
            allies.Clear();
            enemies.Clear();
            combatantIDsToModels.Clear();
            if (castingAbilityItemModel != null)
            {
                castingAbilityItemModel.NumCastTurnsElapsed = 0;
                if (castingAbilityItemModel.AbilityDurationData != null)
                {
                    castingAbilityItemModel.AbilityDurationData.NumTurnsElapsed = 0;
                }
                castingAbilityItemModel = null;
            }

            targetCombatantModels.Clear();
            ownerCombatantModel = null;
        }

        public void AddOtherCombatant(CombatantModel otherCombatantModel)
        {
            if (ownerCombatantModel == null)
            {
                throw new Exception("Owner CombatantModel must be set before specifying others.");
            }

            if (combatantIDsToModels.ContainsKey(otherCombatantModel.ID))
            {
                Log.Warning("Other Combatant with ID: " + otherCombatantModel.ID + " is already being considered.");
                return;
            }

            if (otherCombatantModel.ID == ownerCombatantModel.ID)
            {
                Log.Warning("Other Combatant with ID: " + otherCombatantModel.ID + " can't be owner.");
                return;
            }

            if (otherCombatantModel.TeamID == ownerCombatantModel.TeamID)
            {
                allies.Add(otherCombatantModel);
            }
            else
            {
                enemies.Add(otherCombatantModel);
            }
            combatantIDsToModels.Add(otherCombatantModel.ID, otherCombatantModel);
        }

        public CombatActionModel GetCombatAction(out AbilityItemModel abilityItemModel, ref List<CombatantModel> targets)
        {
            if (!ownerCombatantModel.IsAlive())
            {
                throw new Exception("Dead CombatantModel can't perform actions.");
            }

            // If already casting.
            if (castingAbilityItemModel != null)
            {
                castingAbilityItemModel.NumCastTurnsElapsed++;
                abilityItemModel = castingAbilityItemModel;

                bool allTargetsDead = true;
                for (int a = 0; a < targetCombatantModels.Count; a++)
                {
                    if (targetCombatantModels[a].IsAlive())
                    {
                        allTargetsDead = false;
                        break;
                    }
                }

                // If all targets died during cast, then cancel cast
                if (allTargetsDead)
                {
                    castingAbilityItemModel.NumCastTurnsElapsed = 0;
                    castingAbilityItemModel = null;
                    return new CombatActionModel(
                        ownerCombatantModel.ID, CombatActionType.CancelCastingAbility, abilityItemModel.ID, -1, 0, 0, false);
                }

                // Determine number of total cast turns considering cast time stat modifiers
                int modifiedTotalCastTurns = ownerCombatantModel.GetStatModifiedAbilityTotalCastTurns(castingAbilityIndex);

                // Handle cast completion (>= insteand of == as expired cast time debuffs can cause user to cast beyond the unmodified amount)
                if (castingAbilityItemModel.NumCastTurnsElapsed >= modifiedTotalCastTurns)
                {
                    // Reset ability cast turns and clear reference
                    castingAbilityItemModel.NumCastTurnsElapsed = 0;
                    castingAbilityItemModel = null;
                    
                    // Output alive targets only.
                    for (int t = 0; t < targetCombatantModels.Count; t++)
                    {
                        if (targetCombatantModels[t].IsAlive())
                        {
                            targets.Add(targetCombatantModels[t]);
                        }
                    }

                    int abilityDurationTurnsRemaining = abilityItemModel.AbilityDurationType == AbilityDurationType.Immediate ? 0 :
                        abilityItemModel.AbilityDurationData.NumTurnsRemaining;

                    // -1 target id signifies multiple targets.
                    return new CombatActionModel(
                        ownerCombatantModel.ID, CombatActionType.ApplyAbility, abilityItemModel.ID,
                        targets.Count == 1 ? targets[0].ID : -1, 0, abilityDurationTurnsRemaining, false);
                }

                // Still casting, so perform wait.
                return new CombatActionModel(ownerCombatantModel.ID, CombatActionType.Wait, -1, -1, 0, 0, false);
            }

            // Determine which abilities are available (not on cooldown)
            bool abilityAvailable = false;
            for (int i = 0; i < ownerCombatantModel.AbilityItemModels.Length; i++)
            {
                if (ownerCombatantModel.GetStatModifiedAbilityCoolDownTurnsRemaining(i) > 0)
                {
                    availableAbilities[i] = null;
                }
                else
                {
                    abilityAvailable = true;
                    availableAbilities[i] = ownerCombatantModel.AbilityItemModels[i];
                }
            }

            // If all abilities on cooldown, return wait.
            if (!abilityAvailable)
            {
                abilityItemModel = null;
                return new CombatActionModel(ownerCombatantModel.ID, CombatActionType.Wait, -1, -1, 0, 0, false);
            }

            targetCombatantModels.Clear();
            castingAbilityIndex = -1;
            determineAbilityAndTargets(availableAbilities, ref castingAbilityItemModel, ref castingAbilityIndex, ref targetCombatantModels);
            abilityItemModel = castingAbilityItemModel;

            // If we were unable to determine an AbilityItemModel or find valid targets this turn, perform wait. 
            if (castingAbilityItemModel == null || targetCombatantModels.Count == 0)
            {
                return new CombatActionModel(ownerCombatantModel.ID, CombatActionType.Wait, -1, -1, 0, 0, false);
            }

            // Start casting
            return new CombatActionModel(ownerCombatantModel.ID, CombatActionType.StartCastingAbility, abilityItemModel.ID,
                targetCombatantModels.Count == 1 ? targetCombatantModels[0].ID : -1, 0, abilityItemModel.CastTurns, false);
        }

        protected abstract void determineAbilityAndTargets(AbilityItemModel[] availableAbilityItemModels,
            ref AbilityItemModel chosenAbilityItemModel, ref int chosenAbilityIndex, ref List<CombatantModel> chosenTargets);
    }
}
