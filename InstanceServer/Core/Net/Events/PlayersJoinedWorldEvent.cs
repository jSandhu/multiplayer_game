using Common.Net.Events;
using InstanceServer.Core.Player;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class PlayersJoinedWorldEvent
    {
        public const byte EVENT_CODE = EventCodes.PLAYERS_JOINED_WORLD;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public PlayersJoinedWorldEvent(ServerPlayerModel[] joinedServerPlayerModels)
        {
            object[] playerModelsAsObjectArrays = new object[joinedServerPlayerModels.Length];
            for (int i = 0; i < joinedServerPlayerModels.Length; i++)
            {
                playerModelsAsObjectArrays[i] = joinedServerPlayerModels[i].ToObjectArray();
            }

            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, playerModelsAsObjectArrays);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
