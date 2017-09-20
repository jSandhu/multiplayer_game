using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    public static class SendPlayersJoinedWorldEventsSCMD
    {
        public static void Execute(List<ServerPlayerModel> allPlayers, List<ServerPlayerModel> newlyJoinedPlayers)
        {
            // New players receive all existing (including themselves)
            PlayersJoinedWorldEvent allPlayersJoinedWorldEvent = new PlayersJoinedWorldEvent(allPlayers.ToArray());
            for (int n = 0; n < newlyJoinedPlayers.Count; n++)
            {
                if (newlyJoinedPlayers[n].ClientConnection.Connected)
                {
                    newlyJoinedPlayers[n].ClientConnection.SendEvent(allPlayersJoinedWorldEvent.EventData, allPlayersJoinedWorldEvent.SendParameters);
                }
            }

            // Existing players only receive the new players
            PlayersJoinedWorldEvent newPlayersJoinedWorldEvent = new PlayersJoinedWorldEvent(newlyJoinedPlayers.ToArray());
            for (int i = 0; i < allPlayers.Count; i++)
            {
                bool isPreExistingPlayer = !newlyJoinedPlayers.Contains(allPlayers[i]);
                if (isPreExistingPlayer && allPlayers[i].ClientConnection.Connected)
                {
                    allPlayers[i].ClientConnection.SendEvent(newPlayersJoinedWorldEvent.EventData, newPlayersJoinedWorldEvent.SendParameters);
                }
            }
        }
    }
}
