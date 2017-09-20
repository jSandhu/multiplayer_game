using InstanceServer.Core.Net.Services.Session;
using InstanceServer.Core.Player;
using System;

namespace InstanceServer.Tests.Mocking.Net.Services.Session
{
    class MockSessionService : SessionServiceBase
    {
        public const string VALID_SESSION_ID = "515A454278A6760F---153E-8D4E26BA995C21D-D24AF2B3EFB8123F.D1D7D7495C60604A";
        public const string TEST_USER_ID = "515A454278A6760F";
        public const string TEST_USER_NAME = "user display name test";

        protected override void validateSession(string sessionID, Action<PlayerAuthenticationModel> successHandler, Action failureHandler)
        {
            if (sessionID == VALID_SESSION_ID)
            {
                successHandler(new PlayerAuthenticationModel(sessionID, TEST_USER_ID, TEST_USER_NAME));
            }
            else
            {
                failureHandler();
            }
        }
    }
}
