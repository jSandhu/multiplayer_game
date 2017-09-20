using Common.Combat;
using Common.World.Zone;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Commands
{
    /// <summary>
    /// Send message to players that specified zone has started. Also sends all combatants in zone
    /// so players can sync their local models.
    /// </summary>
    public static class SendZoneStartedEventsSCMD
    {
        public static void Execute(List<ServerPlayerModel> eventReceivers, ZoneModel zoneModel, int turnNumber)
        {
            CombatantModel[] allCombatantModels = new CombatantModel[eventReceivers.Count + zoneModel.EnemyNpcModels.Length];
            for (int e = 0; e < eventReceivers.Count; e++)
            {
                allCombatantModels[e] = eventReceivers[e].CombatantModel;
            }
            for (int n = 0; n < zoneModel.EnemyNpcModels.Length; n++)
            {
                allCombatantModels[eventReceivers.Count + n] = zoneModel.EnemyNpcModels[n].CombatantModel;
            }

            ZoneStartedEvent zoneStartedEvent = new ZoneStartedEvent(zoneModel.ID, turnNumber, allCombatantModels);
            for (int i = 0; i < eventReceivers.Count; i++)
            {
                if (eventReceivers[i].ClientConnection.Connected)
                {
                    eventReceivers[i].ClientConnection.SendEvent(zoneStartedEvent.EventData, zoneStartedEvent.SendParameters);
                }
            }
        }
    }
}
