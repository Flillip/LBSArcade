using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Linq;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace LBSArcade
{
    internal class Intro
    {
        public bool Done { get; private set; }
        public bool IntroGifDone { get; private set; }

        private Texture2D[] logo;
        private int[] delays;
        private float logoTimer;
        private int logoIndex;
        private int logoLengthMultiplier;
        private Color backgroundColor;
        private FadeTransition fadeTransition;
       

        public void Load()
        {
            this.fadeTransition = new();
            this.fadeTransition.Load();

            this.logoLengthMultiplier = Settings.GetData<int>("introLength");
            this.backgroundColor = Settings.GetColor("introBackgroundColor");

            Task<Texture2D[]> t = Convert(Image.Load("./Content/lbs-logo.gif"));
            this.logo = t.Result;
        }

        public void Restart()
        {
            this.logoTimer = 0;
            this.logoIndex = 0;
            this.Done = false;
            this.IntroGifDone = false;
            this.fadeTransition.Restart();
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            this.logoTimer += delta;

            if (this.logoTimer > this.delays[this.logoIndex % this.logo.Length] / 100)
            {
                this.logoTimer = 0;
                this.logoIndex++;
            }
        }

        public void UpdateTransition(GameTime gameTime)
        {
            this.fadeTransition.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            Game.Instance.GraphicsDevice.Clear(backgroundColor);
            Vector2 middle = Game.ScreenSize / 2f - new Vector2(this.logo[0].Width, this.logo[0].Height) / 2f;
            spriteBatch.Draw(this.logo[this.logoIndex % this.logo.Length], middle, Color.White);


            this.IntroGifDone = this.logoIndex > (this.logo.Length) * this.logoLengthMultiplier;
        }

        public void RenderTransition(SpriteBatch spriteBatch)
        {
            this.fadeTransition.Render(spriteBatch);
            Done = this.IntroGifDone && this.fadeTransition.Done;
        }

        private Task<Texture2D[]> Convert(Image gif)
        {
            Size size = gif.Size();
            int iSize = size.Width * size.Height;
            Texture2D[] textures = new Texture2D[gif.Frames.Count];

            this.delays = new int[gif.Frames.Count];

            int i = 0;

            // spooky code
            Span<Rgba32> data = stackalloc Rgba32[iSize];

            foreach (ImageFrame<Rgba32> frame in gif.Frames.Cast<ImageFrame<Rgba32>>())
            {
                Color[] colors = new Color[iSize];

                frame.CopyPixelDataTo(data);

                textures[i] = new Texture2D(Game.Instance.GraphicsDevice, size.Width, size.Height);
                textures[i].SetData(data.ToArray().Select((col) => new Color(col.R, col.G, col.B)).ToArray());
                this.delays[i] = frame.Metadata.GetGifMetadata().FrameDelay * 10;

                i++;
            }


            return Task.FromResult(textures);
        }
    }
}
