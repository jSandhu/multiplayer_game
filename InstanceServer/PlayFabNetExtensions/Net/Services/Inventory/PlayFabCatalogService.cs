using System;
using Common.Inventory;
using InstanceServer.Core.Net.Services.Inventory;
using PlayFab.ServerModels;
using PlayFab;
using InstanceServer.Core.Logging;
using Common.ExtensionUtils;
using Common.ExtensionUtils.PlayFab;

namespace InstanceServer.PlayFabNetExtensions.Net.Services.Inventory
{
    public class PlayFabCatalogService : CatalogServiceBase
    {
        public PlayFabCatalogService()
        {
            PlayFabSetup.Init();
        }

        protected override void getCatalog(string catalogID, Action<CatalogModel> onSuccessHandler, Action onFailureHandler)
        {
            GetCatalogItemsRequest request = new GetCatalogItemsRequest();
            request.CatalogVersion = catalogID;

            IJsonParser jsonParser = new PlayFabServerJsonParser();
            PlayFabServerAPI.GetCatalogItemsAsync(request).ContinueWith(t => {
                if (t.Result.Error != null)
                {
                    Log.Error("PlayFabCatalogService error getting catalog: " + t.Result.Error.ErrorMessage);
                    onFailureHandler();
                }
                else
                {
                    CatalogModel catalogModel = new CatalogModel(catalogID);
                    for (int i = 0; i < t.Result.Result.Catalog.Count; i++)
                    {
                        try {
                            CatalogItem catalogItem = t.Result.Result.Catalog[i];
                            ItemModel itemModel = PlayFabItemFactory.GetItemModelFromCatalogItem(
                                catalogItem.ItemId,
                                catalogItem.ItemClass,
                                catalogItem.DisplayName,
                                catalogItem.Description,
                                catalogItem.CustomData,
                                jsonParser);
                            catalogModel.AddItemWithStackCount(itemModel);
                        } catch (Exception) {
                            Log.Warning("PlayFabCatalogService couldn't parse item with ID: " + 
                                t.Result.Result.Catalog[i].ItemId);
                            continue;
                        }
                    }
                    onSuccessHandler(catalogModel);
                }
            });
        }
    }
}
