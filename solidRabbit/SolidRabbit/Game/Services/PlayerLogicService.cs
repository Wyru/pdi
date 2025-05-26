
using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;

namespace SolidRabbit.Game.Services
{
    public class PlayerLogicService : IPlayerLogicService
    {
        private readonly IGameGrid _gameGrid;
        private readonly IGameEntityService _gameEntityService;

        public PlayerLogicService(IGameGrid gameGrid, IGameEntityService gameEntityService) {
            _gameGrid = gameGrid;
            _gameEntityService = gameEntityService;
        }

        public Player CreateNewPlayer(Position initialPosition, System.Drawing.Color color) {
            var player = new Player(initialPosition, color);
            _gameEntityService.AddOrUpdatePlayer(player);
            return player;
        }

        public bool TryMovePlayer(Guid playerId, int deltaX, int deltaY) {
            var player = _gameEntityService.GetPlayer(playerId);
            if (player == null) {
                return false;
            }

            Position currentPos = player.CurrentPosition;
            int newX = currentPos.X + deltaX;
            int newY = currentPos.Y + deltaY;
            Position attemptedPosition = new Position(newX, newY);

            if (!_gameGrid.IsPositionValid(attemptedPosition)) {
                return false;
            }

            var entities = _gameEntityService.GetAllEntities()
                .Where(e => e.Id != playerId);

            bool collides = entities.Any(e => e.CurrentPosition.Equals(attemptedPosition));

            if (collides) {
                return false;
            }

            player.Move(deltaX, deltaY, _gameGrid.Width, _gameGrid.Height);
            return true;
        }
    }
}