using Photon.SocketServer;
using System;

namespace InstanceServer.Core.Net
{
    /// <summary>
    /// Methods required for interfacing with a network connection.  Allows for mock implementations
    /// for texting purposes.
    /// </summary>
    public interface IClientConnection
    {
        event Action<IClientConnection> Disconnected;
        bool Connected { get; }
        int ConnectionId { get; }
        void Disconnect();
        void DisconnectAfterDelay();
        SendResult SendEvent(IEventData eventData, SendParameters sendParameters);
        SendResult SendOperationResponse(OperationResponse operationResponse, SendParameters sendParameters);
        void AddOperationRequestListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler);
        void AddOperationRequestOnceListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler);
        void RemoveOperationRequestListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler);
        void RemoveOperationRequestOnceListener(byte operationCode, Action<OperationRequest, IClientConnection, SendParameters> handler);
    }
}
