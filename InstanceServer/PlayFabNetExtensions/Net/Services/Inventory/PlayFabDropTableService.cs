using InstanceServer.Core.Net.Services.Inventory;
using System;
using Common.Inventory;
using PlayFab.AdminModels;
using PlayFab;
using InstanceServer.Core.Logging;

namespace InstanceServer.PlayFabNetExtensions.Net.Services.Inventory
{
    public class PlayFabDropTableService : DropTableServiceBase
    {
        public PlayFabDropTableService()
        {
            PlayFabSetup.Init();
        }

        protected override void getDropTables(CatalogModel catalogModel, Action<DropTableCollection> onSuccessHandler, Action onFailureHandler)
        {
            GetRandomResultTablesRequest request = new GetRandomResultTablesRequest() { CatalogVersion = catalogModel.ID};
            PlayFabAdminAPI.GetRandomResultTablesAsync(request).ContinueWith(t => {
                if (t.Result.Error != null)
                {
                    Log.Error("PlayFabDropTableService drop tables error: " + t.Result.Error.ErrorMessage);
                    onFailureHandler();
                }
                else
                {
                    DropTableCollection dropTableCollection = new DropTableCollection();
                    foreach(var kvp in t.Result.Result.Tables)
                    {
                        int id = int.Parse(kvp.Value.TableId);
                        DropTableModel dropTableModel = new DropTableModel(id);
                        dropTableCollection.Add(id, dropTableModel);
                        for (int i = 0; i < kvp.Value.Nodes.Count; i++)
                        {
                            int itemID = int.Parse(kvp.Value.Nodes[i].ResultItem);
                            float weight = kvp.Value.Nodes[i].Weight;
                            ItemModel itemModel = catalogModel.GetItemByID(itemID);
                            dropTableModel.AddItem(itemModel, weight);
                        }
                    }
                    onSuccessHandler(dropTableCollection);
                }
            });
        }
    }
}
