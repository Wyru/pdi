// SolidRabbit.Core/Domain/Player.cs
using SolidRabbit.Core.Contracts;
using System.Drawing;

namespace SolidRabbit.Core.Domain
{
    public class Player : IGameEntity
    {
        public Guid Id { get; }
        public Position CurrentPosition { get; private set; }
        public Color Color { get; set; }

        public Player(Position initialPosition, Color color) {
            Id = Guid.NewGuid();
            CurrentPosition = initialPosition;
            Color = color;
        }

        public Player(Position initialPosition, Color color, Guid id) {
            Id = id;
            CurrentPosition = initialPosition;
            Color = color;
        }

        public void SetPosition(Position newPosition) {
            CurrentPosition = newPosition;
        }

        public void Move(int deltaX, int deltaY, int gridWidth, int gridHeight) {
            int newX = Math.Clamp(CurrentPosition.X + deltaX, 0, gridWidth - 1);
            int newY = Math.Clamp(CurrentPosition.Y + deltaY, 0, gridHeight - 1);
            SetPosition(new Position(newX, newY));
        }
    }
}