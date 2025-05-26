
using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;


namespace SolidRabbit.Game.Services
{
    public class EnemyAIService : IEnemyAIService
    {
        private readonly IGameGrid _gameGrid;
        private readonly IGameEntityService _gameEntityService;
        private readonly Random _random;

        public EnemyAIService(IGameGrid gameGrid, IGameEntityService gameEntityService) {
            _gameGrid = gameGrid;
            _gameEntityService = gameEntityService;
            _random = new Random();
        }

        public Enemy CreateNewEnemy(Position initialPosition) {
            var enemy = new Enemy(initialPosition);
            _gameEntityService.AddOrUpdateEnemy(enemy);
            return enemy;
        }

        public bool DetermineAndMove(Guid enemyId) {
            var enemy = _gameEntityService.GetEnemy(enemyId);
            if (enemy == null) {
                return false;
            }

            var playerTargets = _gameEntityService.GetAllPlayers().ToList();
            if (!playerTargets.Any()) {
                return false;
            }

            // AI simples: persegue o player mais próximo ou um player aleatório se múltiplos
            Player targetPlayer = playerTargets.OrderBy(p => Distance(enemy.CurrentPosition, p.CurrentPosition)).FirstOrDefault();

            if (targetPlayer == null) return false; // Sem player para perseguir

            Position currentPos = enemy.CurrentPosition;
            Position newAttemptedPos = currentPos;

            int deltaX = 0;
            int deltaY = 0;

            // Prioriza movimento X
            if (targetPlayer.CurrentPosition.X > currentPos.X) deltaX = 1;
            else if (targetPlayer.CurrentPosition.X < currentPos.X) deltaX = -1;

            newAttemptedPos = new Position(currentPos.X + deltaX, currentPos.Y);

            // Se não moveu em X ou colidiria, tentar mover em Y
            if (newAttemptedPos.Equals(currentPos) || IsCollision(newAttemptedPos, enemy.Id)) {
                deltaX = 0; // Reset X
                if (targetPlayer.CurrentPosition.Y > currentPos.Y) deltaY = 1;
                else if (targetPlayer.CurrentPosition.Y < currentPos.Y) deltaY = -1;
                newAttemptedPos = new Position(currentPos.X + deltaX, currentPos.Y + deltaY); // Aqui deltaX é 0
            }


            // Fallback para movimento aleatório se a direção ideal não for possível (evita ficar parado)
            if (newAttemptedPos.Equals(currentPos) || IsCollision(newAttemptedPos, enemy.Id) || !_gameGrid.IsPositionValid(newAttemptedPos)) {
                // Tenta uma direção aleatória
                int randomDeltaX = _random.Next(-1, 2);
                int randomDeltaY = _random.Next(-1, 2);
                newAttemptedPos = new Position(currentPos.X + randomDeltaX, currentPos.Y + randomDeltaY);
            }


            // Final check for validity and collision (com todos os players e outros inimigos)
            if (_gameGrid.IsPositionValid(newAttemptedPos) && !IsCollision(newAttemptedPos, enemy.Id)) {
                enemy.SetPosition(newAttemptedPos);
                return true;
            }
            else {
                return false;
            }
        }

        private bool IsCollision(Position position, Guid currentEntityId) {
            // Verifica colisão com todos os outros players e inimigos, exceto a entidade atual
            return _gameEntityService.GetAllEntities()
                .Any(e => e.Id != currentEntityId && e.CurrentPosition.Equals(position));
        }

        private double Distance(Position p1, Position p2) {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }
}