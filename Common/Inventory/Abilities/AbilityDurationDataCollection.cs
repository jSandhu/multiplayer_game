using Common.Serialization;
using System;
using System.Collections.Generic;

namespace Common.Inventory.Abilities
{
    /// <summary>
    /// Collection of AbilityDurationDatas.  Represents ability duration data for a specific 
    /// group of effects (ex: All Regeneration Over Time abilities).
    /// 
    /// Provides helper functions to advance and manage AbilityDurationData application and
    /// removal.
    /// </summary>
    public class AbilityDurationDataCollection : ISerializableModel
    {
        public const int MAX_APPLIED_ABILITIES = 256;

        private bool[] isActive = new bool[MAX_APPLIED_ABILITIES];
        private AbilityDurationData[] abilityDurationDatas = new AbilityDurationData[MAX_APPLIED_ABILITIES];
        private int nextIndex;

        public void Add(AbilityDurationData abilityDurationData)
        {
            for (int i = 0; i < MAX_APPLIED_ABILITIES; i++)
            {
                if (nextIndex == MAX_APPLIED_ABILITIES)
                {
                    nextIndex = 0;
                }
                if (!isActive[nextIndex])
                {
                    abilityDurationDatas[nextIndex] = abilityDurationData;
                    isActive[nextIndex] = true;
                    nextIndex++;
                    return;
                }
                nextIndex++;
            }
            throw new Exception("Maximum reached.");
        }

        public void AdvanceAbilityDurations(AbilityDurationData[] tickedAbilities)
        {
            if (tickedAbilities.Length != MAX_APPLIED_ABILITIES)
            {
                throw new Exception("Input length must == " + MAX_APPLIED_ABILITIES);
            }

            int tickedAbilityIndex = 0;
            for (int i = 0; i < abilityDurationDatas.Length; i++)
            {
                if (!isActive[i])
                {
                    continue;
                }

                abilityDurationDatas[i].NumTurnsElapsed++;
                if (abilityDurationDatas[i].IsTicking())
                {
                    tickedAbilities[tickedAbilityIndex++] = abilityDurationDatas[i];
                }
            }

            // Clear out remainder of ticked abilities array
            for (; tickedAbilityIndex < MAX_APPLIED_ABILITIES; tickedAbilityIndex++)
            {
                tickedAbilities[tickedAbilityIndex] = null;
            }
        }

        public void RemoveAndResetExpired(AbilityDurationData[] expiredAbilitiesRef)
        {
            if (expiredAbilitiesRef.Length != MAX_APPLIED_ABILITIES)
            {
                throw new Exception("Input length must == " + MAX_APPLIED_ABILITIES);
            }
            int expiredIndex = 0;
            for (int i = 0; i < MAX_APPLIED_ABILITIES; i++)
            {
                AbilityDurationData abilityDurationData = abilityDurationDatas[i];
                if (isActive[i] && (abilityDurationData.NumTurnsElapsed / abilityDurationData.TurnsPerTick == abilityDurationData.MaxTicks))
                {
                    isActive[i] = false;
                    expiredAbilitiesRef[expiredIndex++] = abilityDurationDatas[i];
                    abilityDurationDatas[i].Reset();
                    abilityDurationDatas[i] = null;
                }
            }

            // Clear out remainder of array
            for (; expiredIndex < MAX_APPLIED_ABILITIES; expiredIndex++)
            {
                expiredAbilitiesRef[expiredIndex] = null;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < MAX_APPLIED_ABILITIES; i++)
            {
                isActive[i] = false;
                abilityDurationDatas[i] = null;
                nextIndex = 0;
            }
        }

        public object[] ToObjectArray()
        {
            List<AbilityDurationData> activeAbilities = new List<AbilityDurationData>();
            for (int i = 0; i < abilityDurationDatas.Length; i++)
            {
                if (isActive[i])
                {
                    activeAbilities.Add(abilityDurationDatas[i]);
                }
            }
            object[] activeAbilitiesAsObjectArrays = new object[activeAbilities.Count];
            for (int a = 0; a < activeAbilities.Count; a++)
            {
                activeAbilitiesAsObjectArrays[a] = activeAbilities[a].ToObjectArray();
            }

            return activeAbilitiesAsObjectArrays;
        }

        public void FromObjectArray(object[] properties)
        {
            Clear();
            for (int i = 0; i < properties.Length; i++)
            {
                AbilityDurationData aDurDat = new AbilityDurationData();
                aDurDat.FromObjectArray(properties[i] as object[]);
                Add(aDurDat);
            }
        }
    }
}
