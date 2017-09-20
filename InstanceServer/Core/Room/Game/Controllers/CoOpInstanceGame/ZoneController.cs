using Common.Combat.Actions;
using Common.Inventory.Abilities;
using Common.Net.OperationRequests;
using Common.World.Zone;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Net;
using InstanceServer.Core.Net.OperationRequests;
using InstanceServer.Core.Npcs;
using InstanceServer.Core.Player;
using InstanceServer.Core.Room.Game.Commands;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Controllers.CoOpInstanceGame
{
    public class ZoneController
    {
        private const int ZONE_START_DELAY_TURNS = 2;

        public event Action<ZoneController> Completed;

        public ZoneModel ZoneModel;
        public List<ServerPlayerModel> AllPlayersinZone = new List<ServerPlayerModel>();

        private CoOpInstanceCombatController combatController;
        private ServerNpcModel[] currentZoneNPCs;
        private Dictionary<IClientConnection, ServerPlayerModel> clientConnectionToServerPlayerModel = new Dictionary<IClientConnection, ServerPlayerModel>();
        private bool isDestroyed;

        public ZoneController(ZoneModel zoneModel)
        {
            this.ZoneModel = zoneModel;

            this.combatController = new CoOpInstanceCombatController();
            this.combatController.CombatTurnCompleted = onCombatTurnCompleted;
        }

        public void Destroy()
        {
            if (isDestroyed)
            {
                return;
            }
            isDestroyed = true;

            for (int i = 0; i < AllPlayersinZone.Count; i++)
            {
                removePlayerReferences(AllPlayersinZone[i]);
            }
            combatController.StopAndClear();
            combatController = null;
            clientConnectionToServerPlayerModel = null;
            AllPlayersinZone = null;
            Completed = null;
            ZoneModel = null;
        }

        public void AddPlayers(List<ServerPlayerModel> newlyJoinedServerPlayerModels)
        {
            Log.Info("Adding " + newlyJoinedServerPlayerModels.Count + " players to zone " + ZoneModel.ID);
            for (int i = 0; i < newlyJoinedServerPlayerModels.Count; i++)
            {
                AllPlayersinZone.Add(newlyJoinedServerPlayerModels[i]);
                clientConnectionToServerPlayerModel.Add(newlyJoinedServerPlayerModels[i].ClientConnection, newlyJoinedServerPlayerModels[i]);
                newlyJoinedServerPlayerModels[i].Disconnected += onPlayerDisconnected;
            }

            if (!combatController.IsRunning)
            {
                // If the zone hasn't started yet, immediately send ZoneStarted events.
                SendZoneStartedEventsSCMD.Execute(
                    newlyJoinedServerPlayerModels, ZoneModel, combatController.TurnDispatcher.TurnNumber);
            }
            else
            {
                // Otherwise wait until combatController has completed its current turn.  This will ensure that 
                // the combatantModels sent with the event are up to date.
                Action sendZoneStartedOnPostUpdateAction = null;
                sendZoneStartedOnPostUpdateAction = () =>
                {
                    SendZoneStartedEventsSCMD.Execute(
                        newlyJoinedServerPlayerModels, ZoneModel, combatController.TurnDispatcher.TurnNumber);
                    combatController.TurnDispatcher.PostUpdateStarted -= sendZoneStartedOnPostUpdateAction;
                };
                combatController.TurnDispatcher.PostUpdateStarted += sendZoneStartedOnPostUpdateAction;
            }

            // Inform existing players that the new players have joined the zone
            SendPlayersJoinedZoneSCMD.Execute(newlyJoinedServerPlayerModels, AllPlayersinZone);

            for (int i = 0; i < newlyJoinedServerPlayerModels.Count; i++)
            {
                ServerPlayerModel newPlayer = newlyJoinedServerPlayerModels[i];
                combatController.AddCombatantWithBehaviour(newPlayer.CombatantModel, newPlayer.PlayerCombatBehaviour);

                // Start listening to PlayerCombatActionRequests
                newPlayer.ClientConnection.AddOperationRequestListener(OperationCodes.PLAYER_COMBAT_ACTION, onPlayerCombatAction);
            }

            // If a zone isn't active, start it
            if (!combatController.IsRunning)
            {
                // Add npc CombatantModels to combatController
                currentZoneNPCs = new ServerNpcModel[ZoneModel.EnemyNpcModels.Length];
                for (int n = 0; n < ZoneModel.EnemyNpcModels.Length; n++)
                {
                    ServerNpcModel serverNpcModel = ZoneModel.EnemyNpcModels[n] as ServerNpcModel;
                    currentZoneNPCs[n] = serverNpcModel;
                    combatController.AddCombatantWithBehaviour(serverNpcModel.CombatantModel, serverNpcModel.CombatBehaviour);
                }

                Log.InfoHeader("Starting Zone " + ZoneModel.ID);
                combatController.Start(ZONE_START_DELAY_TURNS);
            }
        }

        private void onPlayerCombatAction(OperationRequest operationRequest, IClientConnection clientConnection, SendParameters sendParameters)
        {
            PlayerCombatActionRequest playerCombatActionRequest = PlayerCombatActionRequest.FromOperationRequest(operationRequest);

            ServerPlayerModel serverPlayerModel = clientConnectionToServerPlayerModel[clientConnection];
            AbilityItemModel abilityItemModel = serverPlayerModel.CombatantModel.GetAbilityByID(playerCombatActionRequest.AbilityID);

            clientConnectionToServerPlayerModel[clientConnection].PlayerCombatBehaviour.QueueAbility(
                abilityItemModel, playerCombatActionRequest.TargetCombatantIDs);
        }

        private void onCombatTurnCompleted(CombatActionsCollectionModel combatActionsCollectionModel)
        {
            SendCombatTurnEventsSCMD.Execute(AllPlayersinZone, combatActionsCollectionModel);

            // Determine if zone is completed (All NPCs dead).
            bool zoneCompleted = true;
            for (int i = 0; i < ZoneModel.EnemyNpcModels.Length; i++)
            {
                if (ZoneModel.EnemyNpcModels[i].CombatantModel.IsAlive())
                {
                    zoneCompleted = false;
                    break;
                }
            }
            if (zoneCompleted)
            {
                Log.InfoFooter("Zone " + ZoneModel.ID + " Completed.");
                combatController.StopAndClear();
                ZoneModel.IsCompleted = true;
                new GrantZoneCompletionRewardsCMD(AllPlayersinZone, currentZoneNPCs, ZoneModel.IsFinalZone, onZoneRewardsGranted).Execute();
                return;
            }

            // Determine if zone was failed (All players dead).
            bool zoneFailed = true;
            for (int j = 0; j < AllPlayersinZone.Count; j++)
            {
                if (AllPlayersinZone[j].CombatantModel.IsAlive())
                {
                    zoneFailed = false;
                    break;
                }
            }
            if (zoneFailed)
            {
                Log.InfoFooter("Zone " + ZoneModel.ID + " Failed.");
                combatController.StopAndClear();
                handleZoneFailure();
                return;
            }
        }

        private void onZoneRewardsGranted()
        {
            Completed(this);
        }

        private void handleZoneFailure()
        {
            for (int i = 0; i < AllPlayersinZone.Count; i++)
            {
                // TODO: Send failure event to player;

                // Remove player references and disconnect after delay.
                IClientConnection connection = AllPlayersinZone[i].ClientConnection;
                removePlayerReferences(AllPlayersinZone[i]);
                connection.DisconnectAfterDelay();
            }
        }

        private void removePlayerReferences(ServerPlayerModel serverPlayerModel)
        {
            serverPlayerModel.Disconnected -= onPlayerDisconnected;
            clientConnectionToServerPlayerModel.Remove(serverPlayerModel.ClientConnection);
            AllPlayersinZone.Remove(serverPlayerModel);

            // Stop listening to PlayerCombatActionRequests
            serverPlayerModel.ClientConnection.RemoveOperationRequestListener(OperationCodes.PLAYER_COMBAT_ACTION, onPlayerCombatAction);
        }

        private void onPlayerDisconnected(ServerPlayerModel serverPlayerModel)
        {
            Log.Info(serverPlayerModel.PlayerID + " ZoneController.onPlayerDisconnected");
            removePlayerReferences(serverPlayerModel);

            // Only notify other players of disconnection if game isn't over
            if (!ZoneModel.IsCompleted)
            {
                SendPlayerDisconnectedEventSCMD.Execute(serverPlayerModel.PlayerID, AllPlayersinZone);
            }
        }
    }
}
