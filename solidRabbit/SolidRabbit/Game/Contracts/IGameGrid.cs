using SolidRabbit.Core.Domain;

namespace SolidRabbit.Game.Contracts
{
    public interface IGameGrid
    {
        int Width { get; }
        int Height { get; }
        bool IsPositionValid(Position position);
    }
}
