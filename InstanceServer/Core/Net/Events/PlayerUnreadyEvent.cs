using Common.Net.Events;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class PlayerUnreadyEvent
    {
        public const byte EVENT_CODE = EventCodes.PLAYER_UNREADY;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public PlayerUnreadyEvent(string playerID)
        {
            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, playerID);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
