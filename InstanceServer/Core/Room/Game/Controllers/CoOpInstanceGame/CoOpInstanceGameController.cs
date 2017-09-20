using InstanceServer.Core.Net;
using System.Collections.Generic;
using InstanceServer.Core.Player;
using InstanceServer.Core.Room.Game.Commands;
using Common.World;
using System;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Room.Game.WorldBuilders;
using DIFramework;

namespace InstanceServer.Core.Room.Game.Controllers.CoOpInstanceGame
{
    public class CoOpInstanceGameController : IGameController
    {
        private const int NPC_TEAM_ID = -1;

        public event Action GameCompleted;

        public int MaxPlayers { get { return 4; } }
        public bool IsGameActive { get; private set; }

        private IWorldBuilder worldBuilder;
        private GameDifficultyType gameDifficultyType = GameDifficultyType.Normal;
        private List<ServerPlayerModel> allActiveServerPlayerModels = new List<ServerPlayerModel>();
        private Dictionary<string, ServerPlayerModel> originalPlayerIDToModel = new Dictionary<string, ServerPlayerModel>();
        private WorldModel worldModel;
        private ZoneController[] zoneControllers;
        private bool isGameCompleted;

        public CoOpInstanceGameController()
        {
            this.worldBuilder = DIContainer.GetInstanceByContextID<IWorldBuilder>(InstanceServerApplication.CONTEXT_ID);
        }

        public void SetDifficulty(GameDifficultyType gameDifficultyType)
        {
            this.gameDifficultyType = gameDifficultyType;
        }

        public void Destroy()
        {
            GameCompleted = null;
            isGameCompleted = true;
            for (int i = 0; i < zoneControllers.Length; i++)
            {
                zoneControllers[i].Destroy();
            }
        }

        public void StartGame(List<ServerPlayerModel> initialServerPlayerModels)
        {
            Log.InfoHeader("Starting Game");

            // Build world and send load events.
            worldModel = worldBuilder.Build(initialServerPlayerModels, gameDifficultyType, NPC_TEAM_ID);
            zoneControllers = new ZoneController[worldModel.ZoneModels.Length];
            for (int i = 0; i < zoneControllers.Length; i++)
            {
                zoneControllers[i] = new ZoneController(worldModel.ZoneModels[i]);
                zoneControllers[i].Completed += onZoneCompleted;
            }

            Log.Info("Built World: " + worldModel.ToString());

            new SendLoadWorldEventsCMD(worldModel, initialServerPlayerModels, completedPlayers => {   
                // Players who are able to finish loading the world are considered original players and can re-join
                for (int i = 0; i < initialServerPlayerModels.Count; i++)
                {
                    originalPlayerIDToModel.Add(initialServerPlayerModels[i].PlayerID, initialServerPlayerModels[i]);
                }
                onPlayersLoadedWorld(completedPlayers);
            }).Execute();
        }

        public bool RejoinPlayer(PlayerAuthenticationModel playerAuthModel, IClientConnection clientConnection)
        {
            if (isGameCompleted)
            {
                Log.Info("Can't rejoin player, game is completed.");
                return false;
            }

            ServerPlayerModel originalServerPlayerModel;
            originalPlayerIDToModel.TryGetValue(playerAuthModel.UserID, out originalServerPlayerModel);
            if (originalServerPlayerModel == null)
            {
                return false;
            }

            // If the player is still connected through a previous connection, disconnect it.
            if (originalServerPlayerModel.ClientConnection.Connected)
            {
                originalServerPlayerModel.ClientConnection.Disconnect();
            }

            originalServerPlayerModel.IsRejoining = true;
            originalServerPlayerModel.ClientConnection = clientConnection; // Replace stale connection

            Log.Info("Sending rejoining player SendLoadWorldEventsCMD");
            new SendLoadWorldEventsCMD(worldModel, new List<ServerPlayerModel>(new ServerPlayerModel[] { originalServerPlayerModel}), 
                onPlayersLoadedWorld).Execute();

            return true;
        }

        private void onPlayersLoadedWorld(List<ServerPlayerModel> completedPlayers)
        {
            if (completedPlayers.Count == 0)
            {
                Log.Info("No Players finished loading world, aborting.");

                // If no one successfully loaded the world and the game hadn't started, consider game completed.
                if (!IsGameActive && GameCompleted != null) GameCompleted();

                // Also, if the game is active and a newly loading player failed, do nothing.
                return;
            }

            Log.Info("Players loaded world.");

            IsGameActive = true;

            for (int i = 0; i < completedPlayers.Count; i++)
            {
                // If the player is re-joining, it should already exist in allActiveServerPlayerModels
                if (!completedPlayers[i].IsRejoining)
                {
                    allActiveServerPlayerModels.Add(completedPlayers[i]);
                }
                else
                {
                    Log.Info("Player is rejoining, not adding to allActiveServerPlayerModels");
                }
            }

            // Notify all players that a new bunch has joined.
            // - (existing players notified of new only) 
            // - (new players notified of all existing including themselves)
            SendPlayersJoinedWorldEventsSCMD.Execute(allActiveServerPlayerModels, completedPlayers);

            addPlayersToNextZone(completedPlayers);
        }

        private void addPlayersToNextZone(List<ServerPlayerModel> serverPlayerModels)
        {
            // TODO: determine zone. For now, first incomplete
            ZoneController zoneController = null;
            for (int z = 0; z < zoneControllers.Length; z++)
            {
                if (!worldModel.ZoneModels[z].IsCompleted)
                {
                    zoneController = zoneControllers[z];
                    break;
                }
            }

            if (zoneController == null) throw new Exception("Couldn't determine a starting zone.");

            zoneController.AddPlayers(serverPlayerModels);
        }

        private void onZoneCompleted(ZoneController zoneController)
        {
            if (zoneController.ZoneModel.IsFinalZone)
            {
                Log.Info("World completed!");
                handleGameCompleted();
                return;
            }

            addPlayersToNextZone(zoneController.AllPlayersinZone);
            zoneController.Destroy();
        }

        private void handleGameCompleted()
        {
            isGameCompleted = true;
            SendGameCompletedEventsSCMD.Execute(allActiveServerPlayerModels);
            if (GameCompleted != null) GameCompleted();
        }
    }
}