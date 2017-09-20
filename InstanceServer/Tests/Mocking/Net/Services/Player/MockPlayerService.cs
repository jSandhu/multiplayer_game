using System;
using InstanceServer.Core.Player;
using System.Threading.Tasks;
using DIFramework;
using Common.Inventory.Abilities;
using Common.Combat;
using Common.Inventory;
using InstanceServer.Core.Net.Services.Player;
using InstanceServer.Tests.Mocking.Net.Services.Inventory;
using Common.Player;
using InstanceServer.Core.Combat.Behaviours;

namespace InstanceServer.Tests.Mocking.Net.Services.Player
{
    public class MockPlayerService : PlayerServiceBase
    {
        private CatalogModel catalogModel;

        public MockPlayerService()
        {
            this.catalogModel = DIContainer.GetInstanceByContextID<CatalogModel>(Core.InstanceServerApplication.CONTEXT_ID);
        }

        protected override void getPlayerModelByID(PlayerAuthenticationModel playerAuthModel, PlayerCombatBehaviourBase playerCombatBehaviourBase,
            Action<ServerPlayerModel> onSuccessHandler, Action onFailureHandler)
        {
            if (playerAuthModel.UserID == "testFail")
            {
                onFailureHandler();
                return;
            }

            ServerPlayerModel playerModel = new ServerPlayerModel(playerCombatBehaviourBase);
            playerModel.PlayerID = playerAuthModel.UserID;
            playerModel.Name = playerAuthModel.UserName;

            AbilityItemModel[] mockAbilities = new AbilityItemModel[]
            {
                MockCatalogService.FIREBALL_ABILITY
            };

            playerModel.CombatantModel = new CombatantModel(1, 100, 1, 100, 1, 1, 1, 10, 0f, 10, 0f, PlayerModel.DEFAULT_TEAM_ID, mockAbilities);

            Task.Delay(100).ContinueWith(t => { onSuccessHandler(playerModel); });
        }
    }
}
