using Common.Rarity;
using System;
using System.Collections.Generic;

namespace Common.Inventory
{
    public abstract class ItemCollection
    {
        public List<ItemModel> AllItems = new List<ItemModel>();

        protected Dictionary<int, ItemModel> itemIDToModel = new Dictionary<int, ItemModel>();
        protected Dictionary<string, ItemModel> itemNameToModel = new Dictionary<string, ItemModel>();
        protected Dictionary<Type, List<ItemModel>> itemTypeToModel = new Dictionary<Type, List<ItemModel>>();

        public List<ItemModel> GetAllItemsOfType<T>()
        {
            Type itemType = typeof(T);
            if (!itemTypeToModel.ContainsKey(itemType))
            {
                return null;
            }
            return itemTypeToModel[itemType];
        }

        public List<ItemModel> GetAllItemsOfTypeSortByRarity<T>()
        {
            List<ItemModel> items = GetAllItemsOfType<T>();
            if (items == null)
            {
                return null;
            }

            items.Sort((x, y) => ((int)x.RarityType).CompareTo((int)y.RarityType));
            return items;
        }

        public List<ItemModel> GetAllItemsOfRarity(RarityType rarityType)
        {
            List<ItemModel> result = new List<ItemModel>();
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i].RarityType == rarityType)
                {
                    result.Add(AllItems[i]);
                }
            }
            return result;
        }

        public List<ItemModel> GetAllItemsOfTypeByRarity<T>(RarityType rarityType)
        {
            List<ItemModel> result = new List<ItemModel>();
            List<ItemModel> allItemsOfType = GetAllItemsOfType<T>();
            if (allItemsOfType == null)
            {
                return null;
            }

            for (int i = 0; i < allItemsOfType.Count; i++)
            {
                if (allItemsOfType[i].RarityType == rarityType)
                {
                    result.Add(allItemsOfType[i]);
                }
            }
            return result;
        }

        public ItemModel GetItemByID(int itemID)
        {
            if (itemIDToModel.ContainsKey(itemID))
            {
                return itemIDToModel[itemID];
            }
            return null;
        }

        public ItemModel GetItemByName(string itemName)
        {
            if (itemNameToModel.ContainsKey(itemName))
            {
                return itemNameToModel[itemName];
            }
            return null;
        }

        public virtual void AddItemWithStackCount(ItemModel itemModel)
        {
            // increment stack count if item already exists.
            ItemModel existingItemModel = null;
            itemIDToModel.TryGetValue(itemModel.ID, out existingItemModel);
            if (existingItemModel != null)
            {
                existingItemModel.StackedCount += itemModel.StackedCount;
                return;
            }
            
            AllItems.Add(itemModel);
            itemIDToModel.Add(itemModel.ID, itemModel);

            if (itemNameToModel.ContainsKey(itemModel.Name))
            {
                throw new Exception("DUPLICATE ITEM NAME: ItemCollection already contains an item with name: " + itemModel.Name);
            }

            itemNameToModel.Add(itemModel.Name, itemModel);

            List<ItemModel> itemModelsFromType = null;
            Type itemType = itemModel.GetType();
            itemTypeToModel.TryGetValue(itemType, out itemModelsFromType);
            if (itemModelsFromType == null)
            {
                itemModelsFromType = new List<ItemModel>();
                itemTypeToModel.Add(itemType, itemModelsFromType);
            }
            itemModelsFromType.Add(itemModel);
        }

        public virtual void RemoveItem(ItemModel itemModel, bool removeAllFromStack = false)
        {
            if (removeAllFromStack || itemModel.StackedCount == 1)
            {
                AllItems.Remove(itemModel);
                itemIDToModel.Remove(itemModel.ID);
                itemNameToModel.Remove(itemModel.Name);

                Type itemType = itemModel.GetType();
                if (itemTypeToModel.ContainsKey(itemType))
                {
                    List<ItemModel> itemsOfType = itemTypeToModel[itemType];
                    for (int i = 0; i < itemsOfType.Count; i++)
                    {
                        if (itemsOfType[i].ID == itemModel.ID)
                        {
                            itemsOfType.RemoveAt(i);
                            break;
                        }
                    }
                }
                itemModel.StackedCount = 0;
                return;
            }
            itemModel.StackedCount--;
        }

        public virtual void RemoveItemsByCount(ItemModel itemModel, int count)
        {
            if (count >= itemModel.StackedCount)
            {
                RemoveItem(itemModel, true);
                return;
            }
            itemModel.StackedCount -= count;
        }
    }
}
