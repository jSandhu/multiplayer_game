using System;
using System.Collections.Generic;

namespace Common.Inventory
{
    /// <summary>
    /// Drop table used to award items based on their weight.
    /// </summary>
    public class DropTableModel
    {
        class ItemWeight
        {
            public ItemModel ItemModel;
            public float Weight;
            public float DropPercentage;
            public ItemWeight(ItemModel itemModel, float weight) { ItemModel = itemModel; Weight = weight; DropPercentage = 0; }
        }

        public int ID;

        private List<ItemWeight> itemModelWeights = new List<ItemWeight>();
        private int lastSortedCount = 0;
        private float weightSum = 0;

        public DropTableModel(int id)
        {
            ID = id;
        }

        public void Clear()
        {
            itemModelWeights.Clear();
            lastSortedCount = 0;
            weightSum = 0;
        }

        public bool ContainsItemWithID(int itemID)
        {
            for (int i = 0; i < itemModelWeights.Count; i++)
            {
                if (itemModelWeights[i].ItemModel.ID == itemID)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddItem(ItemModel itemModel, float weight)
        {
            ItemWeight itemWeight = new ItemWeight(itemModel, weight);
            itemModelWeights.Add(itemWeight);
            weightSum += weight;

            // update all drop percentages
            float weightSumRecip = 1f / weightSum;
            for (int i = 0; i < itemModelWeights.Count; i++)
            {
                itemModelWeights[i].DropPercentage = itemModelWeights[i].Weight * weightSumRecip * 100f;
            }
        }

        public ItemModel GetRandomItem(float normalizedSeed)
        {
            if (normalizedSeed > 1f || normalizedSeed < 0f)
            {
                normalizedSeed = 0f;
            }

            if (itemModelWeights.Count == 0)
            {
                throw new Exception("Drop table doesn't contain any items.");
            }

            sortDescByWeights();

            float roll = normalizedSeed * weightSum;
            for (int i = 0; i < itemModelWeights.Count; i++)
            {
                if (roll < itemModelWeights[i].Weight)
                {
                    return itemModelWeights[i].ItemModel;
                }
                roll -= itemModelWeights[i].Weight;
            }

            throw new Exception("Unable to get random item from drop table.");
        }

        private void sortDescByWeights()
        {
            if (lastSortedCount == itemModelWeights.Count)
            {
                return;
            }

            lastSortedCount = itemModelWeights.Count;
            // sort descending (rarer items at end of list)
            itemModelWeights.Sort((x, y) => y.Weight.CompareTo(x.Weight));
        }

        public override string ToString()
        {
            string output = 
            "<DropTableModel>{" +
            "lastSortedCoount : " + lastSortedCount.ToString() + ", " +
            "weightSum : " + weightSum.ToString() + ", " +
            "itemModelWeights : [";
                
            for (int i = 0; i < itemModelWeights.Count; i++)
            {
                if (i > 0) output += ", ";
                output += "{ItemID : " + itemModelWeights[i].ItemModel.ID + ", Weight : " + itemModelWeights[i].Weight + 
                    ", DropPercentage : " + itemModelWeights[i].DropPercentage.ToString("0.0") + "%" + 
                    "}";
            }
                
            output += "]}";

            return output;
        }

    }
}
