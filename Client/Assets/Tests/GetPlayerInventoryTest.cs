using System;
using Net.Services;
using Net.Services.Config;
using Net.Services.CrossPlatform.Login;
using Platform;
using UnityEngine;
using Net.Services.Inventory;
using Player;
using Net.Services.Catalog;
using Common.Inventory;
using DIFramework;

namespace Assets.Tests
{
    public class GetPlayerInventoryTest : MonoBehaviour
    {
        private LoginServiceBase ls;
        private ConfigServiceBase cfs;
        private InventoryServiceBase invs;
        private CatalogServiceBase cats;

        public void Start()
        {
            DIContainer.RegisterContainer(0);
            ls = ServiceResolver.GetForCurrentPlatform<LoginServiceBase>();
            ls.Login(true, onLoginSuccess, onLoginFailed);
        }

        private void onLoginSuccess()
        {
            cfs = ServiceResolver.GetForCurrentPlatform<ConfigServiceBase>();
            cfs.GetPlatformConfig(onConfigSuccess, onConfigFailed);
        }

        private void onConfigSuccess(PlatformConfig obj)
        {
            cats = ServiceResolver.GetForCurrentPlatform<CatalogServiceBase>();
            cats.GetCatalog(PlatformConfig.Instance.CatalogID, onCatalogSuccess, onCatalogFailed);
        }

        private void onCatalogSuccess(CatalogModel obj)
        {
            DIContainer.GetByID(0).RegisterInstance<CatalogModel>(obj);

            invs = ServiceResolver.GetForCurrentPlatform<InventoryServiceBase>();
            invs.GetPlayerInventory(onPlayerInventorySuccess, onPlayerInventoryFailed);
        }

        private void onCatalogFailed()
        {
            throw new NotImplementedException();
        }

        private void onPlayerInventorySuccess(PlayerInventoryModel obj)
        {
            Debug.Log("Got inventory");
        }

        private void onPlayerInventoryFailed()
        {
            throw new NotImplementedException();
        }

        private void onConfigFailed()
        {
            throw new NotImplementedException();
        }

        private void onLoginFailed()
        {
            throw new NotImplementedException();
        }
    }
}
