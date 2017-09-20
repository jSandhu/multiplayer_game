using InstanceServer.Core.Logging;
using InstanceServer.Core.Net;
using Photon.SocketServer;

namespace InstanceServer.Core.Commands
{
    /// <summary>
    /// Creates a ClientPeerConnection.  Logs warning if server setup isn't completed.
    /// </summary>
    public static class CreateClientPeerConnectionSCMD
    {
        public static ClientPeerConnection Execute(InitRequest initRequest, InstanceServerApplication instanceServerApplication)
        {
            ClientPeerConnection clientPeerConnection = new ClientPeerConnection(initRequest);
            Log.Info("InstanceServer.CreatePeer: ClientPeerConnection created.");

            if (!instanceServerApplication.IsSetupCompleted)
            {
                Log.Warning("InstanceServer.CreatePeer: Instance Setup not complete. Diconnecting client.");
                clientPeerConnection.Disconnect();
            }

            return clientPeerConnection;
        }
    }
}
