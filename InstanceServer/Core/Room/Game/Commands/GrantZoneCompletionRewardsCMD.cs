using Common.Inventory;
using Common.NumberUtils;
using DIFramework;
using InstanceServer.Core.Net.Events;
using InstanceServer.Core.Net.Services.Inventory;
using InstanceServer.Core.Npcs;
using InstanceServer.Core.Player;
using System.Collections.Generic;
using System;
using Common.Inventory.Currency;

namespace InstanceServer.Core.Room.Game.Commands
{
    public class GrantZoneCompletionRewardsCMD
    {
        private List<ServerPlayerModel> eventReceivers;
        private ServerNpcModel[] serverNPCModels;
        private bool isFinalZone;
        private Action onCompleteHandler;
        private InventoryServiceBase inventoryService;
        private CatalogModel catalogModel;
        private int numPlayersToReward = 0;
        private ZoneCompletedEvent emptyZoneCompletedEvent;
        private Dictionary<string, Bundle> playerIDToBundle = new Dictionary<string, Bundle>();

        public GrantZoneCompletionRewardsCMD(List<ServerPlayerModel> eventReceivers, ServerNpcModel[] serverNPCModels, 
            bool isFinalZone, Action onCompleteHandler)
        {
            this.eventReceivers = eventReceivers;
            this.serverNPCModels = serverNPCModels;
            this.isFinalZone = isFinalZone;
            this.onCompleteHandler = onCompleteHandler;

            inventoryService = DIContainer.GetInstanceByContextID<InventoryServiceBase>(InstanceServerApplication.CONTEXT_ID);
            catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(InstanceServerApplication.CONTEXT_ID);
        }

        public void Execute()
        {
            // If all players disconnected, don't grant rewards and immediately call onCompleteHandler
            if (eventReceivers == null || eventReceivers.Count == 0)
            {
                onCompleteHandler();
                return;
            }

            numPlayersToReward = eventReceivers.Count;

            for (int i = 0; i < eventReceivers.Count; i++)
            {
                Bundle bundle = buildBundleFromNPCs(serverNPCModels);
                playerIDToBundle.Add(eventReceivers[i].PlayerID, bundle);
                inventoryService.GrantBundleToPlayer(eventReceivers[i].PlayerID, bundle,
                    catalogModel.ID, onGrantBundleSucceeded, onGrantBundleFailed);
            }
        }

        private ServerPlayerModel getServerPlayerAndUpdateRewardCount(string playerID)
        {
            numPlayersToReward--;
            for (int i = 0; i < eventReceivers.Count; i++)
            {
                if (eventReceivers[i].PlayerID == playerID)
                {
                    return eventReceivers[i];
                }
            }
            return null;
        }

        private void onGrantBundleFailed(string playerID)
        {
            if (emptyZoneCompletedEvent == null)
            {
                emptyZoneCompletedEvent = new ZoneCompletedEvent(new Bundle(), isFinalZone);
            }

            ServerPlayerModel serverPlayerModel = getServerPlayerAndUpdateRewardCount(playerID);
            if (serverPlayerModel.ClientConnection.Connected)
            {
                serverPlayerModel.ClientConnection.SendEvent(emptyZoneCompletedEvent.EventData, emptyZoneCompletedEvent.SendParameters);
            }
            checkCompletion();
        }

        private void onGrantBundleSucceeded(string playerID)
        {
            ServerPlayerModel serverPlayerModel = getServerPlayerAndUpdateRewardCount(playerID);
            ZoneCompletedEvent zoneCompletedEvent = new ZoneCompletedEvent(playerIDToBundle[playerID], isFinalZone);
            if (serverPlayerModel.ClientConnection.Connected)
            {
                serverPlayerModel.ClientConnection.SendEvent(zoneCompletedEvent.EventData, zoneCompletedEvent.SendParameters);
            }
            checkCompletion();
        }

        private void checkCompletion()
        {
            if (numPlayersToReward == 0)
            {
                onCompleteHandler();
            }
        }
        
        private Bundle buildBundleFromNPCs(ServerNpcModel[] serverNPCModels)
        {
            ItemModel[] rewardItemModels = new ItemModel[serverNPCModels.Length];
            for (int n = 0; n < serverNPCModels.Length; n++)
            {
                rewardItemModels[n] = serverNPCModels[n].DropTableModel.GetRandomItem(RandomGen.NextNormalizedFloat());
            }

            // TODO: for now award 1 gold and xp on zone completion and multiply by 2 if final zone.
            int goldAmount = 1;
            int xpAmount = 1;
            if (isFinalZone)
            {
                goldAmount *= 2;
                xpAmount *= 2;
            }
            CurrencySymbol[] currencySymbols = new CurrencySymbol[] { CurrencySymbol.GC, CurrencySymbol.XP };
            int[] currencyValues = new int[] { goldAmount, xpAmount };

            return new Bundle(rewardItemModels, currencySymbols, currencyValues);
        }
    }
}
