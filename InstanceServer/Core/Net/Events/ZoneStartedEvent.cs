using Common.Combat;
using Common.Net.Events;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class ZoneStartedEvent
    {
        public const byte EVENT_CODE = EventCodes.ZONE_STARTED;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public ZoneStartedEvent(int zoneID, int turnNumber, CombatantModel[] allZoneCombatants)
        {
            object[] combatantsAsObjects = new object[allZoneCombatants.Length];
            for (int i = 0; i < combatantsAsObjects.Length; i++)
            {
                combatantsAsObjects[i] = allZoneCombatants[i].ToObjectArray();
            }

            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, zoneID);
            Parameters.Add(1, turnNumber);
            Parameters.Add(2, combatantsAsObjects);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
