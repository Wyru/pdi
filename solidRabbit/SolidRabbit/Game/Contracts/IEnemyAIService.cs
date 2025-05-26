using SolidRabbit.Core.Domain;

namespace SolidRabbit.Game.Contracts
{
    public interface IEnemyAIService
    {
        bool DetermineAndMove(Guid enemyId);
        Enemy CreateNewEnemy(Position initialPosition);
    }
}