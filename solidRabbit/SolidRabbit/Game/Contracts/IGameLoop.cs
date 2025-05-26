namespace SolidRabbit.Game.Contracts
{
    public interface IGameLoop
    {
        Task StartLoopAsync(Guid id, CancellationToken cancellationToken);
    }
}