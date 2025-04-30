namespace StateMachine.src.Common
{
    public abstract class GameObject
    {
        public readonly Transform transform;

        protected GameObject() {
            this.transform = new Transform(0,0);
        }

        public virtual string Render() {
            return "X";
        }
    }
}
