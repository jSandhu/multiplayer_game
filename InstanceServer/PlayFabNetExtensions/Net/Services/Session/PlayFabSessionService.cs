using InstanceServer.Core.Net.Services.Session;
using System;
using InstanceServer.Core.Player;
using PlayFab.ServerModels;
using PlayFab;

namespace InstanceServer.PlayFabNetExtensions.Net.Services.Session
{
    public class PlayFabSessionService : SessionServiceBase
    {
        protected override void validateSession(string sessionID, Action<PlayerAuthenticationModel> successHandler, Action failureHandler)
        {
            AuthenticateSessionTicketRequest request = new AuthenticateSessionTicketRequest();
            request.SessionTicket = sessionID;

            PlayFabServerAPI.AuthenticateSessionTicketAsync(request).ContinueWith(t => {
                if (t.Result.Error != null) { failureHandler(); }
                else
                {
                    PlayerAuthenticationModel playerAuthModel = new PlayerAuthenticationModel(sessionID,
                        t.Result.Result.UserInfo.PlayFabId, t.Result.Result.UserInfo.TitleInfo.DisplayName);
                    successHandler(playerAuthModel);
                }
            });
        }
    }
}
