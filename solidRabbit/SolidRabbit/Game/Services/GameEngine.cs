using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Game.Constants;

namespace SolidRabbit.Game.Services
{
    public class GameEngine : IGameEngine
    {
        private readonly IPlayerLogicService playerLogicService;
        private readonly IEnemyAIService enemyAIService;
        private readonly IGameEntityService gameEntityService;
        private readonly Random random;

        public GameEngine(
            IPlayerLogicService playerLogicService,
            IEnemyAIService enemyAIService,
            IGameEntityService gameEntityService) {
            this.playerLogicService = playerLogicService;
            this.enemyAIService = enemyAIService;
            this.gameEntityService = gameEntityService;
            random = new Random();
        }

        public Player RegisterPlayer(System.Drawing.Color color) {
            var initialPosition = new Position(
                random.Next(0, GameConstants.GridWidth),
                random.Next(0, GameConstants.GridHeight));

            return playerLogicService.CreateNewPlayer(initialPosition, color);
        }

        public Enemy RegisterEnemy() {
            var initialPosition = new Position(
                random.Next(0, GameConstants.GridWidth),
                random.Next(0, GameConstants.GridHeight));

            return enemyAIService.CreateNewEnemy(initialPosition);
        }

        public bool TryMovePlayer(Guid playerId, int deltaX, int deltaY) {
            return playerLogicService.TryMovePlayer(playerId, deltaX, deltaY);
        }

        public IEnumerable<Player> GetAllPlayers() => gameEntityService.GetAllPlayers();
        public IEnumerable<Enemy> GetAllEnemies() => gameEntityService.GetAllEnemies();

        public bool PerformEnemyAction(Guid enemyId) {
            return enemyAIService.DetermineAndMove(enemyId);
        }

        public void UpdatePlayerPosition(Guid playerId, int x, int y, int r, int g, int b) {
            var existingPlayer = gameEntityService.GetPlayer(playerId);
            if (existingPlayer != null) {
                existingPlayer.SetPosition(new Position(x, y));
                existingPlayer.Color = System.Drawing.Color.FromArgb(r, g, b);
            }
            else {
                var newPlayer = new Player(new Position(x, y), System.Drawing.Color.FromArgb(r, g, b), playerId);
                gameEntityService.AddOrUpdatePlayer(newPlayer);
            }
        }

        public void UpdateEnemyPosition(Guid enemyId, int x, int y) {
            var existingEnemy = gameEntityService.GetEnemy(enemyId);
            if (existingEnemy != null) {
                existingEnemy.SetPosition(new Position(x, y));
            }
            else {
                var newEnemy = new Enemy(new Position(x, y), enemyId);
                gameEntityService.AddOrUpdateEnemy(newEnemy);
            }
        }

        public void RemoveEntity(Guid entityId) {
            // Tenta remover como player
            if (gameEntityService.GetPlayer(entityId) != null) {
                gameEntityService.RemovePlayer(entityId);
            }
            // Tenta remover como inimigo
            else if (gameEntityService.GetEnemy(entityId) != null) {
                gameEntityService.RemoveEnemy(entityId);
            }
        }

        public Player? GetPlayer(Guid id) {
            return gameEntityService.GetAllPlayers().FirstOrDefault((x) => x.Id == id);
        }

        public Enemy? GetEnemy(Guid id) {
            return gameEntityService.GetAllEnemies().FirstOrDefault((x) => x.Id == id);
        }
    }
}