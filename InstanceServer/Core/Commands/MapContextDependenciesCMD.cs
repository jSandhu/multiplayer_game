using Common.Inventory;
using DIFramework;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.Net;
using InstanceServer.Core.Net.Services.Inventory;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Core.Net.Services.Session;
using InstanceServer.Core.Room;
using InstanceServer.Core.Room.Game.Controllers;
using InstanceServer.Core.Room.Game.Controllers.CoOpInstanceGame;
using InstanceServer.Core.Room.Game.WorldBuilders;
using System;

namespace InstanceServer.Core.Commands
{
    public class MapContextDependenciesCMD<T> where T: IServiceMappings
    {
        private IServiceMappings serviceMappings;
        private Action onCompleteHandler;
        private DIContainer container;

        public MapContextDependenciesCMD(Action onCompleteHandler)
        {
            this.onCompleteHandler = onCompleteHandler;
        }

        public void Execute()
        {
            serviceMappings = Activator.CreateInstance<T>();

            container = DIContainer.RegisterContainer(InstanceServerApplication.CONTEXT_ID);
            
            // Game controller.
            container.MapType<IGameController, CoOpInstanceGameController>();

            // PlayerCombatBehaviour
            container.MapType<PlayerCombatBehaviourBase, ManualInputBehaviour>();

            // SessionService
            container.RegisterInstance<SessionServiceBase>(serviceMappings.GetSessionServiceInstance());

            // Join Room Controller
            container.RegisterInstance<JoinRoomController>(new JoinRoomController());

            // CatalogService
            CatalogServiceBase catalogService = serviceMappings.GetCatalogServiceInstance();
            container.RegisterInstance<CatalogServiceBase>(catalogService);
            catalogService.GetCatalog("PlayerOwnedItems", onCatalogReceived, onGetCatalogFailed);
        }

        private void onGetCatalogFailed()
        {
            throw new NotImplementedException("Couldn't fetch catalog.");
        }

        private void onCatalogReceived(CatalogModel catalogModel)
        {
            container.RegisterInstance<CatalogModel>(catalogModel);

            // PlayerService
            container.RegisterInstance<PlayerServiceBase>(serviceMappings.GetPlayerServiceInstance());

            // IWorldBuilder
            container.RegisterInstance<IWorldBuilder>(new PlayerBasedWorldBuilder());

            // DropTableService
            DropTableServiceBase dropTableService = serviceMappings.GetDropTableServiceInstance();
            container.RegisterInstance<DropTableServiceBase>(dropTableService);
            dropTableService.GetDropTables(onDropTablesReceived, onGetDropTableFailed);

            // InventoryService
            container.RegisterInstance<InventoryServiceBase>(serviceMappings.GetInventoryService());
        }

        private void onGetDropTableFailed()
        {
            throw new NotImplementedException("Couldn't fetch drop tables.");
        }

        private void onDropTablesReceived(DropTableCollection dropTableCollection)
        {
            container.RegisterInstance<DropTableCollection>(dropTableCollection);
            onCompleteHandler();
        }
    }
}
