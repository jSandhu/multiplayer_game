using Common.Net.Events;
using InstanceServer.Core.Player;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Events
{
    public class PlayersJoinedLobbyEvent
    {
        public const byte EVENT_CODE = EventCodes.PLAYERS_JOINED_LOBBY;

        public Dictionary<byte, object> Parameters;
        public EventData EventData;
        public SendParameters SendParameters;

        public PlayersJoinedLobbyEvent (ServerPlayerModel[] serverPlayerModels)
        {
            object[] playerModelsAsObjectArrays = new object[serverPlayerModels.Length];
            for (int i = 0; i < serverPlayerModels.Length; i++)
            {
                playerModelsAsObjectArrays[i] = serverPlayerModels[i].ToObjectArray();
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
