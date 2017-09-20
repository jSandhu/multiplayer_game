using PlayFab;
using System;
using UnityEngine;

namespace PlayFabExtensions
{
    [CreateAssetMenu(menuName = "PlayFab/PlayFab Config")]
    public class PlayFabExtensionsConfig : ScriptableObject
    {
        private const string RESOURCE_PATH = "PlayFabExtensionsConfig";

        public string TitleID;

        public static void Init()
        {
            if (string.IsNullOrEmpty(PlayFab.PlayFabSettings.TitleId))
            {
                PlayFabExtensionsConfig pfc = Resources.Load<PlayFabExtensionsConfig>(RESOURCE_PATH);
                if (pfc == null)
                {
                    throw new Exception("Couldn't load PlayFabExtensionsConfig.asset resource at path: "+RESOURCE_PATH);
                }
                PlayFabSettings.TitleId = pfc.TitleID;
            }
        }
    }
}
