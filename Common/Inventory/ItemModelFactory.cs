using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Common.Inventory
{
    public static class ItemModelFactory
    {
        private static Dictionary<ItemType, ItemModel> itemTypeToInstance;

        private static void buildDictionary()
        {
            itemTypeToInstance = new Dictionary<ItemType, ItemModel>();

            var typelist = Assembly.GetAssembly(typeof(ItemModel)).GetTypes().Where(t =>
                t.IsClass &&
                t.IsAbstract == false &&
                t.IsSubclassOf(typeof(ItemModel))
            );

            foreach (var type in typelist)
            {
                ItemModel itemModel = Activator.CreateInstance(type) as ItemModel;
                itemTypeToInstance.Add(itemModel.ItemType, itemModel);
            }
        }

        public static ItemModel GetItemModelFromItemType(ItemType itemType)
        {
            if (itemTypeToInstance == null)
            {
                buildDictionary();
            }
            return itemTypeToInstance[itemType].Clone();
        }

    }
}
