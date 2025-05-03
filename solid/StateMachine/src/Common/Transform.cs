namespace StateMachine.src.Common
{
    public class Transform
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Transform(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveUp()
        {
            if (Y > 0) Y--;
        }

        public void MoveDown()
        {
            if (Y < 29) Y++;
        }

        public void MoveLeft()
        {
            if (X > 0) X--;
        }

        public void MoveRight()
        {
            if (X < 29) X++;
        }

        public void SetPosition(int x, int y)
        {
            if (x >= 0 && x < 30 && y >= 0 && y < 30)
            {
                X = x;
                Y = y;
            }
        }

        public int Distance(Transform another) {
            return (int) MathF.Sqrt(
                MathF.Pow(another.X - X, 2) +
                MathF.Pow(another.Y - Y, 2)
            );
        }
    }
}
