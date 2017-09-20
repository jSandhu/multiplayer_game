using Common.Inventory;
using Common.Inventory.Abilities;
using Common.Inventory.Equipment;
using System;

namespace InstanceServer.Core.Net.Services.Inventory
{
    public abstract class CatalogServiceBase
    {
        public void GetCatalog(string catalogID, Action<CatalogModel> onSuccessHandler, Action onFailureHandler)
        {
            getCatalog(
                catalogID,
                catalogModel => {
                    updateEquipmentAbilityReferences(catalogModel);
                    onSuccessHandler(catalogModel);
                }, 
                onFailureHandler);
        }

        private void updateEquipmentAbilityReferences(CatalogModel catalogModel)
        {
            for (int i = 0; i < catalogModel.AllItems.Count; i++)
            {
                EquipmentItemModel equipmentItemModel = catalogModel.AllItems[i] as EquipmentItemModel;
                if (equipmentItemModel != null)
                {
                    if (equipmentItemModel.AbilityIDs != null)
                    {
                        equipmentItemModel.AbilityItemModels = new AbilityItemModel[equipmentItemModel.AbilityIDs.Length];
                        for (int a = 0; a < equipmentItemModel.AbilityIDs.Length; a++)
                        {
                            equipmentItemModel.AbilityItemModels[a] = catalogModel.GetItemByID(equipmentItemModel.AbilityIDs[a]) as AbilityItemModel;
                        }
                    }
                }
            }
        }

        protected abstract void getCatalog(string catalogID, Action<CatalogModel> onSuccessHandler, Action onFailureHandler);
    }
}
