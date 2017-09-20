using DIFramework;
using InstanceServer.Core.Commands;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Net;
using InstanceServer.Core.Room;
using Photon.SocketServer;

namespace InstanceServer.Core
{
    public class InstanceServerApplication : ApplicationBase
    {
        public const int CONTEXT_ID = 0;

        public bool IsSetupCompleted {get; private set;}

        protected override void Setup()
        {
            System.Diagnostics.Debugger.Launch();

            Log.SetupLogger(ApplicationRootPath, ApplicationName, BinaryPath);

            Log.InfoHeader("Setup " + ApplicationName);
            Log.Info("BinaryPath: " + BinaryPath);

            new MapContextDependenciesCMD<PlayFabNetExtensions.Net.PlayFabServiceMappings>(onContextSetupCompleted).Execute();
        }

        private void onContextSetupCompleted()
        {
            Log.InfoFooter("Setup Completed");
            IsSetupCompleted = true;

            // Run tests
            new Tests.JoinRoomTestCMD(this).Execute();
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Log.Info("CONNECTION ATTEMPT");
            ClientPeerConnection clientPeerConnection = CreateClientPeerConnectionSCMD.Execute(initRequest, this);
            ListenForClientJoinRoomRequest(clientPeerConnection);
            return clientPeerConnection;
        }

        public void ListenForClientJoinRoomRequest(IClientConnection clientConnection)
        {
            // If connection is valid (Connected == true), then register with JoinRoomController
            if (clientConnection.Connected)
            {
                JoinRoomController jrc = DIContainer.GetInstanceByContextID<JoinRoomController>(InstanceServerApplication.CONTEXT_ID);
                jrc.AddClientConnection(clientConnection);
            }
        }

        protected override void TearDown()
        {
            Log.InfoHeader("Tear Down");
            Log.InfoFooter("Tear Down Completed");
        }
    }
}
