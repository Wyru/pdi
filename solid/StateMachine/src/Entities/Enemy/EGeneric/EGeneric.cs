using Raylib_cs;
using StateMachine.src.Common;
using System;
using System.Xml.Linq;

namespace StateMachine.src.Entities.Enemy.EGeneric
{
    public class EGeneric : GameObject
    {
        public bool IsAlive { get; private set; } =  true;
        
        public readonly string Name;

        public readonly EGenericStateMachine stateMachine;

        Color color = Color.DarkBlue;

        public readonly Player player;

        Random random = new();

        public EGeneric(string name, Player player): base() {
            this.player = player;
            Name = name;
            stateMachine = new EGenericStateMachine(this);
            stateMachine.Start();
        }

        public override void Update() {
            stateMachine.Update();
        }

        public void TakeDamage() {
            IsAlive = false;
        }

        override public void Render() {
            Raylib.DrawRectangle(transform.X * Settings.CELL_GRID_SIZE, transform.Y * Settings.CELL_GRID_SIZE, Settings.CELL_GRID_SIZE, Settings.CELL_GRID_SIZE, color);
        }

        public void SetIdleGraphics() {
            color = Color.DarkBlue;
        }

        public void SetIdlePatrollingGraphics() {
            color = Color.Orange;
        }

        public void SetChasingGraphics() {
            color = Color.Red;
        }

        public bool CanSeePlayer() {
            int distance = transform.Distance(player.transform);
            return distance < 10;
        }


        public void RandomMovement() {
            int direction = random.Next(4);

            switch (direction) {
                case 0:
                    transform.MoveUp();
                    break;
                case 1:
                    transform.MoveRight();
                    break;
                case 2:
                    transform.MoveDown();
                    break;
                case 3:
                    transform.MoveLeft();
                    break;
            }

        }

        public void MoveTowardPlayer() {
            if (transform.X > player.transform.X)
                transform.MoveLeft();
            else if (transform.X < player.transform.X)
                transform.MoveRight();

            if (transform.Y > player.transform.Y)
                transform.MoveUp();
            else if (transform.Y < player.transform.Y)
                transform.MoveDown();
        }



    }
}
