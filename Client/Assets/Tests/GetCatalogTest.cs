using System;
using Net.Services;
using Net.Services.CrossPlatform.Login;
using Platform;
using UnityEngine;
using Net.Services.Catalog;
using Common.Inventory;
using Net.Services.Config;

namespace Tests
{
    public class GetCatalogTest :MonoBehaviour
    {
        private LoginServiceBase ls;
        private ConfigServiceBase confService;
        private CatalogServiceBase cs;

        private void Start()
        {
            ls = ServiceResolver.GetForCurrentPlatform<LoginServiceBase>();
            ls.Login(true, onLoginSuccess, onLoginFailed);
        }

        private void onLoginSuccess()
        {
            confService = ServiceResolver.GetForCurrentPlatform<ConfigServiceBase>();
            confService.GetPlatformConfig(onConfigSucces, onConfigFailed);
        }

        private void onConfigSucces(PlatformConfig obj)
        {
            cs = ServiceResolver.GetForCurrentPlatform<CatalogServiceBase>();
            cs.GetCatalog(PlatformConfig.Instance.CatalogID, onCatalogSuccess, onCatalogFailed);
        }

        private void onConfigFailed()
        {
            throw new NotImplementedException();
        }

        private void onCatalogSuccess(CatalogModel obj)
        {
            Debug.Log("Got catalog.");
        }

        private void onCatalogFailed()
        {
            throw new NotImplementedException();
        }

        private void onLoginFailed()
        {
            throw new NotImplementedException();
        }
    }
}
