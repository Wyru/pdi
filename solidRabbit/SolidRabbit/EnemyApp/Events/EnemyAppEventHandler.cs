using Microsoft.Extensions.Logging;

using SolidRabbit.Core.Events;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Messaging.Contracts;

namespace SolidRabbit.EnemyApp.Events
{
    public class EnemyAppEventHandler
    {
        private readonly IMessagingService messagingService;
        private readonly IGameEngine gameEngine; // Injetado
        private Guid localEnemyId; // Para identificar as próprias mensagens

        public EnemyAppEventHandler(IMessagingService messagingService, IGameEngine gameEngine, ILogger<EnemyAppEventHandler> logger) {
            this.messagingService = messagingService;
            this.gameEngine = gameEngine;
        }

        public async Task InitializeSubscriptions(Guid localEnemyId) {
            this.localEnemyId = localEnemyId;

            // Inimigo se subscreve para receber atualizações de players
            await messagingService.SubscribeAsync<PlayerMovedEvent>(
                queueName: $"player_updates_for_enemy_app_{this.localEnemyId}", // Fila exclusiva para este app
                exchangeName: "player_movement_exchange",
                routingKey: "player.moved",
                handler: HandlePlayerMovedEvent);

            // Inimigo se subscreve para receber atualizações de outros inimigos (não ele mesmo)
            await messagingService.SubscribeAsync<EnemyMovedEvent>(
                queueName: $"other_enemy_updates_for_enemy_app_{this.localEnemyId}", // Fila exclusiva para este app
                exchangeName: "enemy_movement_exchange",
                routingKey: "enemy.moved",
                handler: HandleEnemyMovedEvent);

            // Inimigo se subscreve para remover players (quando outro player se desconecta)
            await messagingService.SubscribeAsync<PlayerRemovedEvent>(
                queueName: $"player_removed_updates_for_enemy_app_{this.localEnemyId}",
                exchangeName: "player_removed_exchange",
                routingKey: "player.removed",
                handler: HandlePlayerRemovedEvent);

            // Inimigo se subscreve para remover inimigos (quando outro inimigo se desconecta)
            await messagingService.SubscribeAsync<EnemyRemovedEvent>(
                queueName: $"enemy_removed_updates_for_enemy_app_{this.localEnemyId}",
                exchangeName: "enemy_removed_exchange",
                routingKey: "enemy.removed",
                handler: HandleEnemyRemovedEvent);

        }

        private void HandlePlayerMovedEvent(PlayerMovedEvent playerEvent) {
            gameEngine.UpdatePlayerPosition(playerEvent.PlayerId, playerEvent.X, playerEvent.Y, playerEvent.R, playerEvent.G, playerEvent.B);
        }

        private void HandleEnemyMovedEvent(EnemyMovedEvent enemyEvent) {
            // Inimigo não precisa processar sua própria mensagem de movimento se ele já está chamando o GameEngine
            // Mas precisa processar a de outros inimigos
            if (enemyEvent.EnemyId == localEnemyId) return;
            gameEngine.UpdateEnemyPosition(enemyEvent.EnemyId, enemyEvent.X, enemyEvent.Y);
        }

        private void HandlePlayerRemovedEvent(PlayerRemovedEvent playerRemovedEvent) {
            gameEngine.RemoveEntity(playerRemovedEvent.PlayerId);
        }

        private void HandleEnemyRemovedEvent(EnemyRemovedEvent enemyRemovedEvent) {
            // Inimigo não precisa processar sua própria mensagem de remoção
            if (enemyRemovedEvent.EnemyId == localEnemyId) return;
            gameEngine.RemoveEntity(enemyRemovedEvent.EnemyId);
        }
    }
}