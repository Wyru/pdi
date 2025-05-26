// SolidRabbit.Game/Services/GameEntityService.cs
using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Core.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SolidRabbit.Game.Services
{
    public class GameEntityService : IGameEntityService
    {
        private readonly ConcurrentDictionary<Guid, Player> _players = new ConcurrentDictionary<Guid, Player>();
        private readonly ConcurrentDictionary<Guid, Enemy> _enemies = new ConcurrentDictionary<Guid, Enemy>();

        public Player GetPlayer(Guid playerId) => _players.GetValueOrDefault(playerId);
        public IEnumerable<Player> GetAllPlayers() => _players.Values;
        public void AddOrUpdatePlayer(Player player) {
            _players.AddOrUpdate(player.Id, player, (id, existing) => {
                existing.SetPosition(player.CurrentPosition);
                existing.Color = player.Color;
                return existing;
            });
        }
        public void RemovePlayer(Guid playerId) => _players.TryRemove(playerId, out _);

        public Enemy GetEnemy(Guid enemyId) => _enemies.GetValueOrDefault(enemyId);
        public IEnumerable<Enemy> GetAllEnemies() => _enemies.Values;
        public void AddOrUpdateEnemy(Enemy enemy) {
            _enemies.AddOrUpdate(enemy.Id, enemy, (id, existing) => {
                existing.SetPosition(enemy.CurrentPosition);
                return existing;
            });
        }
        public void RemoveEnemy(Guid enemyId) => _enemies.TryRemove(enemyId, out _);

        public IEnumerable<IGameEntity> GetAllEntities() {
            return _players.Values.Cast<IGameEntity>().Concat(_enemies.Values.Cast<IGameEntity>());
        }
    }
}