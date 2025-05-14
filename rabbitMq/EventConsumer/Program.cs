using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    private const string ExchangeDirect = "tarefas_exchange";
    private const string QueueTarefas = "tarefas_queue";

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== RABBIT RPG ===");
        Console.WriteLine("Aguardando Quests!");


        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "admin",
            Password = "admin123"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Consumer de notificações (comum a ambos)
        var notifConsumer = new AsyncEventingBasicConsumer(channel);
        notifConsumer.ReceivedAsync += async (model, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"\n[ANÚNCIO] {msg}");
        };


        await channel.ExchangeDeclareAsync(ExchangeDirect, ExchangeType.Direct);
        await channel.QueueDeclareAsync(
            queue: QueueTarefas,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> {
                        { "x-max-priority", 5}
        });
        await channel.QueueBindAsync(queue: QueueTarefas, exchange: ExchangeDirect, routingKey: "quest");

        await channel.BasicQosAsync(0, 1, false); // Justiça entre workers

        var questConsumer = new AsyncEventingBasicConsumer(channel);
        questConsumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var questData = Encoding.UTF8.GetString(body).Split('|');

            if (questData.Length >= 4)
            {
                var (nome, desc, tipo, duracao) = (questData[0], questData[1], int.Parse(questData[2]), int.Parse(questData[3]));

                Console.WriteLine($"\n[QUEST ACEITA] {nome}");
                Console.WriteLine($"Descrição: {desc}");
                Console.WriteLine($"Dificuldade: {ObterDificuldade(tipo)}");

                // Simulação de progresso
                for (int i = 0; i < duracao; i++)
                {
                    await Task.Delay(1000);
                    Console.Write($"\rProgresso: {i + 1}/{duracao}s [{(i + 1) * 100 / duracao}%]");
                }

                Console.WriteLine($"\n[QUEST COMPLETA] {nome} ★");
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await channel.BasicConsumeAsync(queue: QueueTarefas, autoAck: false, consumer: questConsumer);
        Console.WriteLine("\nAguardando quests... (Pressione Ctrl+C para sair)");


        await Task.Delay(-1); // Mantém o programa rodando
    }

    static string ObterDificuldade(int tipo) => tipo switch
    {
        1 => "★★★★★ (30s)",
        2 => "★★★★ (20s)",
        3 => "★★★ (15s)",
        4 => "★★ (10s)",
        5 => "★ (5s)",
        _ => "???"
    };
}