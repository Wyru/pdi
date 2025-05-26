using SolidRabbit.Core.Domain;


namespace SolidRabbit.Core.Contracts
{
    public interface IGameEntity
    {
        Guid Id { get; }
        Position CurrentPosition { get; }
        void SetPosition(Position newPosition);
    }
}
