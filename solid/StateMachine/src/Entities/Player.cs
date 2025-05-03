using Raylib_cs;
using StateMachine.src.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.src.Entities
{
    public class Player : GameObject
    {

        public Player() :base(){}

        public override void Render() {
            
            // Desenhe o player no centro
            Raylib.DrawRectangle(transform.X * Settings.CELL_GRID_SIZE, transform.Y * Settings.CELL_GRID_SIZE, Settings.CELL_GRID_SIZE, Settings.CELL_GRID_SIZE, Color.RayWhite);

        }

    }
}
