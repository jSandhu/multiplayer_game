using System;
using Photon.SocketServer;
using System.Collections.Generic;
using InstanceServer.Core.Logging;
using System.Threading.Tasks;
using InstanceServer.Core.Net;

namespace InstanceServer.Tests.Mocking.Net
{
    public class MockClientConnection : IClientConnection
    {
        private static int CONNECTION_ID_INDEX = 0;
        private const int DELAYED_DISCONNECT_TIME_MS = 4000;

        public event Action<IClientConnection> Disconnected;

        public Action<IEventData, SendParameters> SendEventBehaviourHandler;
        public Action<OperationResponse, SendParameters> SendOperationResponseBehaviourHandler;

        public bool Connected { get; private set; }
        public int ConnectionId { get; private set; }

        private Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>> operationReqListenersByCode =
            new Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>>();
        private Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>> operationReqOnceListenersByCode =
            new Dictionary<byte, Action<OperationRequest, IClientConnection, SendParameters>>();

        public MockClientConnection()
        {
            ConnectionId = CONNECTION_ID_INDEX++;
            Connected = true;
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

        public void Disconnect()
        {
            Log.Info("****************  MOCK Client Disconnected");
            Connected = false;
            
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }

        public void DisconnectAfterDelay()
        {
            Log.Info("**************** Disconnecting MOCK client after " + DELAYED_DISCONNECT_TIME_MS + "MS");
            Task.Delay(DELAYED_DISCONNECT_TIME_MS).ContinueWith(t => { if (Connected) { Disconnect(); } });
        }

        public SendResult SendEvent(IEventData eventData, SendParameters sendParameters)
        {
            Log.Info("SENDING EVENT TO MOCK CLIENT: ConnectionID: " +ConnectionId + " : event: "  + eventData.Code);
            if (!Connected)
            {
                Log.Info("**************** CLIENT IS DISCONNECTED!");
                return SendResult.Disconnected;
            }
            Log.Info("CLIENT HANDLING EVENT");
            SendEventBehaviourHandler(eventData, sendParameters);
            return SendResult.Ok;
        }

        public SendResult SendOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            Log.Info("SENDING OP RESPONSE TO MOCK CLIENT: " + operationResponse.OperationCode);
            if (!Connected)
            {
                return SendResult.Disconnected;
            }
            SendOperationResponseBehaviourHandler(operationResponse, sendParameters);
            return SendResult.Ok;
        }

        public void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
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
