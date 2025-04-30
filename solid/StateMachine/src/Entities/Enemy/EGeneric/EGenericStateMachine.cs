using StateMachine.src.Common.StateMachine.Interfaces;
using StateMachine.src.Entities.Enemy.EGeneric.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.src.Entities.Enemy.EGeneric
{
    public class EGenericStateMachine:IStateMachine
    {
        readonly EGeneric context;

        private static readonly List<Type> validStatesTypes =
        [
            typeof(IdleState),
            typeof(PatrollingState)
        ];

        private readonly List<EGenericState> states;

        public EGenericState? currentState;

        public EGenericStateMachine(EGeneric context) {
            this.context = context;

            states = [];

            foreach (Type type in validStatesTypes) {
                if (Activator.CreateInstance(type, [context]) is EGenericState state)
                    states.Add(state);
            }

        }

        public void Start() {
            currentState = GetState<IdleState>();
        }

        public void Update() {
            if (currentState == null) return;

            var nextState = currentState.Update();

            ChangeState(nextState);

        }

        public EGenericState? GetState<T> () {
            return states.FirstOrDefault((s) => s is T);
        }

        void ChangeState(IState nextState) {
            if (nextState == null) return;
            if (currentState == null) return;

            currentState.OnLeave();
            currentState = (EGenericState) nextState;
            currentState.OnEnter();
        }
    }


}
