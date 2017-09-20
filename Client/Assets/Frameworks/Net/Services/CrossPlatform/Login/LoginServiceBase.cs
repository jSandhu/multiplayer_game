using System;

namespace Net.Services.CrossPlatform.Login
{
    public abstract class LoginServiceBase : ServiceBase
    {
        public string UserID;
        public string Password;

        public void Login(bool autoCreateAccount, Action successHandler, Action failureHandler)
        {
            if (RequiresUserID && string.IsNullOrEmpty(UserID))
            {
                throw new Exception(GetType().ToString() + " requires UserID to Login.");
            }

            if (RequiresPassword && string.IsNullOrEmpty(Password))
            {
                throw new Exception(GetType().ToString() + " requires Password to Login.");
            }

            login(autoCreateAccount, successHandler, failureHandler);
        }

        internal void Login(object onLoginSuccess, Action onLoginFailed)
        {
            throw new NotImplementedException();
        }

        public abstract bool RequiresUserID { get; }
        public abstract bool RequiresPassword { get; }
        protected abstract void login(bool autoCreateAccount, Action successHandler, Action failureHandler);
    }
}
