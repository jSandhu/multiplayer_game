using Common.Inventory.Abilities;

namespace Common.Combat
{
    /// <summary>
    /// Convenience Class for combat calculations. Collection of deltas applied by 
    /// abilities considered to be stat modifiers. Keeps track of all modifiers 
    /// currently applied to this target (CombatantModel owner instance).
    /// 
    /// Ex: Keeps track of how much a "Weaken Armor Curse" decreases this target's
    /// armor.
    /// </summary>
    public class StatModifiersDeltas
    {
        public int ArmorStatModifiersDelta;
        public int SpellResistStatModifiersDelta;
        public int MeleeDamageStatModifiersDelta;
        public float MeleeCritStatModifiersDelta;
        public int SpellDamageStatModifiersDelta;
        public float SpellCritStatModifiersDelta;
        public int AbilityCoolDownTurnsPercentModifier;
        public int AbilityCastTurnsPercentModifierDelta;

        public void AddModifier(int amount, AbilityEffectType abilityEffectType)
        {
            appendDeltasByEffectType(amount, abilityEffectType);
        }

        public void RemoveModifier(int amount, AbilityEffectType abilityEffectType)
        {
            appendDeltasByEffectType(-amount, abilityEffectType);
        }

        private void appendDeltasByEffectType(int amount, AbilityEffectType abilityEffectType)
        {
            switch(abilityEffectType)
            {
                case AbilityEffectType.ArmorModifier: ArmorStatModifiersDelta += amount; break;
                case AbilityEffectType.SpellResistModifier: SpellResistStatModifiersDelta += amount; break;
                case AbilityEffectType.MeleeDamageModifier: MeleeDamageStatModifiersDelta += amount; break;
                case AbilityEffectType.MeleeCrit: MeleeCritStatModifiersDelta += amount; break;
                case AbilityEffectType.SpellDamageModifier: SpellDamageStatModifiersDelta += amount; break;
                case AbilityEffectType.SpellCrit: SpellCritStatModifiersDelta += amount; break;
                case AbilityEffectType.AbilityCoolDownTurnsPercentModifier:AbilityCoolDownTurnsPercentModifier += amount; break;
                case AbilityEffectType.AbilityCastTurnsPercentModifier: AbilityCastTurnsPercentModifierDelta += amount; break;
            }
        }
    }
}
