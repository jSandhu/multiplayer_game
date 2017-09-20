using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Lobby.Commands
{
    public static class SendPlayerUnreadyEventSCMD
    {
        public static void Execute(IClientConnection unreadyClientConnection, List<ServerPlayerModel> allServerPlayerModels)
        {
            ServerPlayerModel unreadyServerPlayerModel = null;
            for (int i = 0; i < allServerPlayerModels.Count; i++)
            {
                if (allServerPlayerModels[i].ClientConnection == unreadyClientConnection)
                {
                    unreadyServerPlayerModel = allServerPlayerModels[i];
                    break;
                }
            }

            // Protect against spam
            if (!unreadyServerPlayerModel.IsReady)
            {
                return;
            }
            unreadyServerPlayerModel.IsReady = false;

            PlayerUnreadyEvent playerUnreadyEvent = new PlayerUnreadyEvent(unreadyServerPlayerModel.PlayerID);
            for (int j = 0; j < allServerPlayerModels.Count; j++)
            {
                if (allServerPlayerModels[j].ClientConnection.Connected)
                {
                    allServerPlayerModels[j].ClientConnection.SendEvent(playerUnreadyEvent.EventData, playerUnreadyEvent.SendParameters);
                }
            }
        }
    }
}
