using Common.Inventory.Equipment;
using Common.Player;
using InstanceServer.Core.Combat.Behaviours;
using InstanceServer.Core.Net;
using System;

namespace InstanceServer.Core.Player
{
    public class ServerPlayerModel : PlayerModel
    {
        public event Action<ServerPlayerModel> Disconnected;

        public bool IsReady;
        public PlayerCombatBehaviourBase PlayerCombatBehaviour { get; private set; }

        private IClientConnection clientConnection;
        public IClientConnection ClientConnection
        {
            get { return clientConnection; }
            set
            {
                if (clientConnection != null)
                {
                    clientConnection.Disconnected -= onDisconnected;
                }
                clientConnection = value;

                if (clientConnection != null)
                {
                    clientConnection.Disconnected += onDisconnected;
                }
            }
        }

        public ServerPlayerModel(PlayerCombatBehaviourBase playerCombatBehaviour) {
             PlayerCombatBehaviour = playerCombatBehaviour;
        }

        public ServerPlayerModel(PlayerCombatBehaviourBase playerCombatBehaviour, string playerID, string name, EquipmentItemModel equippedWeaponItemModel, 
            EquipmentItemModel equippedOffHandItemModel, EquipmentItemModel equippedArmorItemModel, IClientConnection clientConnection) : 
            base(playerID, name, equippedWeaponItemModel, equippedOffHandItemModel, equippedArmorItemModel)
        {
            this.ClientConnection = clientConnection;
            PlayerCombatBehaviour = playerCombatBehaviour;
        }

        private void onDisconnected(IClientConnection clientConnection)
        {
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }
    }
}
