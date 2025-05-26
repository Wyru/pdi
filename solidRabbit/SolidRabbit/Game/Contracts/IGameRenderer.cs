using SolidRabbit.Core.Domain;
using System.Drawing;

namespace SolidRabbit.Game.Contracts
{
    public interface IGameRenderer
    {
        void Init();
        bool ShouldClose();
        void BeginDrawing();
        void EndDrawing();
        void DrawGrid(int width, int height, int cellSize);
        void DrawEntity(Position position, int cellSize, Color color);
        void Close();
    }
}