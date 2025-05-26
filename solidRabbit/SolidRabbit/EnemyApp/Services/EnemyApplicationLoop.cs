using Microsoft.Extensions.Logging;
using SolidRabbit.Core.Events;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Messaging.Contracts;

namespace SolidRabbit.EnemyApp.Services
{
    public class EnemyApplicationLoop : IGameLoop
    {
        private readonly IGameEngine _gameEngine;
        private readonly IMessagingService _messagingService;
        private readonly ILogger<EnemyApplicationLoop> _logger;

        public EnemyApplicationLoop(
            IGameEngine gameEngine,
            IMessagingService messagingService,
            ILogger<EnemyApplicationLoop> logger) {
            _gameEngine = gameEngine;
            _messagingService = messagingService;
            _logger = logger;
        }

        public async Task StartLoopAsync(Guid localEnemyId, CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                if (_gameEngine.PerformEnemyAction(localEnemyId)) {
                    var enemy = _gameEngine.GetEnemy(localEnemyId);
                    if (enemy != null) {
                        await PublishEnemyPosition(enemy);
                    }
                }
                await Task.Delay(500, cancellationToken);
            }
            _logger.LogInformation("Enemy application loop ended.");
        }

        private async Task PublishEnemyPosition(SolidRabbit.Core.Domain.Enemy enemy) {
            var enemyEvent = new EnemyMovedEvent {
                EnemyId = enemy.Id,
                X = enemy.CurrentPosition.X,
                Y = enemy.CurrentPosition.Y
            };
            await _messagingService.PublishAsync(
                exchangeName: "enemy_movement_exchange",
                routingKey: "enemy.moved",
                message: enemyEvent);
            _logger.LogDebug($"Published enemy position: ({enemy.CurrentPosition.X},{enemy.CurrentPosition.Y})");
        }
    }
}