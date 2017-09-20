using System;
using System.Collections.Generic;
using Common.World;
using InstanceServer.Core.Player;
using Common.World.Zone;
using Common.NumberUtils;
using InstanceServer.Core.Npcs;
using Common.Rarity;
using Common.Npcs;
using Common.Inventory.Abilities;
using Common.Combat;
using Common.Combat.Commands;
using InstanceServer.Core.Combat.Behaviours;
using Common.Inventory;
using DIFramework;
using InstanceServer.Core.Room.Game.WorldBuilders.Commands;

namespace InstanceServer.Core.Room.Game.WorldBuilders
{
    public class PlayerBasedWorldBuilder : IWorldBuilder
    {
        private const int NPC_LEVELS_PER_DIFFICULTY_MULT = 2;
        private const int NUM_ZONES_PER_WORLD = 3;
        private const int MAX_NPCS_PER_ZONE = 3;

        private static int NumNpcRoles = Enum.GetNames(typeof(NpcRole)).Length;
        private static int NumNpcRaces = Enum.GetNames(typeof(NpcRace)).Length;
        private static int NumNpcSizes = Enum.GetNames(typeof(NpcSize)).Length;

        private CatalogModel catalogModel;

        public PlayerBasedWorldBuilder()
        {
            this.catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(InstanceServerApplication.CONTEXT_ID);
        }

        public WorldModel Build(List<ServerPlayerModel> serverPlayerModels, GameDifficultyType gameDifficultyType, int npcTeamID)
        {
            // calculate average level (higher game difficulty increases npc level)
            float levelsSum = 0;
            for (int i = 0; i < serverPlayerModels.Count; i++)
            {
                levelsSum += serverPlayerModels[i].CombatantModel.Level;
            }
            int averageNpcLevelPlusDifficulty = (int)(levelsSum / serverPlayerModels.Count + .5f) + (int)gameDifficultyType * NPC_LEVELS_PER_DIFFICULTY_MULT;

            WorldModel worldModel = new WorldModel();
            worldModel.ZoneModels = new ZoneModel[NUM_ZONES_PER_WORLD];
            for (int j = 0; j < worldModel.ZoneModels.Length; j++)
            {
                // TODO: determine min and max rarities based on player average level
                worldModel.ZoneModels[j] = buildZone(averageNpcLevelPlusDifficulty, RarityType.Common, RarityType.Legendary, npcTeamID, MAX_NPCS_PER_ZONE);
                worldModel.ZoneModels[j].ID = j;
                worldModel.ZoneModels[j].IsFinalZone = j == worldModel.ZoneModels.Length - 1;
            }
            return worldModel;
        }

        private ZoneModel buildZone(int minNpcLevel, RarityType minNpcRarityType, RarityType maxNpcRarityType, int npcTeamID, int maxNpcs)
        {
            int numNpcs = RandomGen.GetIntRange(1, maxNpcs);
            ZoneModel zoneModel = new ZoneModel();
            zoneModel.EnemyNpcModels = new ServerNpcModel[numNpcs];

            for (int i = 0; i < numNpcs; i++)
            {
                // Increase npc level by 1 per zone progression
                int zoneBasedLevel = minNpcLevel + i;

                ServerNpcModel serverNpcModel = buildRandomNPC(zoneBasedLevel, npcTeamID, minNpcRarityType, maxNpcRarityType);
                zoneModel.EnemyNpcModels[i] = serverNpcModel;
            }

            return zoneModel;
        }

        private ServerNpcModel buildRandomNPC(int npcLevel, int npcTeamID, RarityType minRarityType, RarityType maxRarityType)
        {
            RarityType npcRarity = RarityConstants.GetRandomRarityRange(minRarityType, maxRarityType);
            NpcRole npcRole = (NpcRole)RandomGen.GetIntRange(0, NumNpcRoles - 1);
            NpcRace npcRace = (NpcRace)RandomGen.GetIntRange(0, NumNpcRaces - 1);
            NpcSize npcSize = (NpcSize)RandomGen.GetIntRange(0, NumNpcSizes - 1);

            // TODO: determine better naming scheme. For now just join Size+Race+Role;
            string npcName = npcSize.ToString() + " " + npcRace.ToString() + " " + npcRole.ToString();

            // Build combatant model
            AbilityItemModel[] randomAbilities = getRandomNPCAbilities((int)npcRarity + 1, npcLevel);
            CombatantModel combatantModel = BuildCombatantSCMD.Execute(npcLevel, npcTeamID, randomAbilities, npcRarity);

            // TODO: determine behaviour based on role. For now use default
            CombatBehaviourBase combatBehaviour = new AbilityCycleBehaviour();

            ServerNpcModel serverNpcModel = new ServerNpcModel(combatBehaviour, "", npcName, npcRole, npcRace, npcSize, npcRarity, combatantModel);
            DetermineNPCDropTableSCMD.Execute(serverNpcModel);
            return serverNpcModel;
        }

        private AbilityItemModel[] getRandomNPCAbilities(int numAbilities, int level)
        {
            List<AbilityItemModel> randomAbilitiesList = new List<AbilityItemModel>();
            List<ItemModel> allAbilities = catalogModel.GetAllItemsOfType<AbilityItemModel>();

            for (int i = 0; i < numAbilities; i++)
            {
                AbilityItemModel randomAbility = allAbilities[RandomGen.GetIntRange(0, allAbilities.Count - 1)] as AbilityItemModel;
                if (!randomAbilitiesList.Contains(randomAbility))
                {
                    randomAbilitiesList.Add(randomAbility);
                }
            }

            AbilityItemModel[] randomAbilitiesArray = new AbilityItemModel[randomAbilitiesList.Count];
            for (int j = 0; j < randomAbilitiesArray.Length; j++)
            {
                AbilityItemModel clone = new AbilityItemModel();
                clone.CopyFrom(randomAbilitiesList[j], false);
                clone.Level_ZeroBased = level;
                randomAbilitiesArray[j] = clone;
            }

            return randomAbilitiesArray;
        }
    }
}
