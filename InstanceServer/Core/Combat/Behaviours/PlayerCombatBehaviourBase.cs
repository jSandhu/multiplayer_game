using Common.Inventory.Abilities;

namespace InstanceServer.Core.Combat.Behaviours
{
    public abstract class PlayerCombatBehaviourBase : CombatBehaviourBase
    {
        public abstract void QueueAbility(AbilityItemModel abilityItemModel, int[] combatantIDs);
    }
}
