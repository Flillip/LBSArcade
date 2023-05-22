using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBSArcade
{
    internal class Background
    {
        private PathCommand[][] paths;
        private PathCommand[][] flippedPaths;
        private Color[] colors;
        private SvgRenderer svgRenderer;
        private float[] movementSpeeds;
        private Vector2[] positions;
        private Texture2D background;
        private Vector2 offset;
        private bool error;
        private BackgroundOld backgroundOld;

        public Background(string[] paths, string[] flipped, Color[] colors, Texture2D background, float movementSpeed, Vector2 offset)
        {
            try
            {
                this.svgRenderer = new SvgRenderer();
                List<PathCommand[]> pathsCmd = new();
                List<PathCommand[]> prevPathsCmd = new();

                for (int i = 0; i < paths.Length; i++)
                {
                    pathsCmd.Add(this.svgRenderer.ParsePath(paths[i]));
                    prevPathsCmd.Add(this.svgRenderer.ParsePath(flipped[i]));
                }

                this.paths = pathsCmd.ToArray();
                this.flippedPaths = prevPathsCmd.ToArray();
                this.colors = colors;
                this.background = background;
                this.offset = offset;

                float percent = 1.0f / this.paths.Length;
                this.movementSpeeds = new float[this.paths.Length]
                                            .Select((val, i) => (i + 1) * percent * movementSpeed)
                                            .Reverse()
                                            .ToArray();

                this.positions = new Vector2[this.paths.Length];
                Array.Fill(this.positions, this.offset);
                this.error = false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                this.error = true;

                int amountOfWaves = 4;
                Texture2D[] waves = new Texture2D[amountOfWaves];

                for (int i = 0; i < amountOfWaves; i++)
                    waves[i] = Game.Instance.Content.Load<Texture2D>("Wave" + (i + 1));

                this.backgroundOld = new(waves, movementSpeed);
            }
        }

        public void Update(float delta)
        {
            if (this.error)
            {
                this.backgroundOld.Update(delta);
                return;
            }

            for (int i = 0; i < this.paths.Length; i++)
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
            if (this.error)
            {
                this.backgroundOld.Draw(spriteBatch);
                return;
            }

            for (int i = 0; i < this.paths.Length; i++)
            {
                if (this.positions[i].X > 0)
                    DrawTexture(this.flippedPaths[i], this.positions[i] + new Vector2(Game.ScreenSize.X * (this.movementSpeeds[i] > 0 ? -1 : 1) + 1, 0), this.colors[i]);
                else if (this.positions[i].X < Game.ScreenSize.X)
                    DrawTexture(this.flippedPaths[i], this.positions[i] + new Vector2(Game.ScreenSize.X * (this.movementSpeeds[i] < 0 ? -1 : 1) - 1, 0), this.colors[i]);

                DrawTexture(this.paths[i], this.positions[i], this.colors[i]);
            }

            spriteBatch.Draw(this.background, new Vector2(0, Game.ScreenSize.Y) + this.offset, Color.White);
        }

        private void DrawTexture(PathCommand[] path, Vector2 position, Color color)
        {
            this.svgRenderer.Render(path, color, position);
        }
    }
}
