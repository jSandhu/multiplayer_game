using Common.Inventory.Currency;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Net.Services.Inventory
{
    public class PlayFabInventoryService : InventoryServiceBase
    {
        private const string LEVEL_ZERO_BASED_PROP = "Level_ZeroBased";

        protected override void getPlayerInventory(Action<List<RawPlayerItem>, RawCurrencySet> successHandler, Action failureHandler)
        {
            GetUserInventoryRequest request = new GetUserInventoryRequest();
            PlayFabClientAPI.GetUserInventory(
                request, 
                result => { parseItems(result, successHandler);}, 
                error => {
                    Debug.LogError("Couldn't parse player inventory.");
                    failureHandler();
                }
            );
        }

        private void parseItems(GetUserInventoryResult result, Action<List<RawPlayerItem>, RawCurrencySet> successHandler)
        {
            List<RawPlayerItem> rawPlayerItems = new List<RawPlayerItem>();
            for (int i = 0; i < result.Inventory.Count; i++)
            {
                ItemInstance itemInstance = result.Inventory[i];

                RawPlayerItem rawPlayerItem = new RawPlayerItem();
                rawPlayerItem.ID = int.Parse(itemInstance.ItemId);
                rawPlayerItem.InstanceID = itemInstance.ItemInstanceId;
                rawPlayerItem.StackedCount = itemInstance.RemainingUses == null ? 1 : (int)itemInstance.RemainingUses;
                string levelZeroBased = null;
                if (itemInstance.CustomData != null)
                {
                    itemInstance.CustomData.TryGetValue(LEVEL_ZERO_BASED_PROP, out levelZeroBased);
                }
                rawPlayerItem.Level_ZeroBased = string.IsNullOrEmpty(levelZeroBased) ? 0 : int.Parse(levelZeroBased);
                rawPlayerItem.CustomData = itemInstance.CustomData;
                rawPlayerItems.Add(rawPlayerItem);
            }

            RawCurrencySet currencySet = new RawCurrencySet();
            result.VirtualCurrency.TryGetValue(CurrencySymbol.GC.ToString(), out currencySet.Gold);
            result.VirtualCurrency.TryGetValue(CurrencySymbol.XP.ToString(), out currencySet.Experience);

            successHandler(rawPlayerItems, currencySet);
        }
    }
}
