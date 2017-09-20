using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Net
{
    public abstract class SocketConnectionBase: IPhotonPeerListener
    {
        public event Action Connected;
        public event Action<string> ConnectionFailed;
        public event Action<StatusCode> Disconnected;

        public StatusCode Status { get; private set; }
        public bool IsConnected { get; private set; }

        private Dictionary<byte, Action<EventData>> eventListenersByCode = new Dictionary<byte, Action<EventData>>();
        private Dictionary<byte, Action<OperationResponse>> operationListenersByCode = new Dictionary<byte, Action<OperationResponse>>();

        public void AddEventListener(byte eventCode, Action<EventData> listener)
        {
            if (!eventListenersByCode.ContainsKey(eventCode))
            {
                eventListenersByCode.Add(eventCode, null);
            }
            eventListenersByCode[eventCode] += listener;
        }

        public void RemoveEventListener(byte eventCode, Action<EventData> listener)
        {
            if (!eventListenersByCode.ContainsKey(eventCode))
            {
                return;
            }
            eventListenersByCode[eventCode] -= listener;
        }

        public void AddOperationResponseListener(byte operationCode, Action<OperationResponse> handler)
        {
            if (!operationListenersByCode.ContainsKey(operationCode))
            {
                operationListenersByCode.Add(operationCode, null);
            }
            operationListenersByCode[operationCode] += handler;
        }

        public void RemoveOperationResponseListener(byte operationCode, Action<OperationResponse> handler)
        {
            if (!operationListenersByCode.ContainsKey(operationCode))
            {
                return;
            }
            operationListenersByCode[operationCode] -= handler;
        }

        public void OnEvent(EventData eventData)
        {
            Debug.Log("Connection.OnEvent: " + eventData.Code);

            if (!eventListenersByCode.ContainsKey(eventData.Code))
            {
                return;
            }

            if (eventListenersByCode[eventData.Code] != null)
            {
                eventListenersByCode[eventData.Code](eventData);
            }
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            Debug.Log("Connection.OnOperationResponse: " + operationResponse.OperationCode);

            if (!operationListenersByCode.ContainsKey(operationResponse.OperationCode))
            {
                return;
            }

            if (operationListenersByCode[operationResponse.OperationCode] != null)
            {
                operationListenersByCode[operationResponse.OperationCode](operationResponse);
            }
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            Status = statusCode;
            switch (statusCode)
            {
                case StatusCode.Connect:
                    IsConnected = true;
                    if (Connected != null)
                    {
                        Connected();
                    }
                    break;
                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:
                    if (Disconnected != null)
                    {
                        Disconnected(statusCode);
                    }
                    break;
                default:
                    break;
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            if (!IsConnected && level == DebugLevel.ERROR)
            {
                if (ConnectionFailed != null)
                {
                    ConnectionFailed(message);
                }
            }
            else
            {
                Debug.LogWarning("Connection error: " + level + " : " + message);
            }
        }

        public abstract void Connect(string serverAddress, int port, string applicationName);
        public abstract void Disconnect();
        public abstract void SendOperationRequest(OperationRequest operationRequest, bool reliable = true, bool encrypt = false, byte channel = 0);
    }
}
