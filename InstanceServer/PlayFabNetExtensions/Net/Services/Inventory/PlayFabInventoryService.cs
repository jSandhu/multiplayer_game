using InstanceServer.Core.Net.Services.Inventory;
using System;
using Common.Inventory;
using PlayFab.ServerModels;
using System.Collections.Generic;
using PlayFab;
using Common.Inventory.Currency;
using InstanceServer.Core.Logging;

namespace InstanceServer.PlayFabNetExtensions.Net.Services.Inventory
{
    public class PlayFabInventoryService : InventoryServiceBase
    {
        private const string GRANT_CURRENCIES_FUNC = "GrantMultipleCurrenciesToUser";

        public PlayFabInventoryService()
        {
            PlayFabSetup.Init();
        }

        protected override void grantBundleToPlayer(string playerID, Bundle bundle, string catalogID,
            Action<string> onSuccessHandler, Action<string> onFailureHandler)
        {
            // If there are no items to grant, grant currencies
            if (bundle.ItemModels == null || bundle.ItemModels.Length == 0)
            {
                grantCurrencies(playerID, bundle.CurrencySymbols, bundle.CurrencyValues, onSuccessHandler, onFailureHandler);
                return;
            }

            List<string> itemIDs = new List<string>();
            for (int i = 0; i < bundle.ItemModels.Length; i++)
            {
                itemIDs.Add(bundle.ItemModels[i].ID.ToString());
            }

            GrantItemsToUserRequest request = new GrantItemsToUserRequest()
            {
                CatalogVersion = catalogID,
                PlayFabId = playerID,
                ItemIds = itemIDs
            };

            PlayFabServerAPI.GrantItemsToUserAsync(request).ContinueWith( t=> {
                if (t.Result.Error != null)
                {
                    Log.Error("PlayFabInventoryService error granting items to playerID: " + playerID + " : " + 
                        t.Result.Error.ErrorMessage);
                    onFailureHandler(playerID);
                }
                else
                {
                    grantCurrencies(playerID, bundle.CurrencySymbols, bundle.CurrencyValues, onSuccessHandler, onFailureHandler);
                }
            });
        }

        private void grantCurrencies(string playerID, CurrencySymbol[] currencySymbols,
            int[] currencyValues, Action<string> onSuccessHandler, Action<string> onFailureHandler)
        {
            // If there are no currencies to grant, call success handler.
            if (currencyValues == null || currencyValues.Length == 0)
            {
                onSuccessHandler(playerID);
                return;
            }

            string[] currencyTypes = new string[currencySymbols.Length];
            for (int t = 0; t < currencyTypes.Length; t++)
            {
                currencyTypes[t] = currencySymbols[t].ToString();
            }

            ExecuteCloudScriptServerRequest request = new ExecuteCloudScriptServerRequest()
            {
                PlayFabId = playerID,
                FunctionName = GRANT_CURRENCIES_FUNC,
                FunctionParameter =  new {
                    currencyTypes = currencyTypes,
                    currencyValues = currencyValues
                }
            };

            PlayFabServerAPI.ExecuteCloudScriptAsync(request).ContinueWith(t=> {
                if (t.Result.Error != null)
                {
                    Log.Error("Couldn't grant currencies to playerID : " + playerID + " Error: " + 
                        t.Result.Error.ErrorMessage);
                    onFailureHandler(playerID);
                }
                else
                {
                    onSuccessHandler(playerID);
                }
            });
        }
    }
}
