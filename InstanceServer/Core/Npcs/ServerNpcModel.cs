using Common.Combat;
using Common.Inventory;
using Common.Npcs;
using Common.Rarity;
using InstanceServer.Core.Combat.Behaviours;

namespace InstanceServer.Core.Npcs
{
    public class ServerNpcModel : NpcModel
    {
        public CombatBehaviourBase CombatBehaviour;
        public DropTableModel DropTableModel;

        public ServerNpcModel() { }

        public ServerNpcModel(CombatBehaviourBase combatantBehaviour, string id, string name, NpcRole npcRole, NpcRace npcRace, 
            NpcSize npcSize, RarityType rarityType, CombatantModel combatantModel, string[] alwaysDroppedItemIDs = null) : base(id, 
                name, npcRole, npcRace, npcSize, rarityType, combatantModel, alwaysDroppedItemIDs)
        {
            this.CombatBehaviour = combatantBehaviour;
        }
    }
}
