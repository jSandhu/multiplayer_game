using System;
using System.Collections.Generic;
using Common.World;
using InstanceServer.Core.Player;
using InstanceServer.Core.Room.Game;
using InstanceServer.Core.Room.Game.WorldBuilders;
using Common.Inventory;
using DIFramework;
using InstanceServer.Core;
using Common.World.Zone;
using Common.Rarity;
using InstanceServer.Core.Npcs;
using Common.Npcs;
using Common.NumberUtils;
using Common.Inventory.Abilities;
using Common.Combat;
using Common.Combat.Commands;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.Room.Game.WorldBuilders.Commands;

namespace InstanceServer.Tests.Mocking.Room.WorldBuilders
{
    public class TestWorldBuilder : IWorldBuilder
    {
        private static int NumNpcRoles = Enum.GetNames(typeof(NpcRole)).Length;
        private static int NumNpcRaces = Enum.GetNames(typeof(NpcRace)).Length;
        private static int NumNpcSizes = Enum.GetNames(typeof(NpcSize)).Length;

        private const int NPC_LEVELS_PER_DIFFICULTY_MULT = 2;

        private CatalogModel catalogModel;

        private AbilityItemModel spellDMGDot_TestAbility = 
            new AbilityItemModel(99999, "Spell DOT Test ability", "Spell DOT Test ability desc", 
                RarityType.Common,
                1,
                1, 
                AbilityEffectType.SpellDamage, 
                AbilityTargetType.EnemySingle, 
                AbilityDurationType.MultiTurn, 
                2, 8, 0, true, 1, 1, 10);

        private AbilityItemModel spellResistDebuff_TestAbility =
            new AbilityItemModel(11111, "Spell resist debuff test ability", "Spell resist debuff",
            RarityType.Common,
            1,
            1,
            AbilityEffectType.SpellResistModifier,
            AbilityTargetType.EnemySingle,
            AbilityDurationType.MultiTurn,
            3, 3 , 0, true, -12, 1, 6);

        public TestWorldBuilder()
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
            worldModel.ZoneModels = new ZoneModel[2];
            for (int j = 0; j < worldModel.ZoneModels.Length; j++)
            {
                worldModel.ZoneModels[j] = buildZone(averageNpcLevelPlusDifficulty, RarityType.Common, RarityType.Legendary, npcTeamID, 1);
                worldModel.ZoneModels[j].ID = j;
                worldModel.ZoneModels[j].IsFinalZone = j == worldModel.ZoneModels.Length - 1;
            }
            return worldModel;
        }

        private ZoneModel buildZone(int minNpcLevel, RarityType minNpcRarityType, RarityType maxNpcRarityType, int npcTeamID, int maxNpcs)
        {
            int numNpcs = 1;
            ZoneModel zoneModel = new ZoneModel();
            zoneModel.EnemyNpcModels = new ServerNpcModel[numNpcs];

            for (int i = 0; i < numNpcs; i++)
            {
                int zoneBasedLevel = minNpcLevel;

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
            return new AbilityItemModel[] {
                spellDMGDot_TestAbility.Clone() as AbilityItemModel,
                spellResistDebuff_TestAbility.Clone() as AbilityItemModel
            };
        }
    }
}