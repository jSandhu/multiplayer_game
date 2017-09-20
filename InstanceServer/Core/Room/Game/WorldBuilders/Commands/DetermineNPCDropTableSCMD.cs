using Common.Inventory;
using DIFramework;
using InstanceServer.Core.Npcs;

namespace InstanceServer.Core.Room.Game.WorldBuilders.Commands
{
    public static class DetermineNPCDropTableSCMD
    {
        private static DropTableCollection dropTableCollection;

        public static void Execute(ServerNpcModel serverNPCModel)
        {
            if (dropTableCollection == null)
            {
                dropTableCollection = DIContainer.GetInstanceByContextID<DropTableCollection>(InstanceServerApplication.CONTEXT_ID);
            }

            // TODO: determine drop table.  For now just use the first one available.
            foreach(var kvp in dropTableCollection)
            {
                serverNPCModel.DropTableModel = kvp.Value;
                return;
            }
        }
    }
}
