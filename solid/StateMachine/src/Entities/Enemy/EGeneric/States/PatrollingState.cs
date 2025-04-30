namespace StateMachine.src.Entities.Enemy.EGeneric.States
{
    public class PatrollingState:EGenericState
    {
        public PatrollingState(EGeneric context) : base(context) {
        }

        public override void OnEnter() {
            base.OnEnter();
        }

        public override void OnLeave() {
            Console.WriteLine("Inimigo saindo da patrulha");
        }

        public override EGenericState? Update() {
            Console.WriteLine("Inimigo está patrulhando...");

            // Após 4 segundos, volta para estado ocioso
            if (StateTimeExpired(3)) {
                return context.stateMachine.GetState<IdleState>();
            }

            return null;
        }
    }
}
