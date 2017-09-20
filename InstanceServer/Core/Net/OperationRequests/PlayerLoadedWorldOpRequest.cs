using Common.Net.OperationRequests;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.OperationRequests
{
    public class PlayerLoadedWorldOpRequest
    {
        public static readonly byte OP_CODE = OperationCodes.PLAYER_LOADED_WORLD;

        public OperationRequest ToOperationRequest()
        {
            return new OperationRequest(OP_CODE, new Dictionary<byte, object>());
        }

        public static PlayerLoadedWorldOpRequest FromOperationRequest(OperationRequest operationRequest)
        {
            if (operationRequest.OperationCode != OperationCodes.PLAYER_LOADED_WORLD)
            {
                throw new Exception("Invalid code for generating PlayerReadyOpRequest: " + operationRequest.OperationCode);
            }

            return new PlayerLoadedWorldOpRequest();
        }
    }
}
