using SolidRabbit.Core.Domain;
using System.Drawing;

namespace SolidRabbit.Game.Contracts
{
    public interface IGameEngine
    {
        Player RegisterPlayer(Color color);
        Enemy RegisterEnemy();

        bool TryMovePlayer(Guid playerId, int deltaX, int deltaY);
        IEnumerable<Player> GetAllPlayers();
        Player? GetPlayer(Guid id);
        IEnumerable<Enemy> GetAllEnemies();

        Enemy? GetEnemy(Guid id);
        bool PerformEnemyAction(Guid enemyId);

        void UpdatePlayerPosition(Guid playerId, int x, int y, int r, int g, int b);
        void UpdateEnemyPosition(Guid enemyId, int x, int y);
        void RemoveEntity(Guid entityId);
    }
}