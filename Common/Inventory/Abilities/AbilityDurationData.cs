using Common.Serialization;

namespace Common.Inventory.Abilities
{
    /// <summary>
    /// Duration data used by/for AbilityItemModels that perform an 
    /// effect over time (AbilityDurationType == MultiTurn).
    /// </summary>
    public class AbilityDurationData : ISerializableModel
    {
        // Serializable data
        public int OriginAbilityID;
        public int OwnerCombatantID;
        public int TurnsPerTick;
        public int MaxTicks;
        public int NumTurnsElapsed;

        // Non serialized data
        public int PerTickAmount;
        public AbilityEffectType AbilityEffectType;
        public AbilityTargetType AbilityTargetType;
        public bool IsCrit;

        public int NumTurnsRemaining { get { return MaxTicks * TurnsPerTick - NumTurnsElapsed; } }

        public AbilityDurationData() { }

        public bool IsFriendly()
        {
            return AbilityTargetType == AbilityTargetType.FriendlySingle ||
                AbilityTargetType == AbilityTargetType.FriendlyAll;
        }

        public bool IsTicking()
        {
            return NumTurnsElapsed % TurnsPerTick == 0 &&
                   (NumTurnsElapsed / TurnsPerTick) <= MaxTicks;
        }

        private bool isExpired;
        public bool IsExpired()
        {
            if (!isExpired)
            {
                isExpired = (NumTurnsElapsed / TurnsPerTick) >= MaxTicks;
            }
            return isExpired;
        }

        public void Reset()
        {
            NumTurnsElapsed = 0;
        }

        public AbilityDurationData Clone(bool reset)
        {
            AbilityDurationData clone = new AbilityDurationData();
            clone.OriginAbilityID = OriginAbilityID;
            clone.OwnerCombatantID = OwnerCombatantID;
            clone.PerTickAmount = PerTickAmount;
            clone.TurnsPerTick = TurnsPerTick;
            clone.MaxTicks = MaxTicks;
            clone.NumTurnsElapsed = NumTurnsElapsed;
            clone.AbilityEffectType = AbilityEffectType;
            clone.AbilityTargetType = AbilityTargetType;
            clone.IsCrit = IsCrit;

            if (reset)
            {
                clone.Reset();
            }

            return clone;    
        }

        public object[] ToObjectArray()
        {
            return new object[]
            {
                OriginAbilityID,
                OwnerCombatantID,
                TurnsPerTick,
                MaxTicks,
                NumTurnsElapsed
            };
        }

        public void FromObjectArray(object[] properties)
        {
            int index = 0;
            OriginAbilityID = (int)properties[index++];
            OwnerCombatantID = (int)properties[index++];
            TurnsPerTick = (int)properties[index++];
            MaxTicks = (int)properties[index++];
            NumTurnsElapsed = (int)properties[index++];
        }
    }
}
