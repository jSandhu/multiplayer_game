using System.Collections.Generic;
using Common.Combat;
using Common.Inventory.Abilities;

namespace InstanceServer.Core.Combat.Behaviours
{
    /// <summary>
    /// CombatBehaviourBase implementation used by Players.  Used
    /// to queue up user inputted abilities.
    /// </summary>
    public class ManualInputBehaviour : PlayerCombatBehaviourBase
    {
        private AbilityItemModel nextAbilityItemModel;
        private List<CombatantModel> nextTargetCombatants = new List<CombatantModel>();

        public override void QueueAbility(AbilityItemModel abilityItemModel, int[] combatantIDs)
        {
            if (castingAbilityItemModel != null || nextAbilityItemModel != null)
            {
                // Ignore if ability is already queued
                Logging.Log.Info("An ability is already queued for player with combatant ID: " + ownerCombatantModel.ID + ", ignoring.");
                return;
            }

            nextAbilityItemModel = abilityItemModel;
            int abilityIndex = ownerCombatantModel.GetAbilityIndex(abilityItemModel.ID);
            nextTargetCombatants.Clear();
            for (int i = 0; i < combatantIDs.Length; i++)
            {
                CombatantModel targetCombatantModel = null;
                if (combatantIDsToModels.TryGetValue(combatantIDs[i], out targetCombatantModel))
                {
                    // Ignore if ability is still cooling down
                    if (ownerCombatantModel.GetStatModifiedAbilityCoolDownTurnsRemaining(abilityIndex) > 0)
                    {
                        Logging.Log.Info("Ability with ID: " + abilityItemModel.ID + " is cooling down, ignoring.");
                        continue;
                    }

                    // Ignore if combatant is already dead
                    if (!targetCombatantModel.IsAlive())
                    {
                        Logging.Log.Info("Specified target with combatant ID: " + targetCombatantModel.ID + " is dead, ignoring");
                        continue;
                    }

                    bool isAbilityFriendly = abilityItemModel.IsFriendly();
                    
                    // Ignore if attempting to cast friendly ability on enemy
                    if (isAbilityFriendly && !allies.Contains(targetCombatantModel))
                    {
                        Logging.Log.Info("Can't apply friendly ability with ID " + abilityItemModel.ID + " to an enemy, ignoring.");
                        continue;
                    }

                    // Ignore if attempting to cast offensive ability on ally
                    if (!isAbilityFriendly && !enemies.Contains(targetCombatantModel))
                    {
                        Logging.Log.Info("Can't apply offensive ability with ID " + abilityItemModel.ID + " to an ally, ignoring.");
                        continue;
                    }

                    nextTargetCombatants.Add(targetCombatantModel);
                }
                else
                {
                    // Ignore if invalid combatant ID specified.
                    Logging.Log.Info("Invalid combatant ID specified:  " + combatantIDs[i] + ", ignoring.");
                    continue;
                }
            }
        }

        protected override void determineAbilityAndTargets(AbilityItemModel[] availableAbilityItemModels, 
            ref AbilityItemModel chosenAbilityItemModel, ref int chosenAbilityIndex, ref List<CombatantModel> chosenTargets)
        {
            if (nextAbilityItemModel == null)
            {
                return;
            }

            chosenAbilityItemModel = nextAbilityItemModel;
            nextAbilityItemModel = null;

            bool isAbilityValid = false;
            for (int a = 0; a < availableAbilityItemModels.Length; a++)
            {
                if (availableAbilityItemModels[a] == null)
                {
                    continue;
                }

                if (chosenAbilityItemModel.ID == availableAbilityItemModels[a].ID)
                {
                    isAbilityValid = true;
                    chosenAbilityIndex = a;
                    break;
                }
            }

            if (!isAbilityValid)
            {
                Logging.Log.Info("Player specified invalid ID.  CombatantID: " + ownerCombatantModel.ID + ", AbilityID: " + chosenAbilityItemModel.ID);
                chosenAbilityItemModel = null;
                return;
            }

            for (int i = 0; i < nextTargetCombatants.Count; i++)
            {
                chosenTargets.Add(nextTargetCombatants[i]);
            }
            nextTargetCombatants.Clear();
        }
    }
}
