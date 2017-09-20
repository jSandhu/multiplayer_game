using System;

namespace Common.Inventory
{
    /// <summary>
    /// Catalog containing various ItemModel implementations.
    /// 
    /// Used as source of truth for item descriptions and custom data, as player owned instances shouldn't retain
    /// that data.
    /// </summary>
    public class CatalogModel : ItemCollection
    {
        public string ID;

        public CatalogModel(string id)
        {
            ID = id;
        }

        public override void AddItemWithStackCount(ItemModel itemModel)
        {
            if (itemIDToModel.ContainsKey(itemModel.ID))
            {
                throw new Exception("Item with ID: " + itemModel.ID + " already exists in this catalog.");
            }

            base.AddItemWithStackCount(itemModel);
        }
    }
}
