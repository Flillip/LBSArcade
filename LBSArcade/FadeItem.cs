using Microsoft.Xna.Framework;
using System;

namespace LBSArcade
{
    internal class FadeItem
    {
        public bool Done { get; private set; }
        public Vector2 Position { get; private set; }
        public float Scale
        {
            get
            {
                if (delay > 0)
                    return this.reverse ? 0f : 2.0f;
                else if (radians > MathHelper.Pi)
                {
                    this.Done = true;
                    return this.reverse ? 2.0f : 0f;
                }

                return this.reverse ? (float)Math.Abs(Math.Cos(radians) - 1) : (float)Math.Cos(radians) + 1;
            }
        }

        private float delay;
        private float radians;
        private bool reverse;

        private float originalDelay;
        private float originalRadians;

        public FadeItem(Vector2 position, float delay, bool reverse)
        {
            this.Position = position;
            this.delay = delay;
            this.reverse = reverse;
            this.originalDelay = delay;
            this.originalRadians = radians;
        }

        public void Restart()
        {
            this.delay = this.originalDelay;
            this.radians = this.originalRadians;
            this.Done = false;
        }

        public void Update(float delta)
        {
            delay -= delta;

            if (delay < 0f)
                radians += delta / 200.0f;
        }
    }
}
