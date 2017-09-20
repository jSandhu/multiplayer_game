using Common.Inventory;
using DIFramework;
using System;

namespace InstanceServer.Core.Net.Services.Inventory
{
    public abstract class DropTableServiceBase
    {
        private CatalogModel catalogModel;

        public void GetDropTables(Action<DropTableCollection> onSuccessHandler, Action onFailureHandler)
        {
            if (catalogModel == null)
            {
                catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(InstanceServerApplication.CONTEXT_ID);
            }
            getDropTables(catalogModel, onSuccessHandler, onFailureHandler);
        }

        protected abstract void getDropTables(CatalogModel catalogModel, Action<DropTableCollection> onSuccessHandler, Action onFailureHandler);
    }
}
