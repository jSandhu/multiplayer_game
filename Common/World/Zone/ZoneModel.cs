using Common.Npcs;
using Common.Serialization;

namespace Common.World.Zone
{
    /// <summary>
    /// A zone contains NPCs (usually ennemies)  and is a sub section of the world.
    /// </summary>
    public class ZoneModel : ISerializableModel
    {
        public int ID;
        public bool IsFinalZone;
        public bool IsCompleted;
        public NpcModel[] EnemyNpcModels;

        public void FromObjectArray(object[] properties)
        {
            int i = 0;
            ID = (int)properties[i++];
            IsFinalZone = (bool)properties[i++];
            IsCompleted = (bool)properties[i++];

            object[] npcObjects = properties[i++] as object[];
            EnemyNpcModels = new NpcModel[npcObjects.Length];
            for (int j = 0; j < EnemyNpcModels.Length; j++)
            {
                EnemyNpcModels[j] = new NpcModel();
                EnemyNpcModels[j].FromObjectArray(npcObjects[j] as object[]);
            }
        }

        public object[] ToObjectArray()
        {
            object[] npcObjects = new object[EnemyNpcModels.Length];
            for (int i = 0; i < npcObjects.Length; i++)
            {
                npcObjects[i] = EnemyNpcModels[i].ToObjectArray();
            }

            return new object[]
            {
                ID,
                IsFinalZone,
                IsCompleted,
                npcObjects
            };
        }

        public override string ToString()
        {
            string[] enemyNPCModelStrings = new string[EnemyNpcModels.Length];
            for (int i = 0; i < enemyNPCModelStrings.Length; i++)
            {
                enemyNPCModelStrings[i] = EnemyNpcModels[i].ToString();
            }

            return "<ZoneModel>{" +
                "ID: " + ID + ", " +
                "IsCompleted: " + IsCompleted.ToString() + ", " +
                "EnemyNpcModels: [" + string.Join(",", enemyNPCModelStrings) + "]" + 
                "}";
        }
    }
}
