using Microsoft.Extensions.DependencyInjection;
using SolidRabbit.EnemyApp.Services;
using SolidRabbit.EnemyApp.Events;
using SolidRabbit.Game.Contracts; 
using SolidRabbit.Messaging.Contracts;

namespace SolidRabbit.EnemyApp
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var host = Startup.CreateHostBuilder(args).Build();

            var messagingService = host.Services.GetRequiredService<IMessagingService>();
            messagingService.StartConsuming();

            var gameEngine = host.Services.GetRequiredService<IGameEngine>();
            var localEnemy = gameEngine.RegisterEnemy();

            var eventHandler = host.Services.GetRequiredService<EnemyAppEventHandler>();
            await eventHandler.InitializeSubscriptions(localEnemy.Id);

            var appLoop = host.Services.GetRequiredService<EnemyApplicationLoop>();
            var cts = new CancellationTokenSource();
            await appLoop.StartLoopAsync(localEnemy.Id, cts.Token);

            Console.WriteLine("Enemy running. Press any key to exit.");
            Console.ReadKey();

            cts.Cancel();
            gameEngine.RemoveEntity(localEnemy.Id);
            await PublishEnemyRemoved(messagingService, localEnemy.Id);

            messagingService.Dispose();
            host.Dispose();
        }

        private static async Task PublishEnemyRemoved(IMessagingService messagingService, Guid enemyId) {
            var enemyRemovedEvent = new SolidRabbit.Core.Events.EnemyRemovedEvent {
                EnemyId = enemyId
            };
            await messagingService.PublishAsync(
                exchangeName: "enemy_removed_exchange",
                routingKey: "enemy.removed",
                message: enemyRemovedEvent);
        }
    }
}