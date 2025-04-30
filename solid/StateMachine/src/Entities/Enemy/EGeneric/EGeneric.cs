using StateMachine.src.Common;
using System.Xml.Linq;

namespace StateMachine.src.Entities.Enemy.EGeneric
{
    public class EGeneric : GameObject
    {
        public bool IsAlive { get; private set; } =  true;
        
        public readonly string Name;

        public readonly EGenericStateMachine stateMachine;


        public EGeneric(string name): base() {
            Name = name;
            stateMachine = new EGenericStateMachine(this);
            stateMachine.Start();
        }

        public void Update() {
            stateMachine.Update();
        }

        public void TakeDamage() {
            IsAlive = false;
        }

        override public string Render() {
            if (IsAlive) return "E";
            return "-";
        }

    }
}
