using StateMachine.src.Common;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace StateMachine.src.World
{
    public class Map
    {

        private const int Size = 30;
        private string[,] grid;
        private List<GameObject> objects;

        public Map() {
            grid = new string[Size, Size];
            objects = [];
            InitializeConsole();
        }

        public void AddObject(GameObject obj) {
            objects.Add(obj);
        }


        private void InitializeConsole() {
            Console.CursorVisible = false;
            Console.SetWindowSize(Size * 2 + 5, Size + 10);
        }

        public void UpdateGrid() {
            // Limpa o grid
            Array.Clear(grid, 0, grid.Length);

            // Atualiza posições dos objetos
            foreach (var obj in objects) {
                if (obj.transform.X >= 0 && obj.transform.X < Size && obj.transform.Y >= 0 && obj.transform.Y < Size) {
                    grid[obj.transform.X, obj.transform.Y] = obj.Render();
                }
            }
        }


        public void Render() {
            Console.SetCursorPosition(0, 0);

            var buffer = new System.Text.StringBuilder();

            for (int y = 0; y < Size; y++) {
                for (int x = 0; x < Size; x++) {
                    buffer.Append(grid[x, y] != null ? grid[x, y] + " ": "- ");
                }
                buffer.AppendLine();
            }

            Console.Write(buffer.ToString());
        }

    }
}
