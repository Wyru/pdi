using Raylib_cs;
using StateMachine.src.Common;
using StateMachine.src.Entities;

namespace StateMachine.src
{
    public class GameWindow
    {

        private static readonly Color gridColor = Color.DarkGreen;
        private static readonly Color playerColor = Color.RayWhite;
        private static readonly Color backgroundColor = Color.Black;

        private readonly GameObject player;

        private readonly IEnumerable<GameObject> entities;
        
        public GameWindow( GameObject player, IEnumerable<GameObject> entities ) {


            Raylib.SetTargetFPS(60);
            Raylib.SetWindowState(ConfigFlags.Msaa4xHint);
            Raylib.InitWindow(Settings.WIDTH, Settings.HEIGHT, "State Machine Test!");

            this.player = player;
            this.entities = entities;

            // Game loop
            while (!Raylib.WindowShouldClose()) {
                // Desenho
                HandleInput();
                OnUpdate();
                OnRender();
            }

            Raylib.CloseWindow();

        }

        void OnUpdate() {
            player.Update();

            foreach (var entitie in entities) {
                entitie.Update();
            }
        }

        void OnRender() {
            Raylib.BeginDrawing();
            DrawGrid();
            DrawEntities();
            Raylib.EndDrawing();
        }

        private void DrawGrid() {
            Raylib.ClearBackground(backgroundColor);
            for (int i = 0; i < Settings.GRID_SIZE; i++) {
                Raylib.DrawLine(i * Settings.CELL_GRID_SIZE, 0, i * Settings.CELL_GRID_SIZE, Settings.HEIGHT, gridColor);
                Raylib.DrawLine(0, i * Settings.CELL_GRID_SIZE, Settings.WIDTH, i * Settings.CELL_GRID_SIZE, gridColor);
            }
        }

        private void DrawEntities() {
            player.Render();

            foreach (var entitie in entities) {
                entitie.Render();
            }
        }

        private void HandleInput() {
            if (Raylib.IsKeyPressed(KeyboardKey.W)) {
                player.transform.MoveUp();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.S)) {
                player.transform.MoveDown();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.A)) {
                player.transform.MoveLeft();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.D)) {
                player.transform.MoveRight();
            }
        }
    }
}
