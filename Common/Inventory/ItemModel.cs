using Common.Rarity;
using Common.Serialization;

namespace Common.Inventory
{
    public enum ItemType { Ability, Equipment};

    /// <summary>
    /// Base class for Items.
    /// </summary>
    public abstract class ItemModel : ISerializableModel
    {
        public const int NUM_SERIALIZED_BASE_PROPERTIES = 3;
        public const int UNBUILT_ITEM_LEVEL = -1;

        public int ID;
        public string InstanceID = string.Empty;
        public string Name;
        public string Description;
        public RarityType RarityType = RarityType.None;
        public int StackedCount = 1;

        public bool HasBuiltStatsFromLevel { get; private set; }

        private int _level_ZeroBased = UNBUILT_ITEM_LEVEL;
        public int Level_ZeroBased {
            get { return _level_ZeroBased; }
            set
            {
                _level_ZeroBased = value;
                buildStatsFromLevel();
                HasBuiltStatsFromLevel = true;
            }
        }

        // Player owned items should have a non-empty InstanceID.
        public bool IsInstanceIDValid() { return InstanceID != string.Empty; }

        public ItemModel() { }

        public ItemModel(int ID, int level)
        {
            this.ID = ID;
            this.Level_ZeroBased = level;
        }

        public ItemModel(int ID, string name, string description, int level, RarityType rarityType, int stackedCount) : this(ID, level)
        {
            this.Name = name;
            this.Description = description;
            this.RarityType = rarityType;
            this.StackedCount = stackedCount;
        }

        public void CopyFrom(ItemModel itemModel, bool copyStackCount)
        {
            ID = itemModel.ID;
            InstanceID = itemModel.InstanceID;
            Name = itemModel.Name;
            Description = itemModel.Description;
            RarityType = itemModel.RarityType;
            _level_ZeroBased = itemModel.Level_ZeroBased; // Copy to private member to avoid redundant re-building of stats.
            HasBuiltStatsFromLevel = itemModel.HasBuiltStatsFromLevel;
            if (copyStackCount)
            {
                StackedCount = itemModel.StackedCount;
            }

            copyExtendedProperties(itemModel);
        }

        public object[] ToObjectArray()
        {
            object[] serializedProperties = serializeExtendedProperties(NUM_SERIALIZED_BASE_PROPERTIES);
            serializedProperties[0] = ID;
            serializedProperties[1] = InstanceID;
            serializedProperties[2] = Level_ZeroBased;

            return serializedProperties;
        }

        public void FromObjectArray(object[] properties)
        {
            ID = (int)properties[0];
            InstanceID = properties[1] as string;

            // Copy to private member to avoid redundant re-building of stats.
            // NOTE: It will be required to re-set this ItemModel via Level setter once its base stats
            // have been determined. Until then HasBuiltStatsFromLevel should be false.
            _level_ZeroBased = (int)properties[2]; 

            deserializeExtendedProperties(NUM_SERIALIZED_BASE_PROPERTIES, properties);
        }

        public override string ToString()
        {
            string childClassName = null;
            string commaSeparatedProps = null;
            getCommaSeparatedToStringProps(out childClassName, out commaSeparatedProps);

            string output = "<" + childClassName + ">{" +
                "ID: " + ID + ", " +
                "InstanceID: " + InstanceID + ", " +
                "Name: " + Name + ", " +
                "Description: " + Description + ", " +
                "Level: " + Level_ZeroBased + ", " +
                "RarityType: " + RarityType.ToString() + ", " +
                "StackedCount: " + StackedCount + ", " +
                commaSeparatedProps +
                "}";

            return output;
        }

        public abstract ItemType ItemType { get; }
        public abstract ItemModel Clone();
        protected abstract void buildStatsFromLevel();
        protected abstract void copyExtendedProperties(ItemModel sourceItemModel);
        protected abstract object[] serializeExtendedProperties(int startOffset);
        protected abstract void deserializeExtendedProperties(int startOffset, object[] source);
        protected abstract void getCommaSeparatedToStringProps(out string className, out string commaSeparatedProps);
    }
}
