using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    public static class SendPlayerDisconnectedEventSCMD
    {
        public static void Execute(string disconnectedPlayerID, List<ServerPlayerModel> allServerPlayerModels)
        {
            PlayerDisconnectedEvent playerDisconnectedEvent = new PlayerDisconnectedEvent(disconnectedPlayerID);
            for (int i = 0; i < allServerPlayerModels.Count; i++)
            {
                if (allServerPlayerModels[i].PlayerID != disconnectedPlayerID && allServerPlayerModels[i].ClientConnection.Connected)
                {
                    allServerPlayerModels[i].ClientConnection.SendEvent(playerDisconnectedEvent.EventData, playerDisconnectedEvent.SendParameters);
                }
            }
        }
    }
}
