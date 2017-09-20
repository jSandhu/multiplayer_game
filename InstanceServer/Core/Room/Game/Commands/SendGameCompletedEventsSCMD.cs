using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    public static class SendGameCompletedEventsSCMD
    {
        public static void Execute(List<ServerPlayerModel> eventReceivers)
        {
            GameCompletedEvent gameCompletedEvent = new GameCompletedEvent();
            for (int i = 0; i < eventReceivers.Count; i++)
            {
                if (eventReceivers[i].ClientConnection.Connected)
                {
                    eventReceivers[i].ClientConnection.SendEvent(gameCompletedEvent.EventData, gameCompletedEvent.SendParameters);
                }
            }
        }
    }
}
