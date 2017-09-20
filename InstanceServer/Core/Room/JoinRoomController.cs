using InstanceServer.Core.Net;
using InstanceServer.Core.Net.OperationRequests;
using Photon.SocketServer;
using InstanceServer.Core.Player;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Room.Commands;
using System;
using InstanceServer.Core.OperationResponses;
using Common.Net.OperationResponses;
using System.Collections.Concurrent;

namespace InstanceServer.Core.Room
{
    /// <summary>
    /// Listens to clients for JoinRoomOpRequests and tries to place them in a new or existing room.
    /// </summary>
    public class JoinRoomController
    {
        private ConcurrentDictionary<string, RoomContext> roomIDToContext = new ConcurrentDictionary<string, RoomContext>();

        // Cache actions
        private Action<PlayerAuthenticationModel, IClientConnection, JoinRoomOpRequest> sessionValidatedHandler;
        private Action<IClientConnection> sessionValidationFailedHandler;

        public JoinRoomController()
        {
            sessionValidatedHandler = onSessionValidated;
            sessionValidationFailedHandler = onClientDisconnected;
        }

        /// <summary>
        /// Start listening to ClientPeerConnection for a JoinRoomOpRequest.
        /// </summary>
        /// <param name="clientConnection"></param>
        public void AddClientConnection(IClientConnection clientConnection)
        {
            clientConnection.AddOperationRequestOnceListener(JoinRoomOpRequest.OP_CODE, onJoinRoomRequestReceived);
            clientConnection.Disconnected += onClientDisconnected;
        }

        private void onClientDisconnected(IClientConnection clientConnection)
        {
            clientConnection.RemoveOperationRequestOnceListener(JoinRoomOpRequest.OP_CODE, onJoinRoomRequestReceived);
            clientConnection.Disconnected -= onClientDisconnected;
        }

        private void onJoinRoomRequestReceived(OperationRequest operationRequest, IClientConnection clientConnection, SendParameters sendParameters)
        {
            JoinRoomOpRequest joinRoomOpRequest = JoinRoomOpRequest.FromOperationRequest(operationRequest);

            // validate client's session
            new ValidateSessionCMD(joinRoomOpRequest, clientConnection, sessionValidatedHandler, sessionValidationFailedHandler).Execute();
        }

        private void onSessionValidated(PlayerAuthenticationModel playerAuthModel, IClientConnection clientConnection,
            JoinRoomOpRequest joinRoomOpRequest)
        {
            Log.Info("JoinRoomController: player session validated " + playerAuthModel.UserID + " ==> " + joinRoomOpRequest.RoomID );

            // Once client's session is validated, determine if the room will be a new or existing one.
            RoomContext roomContext = null;
            if(!roomIDToContext.TryGetValue(joinRoomOpRequest.RoomID, out roomContext))
            {
                Log.Info("Creating new room with ID: " + joinRoomOpRequest.RoomID);
                roomContext = new RoomContext(joinRoomOpRequest.RoomID, joinRoomOpRequest.Password);
                RoomContext addedRoomContext = roomIDToContext.GetOrAdd(roomContext.ID, roomContext);
                if (roomContext == addedRoomContext)
                {
                    roomContext.Destroyed += onRoomDestroyed;
                }
                else
                {
                    roomContext = addedRoomContext;
                }
            }
            else
            {
                Log.Info("Joining existing room with ID: " + roomContext.ID);
            }

            // Try adding client to the determined room.
            JoinRoomOpResponse joinRoomOpResponse = null;
            JoinRoomResponseCode joinRoomResponseCode = roomContext.TryAddClientConnection(playerAuthModel, clientConnection, joinRoomOpRequest.Password);

            // If we couldn't place the client into the room, respond with failure code.
            if (joinRoomResponseCode != JoinRoomResponseCode.AddedToLobby && joinRoomResponseCode != JoinRoomResponseCode.AddedToExistingGame)
            {
                joinRoomOpResponse = new JoinRoomOpResponse(false, joinRoomResponseCode);
                clientConnection.SendOperationResponse(joinRoomOpResponse.OperationResponse, joinRoomOpResponse.SendParameters);
                clientConnection.DisconnectAfterDelay();
                Log.Info("JoinRoomController: Couldn't add clientPeerConnection with ID " + clientConnection.ConnectionId + " to room with ID " + roomContext.ID + ". Code : " + joinRoomResponseCode);
                return;
            }

            // If we reach this point, the client was successfully added to a room, so respond with success.
            joinRoomOpResponse = new JoinRoomOpResponse(true, JoinRoomResponseCode.AddedToLobby);
            clientConnection.SendOperationResponse(joinRoomOpResponse.OperationResponse, joinRoomOpResponse.SendParameters);
        }

        private void onRoomDestroyed(RoomContext roomContext)
        {
            RoomContext removedRoom = null;
            roomIDToContext.TryRemove(roomContext.ID, out removedRoom);
            removedRoom.Destroyed -= onRoomDestroyed;
        }
    }
}
