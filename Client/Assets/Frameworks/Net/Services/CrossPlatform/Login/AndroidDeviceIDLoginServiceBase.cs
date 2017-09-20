#if UNITY_ANDROID
using Platform;
using System;
using UnityEngine;

namespace Net.Services.CrossPlatform.Login
{
    public abstract class AndroidDeviceIDLoginServiceBase : LoginServiceBase
    {
        public override int Priority { get { return (int)PriorityType.Default + 1; } }
        public override bool AvailableForPlatform(PlatformType platformType) { return platformType == PlatformType.Android; }
        public override bool RequiresUserID { get { return false; } }
        public override bool RequiresPassword { get { return false; } }

        protected override void login(bool autoCreateAccount, Action successHandler, Action failureHandler)
        {
            loginWithAndroidDeviceID(SystemInfo.deviceUniqueIdentifier, autoCreateAccount, successHandler, failureHandler);
        }

        protected abstract void loginWithAndroidDeviceID(
            string androidDeviceID, bool autoCreateAccount, Action successHandler, Action failureHandler);
    }
}
#endif