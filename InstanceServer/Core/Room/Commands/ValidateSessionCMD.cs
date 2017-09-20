using DIFramework;
using InstanceServer.Core.Net;
using System;
using InstanceServer.Core.Player;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Net.OperationRequests;
using InstanceServer.Core.Net.Services.Session;

namespace InstanceServer.Core.Room.Commands
{
    public class ValidateSessionCMD
    {
        private JoinRoomOpRequest joinRoomOpRequest;
        private IClientConnection clientPeerConnection;
        private Action<PlayerAuthenticationModel, IClientConnection, JoinRoomOpRequest> successHandler;
        private Action<IClientConnection> failureHandler;
        private SessionServiceBase sessionService;

        public ValidateSessionCMD(JoinRoomOpRequest joinRoomOpRequest, IClientConnection clientConnection, 
            Action<PlayerAuthenticationModel, IClientConnection, JoinRoomOpRequest> successHandler, Action<IClientConnection> failureHandler)
        {
            this.joinRoomOpRequest = joinRoomOpRequest;
            this.clientPeerConnection = clientConnection;
            this.successHandler = successHandler;
            this.failureHandler = failureHandler;

            this.sessionService = DIContainer.GetInstanceByContextID<SessionServiceBase>(InstanceServerApplication.CONTEXT_ID);
        }

        public void Execute()
        {
            sessionService.ValidateSession(joinRoomOpRequest.SessionID, onValidated, onValidationFailed);
        }

        private void onValidationFailed()
        {
            Log.Warning("PLAYER CONNECTION FAILED");

            // Disconnect the client
            clientPeerConnection.Disconnect();

            failureHandler(clientPeerConnection);
        }

        private void onValidated(PlayerAuthenticationModel playerAuthModel)
        {
            successHandler(playerAuthModel, clientPeerConnection, joinRoomOpRequest);
        }
    }
}
