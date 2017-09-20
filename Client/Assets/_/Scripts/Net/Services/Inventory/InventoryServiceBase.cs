using Common.Inventory;
using DIFramework;
using Platform;
using Player;
using System;
using System.Collections.Generic;

namespace Net.Services.Inventory
{
    public abstract class InventoryServiceBase : ServiceBase
    {
        protected struct RawCurrencySet
        {
            public int Gold;
            public int Experience;
        }

        protected struct RawPlayerItem
        {
            public int ID;
            public string InstanceID;
            public int Level_ZeroBased;
            public int StackedCount;
            public Dictionary<string, string> CustomData;
        }

        public override int Priority { get { return (int)PriorityType.Default; } }
        public override bool AvailableForPlatform(PlatformType platformType) { return true; }

        public void GetPlayerInventory(Action<PlayerInventoryModel> successHandler, Action failureHandler)
        {
            getPlayerInventory(
                (rawPlayerItems, rawCurrencySet) => {
                    successHandler(buildPlayerInventory(rawPlayerItems, rawCurrencySet));
                }, 
                failureHandler);
        }

        private PlayerInventoryModel buildPlayerInventory(List<RawPlayerItem> rawPlayerItems, RawCurrencySet currencySet)
        {
            CatalogModel catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(GameContext.ID);

            PlayerInventoryModel playerInventory = new PlayerInventoryModel();
            for (int i = 0; i < rawPlayerItems.Count; i++)
            {
                RawPlayerItem rawItem = rawPlayerItems[i];
                ItemModel playerOwnedItemModel = catalogModel.GetItemByID(rawItem.ID).Clone();
                playerOwnedItemModel.InstanceID = rawItem.InstanceID;
                playerOwnedItemModel.StackedCount = rawItem.StackedCount;
                playerOwnedItemModel.Level_ZeroBased = rawItem.Level_ZeroBased;

                playerInventory.AddItemWithStackCount(playerOwnedItemModel);
            }
            playerInventory.GoldCurrency = currencySet.Gold;
            playerInventory.ExperienceCurrency = currencySet.Experience;

            return playerInventory;
        }

        protected abstract void getPlayerInventory(Action<List<RawPlayerItem>, RawCurrencySet> successHandler, Action failureHandler);
    }
}
