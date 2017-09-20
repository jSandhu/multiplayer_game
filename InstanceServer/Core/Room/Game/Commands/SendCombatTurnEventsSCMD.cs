using Common.Combat.Actions;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    public class SendCombatTurnEventsSCMD
    {
        public static void Execute(List<ServerPlayerModel> eventReceivers, CombatActionsCollectionModel combatActionsCollectionModel)
        {
            CombatTurnEvent combatTurnEvent = new CombatTurnEvent(combatActionsCollectionModel);

            Logging.Log.Info("-- SENDING CombatTurnEvent to clients: \n" + combatActionsCollectionModel.ToString() );

            for (int i = 0; i < eventReceivers.Count; i++)
            {
                if (eventReceivers[i].ClientConnection.Connected)
                {
                    eventReceivers[i].ClientConnection.SendEvent(combatTurnEvent.EventData, combatTurnEvent.SendParameters);
                }
            }
        }
    }
}
