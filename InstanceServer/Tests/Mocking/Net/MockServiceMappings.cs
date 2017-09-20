using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Services.Inventory;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Net.Services.Session;
using InstanceServer.Tests.Mocking.Net.Services.Inventory;
using InstanceServer.Tests.Mocking.Net.Services.Session;

namespace InstanceServer.Tests.Mocking.Net
{
    public class MockServiceMappings : IServiceMappings
    {
        public CatalogServiceBase GetCatalogServiceInstance()
        {
            return new MockCatalogService();
        }

        public DropTableServiceBase GetDropTableServiceInstance()
        {
            throw new System.NotImplementedException();
        }

        public InventoryServiceBase GetInventoryService()
        {
            throw new System.NotImplementedException();
        }

        public PlayerServiceBase GetPlayerServiceInstance()
        {
            return null;
        }

        public SessionServiceBase GetSessionServiceInstance()
        {
            return new MockSessionService();
        }
    }
}
