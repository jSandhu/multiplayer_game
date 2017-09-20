using Common.Inventory;
using Common.Inventory.Abilities;
using Common.Inventory.Equipment;
using Common.Rarity;
using System;

namespace Common.ExtensionUtils.PlayFab
{
    class ItemModelRaw
    {
        public string RarityType = null;

        public void ApplyToItemModel(ItemModel itemModel)
        {
            itemModel.RarityType = (RarityType)int.Parse(RarityType);
        }
    }

    class AbilityItemModelRaw
    {
        public string AbilityEffectType = null;
        public string AbilityTargetType = null;
        public string AbilityDurationType = null;
        public string CastTurns = null;
        public string CoolDownTurns = null;
        public string ImmediateAmout = null;
        public string IsSpellBased = null;
        public string AbilityDurationData = null;

        public void ApplyToAbilityItemModel(AbilityItemModel abilityItemModel, IJsonParser jsonParser)
        {
            abilityItemModel.AbilityEffectType = (AbilityEffectType)int.Parse(AbilityEffectType);
            abilityItemModel.AbilityTargetType = (AbilityTargetType)int.Parse(AbilityTargetType);
            abilityItemModel.AbilityDurationType = (AbilityDurationType)int.Parse(AbilityDurationType);
            abilityItemModel.CastTurns = int.Parse(CastTurns);
            abilityItemModel.CoolDownTurns = int.Parse(CoolDownTurns);
            abilityItemModel.ImmediateAmout = int.Parse(ImmediateAmout);
            abilityItemModel.IsSpellBased = int.Parse(IsSpellBased) == 1;

            AbilityDurationDataRaw abilityDurationDataRaw = string.IsNullOrEmpty(AbilityDurationData) ? null :
                jsonParser.DeserializeObject<AbilityDurationDataRaw>(AbilityDurationData);
            if (abilityDurationDataRaw != null)
            {
                abilityItemModel.AbilityDurationData = new AbilityDurationData();
                abilityDurationDataRaw.ApplyToDurationData(abilityItemModel.AbilityDurationData, abilityItemModel);
            }
        }
    }

    class AbilityDurationDataRaw
    {
        public string PerTickAmount = null;
        public string TurnsPerTick = null;
        public string MaxTicks = null;

        public void ApplyToDurationData(AbilityDurationData abilityDurationData, AbilityItemModel parentAbility)
        {
            abilityDurationData.OriginAbilityID = parentAbility.ID;
            abilityDurationData.PerTickAmount = int.Parse(PerTickAmount);
            abilityDurationData.TurnsPerTick = int.Parse(TurnsPerTick);
            abilityDurationData.MaxTicks = int.Parse(MaxTicks);
            abilityDurationData.AbilityEffectType = parentAbility.AbilityEffectType;
            abilityDurationData.AbilityTargetType = parentAbility.AbilityTargetType;
        }
    }

    class EquipmentItemModelRaw
    {
        public string EquipmentType = null;
        public string MaxHealthBoost = null;
        public string HealthRegenBoost = null;
        public string MaxManaBoost = null;
        public string ManaRegenBoost = null;
        public string ArmorBoost = null;
        public string SpellResistBoost = null;
        public string MeleeDamageBoost = null;
        public string MeleeCritBoost = null;
        public string SpellDamageBoost = null;
        public string SpellCritBoost = null;
        public string AbilityIDs = null;

        public void ApplyToEquipmentItemModel(EquipmentItemModel equipmentItemModel)
        {
            equipmentItemModel.EquipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), EquipmentType);
            equipmentItemModel.MaxHealthBoost = int.Parse(MaxHealthBoost);
            equipmentItemModel.HealthRegenBoost = int.Parse(HealthRegenBoost);
            equipmentItemModel.MaxManaBoost = int.Parse(MaxManaBoost);
            equipmentItemModel.ManaRegenBoost = int.Parse(ManaRegenBoost);
            equipmentItemModel.ArmorBoost = int.Parse(ArmorBoost);
            equipmentItemModel.SpellResistBoost = int.Parse(SpellResistBoost);
            equipmentItemModel.MeleeDamageBoost = int.Parse(MeleeDamageBoost);
            equipmentItemModel.MeleeCritBoost = float.Parse(MeleeCritBoost);
            equipmentItemModel.SpellDamageBoost = int.Parse(SpellDamageBoost);
            equipmentItemModel.SpellCritBoost = float.Parse(SpellCritBoost);

            string[] abilityIDStrings = string.IsNullOrEmpty(AbilityIDs) ? null : AbilityIDs.Trim().Split(',');
            if (abilityIDStrings != null)
            {
                int[] abilityIDs = new int[abilityIDStrings.Length];
                for (int i = 0; i < abilityIDs.Length; i++)
                {
                    abilityIDs[i] = int.Parse(abilityIDStrings[i]);
                }
                equipmentItemModel.AbilityIDs = abilityIDs;
            }
        }
    }
}
