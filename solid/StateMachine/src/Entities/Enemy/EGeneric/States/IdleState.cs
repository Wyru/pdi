using StateMachine.src.Common.StateMachine.Interfaces;

namespace StateMachine.src.Entities.Enemy.EGeneric.States
{
    public class IdleState : EGenericState
    {
        public IdleState(EGeneric dog) : base(dog) { }

        public override void OnEnter() {
            base.OnEnter();
            context.SetIdleGraphics();
        }

        public override void OnLeave() {}

        public override IState? Update() {

            // Após 3 segundos, começa a patrulhar
            if (StateTimeExpired(3)) {
                return context.stateMachine.GetState<PatrollingState>();
            }

            return null;
        }
    }
}
