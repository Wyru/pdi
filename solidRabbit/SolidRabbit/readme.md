# SolidRabbit

Este projeto é um simples, construído em .NET, que demonstra a aplicação de **princípios de design de software (SOLID)** e **arquitetura distribuída** usando um **Sistema de Eventos** e **RabbitMQ** para comunicação entre os clientes de jogo. O objetivo é permitir que múltiplos jogadores (controlados por diferentes instâncias do aplicativo) interajam em um mundo compartilhado, juntamente com inimigos controlados por IA, tudo sincronizado por **mensagens assíncronas**.

---

## 1. O que é um Sistema de Eventos?

Em sua essência, um **Sistema de Eventos** é um padrão de arquitetura de software onde a comunicação entre os componentes é baseada na **produção e consumo de eventos**.

* **Evento:** Representa um fato ou algo que aconteceu no sistema (ex: "jogador se moveu", "inimigo atacou", "item coletado").
* **Produtor (Publisher):** Um componente que detecta um evento e o publica em um canal. Ele não se importa com quem vai consumir o evento, apenas que ele aconteceu.
* **Consumidor (Subscriber):** Um componente que escuta em um ou mais canais por eventos de seu interesse e reage a eles. O consumidor não sabe quem produziu o evento.

**Vantagens de um Sistema de Eventos:**

* **Desacoplamento:** Produtores e consumidores não precisam ter conhecimento direto uns dos outros, tornando o sistema mais flexível e fácil de manter.
* **Escalabilidade:** Novas funcionalidades (consumidores) podem ser adicionadas sem modificar os produtores existentes.
* **Assincronicidade:** Os eventos podem ser processados em segundo plano, melhorando a responsividade e a experiência do usuário.
* **Resiliência:** Se um consumidor falhar, outros podem continuar operando ou os eventos podem ser reprocessados.

Neste projeto, quando um player se move, um "Player Moved Event" é publicado. Outras instâncias do jogo (sejam de player ou inimigo) consomem esse evento para atualizar a posição desse player em suas telas. Da mesma forma, inimigos publicam seus movimentos.

---

## 2. RabbitMQ: Principais Funcionalidades e Características

O **RabbitMQ** é um popular **message broker** (intermediário de mensagens) de código aberto que implementa o padrão Advanced Message Queuing Protocol (AMQP). Ele atua como o "coração" do nosso sistema de eventos, garantindo que as mensagens sejam entregues de forma confiável e eficiente entre os componentes do jogo.

**Principais Funcionalidades e Características:**

* **Message Broker:** RabbitMQ não é apenas um servidor, mas um intermediário que armazena mensagens, roteia-as para as filas corretas e as entrega aos consumidores.
* **Assincronicidade por Natureza:** Ele é projetado para lidar com comunicações assíncronas, permitindo que os produtores enviem mensagens e continuem seu trabalho sem esperar que os consumidores as processem imediatamente.
* **Filas (Queues):** Mensagens são armazenadas em filas esperando para serem consumidas.
* **Exchanges (Intercâmbios):** Produtores enviam mensagens para exchanges, não diretamente para filas. Exchanges são responsáveis por rotear mensagens para uma ou mais filas, com base em regras definidas (tipos de exchange como `direct`, `fanout`, `topic`, `headers`).
    * `Fanout`: Roteia mensagens para *todas* as filas que estão vinculadas a ele. Ideal para cenários de broadcast (como um evento de movimento de player para todos os clientes).
    * `Direct`: Roteia mensagens para filas cuja chave de roteamento (routing key) corresponde exatamente à chave definida na fila.
    * `Topic`: Roteia mensagens para filas que correspondem a um padrão wildcard na chave de roteamento.
* **Bindings (Ligações):** Conexões entre exchanges e filas, usando uma "routing key" para determinar quais mensagens (baseado na chave de roteamento da mensagem) devem ir para quais filas.
* **Durabilidade:** Filas e mensagens podem ser configuradas como duráveis, o que significa que elas sobreviverão a reinicializações do RabbitMQ, garantindo que nenhuma mensagem seja perdida.
* **Confirmação do Consumidor (Acknowledgements):** Os consumidores podem enviar confirmações de volta ao RabbitMQ, indicando que processaram uma mensagem com sucesso. Isso garante que as mensagens só sejam removidas da fila após serem processadas.
* **Alta Disponibilidade e Clusterização:** RabbitMQ pode ser configurado em cluster para alta disponibilidade e escalabilidade.

Neste projeto, o RabbitMQ é usado para sincronizar as posições dos players e inimigos entre as diferentes instâncias do jogo, garantindo que todos os clientes vejam o mesmo estado do mundo de jogo.

---

## 3. Como o Projeto é Organizado

O projeto `SolidRabbit` é dividido em várias bibliotecas e aplicativos para garantir a modularidade, a clareza das responsabilidades e a aplicação dos **princípios SOLID**.

### Detalhes da Organização por Projeto

* **`SolidRabbit.Core`**:
    * **Propósito:** Contém os blocos de construção mais fundamentais e agnósticos da aplicação.
    * **Responsabilidade:** Define entidades de domínio (`Player`, `Enemy`, `Position`), contratos básicos (`IGameEntity`) e objetos de eventos (`PlayerMovedEvent`, `EnemyMovedEvent`, etc.). Não possui dependências de implementação específicas de UI, IA ou mensageria.

* **`SolidRabbit.Messaging`**:
    * **Propósito:** Abstrair e encapsular a lógica de comunicação de mensagens.
    * **Responsabilidade:** Fornecer uma interface genérica (`IMessagingService`) para publicação e subscrição de mensagens. A implementação específica do RabbitMQ reside aqui, garantindo que outras partes do sistema não precisem saber os detalhes de como as mensagens são enviadas ou recebidas.

* **`SolidRabbit.Game`**:
    * **Propósito:** Centralizar toda a lógica de negócio e regras do jogo.
    * **Responsabilidade:** Gerencia o estado do mundo do jogo (grade, entidades), a lógica de movimento do player, o comportamento da IA dos inimigos e a orquestração geral do jogo. Ele expõe uma interface (`IGameEngine`) que os aplicativos clientes usam para interagir com o jogo, mantendo a UI e a IA separadas da lógica central. Este projeto não tem dependência de UI ou mensageria direta, apenas do `SolidRabbit.Core`.

* **`SolidRabbit.PlayerApp`**:
    * **Propósito:** Ser a interface de usuário e o ponto de controle para um jogador humano.
    * **Responsabilidade:** Lidar com a renderização gráfica (usando Raylib-cs), capturar o input do jogador e traduzir esses inputs em chamadas para o `SolidRabbit.Game.IGameEngine`. Ele se inscreve em eventos do RabbitMQ para atualizar sua visão do mundo de jogo e publica eventos sobre as ações do player.

* **`SolidRabbit.EnemyApp`**:
    * **Propósito:** Simular um inimigo controlado por inteligência artificial.
    * **Responsabilidade:** Contém a lógica de IA do inimigo, determina seus movimentos e interage com o `SolidRabbit.Game.IGameEngine` para atualizar sua posição no mundo. Ele também se inscreve em eventos do RabbitMQ (para saber a posição dos players) e publica seus próprios movimentos.

### Princípios de Design Aplicados

* **SOLID:** A arquitetura é fortemente influenciada pelos princípios SOLID, promovendo alta coesão e baixo acoplamento.
    * **SRP (Single Responsibility Principle):** Cada projeto e classe tem uma responsabilidade bem definida.
    * **OCP (Open/Closed Principle):** O sistema é aberto para extensão (ex: adicionar novos tipos de inimigos, novas UIs) mas fechado para modificação de seu core.
    * **LSP (Liskov Substitution Principle):** Interfaces e abstrações permitem a substituição de implementações sem quebrar o sistema.
    * **ISP (Interface Segregation Principle):** Interfaces são pequenas e coesas.
    * **DIP (Dependency Inversion Principle):** Dependências são feitas em abstrações (interfaces) em vez de implementações concretas, especialmente no `SolidRabbit.Game`.
* **Injeção de Dependência (DI):** Usada extensivamente com `Microsoft.Extensions.Hosting` para gerenciar o ciclo de vida dos serviços e a resolução de dependências.
* **Assincronicidade:** O uso de RabbitMQ e `async/await` promove um comportamento assíncrono para operações de rede e processamento de eventos, mantendo a responsividade da aplicação.
* **Thread Safety:** Com a implementação da fila de ações no `PlayerAppLoop`, o projeto garante que as interações com a Raylib (que não é thread-safe) ocorram em um único thread, evitando corrupção de memória.
