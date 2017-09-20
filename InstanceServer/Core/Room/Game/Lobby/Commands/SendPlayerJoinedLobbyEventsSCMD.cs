using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Lobby.Commands
{
    public static class SendPlayerJoinedLobbyEventsSCMD
    {
        public static void Execute(ServerPlayerModel newServerPlayerModel, List<ServerPlayerModel> allServerPlayerModels)
        {
            // Send PlayersJoinedEvent to added player with all players including added player
            PlayersJoinedLobbyEvent existingPlayersEvent = new PlayersJoinedLobbyEvent(allServerPlayerModels.ToArray());
            if (newServerPlayerModel.ClientConnection.Connected)
            {
                newServerPlayerModel.ClientConnection.SendEvent(existingPlayersEvent.EventData, existingPlayersEvent.SendParameters);
            }

            // Send PlayersJoinedEvent to OTHER players with just added player
            ServerPlayerModel[] newServerPlayerModelArray = new ServerPlayerModel[] { newServerPlayerModel };
            PlayersJoinedLobbyEvent newPlayerEvent = new PlayersJoinedLobbyEvent(newServerPlayerModelArray);
            for (int j = 0; j < allServerPlayerModels.Count; j++)
            {
                if (allServerPlayerModels[j] != newServerPlayerModel)
                {
                    if (allServerPlayerModels[j].ClientConnection.Connected)
                    {
                        allServerPlayerModels[j].ClientConnection.SendEvent(newPlayerEvent.EventData, newPlayerEvent.SendParameters);
                    }
                }
            }
        }
    }
}
