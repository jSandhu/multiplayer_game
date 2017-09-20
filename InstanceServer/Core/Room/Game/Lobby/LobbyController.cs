using DIFramework;
using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Player;
using System.Collections.Generic;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Room.Game.Lobby.Commands;
using InstanceServer.Core.Room.Game.Commands;
using System;
using InstanceServer.Core.Net.OperationRequests;
using Photon.SocketServer;
using InstanceServer.Core.Combat.Behaviours;

namespace InstanceServer.Core.Room.Game.Lobby
{
    public class LobbyController
    {
        private Action<List<ServerPlayerModel>> onAllPlayersReadyHandler;
        private PlayerServiceBase playerService;
        private Dictionary<IClientConnection, string> clientPeerConnectionToPlayerID = new Dictionary<IClientConnection, string>();
        private Dictionary<string, IClientConnection> playerIDToConnection = new Dictionary<string, IClientConnection>();
        private List<ServerPlayerModel> activeServerPlayerModels = new List<ServerPlayerModel>();

        public LobbyController(Action<List<ServerPlayerModel>> onAllPlayersReadyHandler)
        {
            this.onAllPlayersReadyHandler = onAllPlayersReadyHandler;
            this.playerService = DIContainer.GetInstanceByContextID<PlayerServiceBase>(InstanceServerApplication.CONTEXT_ID);
        }

        public void AddPlayerForConnection(PlayerAuthenticationModel playerAuthModel, IClientConnection clientConnection)
        {
            clientPeerConnectionToPlayerID.Add(clientConnection, playerAuthModel.UserID);
            playerIDToConnection.Add(playerAuthModel.UserID, clientConnection);
            clientConnection.Disconnected += onClientDisconnected;

            PlayerCombatBehaviourBase playerCombatBehaviour = DIContainer.CreateInstanceByContextID<PlayerCombatBehaviourBase>(InstanceServerApplication.CONTEXT_ID);
            playerService.GetPlayerModelByID(playerAuthModel, playerCombatBehaviour, onPlayerModelReceived, onGetPlayerModelFailed);
        }

        private void onGetPlayerModelFailed(PlayerAuthenticationModel failedPlayerAuthModel)
        {
            Log.Info("LobbyController.onGetPlayerModelFailed: Failed to load ServerPlayerModel for player with ID: " + failedPlayerAuthModel.UserID);
            playerIDToConnection[failedPlayerAuthModel.UserID].Disconnect();
        }

        private void onPlayerModelReceived(ServerPlayerModel serverPlayerModel)
        {
            // Unready all players
            for (int i = 0; i < activeServerPlayerModels.Count; i++)
            {
                SendPlayerUnreadyEventSCMD.Execute(activeServerPlayerModels[i].ClientConnection, activeServerPlayerModels);
            }

            serverPlayerModel.ClientConnection = playerIDToConnection[serverPlayerModel.PlayerID];
            activeServerPlayerModels.Add(serverPlayerModel);

            serverPlayerModel.ClientConnection.AddOperationRequestListener(PlayerReadyOpRequest.OP_CODE, onPlayerReady);
            serverPlayerModel.ClientConnection.AddOperationRequestListener(PlayerUnreadyOpRequest.OP_CODE, onPlayerUnready);

            SendPlayerJoinedLobbyEventsSCMD.Execute(serverPlayerModel, activeServerPlayerModels);

            Log.Info("LobbyController listening for player ready/unready");
        }

        private void onPlayerReady(OperationRequest opRequest, IClientConnection clientConnection, SendParameters sendParameters)
        {
            SendPlayerReadyEventSCMD.Execute(clientConnection, activeServerPlayerModels);
            checkAllPlayersReady();
        }

        private void onPlayerUnready(OperationRequest opRequest, IClientConnection clientConnection, SendParameters sendParameters)
        {
            SendPlayerUnreadyEventSCMD.Execute(clientConnection, activeServerPlayerModels);
        }

        private void checkAllPlayersReady()
        {
            for (int i = 0; i < activeServerPlayerModels.Count; i++)
            {
                if (!activeServerPlayerModels[i].IsReady)
                {
                    return;
                }
            }

            // remove listeners
            for (int j = 0; j < activeServerPlayerModels.Count; j++)
            {
                activeServerPlayerModels[j].ClientConnection.RemoveOperationRequestListener(PlayerReadyOpRequest.OP_CODE, onPlayerReady);
                activeServerPlayerModels[j].ClientConnection.RemoveOperationRequestListener(PlayerUnreadyOpRequest.OP_CODE, onPlayerUnready);
                activeServerPlayerModels[j].ClientConnection.Disconnected -= onClientDisconnected;
            }

            clientPeerConnectionToPlayerID.Clear();
            playerIDToConnection.Clear();

            onAllPlayersReadyHandler(activeServerPlayerModels);
        }

        private void onClientDisconnected(IClientConnection clientConnection)
        {
            clientConnection.Disconnected -= onClientDisconnected;
            string disconnectedPlayerID = clientPeerConnectionToPlayerID[clientConnection];

            for (int i = activeServerPlayerModels.Count - 1; i >= 0; i--)
            {
                if (activeServerPlayerModels[i].PlayerID == disconnectedPlayerID)
                {
                    activeServerPlayerModels.RemoveAt(i);
                }
                else
                {
                    // unready all players
                    SendPlayerUnreadyEventSCMD.Execute(activeServerPlayerModels[i].ClientConnection, activeServerPlayerModels);
                }
            }

            playerIDToConnection.Remove(disconnectedPlayerID);
            clientPeerConnectionToPlayerID.Remove(clientConnection);

            SendPlayerDisconnectedEventSCMD.Execute(disconnectedPlayerID, activeServerPlayerModels);
        }
    }
}
