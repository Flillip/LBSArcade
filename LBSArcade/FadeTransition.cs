using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace LBSArcade
{
    internal class FadeTransition
    {
        public bool Done { get; private set; }

        private Texture2D circle;
        private List<FadeItem> fadeItems = new List<FadeItem>();
        private List<FadeItem> reverseFadeItems = new List<FadeItem>();

        private static int circleSize = 64;
        private static int halfCircleSize = circleSize / 2;

        public void Load()
        {
            int delay = Settings.GetData<int>("introLength") * 1000 - Settings.GetData<int>("transitionLength");
            float divider = Settings.GetData<float>("transitionAnimationLength");

            for (int x = halfCircleSize; x - halfCircleSize < Game.ScreenSize.X; x += circleSize)
            {
                for (int y = halfCircleSize; y - halfCircleSize < Game.ScreenSize.Y; y += circleSize)
                {
                    this.fadeItems.Add(new(new(x, y), (x + y) / divider, false));
                    this.reverseFadeItems.Add(new(new(x, y), (x + y) / divider + delay, true));
                }
            }

            this.circle = Game.Instance.Content.Load<Texture2D>("Circle");
        }

        public void Restart()
        {
            this.fadeItems.ForEach(fadeItem => fadeItem.Restart());
            this.reverseFadeItems.ForEach(fadeItem => fadeItem.Restart());
            this.Done = false;
        }

        public void Update(GameTime gameTime)
        {
            if (this.Done)
                return;

            if (this.reverseFadeItems.Last().Done == true)
            {
                foreach (FadeItem fadeItem in this.fadeItems)
                {
                    fadeItem.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }

                this.Done = this.fadeItems.Last().Done;
            }

            else
            {
                foreach (FadeItem fadeItem in this.reverseFadeItems)
                {
                    fadeItem.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                }
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (this.Done)
                return;

            if (this.reverseFadeItems.Last().Done == true)
            {
                foreach (FadeItem fadeItem in this.fadeItems)
                {
                    spriteBatch.Draw(this.circle, fadeItem.Position, null, Color.White,
                                        0f, new Vector2(halfCircleSize, halfCircleSize), fadeItem.Scale, SpriteEffects.None, 0);
                }
            }

            else
            {
                foreach (FadeItem fadeItem in this.reverseFadeItems)
                {
                    spriteBatch.Draw(this.circle, fadeItem.Position, null, Color.White,
                                        0f, new Vector2(halfCircleSize, halfCircleSize), fadeItem.Scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
