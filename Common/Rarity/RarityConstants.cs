using Common.NumberUtils;
using System;
using System.Collections.Generic;

namespace Common.Rarity
{
    struct RarityNormalizedWeightPair
    {
        public RarityType RarityType;
        public float NormalizedWeight;

        public RarityNormalizedWeightPair(RarityType rarityType, float normalizedWeight)
        {
            this.RarityType = rarityType;
            this.NormalizedWeight = normalizedWeight;
        }
    }
    public enum RarityType { None = -1, Common, Uncommon, Rare, Epic, Legendary};

    public static class RarityConstants
    {
        private static RarityType[] allRarities;
        public static RarityType[] AllRarities {
            get {
                if (allRarities == null)
                {
                    var enums = Enum.GetValues(typeof(RarityType));
                    allRarities = new RarityType[enums.Length];
                    enums.CopyTo(allRarities, 0);
                }
                return allRarities;
            }
        }

        private static Dictionary<RarityType, float> rarityTypeToWeight = new Dictionary<RarityType, float>()
        {
            { RarityType.Common, 0.5f},
            { RarityType.Uncommon, 0.35f},
            { RarityType.Rare, 0.15f},
            { RarityType.Epic, 0.05f},
            { RarityType.Legendary, 0.005f}
        };

        public static RarityType GetRandomRarity()
        {
            return GetRandomRarityRange(RarityType.Common, RarityType.Legendary);
        }

        public static RarityType GetRandomRarityRange(RarityType minRarity, RarityType maxRarity)
        {
            if (minRarity >= maxRarity)
            {
                throw new Exception("minRarity must < maxRarity");
            }

            float normalizedRandom = RandomGen.NextNormalizedFloat();

            int i = 0;
            float weightSum = 0;
            for (i = 0; i < AllRarities.Length; i++)
            {
                if (AllRarities[i] >= minRarity && AllRarities[i] <= maxRarity)
                {
                    weightSum += rarityTypeToWeight[AllRarities[i]];
                }
            }

            float weightSumRecip = 1f / weightSum;
            RarityType output = minRarity;
            for (i = AllRarities.Length - 1; i >= 0; i--)
            {
                if (AllRarities[i] >= minRarity && AllRarities[i] <= maxRarity)
                {
                    float normalizedWeight = rarityTypeToWeight[AllRarities[i]] * weightSumRecip;
                    if (normalizedRandom <= normalizedWeight)
                    {
                        output = AllRarities[i];
                        break;
                    } 
                }
            }

            return output;
        }
    }
}
