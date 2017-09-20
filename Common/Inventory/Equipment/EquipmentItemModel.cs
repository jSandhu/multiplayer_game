using System;
using Common.Inventory.Abilities;
using Common.Rarity;

namespace Common.Inventory.Equipment
{
    /// <summary>
    /// ItemModel implementation for equipment that can be worn by a Player.
    /// 
    /// EquipmentItemModels provide various boosts to stats and can grant aditional abilities.
    /// </summary>
    public class EquipmentItemModel : ItemModel
    {
        public override ItemType ItemType { get { return ItemType.Equipment; } }

        public EquipmentType EquipmentType;
        public int MaxHealthBoost;
        public int HealthRegenBoost;
        public int MaxManaBoost;
        public int ManaRegenBoost;
        public int ArmorBoost;
        public int SpellResistBoost;
        public int MeleeDamageBoost;
        public float MeleeCritBoost;
        public int SpellDamageBoost;
        public float SpellCritBoost;
        public AbilityItemModel[] AbilityItemModels;

        // Used for parsing from raw data.
        public int[] AbilityIDs;

        public EquipmentItemModel() { }

        public EquipmentItemModel(int ID, string name, string description, int level, RarityType rarityType, int stackedCount, EquipmentType equipmentType,
            int maxHealthBoost, int healthRegenBoost, int maxManaBoost, int manaRegenBoost, int armorBoost, int spellResistBoost,
            int meleeDamageBoost, float meleeCritBoost, int spellDamageBoost, float spellCritBoost,
            AbilityItemModel[] abilityItemModels) : base(ID, name, description, level, rarityType, stackedCount)
        {
            this.EquipmentType = equipmentType;
            this.MaxHealthBoost = maxHealthBoost;
            this.HealthRegenBoost = healthRegenBoost;
            this.MaxManaBoost = maxManaBoost;
            this.ArmorBoost = armorBoost;
            this.SpellResistBoost = spellResistBoost;
            this.MeleeDamageBoost = meleeDamageBoost;
            this.MeleeCritBoost = meleeCritBoost;
            this.SpellDamageBoost = spellDamageBoost;
            this.SpellCritBoost = spellCritBoost;
            this.AbilityItemModels = abilityItemModels;
        }

        protected override void buildStatsFromLevel()
        {
            int oneBasedLevel = Level_ZeroBased + 1;
            MaxHealthBoost *= oneBasedLevel;
            HealthRegenBoost *= oneBasedLevel;
            MaxManaBoost *= oneBasedLevel;
            ArmorBoost *= oneBasedLevel;
            SpellResistBoost *= oneBasedLevel;
            MeleeDamageBoost *= oneBasedLevel;
            MeleeCritBoost *= oneBasedLevel;
            SpellDamageBoost *= oneBasedLevel;
            SpellCritBoost *= oneBasedLevel;

            if (AbilityItemModels != null)
            {
                for (int i = 0; i < AbilityItemModels.Length; i++)
                {
                    AbilityItemModels[i].Level_ZeroBased = Level_ZeroBased;
                }
            }
        }

        public override ItemModel Clone()
        {
            EquipmentItemModel clone = new EquipmentItemModel();
            clone.CopyFrom(this, true);
            return clone;
        }

        protected override void copyExtendedProperties(ItemModel sourceItemModel)
        {
            EquipmentItemModel equipmentItemModel = sourceItemModel as EquipmentItemModel;
            if (equipmentItemModel == null)
            {
                throw new InvalidCastException("Invalid type.");
            }
            EquipmentType = equipmentItemModel.EquipmentType;
            MaxHealthBoost = equipmentItemModel.MaxHealthBoost;
            HealthRegenBoost = equipmentItemModel.HealthRegenBoost;
            MaxManaBoost = equipmentItemModel.MaxManaBoost;
            ArmorBoost = equipmentItemModel.ArmorBoost;
            SpellResistBoost = equipmentItemModel.SpellResistBoost;
            MeleeDamageBoost = equipmentItemModel.MeleeDamageBoost;
            MeleeCritBoost = equipmentItemModel.MeleeCritBoost;
            SpellDamageBoost = equipmentItemModel.SpellDamageBoost;
            SpellCritBoost = equipmentItemModel.SpellCritBoost;

            if (equipmentItemModel.AbilityItemModels == null)
            {
                AbilityItemModels = null;
            }
            else
            {
                AbilityItemModels = new AbilityItemModel[equipmentItemModel.AbilityItemModels.Length];
                for (int i = 0; i < AbilityItemModels.Length; i++)
                {
                    AbilityItemModels[i] = new AbilityItemModel();
                    AbilityItemModels[i].CopyFrom(equipmentItemModel.AbilityItemModels[i], true);
                }
            }
        }

        protected override object[] serializeExtendedProperties(int startOffset)
        {
            // Has no additional properties to serialize.
            // Abilities can be determined from catalog data and their levels are equal to equipment level
            return new object[startOffset];
        }

        protected override void deserializeExtendedProperties(int startOffset, object[] source)
        {
            // Has no additional properties to deserialize
            // Abilities can be determined from catalog data and their levels are equal to equipment level
        }

        protected override void getCommaSeparatedToStringProps(out string className, out string commaSeparatedProps)
        {
            className = "EquipmentItemModel";

            string[] abilityStrings = new string[AbilityItemModels.Length];
            for (int i = 0; i < abilityStrings.Length; i++)
            {
                abilityStrings[i] = AbilityItemModels[i].ToString();
            }
            string abilities = "[" + string.Join(",", abilityStrings) + "]";

            commaSeparatedProps =
                "EquipmentType: " + EquipmentType.ToString() + ", " +
                "MaxHealthBoost: " + MaxHealthBoost + ", " +
                "HealthRegenBoost: " + HealthRegenBoost + ", " +
                "MaxManaBoost: " + MaxManaBoost + ", " +
                "ArmorBoost: " + ArmorBoost + ", " +
                "SpellResistBoost: " + SpellResistBoost + ", " +
                "MeleeDamageBoost: " + MeleeDamageBoost + ", " +
                "MeleeCritBoost: " + MeleeCritBoost + ", " +
                "SpellDamageBoost: " + SpellDamageBoost + ", " +
                "SpellCritBoost: " + SpellCritBoost + ", " +
                "AbilityItemModels: " + abilities;
        }
    }
}
