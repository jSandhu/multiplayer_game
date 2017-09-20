using System;
using Platform;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

namespace Net.Services.Config
{
    public class RawConfigSet
    {
        public RawConfig Default = null;
        public RawConfig Android = null;
        public RawConfig iOS = null;
        public RawConfig Steam = null;
        public RawConfig Kongregate = null;
        public RawConfig ArmorGames = null;

        public RawConfig GetRawConfigByPlatform(PlatformType platformType)
        {
            switch(platformType)
            {
                case PlatformType.Default: return Default;
                case PlatformType.Android: return Default;
                case PlatformType.iOS: return Default;
                case PlatformType.Steam: return Default;
                case PlatformType.Kongregate: return Default;
                case PlatformType.ArmorGames: return Default;
            }
            throw new Exception("Unhandled PlatformType: " + platformType.ToString());
        }
    }

    [Serializable]
    public class RawConfig
    {
        public string CatalogID = null;
        public string StoreID = null;
        public string AssetBundleBasePath = null;
    }

    public class PlayFabConfigService : ConfigServiceBase
    {
        private const string CONFIG_VERSION_PREFIX = "config-";

        protected override void getPlatformConfig(PlatformConfig platformConfig, Action<PlatformConfig> successHandler, Action failureHandler)
        {
            string configKey = CONFIG_VERSION_PREFIX + platformConfig.Version;

            GetTitleDataRequest request = new GetTitleDataRequest()
            {
                Keys = new List<string> { configKey }
            };
            PlayFabClientAPI.GetTitleData(request,
                result => {
                    parseConfig(configKey, platformConfig, result, successHandler, failureHandler);
                },
                error => {
                    Debug.LogError("Couldn't get config for version: " + platformConfig.Version);
                    failureHandler();
                });
        }

        private void parseConfig(string configProperty, PlatformConfig platformConfig, 
            GetTitleDataResult result, Action<PlatformConfig> successHandler, Action failureHandler)
        {
            string configValue;
            if (!result.Data.TryGetValue(configProperty, out configValue))
            {
                throw new Exception("Couldn't find Title Data property : " + configProperty);
            }

            RawConfigSet configSet = JsonUtility.FromJson<RawConfigSet>(configValue);
            RawConfig rawConfig = configSet.GetRawConfigByPlatform(platformConfig.PlatformType);
            platformConfig.CatalogID = rawConfig.CatalogID;
            platformConfig.StoreID = rawConfig.StoreID;
            platformConfig.AssetBundleBasePath = rawConfig.AssetBundleBasePath;

            successHandler(platformConfig);
        }
    }
}
