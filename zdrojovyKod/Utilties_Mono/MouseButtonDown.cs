using Microsoft.Xna.Framework;

namespace Utilties_Mono
{
    public struct MouseButtonDown
    {
        public Point Position { get; private set; }
        public bool IsPressed { get; private set; }
        public double ReleasedTime { get; private set; }

        public void Press(Point position)
        {
            this.Position = position;
            this.IsPressed = true;
        }

        public void Release(double time)
        {
            this.ReleasedTime = time;
            this.IsPressed = false;
        }
    }
}
