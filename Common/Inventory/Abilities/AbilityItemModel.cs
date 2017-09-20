using System;
using Common.Rarity;

namespace Common.Inventory.Abilities
{
    /// <summary>
    /// ItemModel for an Ability that can be used by Players or NPCs
    /// to inflict damage on enemies, or help team mates (ex: FireBall, Heal, WeakenArmor....).
    /// 
    /// Abilities can be applied immediately or may apply an effect over time.
    /// 
    /// Effect over time abilities (AbilityDurationType == MultiTurn)
    /// provide the AbilityDurationData member for per-turn processing.
    /// </summary>
    public class AbilityItemModel : ItemModel
    {
        public override ItemType ItemType { get { return ItemType.Ability; } }

        private int _coolDownTurns;
        public int CoolDownTurns {
            get { return _coolDownTurns; }
            set { _coolDownTurns = value; CoolDownTurnsElapsed = value; }   // reset turns elapsed
        }

        public AbilityEffectType AbilityEffectType;
        public AbilityTargetType AbilityTargetType;
        public AbilityDurationType AbilityDurationType;
        public int CastTurns;
        public int NumCastTurnsElapsed;
        public int CoolDownTurnsElapsed;
        public int ImmediateAmout;
        public bool IsSpellBased;
        public AbilityDurationData AbilityDurationData;

        public AbilityItemModel() : base() { }

        public AbilityItemModel(int ID, string name, string description, RarityType rarityType, int stackedCount, int level, 
            AbilityEffectType abilityEffectType, AbilityTargetType abilityTargetType, AbilityDurationType abilityDurationType, 
            int castTurns, int coolDownTurns, int immediateAmount, bool isSpellBased, int perTickAmount, int turnsPerTick, int maxTicks) : base(ID, name, description, level, rarityType, stackedCount)
        {
            this.AbilityEffectType = abilityEffectType;
            this.AbilityTargetType = abilityTargetType;
            this.AbilityDurationType = abilityDurationType;
            this.CastTurns = castTurns;
            this.CoolDownTurns = coolDownTurns;
            this.ImmediateAmout = immediateAmount;
            this.IsSpellBased = isSpellBased;

            if (abilityDurationType == AbilityDurationType.MultiTurn)
            {
                this.AbilityDurationData = new AbilityDurationData();
                this.AbilityDurationData.OriginAbilityID = ID;
                this.AbilityDurationData.PerTickAmount = perTickAmount;
                this.AbilityDurationData.TurnsPerTick = turnsPerTick;
                this.AbilityDurationData.MaxTicks = maxTicks;
                this.AbilityDurationData.AbilityEffectType = abilityEffectType;
                this.AbilityDurationData.AbilityTargetType = abilityTargetType;
            }
        }

        public bool IsFriendly()
        {
            return AbilityTargetType == AbilityTargetType.FriendlySingle ||
                AbilityTargetType == AbilityTargetType.FriendlyAll;
        }

        public bool IsStatModifier()
        {
            return
                AbilityEffectType == AbilityEffectType.SpellDamageModifier ||
                AbilityEffectType == AbilityEffectType.MeleeDamageModifier ||
                AbilityEffectType == AbilityEffectType.ArmorModifier ||
                AbilityEffectType == AbilityEffectType.SpellResistModifier ||
                AbilityEffectType == AbilityEffectType.AbilityCastTurnsPercentModifier ||
                AbilityEffectType == AbilityEffectType.AbilityCoolDownTurnsPercentModifier;
        }

        protected override void buildStatsFromLevel()
        {
            int oneBasedLevel = 1 + Level_ZeroBased;
            int newCastTurns = (int)(CastTurns / (1f + oneBasedLevel * 0.01f));
            CastTurns = newCastTurns >= 1 ? newCastTurns : 1;
            int newCoolDownTurns = (int)(CoolDownTurns / (1f + oneBasedLevel * 0.01f));
            CoolDownTurns = newCoolDownTurns > 1 ? newCoolDownTurns : 1;
            ImmediateAmout *= oneBasedLevel;
            if (AbilityDurationType == AbilityDurationType.MultiTurn)
            {
                AbilityDurationData.PerTickAmount *= oneBasedLevel;
            }
        }

        public override ItemModel Clone()
        {
            AbilityItemModel clone = new AbilityItemModel();
            clone.CopyFrom(this, true);
            return clone;
        }

        protected override void copyExtendedProperties(ItemModel sourceItemModel)
        {
            AbilityItemModel abilityItemModel = sourceItemModel as AbilityItemModel;
            if (abilityItemModel == null)
            {
                throw new InvalidCastException("Invalid type.");
            }
            AbilityEffectType = abilityItemModel.AbilityEffectType;
            AbilityTargetType = abilityItemModel.AbilityTargetType;
            AbilityDurationType = abilityItemModel.AbilityDurationType;
            CastTurns = abilityItemModel.CastTurns;
            CoolDownTurns = abilityItemModel.CoolDownTurns;
            CoolDownTurnsElapsed = abilityItemModel.CoolDownTurnsElapsed;
            ImmediateAmout = abilityItemModel.ImmediateAmout;
            IsSpellBased = abilityItemModel.IsSpellBased;

            if (AbilityDurationType == AbilityDurationType.MultiTurn)
            {
                AbilityDurationData = abilityItemModel.AbilityDurationData.Clone(false);
            }
        }

        protected override object[] serializeExtendedProperties(int startOffset)
        {
            // Has no additional properties to serialize.
            // Abilities can be determined from catalog data and their levels are equal to equipment level
            return new object[startOffset];
        }

        protected override void deserializeExtendedProperties(int startOffset, object[] source)
        {
            // Has no additional properties to deserialize
            // Abilities can be determined from catalog data and their levels are equal to equipment level
        }

        protected override void getCommaSeparatedToStringProps(out string className, out string commaSeparatedProps)
        {
            className = "AbilityItemModel";

            commaSeparatedProps = 
                "AbilityEffectType: " + AbilityEffectType.ToString() + ", " +
                "AbilityTargetType: " + AbilityTargetType.ToString() + ", " +
                "AbilityDurationType: " + AbilityDurationType.ToString() + ", " +
                "CastTurns: " + CastTurns.ToString() + ", " +
                "CoolDownTurns: " + CoolDownTurns.ToString() + ", " +
                "CoolDownTurnsElapsed: " + CoolDownTurnsElapsed.ToString() + ", " +
                "ImmediateAmmount: " + ImmediateAmout + ", " +
                "IsSpellBased: " + IsSpellBased.ToString();

            if (AbilityDurationType == AbilityDurationType.MultiTurn)
            {
                commaSeparatedProps += ", " +
                    "AbilityDurationData.OriginAbilityID: " + AbilityDurationData.OriginAbilityID.ToString() + ", " +
                    "AbilityDurationData.PerTickAmount: " + AbilityDurationData.PerTickAmount.ToString() + ", " +
                    "AbilityDurationData.TurnsPerTick: " + AbilityDurationData.TurnsPerTick.ToString() + ", " +
                    "AbilityDurationData.MaxTicks: " + AbilityDurationData.MaxTicks.ToString() + ", " +
                    "AbilityDurationData.AbilityEffectType" + AbilityDurationData.AbilityEffectType.ToString() + ", " +
                    "AbilityDurationData.AbilityTargetType" + AbilityDurationData.AbilityTargetType.ToString() + ", " +
                    "AbilityDurationData.NumTurnsElapsed: " + AbilityDurationData.NumTurnsElapsed.ToString();
            }
        }
    }
}
