using Common.Serialization;
using Common.World.Zone;

namespace Common.World
{
    /// <summary>
    /// A world consists of zones.
    /// </summary>
    public class WorldModel : ISerializableModel
    {
        public ZoneModel[] ZoneModels;

        public void FromObjectArray(object[] properties)
        {
            int i = 0;
            object[] zoneObjects = properties[i++] as object[];
            ZoneModels = new ZoneModel[zoneObjects.Length];
            for (int j = 0; j < ZoneModels.Length; j++)
            {
                ZoneModels[j] = new ZoneModel();
                ZoneModels[j].FromObjectArray(zoneObjects[j] as object[]);
            }
        }

        public object[] ToObjectArray()
        {
            object[] zoneObjects = new object[ZoneModels.Length];
            for (int i = 0; i < zoneObjects.Length; i++)
            {
                zoneObjects[i] = ZoneModels[i].ToObjectArray();
            }

            return new object[]
            {
                zoneObjects
            };
        }

        public override string ToString()
        {
            string[] zoneStrings = new string[ZoneModels.Length];
            for (int i = 0; i < zoneStrings.Length; i++)
            {
                zoneStrings[i] = ZoneModels[i].ToString();
            }

            return "<WorldModel>{" +
                "ZoneModels: [" + string.Join(",", zoneStrings) + "]" +
                "}";
            
        }
    }
}
