#if UNITY_ANDROID
using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFabExtensions;

namespace Net.Services.CrossPlatform.Login
{
    public class PlayFabAndroidDeviceIDLoginService : AndroidDeviceIDLoginServiceBase
    {
        public PlayFabAndroidDeviceIDLoginService()
        {
            PlayFabExtensionsConfig.Init();
        }

        protected override void loginWithAndroidDeviceID(
            string androidDeviceID, bool autoCreateAccount, Action successHandler, Action failureHandler)
        {
            LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest()
            {
                AndroidDeviceId = androidDeviceID,
                CreateAccount = autoCreateAccount
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, 
                result => {
                    successHandler();
                }, 
                error => {
                    failureHandler();
                });
        }
    }
}
#endif