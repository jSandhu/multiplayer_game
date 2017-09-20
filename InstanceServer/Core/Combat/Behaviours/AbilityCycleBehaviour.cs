using System;
using System.Collections.Generic;
using Common.Combat;
using Common.Inventory.Abilities;

namespace InstanceServer.Core.Combat.Behaviours
{
    class AbilityCycleBehaviour : CombatBehaviourBase
    {
        private int abilityIndex = -1;

        protected override void determineAbilityAndTargets(AbilityItemModel[] availableAbilityItemModels, 
            ref AbilityItemModel chosenAbilityItemModel, ref int chosenAbilityIndex, ref List<CombatantModel> chosenTargets)
        {
            chosenAbilityItemModel = null;
            for (int i = 0; i < availableAbilityItemModels.Length; i++)
            {
                abilityIndex++;

                if (abilityIndex == availableAbilityItemModels.Length)
                {
                    abilityIndex = 0;
                }

                if (availableAbilityItemModels[abilityIndex] == null)
                {
                    continue;
                }

                if (availableAbilityItemModels[abilityIndex].AbilityTargetType == AbilityTargetType.EnemySingle)
                {
                    chosenAbilityItemModel = availableAbilityItemModels[abilityIndex];
                    chosenAbilityIndex = abilityIndex;
                    break;
                }
            }

            if (chosenAbilityItemModel != null)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].IsAlive())
                    {
                        chosenTargets.Add(enemies[0]);
                        return;
                    }
                }
            }
        }
    }
}
