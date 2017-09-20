using Common.Net.OperationRequests;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.OperationRequests
{
    public class JoinRoomOpRequest
    {
        public static readonly byte OP_CODE = OperationCodes.JOIN_ROOM;

        public string SessionID;
        public string RoomID;
        public string Password;

        public JoinRoomOpRequest(string sessionID, string roomID, string roomPassword)
        {
            SessionID = sessionID;
            RoomID = roomID;
            Password = roomPassword;
        }

        public static JoinRoomOpRequest FromOperationRequest(OperationRequest operationRequest)
        {
            if (operationRequest.OperationCode != OperationCodes.JOIN_ROOM)
            {
                throw new Exception("Invalid code for generating JoinRoomOpRequest: " + operationRequest.OperationCode);
            }

            return new JoinRoomOpRequest(   operationRequest.Parameters[0] as string, 
                                            operationRequest.Parameters[1] as string, 
                                            operationRequest.Parameters[2] as string);
        }

        public OperationRequest ToOperationRequest()
        {
            return new OperationRequest(OP_CODE, new Dictionary<byte, object>()
            {
                {0, SessionID },
                {1, RoomID },
                {2, Password }
            });
        }
    }
}
