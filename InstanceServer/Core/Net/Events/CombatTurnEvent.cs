using Common.Combat.Actions;
using Common.Net.Events;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    /// <summary>
    /// Dispatched to all clients.  Details which CombatActions took place during a turn.
    /// </summary>
    public class CombatTurnEvent
    {
        public const byte EVENT_CODE = EventCodes.COMBAT_TURN;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public CombatTurnEvent(CombatActionsCollectionModel combatActionsCollectionModel)
        {
            object[] combatActionsCollectionAsObjArray = combatActionsCollectionModel.ToObjectArray();

            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, combatActionsCollectionAsObjArray);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
