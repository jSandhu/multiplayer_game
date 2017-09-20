using Common.Inventory;
using Common.Inventory.Abilities;
using Common.Inventory.Equipment;
using System;

namespace Common.ExtensionUtils.PlayFab
{
    public enum PlayFabItemClass { Ability, Equipment}

    public static class PlayFabItemFactory
    {
        public static ItemModel GetItemModelFromCatalogItem(string itemId, string itemClass, string displayName, string description,
            string customData, IJsonParser jsonParser)
        {
            ItemModel itemModel = null;

            PlayFabItemClass playFabItemClass = (PlayFabItemClass)Enum.Parse(typeof(PlayFabItemClass), itemClass);
            switch(playFabItemClass)
            {
                case PlayFabItemClass.Ability : itemModel = parseAbilityItemModel(customData, jsonParser); break;
                case PlayFabItemClass.Equipment: itemModel = parseEquipmentItemModel(customData, jsonParser); break;
            }

            if (itemModel != null)
            {
                itemModel.ID = int.Parse(itemId);
                itemModel.Name = displayName;
                itemModel.Description = description;
                ItemModelRaw itemModelRaw = jsonParser.DeserializeObject<ItemModelRaw>(customData);
                itemModelRaw.ApplyToItemModel(itemModel);
            }

            return itemModel;
        }

        private static AbilityItemModel parseAbilityItemModel(string customData, IJsonParser jsonParser)
        {
            AbilityItemModel abilityItemModel = new AbilityItemModel();
            AbilityItemModelRaw abilityItemModelRaw = jsonParser.DeserializeObject<AbilityItemModelRaw>(customData);
            abilityItemModelRaw.ApplyToAbilityItemModel(abilityItemModel, jsonParser);
            return abilityItemModel;
        }

        private static ItemModel parseEquipmentItemModel(string customData, IJsonParser jsonParser)
        {
            EquipmentItemModel equipmentItemModel = new EquipmentItemModel();
            EquipmentItemModelRaw equipmentItemModelRaw = jsonParser.DeserializeObject<EquipmentItemModelRaw>(customData);
            equipmentItemModelRaw.ApplyToEquipmentItemModel(equipmentItemModel);
            return equipmentItemModel;
        }
    }
}
