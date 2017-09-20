using Common.Inventory;
using Common.Inventory.Abilities;
using Common.Inventory.Equipment;
using Common.Rarity;
using System;

namespace Common.Combat.Commands
{
    /// <summary>
    /// Convenience command for building a CombatantModel from core stats.
    /// </summary>
    public static class BuildCombatantSCMD
    {
        /// <summary>
        /// Builds a CombatantModel from the specified core stats.
        /// </summary>
        /// <param name="level">Combatant's level</param>
        /// <param name="teamID">Combatant's team ID</param>
        /// <param name="equippedAbilities">Combatant's main abilities</param>
        /// <param name="rarityModifier">Combatant's rarity (Common if Player). Higher rarity increases stats.</param>
        /// <param name="equipmentItemModels">Any equipment being worn</param>
        /// <returns></returns>
        public static CombatantModel Execute(int level, int teamID, AbilityItemModel[] equippedAbilities, 
            RarityType rarityModifier = RarityType.Common, EquipmentItemModel[] equipmentItemModels = null)
        {
            // Make sure equipment stats have been built before considering them in CombatantModel creation.
            if (equipmentItemModels != null)
            {
                for (int e = 0; e < equipmentItemModels.Length; e++)
                {
                    if (!equipmentItemModels[e].HasBuiltStatsFromLevel)
                    {
                        throw new Exception("Equipment stats not built (invalid level): " + equipmentItemModels[e].ID + ", " + equipmentItemModels[e].Name);
                    }
                }
            }

            // Make sure ability stats have been built before assigning them to CombatantModel
            for (int i = 0; i < equippedAbilities.Length; i++)
            {
                if (!equippedAbilities[i].HasBuiltStatsFromLevel)
                {
                    throw new Exception("Ability " + equippedAbilities[i].ID + "("+equippedAbilities[i].Name+")" + " stats not built (invalid level)");
                }
            }

            // Build combatant model considering level and rarity for core stats.
            CombatantModel combatantModel = buildCombatantModel(rarityModifier, level, teamID);

            if (equipmentItemModels != null)
            {
                // If the combatant is wearing equipment, boost its stats and merge its existing abilities with
                // any provided by the equipment.
                boostCombatantStatsFromEquipment(combatantModel, equipmentItemModels);
                combatantModel.AbilityItemModels = mergeAbilities(equippedAbilities, equipmentItemModels);
            }
            else
            {
                // Otherwise simply assign the combatant's abilities to the ones that are specified.
                combatantModel.AbilityItemModels = equippedAbilities;
            }

            return combatantModel;
        }

        /// <summary>
        /// Builds a combatant by determining its core stats from level and rarity.
        /// </summary>
        /// <param name="rarityModifier">Combatant's rarity (common if Player)</param>
        /// <param name="level">Level</param>
        /// <param name="teamID">Combatant's team ID</param>
        /// <returns></returns>
        private static CombatantModel buildCombatantModel(RarityType rarityModifier, int level, int teamID)
        {
            int oneBasedLevel = level + 1;

            int maxHealth = oneBasedLevel * 50 + oneBasedLevel * 15 * (int)rarityModifier;
            int healthRegenPerTurn = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            int maxMana = oneBasedLevel * 50 + oneBasedLevel * 15 * (int)rarityModifier;
            int manaRegenPerTurn = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            int armor = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            int spellResist = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            int meleeDamage = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            float meleeCritNormalized = oneBasedLevel * 0.001f;
            int spellDamage = oneBasedLevel + oneBasedLevel * (int)rarityModifier;
            float spellCritNormalized = oneBasedLevel * 0.001f;

            return new CombatantModel(
                level, maxHealth, healthRegenPerTurn, maxMana, manaRegenPerTurn, armor, spellResist, 
                meleeDamage, meleeCritNormalized, spellDamage, spellCritNormalized, teamID, null);
        }

        /// <summary>
        /// Applies any boosts from equipment to a CombatantModel's existing stats.
        /// </summary>
        /// <param name="combatantModel">The CombatantModel</param>
        /// <param name="equipmentItemModels">Any equipment being worn.</param>
        private static void boostCombatantStatsFromEquipment(CombatantModel combatantModel, EquipmentItemModel[] equipmentItemModels)
        {
            for (int i = 0; i < equipmentItemModels.Length; i++)
            {
                EquipmentItemModel equipmentItemModel = equipmentItemModels[i];
                combatantModel.MaxHealth += equipmentItemModel.MaxHealthBoost;
                combatantModel.HealthRegenPerTurn += equipmentItemModel.HealthRegenBoost;
                combatantModel.MaxMana += equipmentItemModel.MaxManaBoost;
                combatantModel.ManaRegenPerTurn += equipmentItemModel.ManaRegenBoost;
                combatantModel.Armor += equipmentItemModel.ArmorBoost;
                combatantModel.SpellResist += equipmentItemModel.SpellResistBoost;
                combatantModel.MeleeDamage += equipmentItemModel.MeleeDamageBoost;
                combatantModel.MeleeCritChanceNormalized += equipmentItemModel.MeleeCritBoost;
                combatantModel.SpellDamage += equipmentItemModel.SpellDamageBoost;
                combatantModel.SpellCritChanceNormalized += equipmentItemModel.SpellCritBoost;
            }
        }

        /// <summary>
        /// Merges equippedAbilities with any abilities provided by specified equipment.
        /// </summary>
        /// <param name="equippedAbilities">Abilities the CombatantModel has equipped</param>
        /// <param name="equipmentItemModels">Any equipment the CombatantModel is wearing</param>
        /// <returns></returns>
        private static AbilityItemModel[] mergeAbilities(AbilityItemModel[] equippedAbilities, EquipmentItemModel[] equipmentItemModels)
        {
            int equippedAbilitiesCount = equippedAbilities == null ? 0 : equippedAbilities.Length;
            int equipmentAbilitiesCount = 0;
            for (int i = 0; i < equipmentItemModels.Length; i++)
            {
                if (equipmentItemModels[i].AbilityItemModels != null)
                {
                    equipmentAbilitiesCount += equipmentItemModels[i].AbilityItemModels.Length;
                }
            }

            AbilityItemModel[] mergedAbilities = new AbilityItemModel[equippedAbilitiesCount + equipmentAbilitiesCount];
            int mergeIndex = 0;
            for (mergeIndex = 0; mergeIndex < equippedAbilitiesCount; mergeIndex++)
            {
                mergedAbilities[mergeIndex] = equippedAbilities[mergeIndex];
            }
            mergeIndex++;

            for (int e = 0; e < equipmentItemModels.Length; e++)
            {
                if (equipmentItemModels[e].AbilityItemModels != null)
                {
                    for (int ea = 0; ea < equipmentItemModels[e].AbilityItemModels.Length; ea++)
                    {
                        mergedAbilities[mergeIndex++] = equipmentItemModels[e].AbilityItemModels[ea];
                    }
                }
            }

            return mergedAbilities;
        }
    }
}
