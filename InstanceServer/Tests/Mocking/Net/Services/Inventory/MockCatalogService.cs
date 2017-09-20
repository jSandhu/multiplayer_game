using System;
using Common.Inventory.Abilities;
using Common.Rarity;
using Common.Inventory;
using InstanceServer.Core.Net.Services.Inventory;

namespace InstanceServer.Tests.Mocking.Net.Services.Inventory
{
    public class MockCatalogService : CatalogServiceBase
    {
        public static AbilityItemModel FIREBALL_ABILITY = new AbilityItemModel(10000000, "FireBall", "A fireball", RarityType.Common, 0, 0, 
            AbilityEffectType.SpellDamage, AbilityTargetType.EnemySingle, AbilityDurationType.Immediate, 6, 3, 1, true, 0, 0, 0);

        protected override void getCatalog(string catalogID, Action<CatalogModel> onSuccessHandler, Action onFailureHandler)
        {
            CatalogModel catalogModel = new CatalogModel(catalogID);
            catalogModel.AddItemWithStackCount(FIREBALL_ABILITY);
            onSuccessHandler(catalogModel);
        }
    }
}
