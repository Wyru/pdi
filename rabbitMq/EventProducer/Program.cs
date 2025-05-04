using RabbitMQ.Client;
using System.Text;

class Program
{
    private const string ExchangeDirect = "tarefas_exchange";
    private const string ExchangeFanout = "notificacoes_exchange";
    private const string QueueTarefas = "tarefas_queue";

    static async Task Main(string[] args) {
        var factory = new ConnectionFactory() {
            HostName = "localhost",
            Port = 5672,
            UserName = "admin",
            Password = "senha123"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Configuração dos Exchanges e Filas
        await channel.ExchangeDeclareAsync(ExchangeDirect, ExchangeType.Direct);
        await channel.ExchangeDeclareAsync(ExchangeFanout, ExchangeType.Fanout);

        // Fila de tarefas (Quests)
        await channel.QueueDeclareAsync(
            queue: QueueTarefas,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?> {
            { "x-max-priority", 5}
        });

        await channel.QueueBindAsync(queue: QueueTarefas, exchange: ExchangeDirect, routingKey: "quest");

        while (true) {
            Console.Clear();
            Console.WriteLine("=== RPG Quest Publisher ===");
            Console.WriteLine("1. Criar Nova Quest");
            Console.WriteLine("2. Gerar 5 Quests Automáticas");
            Console.WriteLine("3. Enviar Notificação Global");
            Console.WriteLine("4. Sair");
            Console.Write("Escolha: ");

            switch (Console.ReadLine()) {
                case "1":
                    await CriarQuestManual(channel);
                    break;
                case "2":
                    await GerarQuestsAutomaticas(channel);
                    break;
                case "3":
                    await EnviarNotificacaoGlobal(channel);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    await Task.Delay(1000);
                    break;
            }
        }
    }

    static async Task CriarQuestManual(IChannel channel) {
        Console.Clear();
        Console.WriteLine("=== Nova Quest ===");

        Console.Write("Nome da Quest: ");
        var nome = Console.ReadLine();

        Console.Write("Descrição: ");
        var descricao = Console.ReadLine();

        Console.WriteLine("\nTipo de Quest:");
        Console.WriteLine("1. Missão Principal (30s)");
        Console.WriteLine("2. Missão Secundária (20s)");
        Console.WriteLine("3. Tarefa Diária (15s)");
        Console.WriteLine("4. Evento Especial (10s)");
        Console.WriteLine("5. Treinamento (5s)");
        Console.Write("Escolha: ");

        if (!int.TryParse(Console.ReadLine(), out var tipo) || tipo < 1 || tipo > 5) {
            Console.WriteLine("Tipo inválido!");
            return;
        }

        var duracao = tipo switch {
            1 => 30,
            2 => 20,
            3 => 15,
            4 => 10,
            5 => 5,
            _ => 10
        };

        var mensagem = $"{nome}|{descricao}|{tipo}|{duracao}";
        await PublicarMensagem(channel, ExchangeDirect, "quest", mensagem);

        Console.WriteLine($"\nQuest '{nome}' publicada!");
        await Task.Delay(1500);
    }

    static async Task GerarQuestsAutomaticas(IChannel channel) {
        var quests = new[] {
            new { Nome = "Dragão da Montanha", Descricao = "Derrote o dragão que aterroriza o reino", Tipo = 1 },
            new { Nome = "Ervas Medicinais", Descricao = "Colete 10 ervas raras na floresta", Tipo = 2 },
            new { Nome = "Caça aos Lobos", Descricao = "Elimine 5 lobos da floresta", Tipo = 3 },
            new { Nome = "Festa do Rei", Descricao = "Participe das celebrações no castelo", Tipo = 4 },
            new { Nome = "Treino com Mestre", Descricao = "Complete 100 golpes de espada", Tipo = 5 }
        };

        Console.Clear();
        Console.WriteLine("Gerando Quests Automáticas...\n");

        foreach (var q in quests) {
            var duracao = q.Tipo switch {
                1 => 30,
                2 => 20,
                3 => 15,
                4 => 10,
                5 => 5,
                _ => 10
            };

            var mensagem = $"{q.Nome}|{q.Descricao}|{q.Tipo}|{duracao}";
            await PublicarMensagem(channel, ExchangeDirect, "quest", mensagem);
            Console.WriteLine($"- {q.Nome} ({duracao}s)");
            await Task.Delay(300);
        }

        Console.WriteLine("\nTodas as quests foram publicadas!");
        await Task.Delay(1500);
    }

    static async Task EnviarNotificacaoGlobal(IChannel channel) {
        Console.Clear();
        Console.WriteLine("=== Notificação Global ===");
        Console.Write("Mensagem: ");
        var msg = Console.ReadLine();

        await PublicarMensagem(channel, ExchangeFanout, "", msg);
        Console.WriteLine("\nNotificação enviada a todos!");
        await Task.Delay(1500);
    }

    static async Task PublicarMensagem(IChannel channel, string exchange, string routingKey, string mensagem) {
        var body = Encoding.UTF8.GetBytes(mensagem);
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: body
        );
    }
}