using StateMachine.src.Common.StateMachine.Interfaces;

namespace StateMachine.src.Entities.Enemy.EGeneric.States
{
    public abstract class EGenericState : IState
    {
        protected EGeneric context;
        protected DateTime stateEnterTime;

        protected EGenericState(EGeneric context) {
            this.context = context;
        }

        public virtual void OnEnter() {
            stateEnterTime = DateTime.Now;
            Console.WriteLine($"Entrando no estado: {GetType().Name}");
        }

        abstract public void OnLeave();

        abstract public IState Update();

        protected bool StateTimeExpired(float seconds) {
            return (DateTime.Now - stateEnterTime).TotalSeconds >= seconds;
        }
    }
}
