using InstanceServer.Core.Player;
using System;

namespace InstanceServer.Core.Net.Services.Session
{
    public abstract class SessionServiceBase
    {
        public void ValidateSession(string sessionID, Action<PlayerAuthenticationModel> successHandler, Action failureHandler)
        {
            validateSession(sessionID, successHandler, failureHandler);
        }

        protected abstract void validateSession(string sessionID, Action<PlayerAuthenticationModel> successHandler, Action failureHandler);
    }
}
