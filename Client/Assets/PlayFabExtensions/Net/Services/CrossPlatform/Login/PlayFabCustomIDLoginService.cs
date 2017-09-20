using PlayFab;
using PlayFab.ClientModels;
using PlayFabExtensions;
using System;

namespace Net.Services.CrossPlatform.Login
{
    public class PlayFabCustomIDLoginService : CustomIDLoginServiceBase
    {
        public PlayFabCustomIDLoginService()
        {
            PlayFabExtensionsConfig.Init();
        }

        protected override void loginWithCustomID(string customID, bool autoCreateAccount, Action successHandler, Action failureHandler)
        {
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
            {
                CustomId = customID,
                CreateAccount = autoCreateAccount
            };

            PlayFabClientAPI.LoginWithCustomID(request, 
                result => {
                    successHandler();
                }, 
                error => {
                    failureHandler();
                });
        }
    }
}
