using System;
using UnityEngine;

namespace Platform
{
    public enum PlatformType { Mock = -1, Default, Android, iOS, Steam, Kongregate, ArmorGames};

    [CreateAssetMenu(menuName = "Platform/Platform Config")]
    public class PlatformConfig : ScriptableObject
    {
        private const string RESOURCE_PATH = "PlatformConfig";

        private static PlatformConfig _instance;
        public static PlatformConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<PlatformConfig>(RESOURCE_PATH);
                    _instance.PlatformType = getCurrentPlatform();
                    if (_instance == null)
                    {
                        throw new Exception("Couldn't load PlatformConfig.asset at path: " + RESOURCE_PATH);
                    }
                }
                return _instance;
            }
        }

        private static PlatformType getCurrentPlatform()
        {
#if PLATFORM_MOCK
            return PlatformType.Mock;
#elif UNITY_EDITOR
            return PlatformType.Default;
#elif UNITY_ANDROID
            return PlatformType.Android;
#elif UNITY_IOS
            return PlatformType.iOS;
#elif PLATFORM_STEAM
            return PlatformType.Steam;
#elif PLATFORM_KONGREGATE
            return PlatformType.Kongregate;
#elif PLATFORM_ARMORGAMES
            return PlatformType.ArmorGames;
#endif
        }

        public int Version;

        [NonSerialized] public PlatformType PlatformType;
        [NonSerialized] public string CatalogID;
        [NonSerialized] public string StoreID;
        [NonSerialized] public string AssetBundleBasePath;
    }
}
