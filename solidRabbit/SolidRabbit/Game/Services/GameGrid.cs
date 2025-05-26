using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Game.Constants;

namespace SolidRabbit.Game.Services
{
    public class GameGrid : IGameGrid
    {
        public int Width { get; }
        public int Height { get; }

        public GameGrid() {
            Width = GameConstants.GridWidth;
            Height = GameConstants.GridHeight;
        }

        public bool IsPositionValid(Position position) {
            return position.X >= 0 && position.X < Width &&
                   position.Y >= 0 && position.Y < Height;
        }
    }
}
