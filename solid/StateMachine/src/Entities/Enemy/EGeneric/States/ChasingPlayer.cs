using StateMachine.src.Common.StateMachine.Interfaces;


namespace StateMachine.src.Entities.Enemy.EGeneric.States
{
    public class ChasingPlayer: EGenericState
    {

        public ChasingPlayer(EGeneric enemy) : base(enemy) { }

        DateTime lastMovement;


        public override void OnEnter() {
            base.OnEnter();
            context.SetChasingGraphics();
            lastMovement = DateTime.Now;

        }

        public override void OnLeave() { }

        public override IState? Update() {

            if ((DateTime.Now - lastMovement).TotalSeconds >= .3f) {
                context.MoveTowardPlayer();
                lastMovement = DateTime.Now;
            }

            if (StateTimeExpired(20)) {
                return context.stateMachine.GetState<PatrollingState>();
            }

            return null;
        }
    }
}
