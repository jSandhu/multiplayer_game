using Common.Combat.Commands;
using Common.Inventory;
using Common.Inventory.Abilities;
using Common.Inventory.Equipment;
using Common.Player;
using Common.Rarity;
using DIFramework;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Player;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Net.Services.Player
{
    public abstract class PlayerServiceBase
    {
        private CatalogModel catalogModel;

        public void GetPlayerModelByID(PlayerAuthenticationModel playerAuthModel,
            PlayerCombatBehaviourBase playerCombatBehaviourInstance,
            Action<ServerPlayerModel> onSuccessHandler, Action<PlayerAuthenticationModel> onFailureHandler)
        {
            getPlayerModelByID(playerAuthModel, playerCombatBehaviourInstance,
                serverPlayerModel => {
                    try {
                        parseEquipmentAndCombatantModel(serverPlayerModel);
                    } catch (Exception) {
                        Log.Warning("PlayerServerBase unable to parse ServerPlayerModel for player with ID: " + playerAuthModel.UserID);
                        onFailureHandler(playerAuthModel);
                    }
                    serverPlayerModel.PlayerID = playerAuthModel.UserID;
                    serverPlayerModel.Name = playerAuthModel.UserName;
                    onSuccessHandler(serverPlayerModel);
                },
                () => { onFailureHandler(playerAuthModel); });
        }

        private void parseEquipmentAndCombatantModel(ServerPlayerModel serverPlayerModel)
        {
            if (catalogModel == null)
            {
                catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(InstanceServerApplication.CONTEXT_ID);
            }

            // Parse equipment
            parseItemModel<EquipmentItemModel>(ref serverPlayerModel.EquippedWeaponItemModel, ref serverPlayerModel.EquippedWeapon);
            parseItemModel<EquipmentItemModel>(ref serverPlayerModel.EquippedOffHandItemModel, ref serverPlayerModel.EquippedOffHand);
            parseItemModel<EquipmentItemModel>(ref serverPlayerModel.EquippedArmorItemModel, ref serverPlayerModel.EquippedArmor);
            List<EquipmentItemModel> allEquipment = new List<EquipmentItemModel>();
            if (serverPlayerModel.EquippedWeaponItemModel != null) allEquipment.Add(serverPlayerModel.EquippedWeaponItemModel);
            if (serverPlayerModel.EquippedOffHandItemModel != null) allEquipment.Add(serverPlayerModel.EquippedOffHandItemModel);
            if (serverPlayerModel.EquippedArmorItemModel != null) allEquipment.Add(serverPlayerModel.EquippedArmorItemModel);

            // Player must have at least 1 Ability
            AbilityItemModel[] equippedAbilityItemModels = new AbilityItemModel[serverPlayerModel.EquippedAbilities.Length];
            for (int i = 0; i < equippedAbilityItemModels.Length; i++)
            {
                parseItemModel<AbilityItemModel>(ref equippedAbilityItemModels[i], ref serverPlayerModel.EquippedAbilities[i]);
            }

            serverPlayerModel.CombatantModel =  
                BuildCombatantSCMD.Execute(serverPlayerModel.Level_ZeroBased, PlayerModel.DEFAULT_TEAM_ID, equippedAbilityItemModels, 
                RarityType.Common, allEquipment.Count == 0 ? null : allEquipment.ToArray());
        }

        private void parseItemModel<T>(ref T itemModelRef, ref ItemIDLevelPair itemIDLevelPair) where T : ItemModel
        {
            if (itemIDLevelPair.ItemModelID == PlayerModel.EMPTY_EQUIPMENT_ID) return;

            itemModelRef = (catalogModel.GetItemByID(itemIDLevelPair.ItemModelID).Clone() as T);
            itemModelRef.Level_ZeroBased = itemIDLevelPair.ItemLevel_ZeroBased;
        }

        protected abstract void getPlayerModelByID(PlayerAuthenticationModel playerAuthModel, PlayerCombatBehaviourBase playerCombatBehaviourInstance,
            Action<ServerPlayerModel> onSuccessHandler, Action onFailureHandler);
    }
}
