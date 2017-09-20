using Common.Combat;
using Common.Rarity;
using Common.Serialization;

namespace Common.Npcs
{
    /// <summary>
    /// Computer controller character. Usually an enemy.
    /// </summary>
    public class NpcModel : ISerializableModel
    {
        public string ID;
        public string Name;
        public NpcRole NpcRole;
        public NpcRace NpcRace;
        public NpcSize NpcSize;
        public RarityType RarityType;
        public CombatantModel CombatantModel;
        public string[] AlwaysDroppedItemIDs;

        public NpcModel() { }

        public NpcModel(string id, string name, NpcRole npcRole, NpcRace npcRace, NpcSize npcSize, RarityType rarityType, 
            CombatantModel combatantModel, string[] alwaysDroppedItemIDs = null)
        {
            this.ID = id;
            this.Name = name;
            this.NpcRole = npcRole;
            this.NpcRace = npcRace;
            this.NpcSize = npcSize;
            this.RarityType = rarityType;
            this.CombatantModel = combatantModel;
            this.AlwaysDroppedItemIDs = alwaysDroppedItemIDs;
        }

        public NpcModel Clone()
        {
            return new NpcModel(ID, Name, NpcRole, NpcRace, NpcSize, RarityType, CombatantModel, AlwaysDroppedItemIDs);
        }

        public object[] ToObjectArray()
        {
            object[] alwaysDroppedItemIdObjects = AlwaysDroppedItemIDs == null ? new object[0] : new object[AlwaysDroppedItemIDs.Length];
            for (int i = 0; i < alwaysDroppedItemIdObjects.Length; i++)
            {
                alwaysDroppedItemIdObjects[i] = AlwaysDroppedItemIDs[i];
            }

            return new object[]
            {
                ID,
                Name,
                (int)NpcRole,
                (int)NpcRace,
                (int)NpcSize,
                (int)RarityType,
                CombatantModel.ToObjectArray(),
                alwaysDroppedItemIdObjects
            };
        }

        public void FromObjectArray(object[] properties)
        {
            int i = 0;
            ID = properties[i++] as string;
            Name = properties[i++] as string;
            NpcRole = (NpcRole)properties[i++];
            NpcRace = (NpcRace)properties[i++];
            NpcSize = (NpcSize)properties[i++];
            RarityType = (RarityType)properties[i++];
            CombatantModel = new CombatantModel();
            CombatantModel.FromObjectArray(properties[i++] as object[]);

            object[] alwaysDroppedItemIdObjects = properties[i++] as object[];
            AlwaysDroppedItemIDs = new string[alwaysDroppedItemIdObjects.Length];
            for (int j = 0; j < AlwaysDroppedItemIDs.Length; j++)
            {
                AlwaysDroppedItemIDs[j] = alwaysDroppedItemIdObjects[j] as string;
            }
        }

        public override string ToString()
        {
            string alwaysDroppedItems = "";
            if (AlwaysDroppedItemIDs != null)
            {
                alwaysDroppedItems = "[" + string.Join("," , AlwaysDroppedItemIDs) + "]";
            }

            string output = "<NpcModel>{" + ", " +
                "ID: " + ID + ", " +
                "Name: " + Name + ", " +
                "NpcRole: " + NpcRole.ToString() + ", " +
                "NpcRace: " + NpcRace.ToString() + ", " +
                "NpcSize: " + NpcSize.ToString() + ", " +
                "RarityType: " + RarityType.ToString() + ", " +
                "CombatantModel: " + CombatantModel.ToString() + ", " +
                "AlwaysDroppedItemIDs: " + alwaysDroppedItems +
                "}";

            return output;
        }
    }
}
