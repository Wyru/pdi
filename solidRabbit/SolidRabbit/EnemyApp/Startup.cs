// SolidRabbit.EnemyApp/Startup.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Game.Services;
using SolidRabbit.Messaging.Contracts;
using SolidRabbit.Messaging.Models;
using SolidRabbit.Messaging.RabbitMQ;
using SolidRabbit.EnemyApp.Events;
using SolidRabbit.EnemyApp.Services;

namespace SolidRabbit.EnemyApp
{
    public static class Startup
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) => {
                    var rabbitMqSettings = new RabbitMqConnectionSettings();
                    hostContext.Configuration.GetSection("RabbitMQ").Bind(rabbitMqSettings);
                    services.AddSingleton(rabbitMqSettings);

                    services.AddSingleton<IMessagingService, RabbitMqService>();

                    services.AddSingleton<IGameGrid, GameGrid>();
                    services.AddSingleton<IGameEntityService, GameEntityService>();
                    services.AddSingleton<IPlayerLogicService, PlayerLogicService>();
                    services.AddSingleton<IEnemyAIService, EnemyAIService>();
                    services.AddSingleton<IGameEngine, GameEngine>();

                    services.AddSingleton<EnemyApplicationLoop>();
                    services.AddSingleton<EnemyAppEventHandler>();
                });
    }
}