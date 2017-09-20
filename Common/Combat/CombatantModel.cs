using Common.Inventory.Abilities;
using Common.Serialization;

namespace Common.Combat
{
    /// <summary>
    /// Describes a player's or NPC's combat stats and abilities.
    /// </summary>
    public class CombatantModel: ISerializableModel
    {
        private static int nextID;
        public static int GetTemporaryInstanceID() {
            if (nextID == int.MaxValue) { nextID = 0; }
            return nextID++;
        }

        public int ID;      // Temporary instance ID used as an identifier during combat sessions.
        public int Level;
        public int CurrentHealth { get; private set; }
        public int MaxHealth;
        public int HealthRegenPerTurn;
        public int CurrentMana { get; private set; }
        public int MaxMana;
        public int ManaRegenPerTurn;
        public int Armor;
        public int SpellResist;
        public int MeleeDamage;
        public float MeleeCritChanceNormalized;
        public int SpellDamage;
        public float SpellCritChanceNormalized;
        public int TeamID;
        public StatModifiersDeltas StatModifiersDeltas = new StatModifiersDeltas();
        public AbilityDurationDataCollection StatModifierAbilityDurations = new AbilityDurationDataCollection();
        public AbilityDurationDataCollection RegenOverTimeAbilityDurations = new AbilityDurationDataCollection();
        public AbilityDurationDataCollection DamageOverTimeAbilityDurations = new AbilityDurationDataCollection();

        private AbilityItemModel[] _abilityItemModels;
        public AbilityItemModel[] AbilityItemModels
        {
            get { return _abilityItemModels; }
            set
            {
                _abilityItemModels = value;
                if (_abilityItemModels != null)
                {
                    for (int i = 0; i < _abilityItemModels.Length; i++)
                    {
                        if (_abilityItemModels[i].AbilityDurationData != null)
                        {
                            _abilityItemModels[i].AbilityDurationData.OwnerCombatantID = ID;
                        }
                    }
                }
            }
        }

        public CombatantModel()
        {
            this.ID = GetTemporaryInstanceID();
        }

        public CombatantModel(int level, int maxHealth, int healthRegenPerTurn, int maxMana, int manaRegenPerTurn, int armor, 
            int spellResist, int meleeDamage, float meleeCritChanceNormalized, int spellDamage, float spellCritChanceNormalized, 
            int teamID, AbilityItemModel[] abilityItemModels)
        {
            this.ID = GetTemporaryInstanceID();
            this.Level = level;
            this.CurrentHealth = maxHealth;
            this.MaxHealth = maxHealth;
            this.HealthRegenPerTurn = healthRegenPerTurn;
            this.CurrentMana = maxMana;
            this.MaxMana = maxMana;
            this.ManaRegenPerTurn = manaRegenPerTurn;
            this.Armor = armor;
            this.SpellResist = spellResist;
            this.MeleeDamage = meleeDamage;
            this.MeleeCritChanceNormalized = meleeCritChanceNormalized;
            this.SpellDamage = spellDamage;
            this.SpellCritChanceNormalized = spellCritChanceNormalized;
            this.TeamID = teamID;
            this.AbilityItemModels = abilityItemModels;
        }

        public void SetCurrentHealth(int newHealth)
        {
            if (newHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            else if(newHealth < 0)
            {
                CurrentHealth = 0;
            }
            else
            {
                CurrentHealth = newHealth;
            }
        }

        public void SetCurrentMana(int newMana)
        {
            if (newMana > MaxMana)
            {
                CurrentMana = MaxMana;
            }
            else if(newMana < 0)
            {
                newMana = 0;
            }
            else
            {
                CurrentMana = newMana;
            }
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public AbilityItemModel GetAbilityByID(int id)
        {
            for (int i = 0; i < _abilityItemModels.Length; i++)
            {
                if (_abilityItemModels[i].ID == id)
                {
                    return _abilityItemModels[i];
                }
            }
            return null;
        }

        public int GetAbilityIndex(int id)
        {
            for (int i = 0; i < _abilityItemModels.Length; i++)
            {
                if (_abilityItemModels[i].ID == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetStatModifiedAbilityCoolDownTurnsRemaining(int abilityIndex)
        {
            AbilityItemModel abilityItemModel = AbilityItemModels[abilityIndex];

            float deltaRatio = 1f + (float)StatModifiersDeltas.AbilityCoolDownTurnsPercentModifier * 0.01f;
            int totalCoolDownTurns = (int)(((float)abilityItemModel.CoolDownTurns) * deltaRatio);
            int turnsRemaining = totalCoolDownTurns - abilityItemModel.CoolDownTurnsElapsed;
            return turnsRemaining < 0 ? 0 : turnsRemaining;
        }

        public int GetStatModifiedAbilityTotalCastTurns(int abilityIndex)
        {
            AbilityItemModel abilityItemModel = AbilityItemModels[abilityIndex];
            float deltaRatio = 1f + (float)StatModifiersDeltas.AbilityCastTurnsPercentModifierDelta * .01f;
            int modifiedCastTurns = (int)(((float)abilityItemModel.CastTurns) * deltaRatio);
            return modifiedCastTurns < 1 ? 1 : modifiedCastTurns;
        }

        public object[] ToObjectArray()
        {
            object[] serializedAbilities = null;
            serializedAbilities = new object[AbilityItemModels.Length];
            for (int i = 0; i < serializedAbilities.Length; i++)
            {
                serializedAbilities[i] = AbilityItemModels[i].ToObjectArray();
            }

            return new object[]
            {
                ID,
                Level,
                CurrentHealth,
                MaxHealth,
                HealthRegenPerTurn,
                CurrentMana,
                MaxMana,
                ManaRegenPerTurn,
                Armor,
                SpellResist,
                MeleeDamage,
                MeleeCritChanceNormalized,
                SpellDamage,
                SpellCritChanceNormalized,
                TeamID,
                StatModifierAbilityDurations.ToObjectArray(),
                RegenOverTimeAbilityDurations.ToObjectArray(),
                DamageOverTimeAbilityDurations.ToObjectArray(),
                serializedAbilities
            };
        }

        public void FromObjectArray(object[] properties)
        {
            int i = 0;
            ID = (int)properties[i++];
            Level = (int)properties[i++];
            CurrentHealth = (int)properties[i++];
            MaxHealth = (int)properties[i++];
            HealthRegenPerTurn = (int)properties[i++];
            CurrentMana = (int)properties[i++];
            MaxMana = (int)properties[i++];
            ManaRegenPerTurn = (int)properties[i++];
            Armor = (int)properties[i++];
            SpellResist = (int)properties[i++];
            MeleeDamage = (int)properties[i++];
            MeleeCritChanceNormalized = (float)properties[i++];
            SpellDamage = (int)properties[i++];
            SpellCritChanceNormalized = (float)properties[i++];
            TeamID = (int)properties[i++];

            object[] statModifierAbDurs = properties[i++] as object[];
            StatModifierAbilityDurations.FromObjectArray(statModifierAbDurs);

            object[] regenOverTimeAbDurs = properties[i++] as object[];
            RegenOverTimeAbilityDurations.FromObjectArray(regenOverTimeAbDurs);

            object[] damageOverTimeAbDurs = properties[i++] as object[];
            DamageOverTimeAbilityDurations.FromObjectArray(damageOverTimeAbDurs);

            object[] serializedAbilities = properties[i++] as object[];
            AbilityItemModel[] parsedAbilities = new AbilityItemModel[serializedAbilities.Length];
            for (int j = 0; j < parsedAbilities.Length; j++)
            {
                parsedAbilities[j] = new AbilityItemModel();
                parsedAbilities[j].FromObjectArray(serializedAbilities[j] as object[]);
            }
            AbilityItemModels = parsedAbilities;
        }

        public override string ToString()
        {
            string[] abilityStrings = new string[_abilityItemModels.Length];
            for (int i = 0; i < _abilityItemModels.Length; i++)
            {
                abilityStrings[i] = _abilityItemModels[i].ToString();
            }
            string abilities = "[" + string.Join(",", abilityStrings) + "]";

            string output = "<CombatantModel>{" +
                "ID: " + ID + ", " +
                "Level: " + Level + ", " +
                "CurrentHealth: " + CurrentHealth + ", " +
                "MaxHealth: " + MaxHealth + ", " +
                "HealthRegenPerTurn: " + HealthRegenPerTurn + ", " +
                "CurrentMana: " + CurrentMana + ", " +
                "MaxMana: " + MaxMana + ", " +
                "ManaRegenPerTurn: " + ManaRegenPerTurn + ", " +
                "Armor: " + Armor + ", " +
                "SpellResist: " + SpellResist + ", " +
                "MeleeDamage: " + MeleeDamage + ", " +
                "MeleeCritChanceNormalized: " + MeleeCritChanceNormalized + ", " +
                "SpellDamage: " + SpellDamage + ", " +
                "SpellCritChanceNormalized: " + SpellCritChanceNormalized + ", " +
                "AbilityItemModels: " + abilities + 
                "}";

            return output;
        }
    }
}
