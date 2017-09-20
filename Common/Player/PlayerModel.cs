using Common.Combat;
using Common.Inventory.Equipment;
using Common.Serialization;

namespace Common.Player
{
    public struct ItemIDLevelPair
    {
        public int ItemModelID;
        public int ItemLevel_ZeroBased;

        public ItemIDLevelPair(int itemModelID, int itemLevel_ZeroBased)
        {
            this.ItemModelID = itemModelID;
            this.ItemLevel_ZeroBased = itemLevel_ZeroBased;
        }

        public static ItemIDLevelPair Parse(string raw)
        {
            string[] parts = raw.Trim().Split(',');
            return new ItemIDLevelPair(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }

    /// <summary>
    /// Model representation of a player controller character.  Can wear equipment to
    /// boost its CombatantModel stats.
    /// </summary>
    public class PlayerModel : ISerializableModel
    {
        public const int EMPTY_EQUIPMENT_ID = -1;
        public const int DEFAULT_TEAM_ID = 0;
        private static readonly object[] EMPTY_OBJECT_ARRAY = new object[0];

        public string PlayerID;
        public string Name;
        public bool IsRejoining;
        public EquipmentItemModel EquippedWeaponItemModel;
        public EquipmentItemModel EquippedOffHandItemModel;
        public EquipmentItemModel EquippedArmorItemModel;
        public CombatantModel CombatantModel;

        // Used for parsing from raw data.
        public ItemIDLevelPair EquippedWeapon;
        public ItemIDLevelPair EquippedOffHand;
        public ItemIDLevelPair EquippedArmor;
        public int Level_ZeroBased;
        public ItemIDLevelPair[] EquippedAbilities;

        public PlayerModel() { }

        public PlayerModel(string playerID, string name, EquipmentItemModel equippedWeaponItemModel, EquipmentItemModel equippedOffHandItemModel,
            EquipmentItemModel equippedArmorItemModel)
        {
            this.PlayerID = playerID;
            this.Name = name;
            this.EquippedWeaponItemModel = equippedWeaponItemModel;
            this.EquippedOffHandItemModel = equippedOffHandItemModel;
            this.EquippedArmorItemModel = equippedArmorItemModel;
        }

        public object[] ToObjectArray()
        {
            return new object[]
            {
                PlayerID,
                Name,
                IsRejoining,
                EquippedWeaponItemModel == null ? EMPTY_OBJECT_ARRAY : EquippedWeaponItemModel.ToObjectArray(),
                EquippedOffHandItemModel == null? EMPTY_OBJECT_ARRAY : EquippedOffHandItemModel.ToObjectArray(),
                EquippedArmorItemModel == null ?  EMPTY_OBJECT_ARRAY : EquippedArmorItemModel.ToObjectArray(),
                CombatantModel.ToObjectArray()
            };
        }

        public void FromObjectArray(object[] properties)
        {
            int i = 0;
            PlayerID = (string)properties[i++];
            Name = (string)properties[i++];
            IsRejoining = bool.Parse(properties[i++] as string);

            object[] serializedWeapon = properties[i++] as object[];
            if (serializedWeapon.Length > 0)
            {
                EquippedWeaponItemModel = new EquipmentItemModel();
                EquippedWeaponItemModel.FromObjectArray(serializedWeapon);
            }

            object[] serializedOffhand = properties[i++] as object[];
            if (serializedOffhand.Length > 0)
            {
                EquippedOffHandItemModel = new EquipmentItemModel();
                EquippedOffHandItemModel.FromObjectArray(serializedOffhand);
            }

            object[] serializedArmor = properties[i++] as object[];
            if (serializedArmor.Length > 0)
            {
                EquippedArmorItemModel = new EquipmentItemModel();
                EquippedArmorItemModel.FromObjectArray(serializedArmor);
            }

            CombatantModel = new CombatantModel();
            CombatantModel.FromObjectArray(properties[i] as object[]);
        }

        public override string ToString()
        {
            return 
                "<PlayerModel>{" +
                    "PlayerID: " + PlayerID + ", " +
                    "Name: " + Name + ", " +
                    "IsRejoining: " + IsRejoining + ", " +
                    "EquippedWeaponItemModel: " + EquippedWeaponItemModel.ToString() + ", " +
                    "EquippedOffHandItemModel: " + EquippedOffHandItemModel.ToString() + ", " +
                    "EquippedArmorItemModel: " + EquippedArmorItemModel.ToString() + ", " +
                    "CombatantModel: " + CombatantModel.ToString() +
                "}";
        }
    }
}
