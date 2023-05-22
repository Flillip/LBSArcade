using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace LBSArcade
{
    internal class BackgroundOld
    {
        private Texture2D[] textures;
        private float[] movementSpeeds;
        private Vector2[] positions;

        public BackgroundOld(Texture2D[] imgs, float movementSpeed)
        {
            this.textures = imgs;

            float percent = 1.0f / this.textures.Length;
            this.movementSpeeds = new float[this.textures.Length]
                                        .Select((val, i) => (i + 1) * percent * movementSpeed)
                                        .Reverse()
                                        .ToArray();

            this.positions = new Vector2[this.textures.Length];
            Array.Fill(this.positions, Vector2.Zero);
        }

        public void Update(float delta)
        {
            for (int i = 0; i < this.textures.Length; i++)
            {
                UpdatePos(ref this.positions[i], this.movementSpeeds[i], delta);
            }
        }

        private void UpdatePos(ref Vector2 pos, float speed, float delta)
        {
            pos.X += speed * delta;
            if (pos.X < -Game.ScreenSize.X)
                pos.X = Game.ScreenSize.X;
            if (pos.X > Game.ScreenSize.X)
                pos.X = -Game.ScreenSize.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.textures.Length; i++)
            {
                DrawTexture(spriteBatch, this.textures[i], this.positions[i], this.movementSpeeds[i]);
            }
        }

        private void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float movementSpeed)
        {
            spriteBatch.Draw(texture, position, Color.White);

            if (position.X > 0)
                spriteBatch.Draw(texture, position + new Vector2(texture.Width * (movementSpeed > 0 ? -1 : 1), 0), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
            else if (position.X < Game.ScreenSize.X)
                spriteBatch.Draw(texture, position + new Vector2(texture.Width * (movementSpeed < 0 ? -1 : 1), 0), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);

        }
    }
}
