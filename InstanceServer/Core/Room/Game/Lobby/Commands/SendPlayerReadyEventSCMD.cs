using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Lobby.Commands
{
    public static class SendPlayerReadyEventSCMD
    {
        public static void Execute(IClientConnection readyClientConnection, List<ServerPlayerModel> allServerPlayerModels)
        {
            ServerPlayerModel readyServerPlayerModel = null;
            for (int i = 0; i < allServerPlayerModels.Count; i++)
            {
                if (allServerPlayerModels[i].ClientConnection == readyClientConnection)
                {
                    readyServerPlayerModel = allServerPlayerModels[i];
                    break;
                }
            }

            // Protect against spam
            if (readyServerPlayerModel.IsReady)
            {
                return;
            }
            readyServerPlayerModel.IsReady = true;

            PlayerReadyEvent playerReadyEvent = new PlayerReadyEvent(readyServerPlayerModel.PlayerID);
            for (int j = 0; j < allServerPlayerModels.Count; j++)
            {
                if (allServerPlayerModels[j].ClientConnection.Connected)
                {
                    allServerPlayerModels[j].ClientConnection.SendEvent(playerReadyEvent.EventData, playerReadyEvent.SendParameters);
                }
            }
        }
    }
}
