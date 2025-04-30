namespace StateMachine.src.Common.StateMachine.Interfaces
{
    public interface IState 
    {
        public void OnEnter();
        public IState? Update();
        public void OnLeave();
    }
}
