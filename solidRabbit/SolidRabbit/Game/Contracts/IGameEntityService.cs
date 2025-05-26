// SolidRabbit.Game/Contracts/IGameEntityService.cs
using SolidRabbit.Core.Domain;
using System.Collections.Generic;

namespace SolidRabbit.Game.Contracts
{
    public interface IGameEntityService
    {
        Player GetPlayer(Guid playerId);
        IEnumerable<Player> GetAllPlayers();
        void AddOrUpdatePlayer(Player player);
        void RemovePlayer(Guid playerId);

        Enemy GetEnemy(Guid enemyId);
        IEnumerable<Enemy> GetAllEnemies();
        void AddOrUpdateEnemy(Enemy enemy);
        void RemoveEnemy(Guid enemyId);

        IEnumerable<SolidRabbit.Core.Contracts.IGameEntity> GetAllEntities();
    }
}