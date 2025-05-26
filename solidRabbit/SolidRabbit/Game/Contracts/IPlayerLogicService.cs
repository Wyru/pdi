using SolidRabbit.Core.Domain;

namespace SolidRabbit.Game.Contracts
{
    public interface IPlayerLogicService
    {
        bool TryMovePlayer(Guid playerId, int deltaX, int deltaY);
        Player CreateNewPlayer(Position initialPosition, System.Drawing.Color color);
    }
}