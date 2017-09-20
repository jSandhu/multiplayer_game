using Common.World;
using InstanceServer.Core.Player;
using System;
using System.Collections.Generic;
using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Events;
using Photon.SocketServer;
using Common.Net.OperationRequests;

namespace InstanceServer.Core.Room.Game.Commands
{
    public class SendLoadWorldEventsCMD
    {
        private WorldModel worldModel;
        private List<ServerPlayerModel> serverPlayerModels;
        private event Action<List<ServerPlayerModel>> onAllPlayersLoadedWorldHandler;
        private int numWorldLoadsCompleted;

        public SendLoadWorldEventsCMD(WorldModel worldModel, List<ServerPlayerModel> serverPlayerModels,
            Action<List<ServerPlayerModel>> onAllPlayersLoadedWorldHandler)
        {
            this.worldModel = worldModel;
            this.serverPlayerModels = serverPlayerModels;
            this.onAllPlayersLoadedWorldHandler = onAllPlayersLoadedWorldHandler;
        }

        public void Execute()
        {
            for (int i = 0; i < serverPlayerModels.Count; i++)
            {
                serverPlayerModels[i].Disconnected += onPlayerDisconnected;
            }

            LoadWorldEvent loadWorldEvent = new LoadWorldEvent(worldModel);
            for (int j = 0; j < serverPlayerModels.Count; j++)
            {
                serverPlayerModels[j].ClientConnection.AddOperationRequestOnceListener(OperationCodes.PLAYER_LOADED_WORLD, onPlayerLoadedWorld);
                serverPlayerModels[j].ClientConnection.SendEvent(loadWorldEvent.EventData, loadWorldEvent.SendParameters);
            }
        }

        private void onPlayerLoadedWorld(OperationRequest operationRequest, IClientConnection clientConnection, SendParameters sendParams)
        {
            // No need to create PlayerLoadedWorldOpRequest from OperationRequest at this time.

            numWorldLoadsCompleted++;
            clientConnection.RemoveOperationRequestOnceListener(OperationCodes.PLAYER_LOADED_WORLD, onPlayerLoadedWorld);
            checkCompletion();
        }

        private void onPlayerDisconnected(ServerPlayerModel serverPlayerModel)
        {
            serverPlayerModel.Disconnected -= onPlayerDisconnected;
            serverPlayerModel.ClientConnection.RemoveOperationRequestOnceListener(OperationCodes.PLAYER_LOADED_WORLD, onPlayerLoadedWorld);
            serverPlayerModels.Remove(serverPlayerModel);
            checkCompletion();
        }

        private void checkCompletion()
        {
            if (numWorldLoadsCompleted != serverPlayerModels.Count)
            {
                return;
            }

            for (int i = 0; i < serverPlayerModels.Count; i++)
            {
                serverPlayerModels[i].Disconnected -= onPlayerDisconnected;
            }

            onAllPlayersLoadedWorldHandler(serverPlayerModels);
        }
    }
}
