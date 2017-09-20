using Common.Net.OperationRequests;
using Common.Net.OperationResponses;
using Photon.SocketServer;
using System.Collections.Generic;

namespace InstanceServer.Core.OperationResponses
{
    public class JoinRoomOpResponse
    {
        public static readonly byte OP_CODE = OperationCodes.JOIN_ROOM;

        public bool Success;
        public JoinRoomResponseCode ResponseCode;
        public OperationResponse OperationResponse;
        public SendParameters SendParameters;

        public JoinRoomOpResponse(bool success, JoinRoomResponseCode responseCode)
        {
            Success = success;
            ResponseCode = responseCode;

            OperationResponse = new OperationResponse(JoinRoomOpResponse.OP_CODE, new Dictionary<byte, object>());
            OperationResponse.ReturnCode = OP_CODE;
            OperationResponse.Parameters.Add(0, Success);
            OperationResponse.Parameters.Add(1, (int)ResponseCode);

            SendParameters = new SendParameters();
            SendParameters.ChannelId = 0;
            SendParameters.Encrypted = false;
            SendParameters.Unreliable = false;
        }
    }
}
