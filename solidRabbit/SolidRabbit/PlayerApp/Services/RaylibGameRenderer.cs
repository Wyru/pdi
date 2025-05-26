using Raylib_cs;
using SolidRabbit.Core.Domain;
using SolidRabbit.Game.Contracts;
using SolidRabbit.Game.Constants;


namespace SolidRabbit.PlayerApp.Services
{
    public class RaylibGameRenderer : IGameRenderer
    {
        public void Init() {
            const int screenWidth = GameConstants.GridWidth * GameConstants.CellSize;
            const int screenHeight = GameConstants.GridHeight * GameConstants.CellSize;
            Raylib.InitWindow(screenWidth, screenHeight, "Solid Rabbit - Player");
            Raylib.SetTargetFPS(60);
        }

        public bool ShouldClose() => Raylib.WindowShouldClose();

        public void BeginDrawing() => Raylib.BeginDrawing();
        public void EndDrawing() => Raylib.EndDrawing();

        public void DrawGrid(int width, int height, int cellSize) {
            Raylib.ClearBackground(Color.Black);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Raylib.DrawRectangleLines(x * cellSize, y * cellSize, cellSize, cellSize, Color.DarkGreen);
                }
            }
        }

        public void Close() => Raylib.CloseWindow();

        public void DrawEntity(Position position, int cellSize, System.Drawing.Color color) {
            Raylib.DrawRectangle(position.X * cellSize, position.Y * cellSize, cellSize, cellSize, ConvertColor(color));
        }

        private static Color ConvertColor(System.Drawing.Color drawingColor) {
            return new Color(drawingColor.R, drawingColor.G, drawingColor.B, drawingColor.A);
        }
    }
}