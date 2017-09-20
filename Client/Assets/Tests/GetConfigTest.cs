using System;
using Net.Services;
using Net.Services.Config;
using Platform;
using UnityEngine;
using Net.Services.CrossPlatform.Login;

namespace Tests
{
    public class GetConfigTest : MonoBehaviour
    {
        private LoginServiceBase ls;

        private void Start()
        {
            ls = ServiceResolver.GetForCurrentPlatform<LoginServiceBase>();
            ls.Login(true, onLoginSuccess, onLoginFailed);
        }

        private void onLoginSuccess()
        {
            ConfigServiceBase cs = ServiceResolver.GetForCurrentPlatform<ConfigServiceBase>();
            cs.GetPlatformConfig(onConfigFetched, onGetConfigFailed);
        }

        private void onLoginFailed()
        {
            throw new NotImplementedException();
        }

        private void onConfigFetched(PlatformConfig obj)
        {
            Debug.Log("got config.");
        }

        private void onGetConfigFailed()
        {
            throw new NotImplementedException();
        }
    }
}
