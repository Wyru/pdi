namespace SolidRabbit.Game.Contracts
{
    public interface IInputService
    {
        (int deltaX, int deltaY) GetMovementInput();
    }
}