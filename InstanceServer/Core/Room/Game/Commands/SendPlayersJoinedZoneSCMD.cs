using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    public static class SendPlayersJoinedZoneSCMD
    {
        public static void Execute(List<ServerPlayerModel> newlyJoinedPlayers, List<ServerPlayerModel> allPlayers)
        {
            PlayersJoinedZoneEvent evt = new PlayersJoinedZoneEvent(newlyJoinedPlayers);
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (!newlyJoinedPlayers.Contains(allPlayers[i]))
                {
                    allPlayers[i].ClientConnection.SendEvent(evt.EventData, evt.SendParameters);
                }
            }
        }
    }
}
