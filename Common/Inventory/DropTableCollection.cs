using System.Collections.Generic;

namespace Common.Inventory
{
    public class DropTableCollection : Dictionary<int, DropTableModel>
    {
        public void GetDropTablesContainingItemID(int itemID, out List<DropTableModel> result)
        {
            result = new List<DropTableModel>();

            foreach (var kvp in this)
            {
                if (kvp.Value.ContainsItemWithID(itemID))
                {
                    result.Add(kvp.Value);
                }
            }
        }
    }
}
