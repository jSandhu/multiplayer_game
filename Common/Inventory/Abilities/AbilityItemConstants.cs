namespace Common.Inventory.Abilities
{
    public enum AbilityTargetType { FriendlySingle, EnemySingle, FriendlyAll, EnemyAll};
    public enum AbilityEffectType {
        SpellDamage, SpellCrit, MeleeDamage, MeleeCrit, ManaBurn, ManaRegen, HealthRegen, SpellDamageModifier, MeleeDamageModifier, ArmorModifier,
        SpellResistModifier, AbilityCastTurnsPercentModifier, AbilityCoolDownTurnsPercentModifier };
    public enum AbilityDurationType { Immediate, MultiTurn}

}
