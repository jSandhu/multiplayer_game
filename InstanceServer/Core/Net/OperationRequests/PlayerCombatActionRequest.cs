using Common.Net.OperationRequests;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.OperationRequests
{
    public class PlayerCombatActionRequest
    {
        public static readonly byte OP_CODE = OperationCodes.PLAYER_COMBAT_ACTION;

        public int AbilityID;
        public int[] TargetCombatantIDs;

        public PlayerCombatActionRequest(int AbilityID, int[] TargetCombatantIDs)
        {
            this.AbilityID = AbilityID;
            this.TargetCombatantIDs = TargetCombatantIDs;
        }

        public static PlayerCombatActionRequest FromOperationRequest(OperationRequest operationRequest)
        {
            if (operationRequest.OperationCode != OperationCodes.PLAYER_COMBAT_ACTION)
            {
                throw new Exception("Invalid code for generating PlayerCombatActionRequest: " + operationRequest.OperationCode);
            }

            return new PlayerCombatActionRequest((int)operationRequest.Parameters[0],
                                                  operationRequest.Parameters[1] as int[]);
        }

        public OperationRequest ToOperationRequest()
        {
            return new OperationRequest(OP_CODE, new Dictionary<byte, object>()
            {
                {0, AbilityID },
                {1, TargetCombatantIDs }
            });
        }
    }
}
