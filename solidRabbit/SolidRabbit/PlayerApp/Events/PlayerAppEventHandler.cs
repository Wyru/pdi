// SolidRabbit.PlayerApp/Events/PlayerAppEventHandler.cs
using Microsoft.Extensions.Logging;
using SolidRabbit.Core.Events;
using SolidRabbit.Game.Contracts; // Depende do GameEngine
using SolidRabbit.Messaging.Contracts;
using SolidRabbit.PlayerApp.Services;

namespace SolidRabbit.PlayerApp.Events
{
    public class PlayerAppEventHandler
    {
        private readonly PlayerApplicationLoop appLoop;
        private readonly IGameEngine gameEngine;
        private readonly IMessagingService messagingService;
        private Guid localPlayerId;

        public PlayerAppEventHandler(IMessagingService messagingService, IGameEngine gameEngine, PlayerApplicationLoop appLoop) {
            this.messagingService = messagingService;
            this.gameEngine = gameEngine;
            this.appLoop = appLoop;
        }

        public async Task InitializeSubscriptions(Guid localPlayerId) {
            this.localPlayerId = localPlayerId;

            // Player se subscreve para receber atualizações de todos os players (incluindo ele mesmo, para consistência, mas deve ignorar a si mesmo)
            await messagingService.SubscribeAsync<PlayerMovedEvent>(
                queueName: $"player_movement_updates_for_player_app_{this.localPlayerId}", // Fila exclusiva para este app
                exchangeName: "player_movement_exchange",
                routingKey: "player.moved",
                handler: HandlePlayerMovedEvent);

            // Player se subscreve para receber atualizações de inimigos
            await messagingService.SubscribeAsync<EnemyMovedEvent>(
                queueName: $"enemy_movement_updates_for_player_app_{this.localPlayerId}", // Fila exclusiva para este app
                exchangeName: "enemy_movement_exchange",
                routingKey: "enemy.moved",
                handler: HandleEnemyMovedEvent);

            // Player se subscreve para remover players (quando outro player se desconecta)
            await messagingService.SubscribeAsync<PlayerRemovedEvent>( // Você precisará criar esta classe de evento
                queueName: $"player_removed_updates_for_player_app_{this.localPlayerId}",
                exchangeName: "player_removed_exchange",
                routingKey: "player.removed",
                handler: HandlePlayerRemovedEvent);

        }

        private void HandlePlayerMovedEvent(PlayerMovedEvent playerEvent) {
            // Enfileira a ação para o loop principal processar
            appLoop.EnqueueGameAction(() => {
                gameEngine.UpdatePlayerPosition(playerEvent.PlayerId, playerEvent.X, playerEvent.Y, playerEvent.R, playerEvent.G, playerEvent.B);
            });
        }

        private void HandleEnemyMovedEvent(EnemyMovedEvent enemyEvent) {
            // Enfileira a ação para o loop principal processar
            appLoop.EnqueueGameAction(() => {
                gameEngine.UpdateEnemyPosition(enemyEvent.EnemyId, enemyEvent.X, enemyEvent.Y);
            });
        }

        private void HandlePlayerRemovedEvent(PlayerRemovedEvent playerRemovedEvent) {
            // Enfileira a ação para o loop principal processar
            appLoop.EnqueueGameAction(() => {
                gameEngine.RemoveEntity(playerRemovedEvent.PlayerId);
            });
        }
    }
}