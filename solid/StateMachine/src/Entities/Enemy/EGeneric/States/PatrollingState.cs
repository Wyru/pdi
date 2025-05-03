namespace StateMachine.src.Entities.Enemy.EGeneric.States
{
    public class PatrollingState:EGenericState
    {
        public PatrollingState(EGeneric context) : base(context) {}

        DateTime lastMovement;

        Random random = new ();

        float patrollingSpeed = .3f;


        public override void OnEnter() {
            base.OnEnter();
            lastMovement = DateTime.Now;
            context.SetIdlePatrollingGraphics();

            patrollingSpeed =   1 - random.NextSingle();
            patrollingSpeed = MathF.Max(patrollingSpeed, .3f);

        }

        public override void OnLeave() {}

        public override EGenericState? Update() {

            if (context.CanSeePlayer()) {
                return context.stateMachine.GetState<ChasingPlayer>();
            }

            if ((DateTime.Now - lastMovement).TotalSeconds >= patrollingSpeed) {
                context.RandomMovement();
                lastMovement = DateTime.Now;
            }

            if (StateTimeExpired(10)) {
                return context.stateMachine.GetState<IdleState>();
            }

            return null;
        }
    }
}
