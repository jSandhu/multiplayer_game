using System;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Player;
using PlayFab.ServerModels;
using PlayFab;
using System.Collections.Generic;
using Common.Player;
using InstanceServer.Core.Logging;
using InstanceServer.Core.Combat.Behaviours;

namespace InstanceServer.PlayFabNetExtensions.Net.Services.Player
{
    public class PlayFabPlayerService : PlayerServiceBase
    {
        private const string WEAPON_ID_AND_LEVEL_KEY = "WeaponIDAndLevel";
        private const string OFFHAND_ID_AND_LEVEL_KEY = "OffHandIDAndLevel";
        private const string ARMOR_ID_AND_LEVEL_KEY = "ArmoreIDAndLevel";
        private const string ZERO_BASED_LEVEL_KEY = "Level_ZeroBased";
        private const string ABILITY_ID_AND_LEVELS_KEY = "AbilityIDAndLevels";
        private static readonly List<string> USER_DATA_KEYS = new List<string>(
            new string[] { WEAPON_ID_AND_LEVEL_KEY, OFFHAND_ID_AND_LEVEL_KEY, ARMOR_ID_AND_LEVEL_KEY, ZERO_BASED_LEVEL_KEY, ABILITY_ID_AND_LEVELS_KEY});

        protected override void getPlayerModelByID(PlayerAuthenticationModel playerAuthModel, PlayerCombatBehaviourBase playerCombatBehaviourInstance, 
            Action<ServerPlayerModel> onSuccessHandler, Action onFailureHandler)
        {
            GetUserDataRequest request = new GetUserDataRequest();
            request.PlayFabId = playerAuthModel.UserID;
            request.Keys = USER_DATA_KEYS;

            PlayFabServerAPI.GetUserDataAsync(request).ContinueWith(t => {
                if (t.Result.Error != null) { onFailureHandler(); }
                else
                {
                    try
                    {
                        ServerPlayerModel serverPlayerModel = parseServerPlayerModelFromData(t.Result.Result.Data, playerCombatBehaviourInstance);
                        onSuccessHandler(serverPlayerModel);
                    }
                    catch(Exception e)
                    {
                        Log.Warning("PlayFabPlayerService unable to parse player with ID: " + playerAuthModel.UserID + " " + e.Message);
                        onFailureHandler();
                    }
                }
            });
        }

        private ServerPlayerModel parseServerPlayerModelFromData(Dictionary<string, UserDataRecord> data, PlayerCombatBehaviourBase playerCombatBehaviourInstance)
        {
            ServerPlayerModel serverPlayerModel = new ServerPlayerModel(playerCombatBehaviourInstance);

            serverPlayerModel.EquippedWeapon = data.ContainsKey(WEAPON_ID_AND_LEVEL_KEY) ? 
                ItemIDLevelPair.Parse(data[WEAPON_ID_AND_LEVEL_KEY].Value) : new ItemIDLevelPair(PlayerModel.EMPTY_EQUIPMENT_ID, 0);

            serverPlayerModel.EquippedOffHand = data.ContainsKey(OFFHAND_ID_AND_LEVEL_KEY) ? 
                ItemIDLevelPair.Parse(data[OFFHAND_ID_AND_LEVEL_KEY].Value) : new ItemIDLevelPair(PlayerModel.EMPTY_EQUIPMENT_ID, 0);

            serverPlayerModel.EquippedArmor = data.ContainsKey(ARMOR_ID_AND_LEVEL_KEY) ?
                ItemIDLevelPair.Parse(data[ARMOR_ID_AND_LEVEL_KEY].Value) : new ItemIDLevelPair(PlayerModel.EMPTY_EQUIPMENT_ID, 0);

            serverPlayerModel.Level_ZeroBased = data.ContainsKey(ZERO_BASED_LEVEL_KEY) ? int.Parse(data[ZERO_BASED_LEVEL_KEY].Value) : 0;

            // User must have at least one ability.
            if (!data.ContainsKey(ABILITY_ID_AND_LEVELS_KEY) || string.IsNullOrEmpty(data[ABILITY_ID_AND_LEVELS_KEY].Value))
            {
                throw new Exception("Player doesn't have any equipped abilities.");
            }
            string[] abilityStrings = data[ABILITY_ID_AND_LEVELS_KEY].Value.Trim().Split(':');
            ItemIDLevelPair[] abilityIDLevelPairs = new ItemIDLevelPair[abilityStrings.Length];
            for (int i = 0; i < abilityStrings.Length; i++)
            {
                abilityIDLevelPairs[i] = ItemIDLevelPair.Parse(abilityStrings[i]);
            }
            serverPlayerModel.EquippedAbilities = abilityIDLevelPairs;

            return serverPlayerModel;
        }
    }
}
