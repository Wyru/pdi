using Raylib_cs;
using SolidRabbit.Game.Contracts;

namespace RogueLikeDotnet.PlayerApp.Services
{
    public class RaylibInputHandler : IInputService
    {
        public (int deltaX, int deltaY) GetMovementInput() {
            int deltaX = 0;
            int deltaY = 0;

            if (Raylib.IsKeyPressed(KeyboardKey.W)) deltaY--;
            if (Raylib.IsKeyPressed(KeyboardKey.S)) deltaY++;
            if (Raylib.IsKeyPressed(KeyboardKey.A)) deltaX--;
            if (Raylib.IsKeyPressed(KeyboardKey.D)) deltaX++;

            return (deltaX, deltaY);
        }
    }
}