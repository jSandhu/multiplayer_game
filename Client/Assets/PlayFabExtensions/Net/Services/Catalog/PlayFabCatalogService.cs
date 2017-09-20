using System;
using Common.Inventory;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using Common.ExtensionUtils;
using Common.ExtensionUtils.PlayFab;
using PlayFabExtensions;

namespace Net.Services.Catalog
{
    public class PlayFabCatalogService : CatalogServiceBase
    {
        public PlayFabCatalogService()
        {
            PlayFabExtensionsConfig.Init();
        }

        public override void GetCatalog(string id, Action<CatalogModel> successHandler, Action failureHandler)
        {
            GetCatalogItemsRequest request = new GetCatalogItemsRequest()
            {
                CatalogVersion = id
            };

            PlayFabClientAPI.GetCatalogItems(request,
                result => { parseItems(id, result, successHandler); },
                error => {
                    Debug.LogError("Couldn't parse catalog.");
                    failureHandler();
                });
        }

        private void parseItems(string id, GetCatalogItemsResult result,  Action<CatalogModel> successHandler)
        {
            IJsonParser jsonParser = new PlayFabClientJsonParser();
            CatalogModel catalogModel = new CatalogModel(id);
            for (int i = 0; i < result.Catalog.Count; i++)
            {
                CatalogItem catalogItem = result.Catalog[i];
                catalogModel.AddItemWithStackCount(
                    PlayFabItemFactory.GetItemModelFromCatalogItem(
                        catalogItem.ItemId,
                        catalogItem.ItemClass,
                        catalogItem.DisplayName,
                        catalogItem.Description,
                        catalogItem.CustomData,
                        jsonParser));
            }
            successHandler(catalogModel);
        }
    }
}
