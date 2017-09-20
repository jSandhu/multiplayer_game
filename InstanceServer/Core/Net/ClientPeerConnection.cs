using System;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using InstanceServer.Core.Logging;
using System.Threading.Tasks;

namespace InstanceServer.Core.Net
{
    public class ClientPeerConnection : ClientPeer, IClientConnection
    {
        private const int DELAYED_DISCONNECT_TIME_MS = 4000;

        public event Action<IClientConnection> Disconnected;

        private Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>> operationReqListenersByCode = 
            new Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>>();

        private Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>> operationReqOnceListenersByCode =
            new Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>>();

        public ClientPeerConnection(InitRequest initRequest) : base(initRequest)
        {
            Log.Info("Connection received : " + initRequest.RemoteIP);
        }

        public void DisconnectAfterDelay()
        {
            Log.Info("Disconnecting client after " + DELAYED_DISCONNECT_TIME_MS + "MS");
            Task.Delay(DELAYED_DISCONNECT_TIME_MS).ContinueWith(t => {if (Connected) { Disconnect();}});
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info("Client Disconnected");
            if (Disconnected != null)
            {
                Disconnected(this);
            }

            Disconnected = null;

            foreach (var kvp in operationReqListenersByCode)
            {
                operationReqListenersByCode[kvp.Key] = null;
            }
            operationReqListenersByCode.Clear();

            foreach (var kvp2 in operationReqOnceListenersByCode)
            {
                operationReqOnceListenersByCode[kvp2.Key] = null;
            }
            operationReqOnceListenersByCode.Clear();
        }

        public void AddOperationRequestListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler)
        {
            if (!operationReqListenersByCode.ContainsKey(operationCode))
            {
                operationReqListenersByCode.Add(operationCode, null);
            }
            operationReqListenersByCode[operationCode] += handler;
        }

        public void AddOperationRequestOnceListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler)
        {
            if (!operationReqOnceListenersByCode.ContainsKey(operationCode))
            {
                operationReqOnceListenersByCode.Add(operationCode, null);
            }
            operationReqOnceListenersByCode[operationCode] += handler;
        }

        public void RemoveOperationRequestListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler)
        {
            if (!operationReqListenersByCode.ContainsKey(operationCode))
            {
                return;
            }
            operationReqListenersByCode[operationCode] -= handler;
        }

        public void RemoveOperationRequestOnceListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler)
        {
            if (!operationReqOnceListenersByCode.ContainsKey(operationCode))
            {
                return;
            }
            operationReqOnceListenersByCode[operationCode] -= handler;
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (operationReqListenersByCode.ContainsKey(operationRequest.OperationCode) &&
                operationReqListenersByCode[operationRequest.OperationCode] != null)
            {
                operationReqListenersByCode[operationRequest.OperationCode](operationRequest, this, sendParameters);
            }

            if (operationReqOnceListenersByCode.ContainsKey(operationRequest.OperationCode) &&
                operationReqOnceListenersByCode[operationRequest.OperationCode] != null)
            {
                operationReqOnceListenersByCode[operationRequest.OperationCode](operationRequest, this, sendParameters);
                operationReqOnceListenersByCode[operationRequest.OperationCode] = null;
            }
        }
    }
}
