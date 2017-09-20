namespace Common.Combat.Actions
{
    public enum CombatActionType
    {
        None, Wait, Spawn, Death, Revive, RegenMana, RegenHealth, ReceiveMeleeDamage, ReceiveSpellDamage,
        StartCastingAbility, CancelCastingAbility, ApplyAbility, ExpireAbility,
        StatIncreasedByDurationAbility, StatDecreasedByDurationAbility, AffectedByDurationAbility
    };

    /// <summary>
    /// Model representation of an INDIVIDUAL combat action performed by/to a CombatantModel.
    /// Used as en element of CombatActionsCollectionModel to communicate all combat actions
    /// (ex: damage dealt and/or taken) for an individual player.
    /// </summary>
    public struct CombatActionModel
    {
        public int CasterCombatantID;
        public CombatActionType CombatActionType;
        public int AbilityID;
        public int TargetCombatantID;
        public int Amount;
        public int TurnsRemaining;
        public bool IsCrit;

        public CombatActionModel(
            int casterCombatantID, CombatActionType combatActionType, int abilityID, int targetCombatantID, 
            int amount, int turnsRemaining, bool isCrit)
        {
            CasterCombatantID = casterCombatantID;
            CombatActionType = combatActionType;
            AbilityID = abilityID;
            TargetCombatantID = targetCombatantID;
            Amount = amount;
            TurnsRemaining = turnsRemaining;
            IsCrit = isCrit;
        }

        public static object[] ToObjectArray(CombatActionModel cam)
        {
            object[] objArray = new object[] {
                cam.CasterCombatantID,
                (int)cam.CombatActionType,
                cam.AbilityID,
                cam.TargetCombatantID,
                cam.Amount,
                cam.TurnsRemaining,
                cam.IsCrit
            };

            return objArray;
        }

        public static CombatActionModel FromObjectArray(object[] objArray)
        {
            return new CombatActionModel(
                (int)objArray[0],
                (CombatActionType)objArray[1],
                (int)objArray[2],
                (int)objArray[3],
                (int)objArray[4],
                (int)objArray[5],
                (bool)objArray[6]);
        }

        public override string ToString()
        {
            return "<CombatActionModel><" +
                "CasterCombatantID: " + CasterCombatantID.ToString() + ", " +
                "CombatActionType: " + CombatActionType.ToString() + ", " +
                "AbilityID: " + AbilityID.ToString() + ", " +
                "TargetCombatantID: " + TargetCombatantID.ToString() + ", " +
                "Amount: " + Amount.ToString() + ", " +
                "TurnsRemaining: " + TurnsRemaining.ToString() +
                "IsCrit: " + IsCrit.ToString() +
                ">";
        }
    }
}
