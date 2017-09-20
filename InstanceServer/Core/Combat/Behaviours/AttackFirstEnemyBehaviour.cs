using System;
using System.Collections.Generic;
using Common.Combat;
using Common.Inventory.Abilities;

namespace InstanceServer.Core.Combat.Behaviours
{
    public class AttackFirstEnemyBehaviour : CombatBehaviourBase
    {
        protected override void determineAbilityAndTargets(AbilityItemModel[] availableAbilityItemModels, 
            ref AbilityItemModel chosenAbilityItemModel, ref int chosenAbilityIndex, ref List<CombatantModel> chosenTargets)
        {
            chosenAbilityItemModel = null;
            for (int i = 0; i < availableAbilityItemModels.Length; i++)
            {
                if (availableAbilityItemModels[i] == null)
                {
                    continue;
                }

                if (availableAbilityItemModels[i].AbilityTargetType == AbilityTargetType.EnemySingle)
                {
                    chosenAbilityItemModel = availableAbilityItemModels[i];
                    chosenAbilityIndex = i;
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
