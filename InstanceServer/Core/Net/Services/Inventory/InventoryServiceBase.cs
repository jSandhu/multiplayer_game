using Common.Inventory;
using System;

namespace InstanceServer.Core.Net.Services.Inventory
{
    public abstract class InventoryServiceBase
    {
        public void GrantBundleToPlayer(string playerID, Bundle bundle, string catalogID,
            Action<string> onSuccessHandler, Action<string> onFailureHandler)
        {
            // If the bundle is empty, immediately return success.
            if ((bundle.ItemModels == null || bundle.ItemModels.Length == 0) &&
                (bundle.CurrencyValues == null || bundle.CurrencyValues.Length == 0))
            {
                onSuccessHandler(playerID);
                return;
            }

            if (bundle.CurrencySymbols != null && bundle.CurrencySymbols.Length != bundle.CurrencyValues.Length)
            {
                throw new Exception("Bundle currency symbols and values array length mismatch.");
            }

            grantBundleToPlayer(playerID, bundle, catalogID, onSuccessHandler, onFailureHandler);
        }

        protected abstract void grantBundleToPlayer(string playerID, Bundle bundle, string catalogID, 
            Action<string> onSuccessHandler, Action<string> onFailureHandler);
    }
}
