using Common.World;
using InstanceServer.Core.Player;
using System.Collections.Generic;

namespace InstanceServer.Core.Room.Game.WorldBuilders
{
    public interface IWorldBuilder
    {
        WorldModel Build(List<ServerPlayerModel> serverPlayerModels, GameDifficultyType gameDifficultyType, int npcTeamID);
    }
}
