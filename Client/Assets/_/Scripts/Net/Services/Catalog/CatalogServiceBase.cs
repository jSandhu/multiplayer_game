using Common.Inventory;
using Platform;
using System;

namespace Net.Services.Catalog
{
    public abstract class CatalogServiceBase : ServiceBase
    {
        public override int Priority { get { return (int)PriorityType.Default; } }
        public override bool AvailableForPlatform(PlatformType platformType) { return true; }

        public abstract void GetCatalog(string id, Action<CatalogModel> successHandler, Action failureHandler);
    }
}
