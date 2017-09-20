using Common.Net.OperationResponses;
using DIFramework;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Net;
using InstanceServer.Core.Player;
using InstanceServer.Core.Room.Game;
using InstanceServer.Core.Room.Game.Controllers;
using InstanceServer.Core.Room.Game.Lobby;
using System;
using System.Collections.Generic;
using System.Timers;

namespace InstanceServer.Core.Room
{
    /// <summary>
    /// Context that handles players joining and leaving a IGameController implementation.
    /// Uses a JoinRoomController to determine when all players are ready to start the game.
    /// </summary>
    public class RoomContext
    {
        private const double TEAR_DOWN_INTERVAL_MS = 10000;

        public event Action<RoomContext> Destroyed;

        public string ID { get; private set; }
        public string Password { get; private set; }

        private bool disposed = false;
        private Timer tearDownTimer = new Timer(TEAR_DOWN_INTERVAL_MS);
        private LobbyController lobbyController;
        private IGameController gameController;
        private List<IClientConnection> connections = new List<IClientConnection>();

        private readonly object _lock = new object();

        public RoomContext(string id, string password)
        {
            this.ID = id;
            this.Password = password;

            tearDownTimer.AutoReset = false;
            tearDownTimer.Elapsed += onTearDownElapsed;

            this.lobbyController = new LobbyController(onLobbyPlayersReady);

            this.gameController = DIContainer.CreateInstanceByContextID<IGameController>(InstanceServerApplication.CONTEXT_ID);
            this.gameController.GameCompleted += onGameCompleted;

            // TODO: allow user to specify difficulty
            this.gameController.SetDifficulty(GameDifficultyType.Normal);
        }

        public JoinRoomResponseCode TryAddClientConnection(PlayerAuthenticationModel playerAuthModel, IClientConnection clientConnection, string roomPassword)
        {
            lock (_lock)
            {
                if (roomPassword != Password)
                {
                    Log.Info("Invalid room password specified");
                    return JoinRoomResponseCode.InvalidRoomPassword;
                }
                if (disposed)
                {
                    Log.Info("Room has expired");
                    return JoinRoomResponseCode.RoomExpired;
                }

                JoinRoomResponseCode responseCode = JoinRoomResponseCode.UnknownFailure;
                if (gameController.IsGameActive)
                {
                    // TODO: rejoin
                    if (!gameController.RejoinPlayer(playerAuthModel, clientConnection))
                    {
                        return responseCode;
                    }
                    responseCode = JoinRoomResponseCode.AddedToExistingGame;
                }
                else
                {
                    // Add to lobby
                    lobbyController.AddPlayerForConnection(playerAuthModel, clientConnection);
                    responseCode = JoinRoomResponseCode.AddedToLobby;
                }

                if (responseCode != JoinRoomResponseCode.AddedToExistingGame && responseCode != JoinRoomResponseCode.AddedToLobby)
                {
                    Log.Info("Failed to join room");
                    return responseCode;
                }

                if (tearDownTimer.Enabled)
                {
                    Log.Info(">>>> Player successfully joined room, disabling tearDownTimer.");
                    tearDownTimer.Enabled = false;
                }

                Log.Info("Player ID: "+ playerAuthModel.UserID +" joined room. State: " + responseCode.ToString());

                connections.Add(clientConnection);
                clientConnection.Disconnected += onClientPeerDisconnected;
                return responseCode;
            }
        }

        private void onLobbyPlayersReady(List<ServerPlayerModel> activePlayers)
        {
            gameController.StartGame(activePlayers);
        }

        /// <summary>
        /// Disconnect all players when game is completed.
        /// </summary>
        private void onGameCompleted()
        {
            gameController.GameCompleted -= onGameCompleted;
            gameController.Destroy();
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].DisconnectAfterDelay();
            }
            disposed = true;
        }

        private void onClientPeerDisconnected(IClientConnection clientConnection)
        {
            lock (_lock)
            {
                clientConnection.Disconnected -= onClientPeerDisconnected;
                connections.Remove(clientConnection);

                // If the room is empty, tear down after delay
                if (connections.Count == 0)
                {
                    Log.Info(">>>> Enabling tearDownTimer.");
                    tearDownTimer.Enabled = true;
                }
            }
        }

        private void onTearDownElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Info("Room with ID: " + ID + " onTearDownElapsed");
            lock (_lock)
            {
                tearDownTimer.Enabled = false;
                tearDownTimer.Dispose();

                disposed = true;

                gameController.Destroy();

                if (Destroyed != null)
                {
                    Destroyed(this);
                }
            }
        }
    }
}
