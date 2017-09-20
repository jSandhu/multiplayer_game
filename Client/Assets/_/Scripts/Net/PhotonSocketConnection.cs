using ExitGames.Client.Photon;
using UpdateDispatch;
using Scripts.Net;

namespace DcClient.Net.SocketServices
{
    public class PhotonSocketConnection : SocketConnectionBase
    {
        private PhotonPeer photonPeer;
        private UpdateDispatcher updateDispatcher;

        public PhotonSocketConnection(ConnectionProtocol connectionProtocol, UpdateDispatcher updateDispatcher)
        {
            photonPeer = new PhotonPeer(this, connectionProtocol);

            this.updateDispatcher = updateDispatcher;
            this.updateDispatcher.Updated += onUpdated;
        }

        public override void Connect(string serverAddress, int port, string applicationName)
        {
            photonPeer.Connect(serverAddress+":"+port, applicationName);
        }

        public override void Disconnect()
        {
            photonPeer.Disconnect();
        }

        public override void SendOperationRequest(OperationRequest operationRequest, bool reliable = true, bool encrypt = false, byte channel = 0)
        {
            photonPeer.OpCustom(operationRequest, reliable, channel, encrypt);
        }

        private void onUpdated()
        {
            photonPeer.Service();
        }
    }
}
