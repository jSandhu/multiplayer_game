using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Services.Inventory;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Net.Services.Session;
using InstanceServer.PlayFabNetExtensions.Net.Services.Inventory;
using InstanceServer.PlayFabNetExtensions.Net.Services.Session;
using InstanceServer.PlayFabNetExtensions.Net.Services.Player;

namespace InstanceServer.PlayFabNetExtensions.Net
{
    public class PlayFabServiceMappings : IServiceMappings
    {
        public CatalogServiceBase GetCatalogServiceInstance()
        {
            return new PlayFabCatalogService();
        }

        public SessionServiceBase GetSessionServiceInstance()
        {
            return new PlayFabSessionService();
        }

        public PlayerServiceBase GetPlayerServiceInstance()
        {
            return new PlayFabPlayerService();
        }

        public DropTableServiceBase GetDropTableServiceInstance()
        {
            return new PlayFabDropTableService();
        }

        public InventoryServiceBase GetInventoryService()
        {
            return new PlayFabInventoryService();
        }
    }
}
