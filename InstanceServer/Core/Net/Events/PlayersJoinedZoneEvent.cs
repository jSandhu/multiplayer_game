using Common.Net.Events;
using InstanceServer.Core.Player;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class PlayersJoinedZoneEvent
    {
        public const byte EVENT_CODE = EventCodes.PLAYERS_JOINED_ZONE;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public PlayersJoinedZoneEvent(List<ServerPlayerModel> joinedServerPlayerModels)
        {
            object[] playerIDs = new object[joinedServerPlayerModels.Count];
            for (int i = 0; i < joinedServerPlayerModels.Count; i++)
            {
                playerIDs[i] = joinedServerPlayerModels[i].PlayerID;
            }

            Parameters = new Dictionary<byte, object>();
            Parameters.Add(0, playerIDs);

            EventData = new EventData(EVENT_CODE);
            EventData.Parameters = Parameters;

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
