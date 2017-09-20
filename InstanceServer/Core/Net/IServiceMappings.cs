using InstanceServer.Core.Net.Services.Inventory;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Net.Services.Session;

namespace InstanceServer.Core.Net
{
    public interface IServiceMappings
    {
        CatalogServiceBase GetCatalogServiceInstance();
        SessionServiceBase GetSessionServiceInstance();
        PlayerServiceBase GetPlayerServiceInstance();
        DropTableServiceBase GetDropTableServiceInstance();
        InventoryServiceBase GetInventoryService();
    }
}
