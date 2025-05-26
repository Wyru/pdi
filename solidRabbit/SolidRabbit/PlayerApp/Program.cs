using Microsoft.Extensions.DependencyInjection;
using SolidRabbit.PlayerApp.Services;
using SolidRabbit.PlayerApp.Events;
using SolidRabbit.Game.Contracts;

namespace SolidRabbit.PlayerApp
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var host = Startup.CreateHostBuilder(args).Build();

            var messagingService = host.Services.GetRequiredService<SolidRabbit.Messaging.Contracts.IMessagingService>();
            messagingService.StartConsuming();

            var gameEngine = host.Services.GetRequiredService<IGameEngine>();

            var random = new Random();
            System.Drawing.Color playerColor = System.Drawing.Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            var localPlayer = gameEngine.RegisterPlayer(playerColor);

            var eventHandler = host.Services.GetRequiredService<PlayerAppEventHandler>();
            await eventHandler.InitializeSubscriptions(localPlayer.Id);

            var appLoop = host.Services.GetRequiredService<PlayerApplicationLoop>();
            await appLoop.StartLoopAsync(localPlayer.Id, CancellationToken.None);

            messagingService.Dispose();
            host.Dispose();
        }
    }
}