using Common.Inventory;
using Common.Net.Events;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class ZoneCompletedEvent
    {
        public const byte EVENT_CODE = EventCodes.ZONE_COMPLETED;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public ZoneCompletedEvent(Bundle rewardBundle, bool isFinalZone)
        {
            Parameters = new Dictionary<byte, object>();

            Parameters.Add(0, rewardBundle.ToObjectArray());
            Parameters.Add(1, isFinalZone);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
