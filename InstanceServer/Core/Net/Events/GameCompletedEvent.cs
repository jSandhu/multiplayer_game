using Common.Net.Events;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class GameCompletedEvent
    {
        public const byte EVENT_CODE = EventCodes.GAME_COMPLETED;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public GameCompletedEvent()
        {
            // Serialize worldModel to byte array

            Parameters = new Dictionary<byte, object>();
            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
