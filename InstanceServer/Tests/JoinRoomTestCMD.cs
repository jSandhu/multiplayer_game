using DIFramework;
using InstanceServer.Core;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Net.OperationRequests;
using InstanceServer.Core.Room.Game.WorldBuilders;
using InstanceServer.Tests.Mocking.Combat.Behaviours;
using InstanceServer.Tests.Mocking.Net;
using InstanceServer.Tests.Mocking.Room.WorldBuilders;
using Photon.SocketServer;
using System.Threading.Tasks;

namespace InstanceServer.Tests
{
    public class JoinRoomTestCMD
    {
        private InstanceServerApplication instanceServerApplication;
        private MockClientConnection mockClientConnection;

        public JoinRoomTestCMD(InstanceServerApplication instanceServerApplication)
        {
            this.instanceServerApplication = instanceServerApplication;

            // use test world builder instead
            DIContainer.GetByID(InstanceServerApplication.CONTEXT_ID).UnregisterInstance<IWorldBuilder>();
            DIContainer.GetByID(InstanceServerApplication.CONTEXT_ID).RegisterInstance<IWorldBuilder>(new TestWorldBuilder());

            // use mock player beaviour instead
            DIContainer.GetByID(InstanceServerApplication.CONTEXT_ID).UnmapType<PlayerCombatBehaviourBase>();
            DIContainer.GetByID(InstanceServerApplication.CONTEXT_ID).MapType<PlayerCombatBehaviourBase, PlayerCastFirstAbilityMockBehaviour>();
        }

        public void Execute()
        {
            connectAndJoinRoom();
        }

        private void connectAndJoinRoom()
        {

            mockClientConnection = new MockClientConnection();
            mockClientConnection.SendEventBehaviourHandler = sendEventBehaviourHandler;
            mockClientConnection.SendOperationResponseBehaviourHandler = sendOperationResponseBehaviourHandler;

            instanceServerApplication.ListenForClientJoinRoomRequest(mockClientConnection);

            // Step 1. Fake join room request
            Task.Delay(100).ContinueWith(t => {
                Core.Logging.Log.Info("---- Mock client "+mockClientConnection.ConnectionId +" sending JoinRoomOpRequest.");
                mockClientConnection.OnOperationRequest(
                    new JoinRoomOpRequest(Mocking.Net.Services.Session.MockSessionService.VALID_SESSION_ID,
                    "testRoom", "testPassword").ToOperationRequest(), new SendParameters());
            });
        }

        private void sendEventBehaviourHandler(IEventData eventData, SendParameters sendParameters)
        {
            // Step 3. Fake player loaded world
            if (eventData.Code == LoadWorldEvent.EVENT_CODE)
            {
                Task.Delay(100).ContinueWith(t => {
                    Core.Logging.Log.Info("---- Mock client " + mockClientConnection.ConnectionId + " sending PlayerLoadedWorldOpRequest.");
                    mockClientConnection.OnOperationRequest(
                        new PlayerLoadedWorldOpRequest().ToOperationRequest(), new SendParameters());
                });
            }

            if (eventData.Code == PlayersJoinedLobbyEvent.EVENT_CODE)
            {
                // Step 2. Fake lobby ready
                Task.Delay(100).ContinueWith(t => {
                    Core.Logging.Log.Info("---- Mock client " + mockClientConnection.ConnectionId + " sending PlayerReadyOpRequest.");
                    mockClientConnection.OnOperationRequest(
                        new PlayerReadyOpRequest().ToOperationRequest(), new SendParameters());
                });
            }

            if (eventData.Code == ZoneStartedEvent.EVENT_CODE)
            {
                Core.Logging.Log.Info("---- Mock client " + mockClientConnection.ConnectionId + " received ZoneStartedEvent.");

                if (!isFakeRejoinCompleted)
                {
                    Task.Delay(20000).ContinueWith(t =>
                    {
                        Core.Logging.Log.Info("----  ****************  Mock client " + mockClientConnection.ConnectionId + " disconnecting.");
                        mockClientConnection.Disconnect();
                        isFakeRejoinCompleted = true;

                        Task.Delay(5000).ContinueWith(t2 =>
                        {
                            Core.Logging.Log.Info("---- ****************  Mock client " + mockClientConnection.ConnectionId + " reconnecting.");
                            connectAndJoinRoom();
                        });
                    });
                }
            }

            if (eventData.Code == CombatTurnEvent.EVENT_CODE)
            {
                Core.Logging.Log.Info("---- Mock client " + mockClientConnection.ConnectionId + " got CombatTurnEvent.");
            }
        }

        private bool isFakeRejoinCompleted;

        private void sendOperationResponseBehaviourHandler(OperationResponse operationResponse, SendParameters sendParameters)
        {

        }
    }
}
