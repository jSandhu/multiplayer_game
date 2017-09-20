using InstanceServer.Core.Net;
using InstanceServer.Core.Player;
using System;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.Controllers
{
    /// <summary>
    /// A GameController is used by the RoomContext to carry out a game type.
    /// GameControllers black-box game logic and can define completely different types of games.
    /// </summary>
    public interface IGameController
    {
        event Action GameCompleted;
        bool IsGameActive { get; }
        void SetDifficulty(GameDifficultyType gameDifficultyType);
        void Destroy();
        void StartGame(List<ServerPlayerModel> initialServerPlayerModels);
        bool RejoinPlayer(PlayerAuthenticationModel playerAuthModel, IClientConnection clientConnection);
    }
}
