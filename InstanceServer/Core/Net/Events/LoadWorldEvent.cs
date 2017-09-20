using Common.Net.Events;
using Common.World;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class LoadWorldEvent
    {
        public const byte EVENT_CODE = EventCodes.LOAD_WORLD;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public LoadWorldEvent(WorldModel worldModel)
        {
            // Serialize worldModel to byte array
            object[] worldModelAsObjectArray = worldModel.ToObjectArray();

            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, worldModelAsObjectArray);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
