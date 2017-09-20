using Common.Net.OperationRequests;
using Photon.SocketServer;
using System;

namespace InstanceServer.Core.Net.OperationRequests
{
    public class PlayerUnreadyOpRequest
    {
        public static readonly byte OP_CODE = OperationCodes.PLAYER_UNREADY;

        public static PlayerUnreadyOpRequest FromOperationRequest(OperationRequest operationRequest)
        {
            if (operationRequest.OperationCode != OperationCodes.PLAYER_UNREADY)
            {
                throw new Exception("Invalid code for generating PlayerUnreadyOpRequest: " + operationRequest.OperationCode);
            }

            return new PlayerUnreadyOpRequest();
        }
    }
}
