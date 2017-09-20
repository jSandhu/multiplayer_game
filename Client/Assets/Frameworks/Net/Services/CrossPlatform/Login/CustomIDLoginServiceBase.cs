using Platform;
using System;
using UnityEngine;

namespace Net.Services.CrossPlatform.Login
{
    public abstract class CustomIDLoginServiceBase : LoginServiceBase
    {
        public override int Priority { get { return (int)PriorityType.Default; } }
        public override bool AvailableForPlatform(PlatformType platformType) { return platformType == PlatformType.Default; }
        public override bool RequiresUserID { get { return false; } }
        public override bool RequiresPassword { get { return false; } }

        protected override void login(bool autoCreateAccount, Action successHandler, Action failureHandler)
        {
            loginWithCustomID(SystemInfo.deviceUniqueIdentifier, autoCreateAccount, successHandler, failureHandler);
        }

        protected abstract void loginWithCustomID(
            string customID, bool autoCreateAccount, Action successHandler, Action failureHandler);
    }
}
