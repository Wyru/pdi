using SolidRabbit.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidRabbit.Core.Domain
{
    public class Enemy : IGameEntity
    {
        public Guid Id { get; }
        public Position CurrentPosition { get; private set; }

        public Enemy(Position initialPosition, Guid? id = null) {
            Id = id ?? Guid.NewGuid();
            CurrentPosition = initialPosition;
        }

        public void SetPosition(Position newPosition) {
            CurrentPosition = newPosition;
        }

        public void MoveTowards(Position target, int gridWidth, int gridHeight) {
            int deltaX = 0;
            int deltaY = 0;

            if (target.X > CurrentPosition.X) deltaX = 1;
            else if (target.X < CurrentPosition.X) deltaX = -1;

            if (target.Y > CurrentPosition.Y) deltaY = 1;
            else if (target.Y < CurrentPosition.Y) deltaY = -1;

            int newX = Math.Clamp(CurrentPosition.X + deltaX, 0, gridWidth - 1);
            int newY = Math.Clamp(CurrentPosition.Y + deltaY, 0, gridHeight - 1);
            SetPosition(new Position(newX, newY));
        }
    }
}
