using SolidRabbit.Core.Events;
using SolidRabbit.Game.Constants;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Messaging.Contracts;
using System.Collections.Concurrent;

namespace SolidRabbit.PlayerApp.Services
{
    public class PlayerApplicationLoop : IGameLoop
    {
        private readonly IGameRenderer renderer;
        private readonly IInputService inputService;
        private readonly IGameEngine gameEngine;
        private readonly IGameGrid gameGrid;
        private readonly IMessagingService messagingService;

        Guid localPlayerId;

        private readonly ConcurrentQueue<Action> gameActions = new ConcurrentQueue<Action>();

        public PlayerApplicationLoop(
            IGameRenderer renderer,
            IInputService inputService,
            IGameEngine gameEngine,
            IGameGrid gameGrid,
            IMessagingService messagingService
            ) {
            this.renderer = renderer;
            this.inputService = inputService;
            this.gameEngine = gameEngine;
            this.gameGrid = gameGrid;
            this.messagingService = messagingService;
        }


        public void EnqueueGameAction(Action action) {
            gameActions.Enqueue(action);
        }

        public async Task StartLoopAsync(Guid localPlayerId, CancellationToken cancellationToken) {

            this.localPlayerId = localPlayerId;

            renderer.Init();

            var player = gameEngine.GetPlayer(localPlayerId);
            Task.Run(async () => await PublishPlayerPosition(player));

            while (!renderer.ShouldClose() && !cancellationToken.IsCancellationRequested) { 
                await Update();
                UpdateScreen();
            }

            renderer.Close();
            gameEngine.RemoveEntity(localPlayerId);
            await PublishPlayerRemoved(localPlayerId);
        }

        private async Task PublishPlayerPosition(SolidRabbit.Core.Domain.Player player) {
            var playerEvent = new PlayerMovedEvent {
                PlayerId = player.Id,
                X = player.CurrentPosition.X,
                Y = player.CurrentPosition.Y,
                R = player.Color.R,
                G = player.Color.G,
                B = player.Color.B
            };
            await messagingService.PublishAsync(
                exchangeName: "player_movement_exchange",
                routingKey: "player.moved",
                message: playerEvent);
        }

        private async Task PublishPlayerRemoved(Guid playerId) {
            var playerRemovedEvent = new PlayerRemovedEvent {
                PlayerId = playerId
            };
            await messagingService.PublishAsync(
                exchangeName: "player_removed_exchange",
                routingKey: "player.removed",
                message: playerRemovedEvent);
        }

        async Task Update() {
            var (deltaX, deltaY) = inputService.GetMovementInput();

            if (deltaX != 0 || deltaY != 0) {
                EnqueueGameAction(() =>
                {
                    if (gameEngine.TryMovePlayer(localPlayerId, deltaX, deltaY)) {
                        var player = gameEngine.GetPlayer(localPlayerId);
                        if (player != null) {
                            Task.Run(async () => await PublishPlayerPosition(player)); // Publica em outro thread para não bloquear o loop de jogo
                        }
                    }
                });
            }

            while (gameActions.TryDequeue(out var action)) {
                action.Invoke();
            }
        }

        void UpdateScreen() {
            var allPlayers = gameEngine.GetAllPlayers().ToList();
            var allEnemies = gameEngine.GetAllEnemies().ToList();

            renderer.BeginDrawing();
            renderer.DrawGrid(gameGrid.Width, gameGrid.Height, GameConstants.CellSize);

            foreach (var player in allPlayers) {
                renderer.DrawEntity(player.CurrentPosition, GameConstants.CellSize, player.Color);
            }

            foreach (var enemy in allEnemies) {
                renderer.DrawEntity(enemy.CurrentPosition, GameConstants.CellSize, System.Drawing.Color.Red);
            }
            renderer.EndDrawing();
        }


        
    }
}