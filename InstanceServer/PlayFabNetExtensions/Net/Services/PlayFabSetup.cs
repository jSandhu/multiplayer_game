using PlayFab;

namespace InstanceServer.PlayFabNetExtensions.Net.Services
{
    public static class PlayFabSetup
    {
        private static bool initialized;
        public static void Init()
        {
            if (!initialized)
            {
                initialized = true;
                PlayFabSettings.DeveloperSecretKey = "RQHPNKCNUIOKU4U9SNXS614GAR9W8DUGNA7NGNKCM1HFY5QYW7";
                PlayFabSettings.TitleId = "153E";
            }
        }
    }
}
