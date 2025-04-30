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
        public Player() :base(){
        }

        public override string Render() {
            return "P";
        }

    }
}
