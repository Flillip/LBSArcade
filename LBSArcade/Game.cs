using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Color = Microsoft.Xna.Framework.Color;

namespace LBSArcade
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance;
        public static Vector2 ScreenSize;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private UI ui;
        private bool uiLoaded = false;
        private Intro intro;

        public Game()
        {
            Instance = this;

            ScreenSize = Settings.GetVector2("screenSize");
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)ScreenSize.X,
                PreferredBackBufferHeight = (int)ScreenSize.Y,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
#if RELEASE
            SetFullscreen(true, true);
            new Thread(new ThreadStart(KeepAracdeOnTop)).Start();
#endif
        }

        public void RestartIntro()
        {
            this.intro.Restart();
        }

        public void SetFullscreen(bool fullscreen, bool borderless)
        {
            this.graphics.IsFullScreen = fullscreen;
            this.graphics.HardwareModeSwitch = true;
        }

        protected override void Initialize()
        {
            this.intro = new();
            this.ui = new();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.intro.Load();

            new Thread(new ThreadStart(LoadUI)).Start();
        }

        private void LoadUI()
        {
            this.ui.LoadUI();
            this.uiLoaded = true;
        }

        protected override void Update(GameTime gameTime)
        {
            ArcadeController.Player1.Update();
            ArcadeController.Player2.Update();
            Keyboard.GetState();

            if (/*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/ Keyboard.GetKeyDown(Keys.Escape))
            {
#if RELEASE
                using FileStream file = File.Create("close");
#endif
                Exit();
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.uiLoaded && this.intro.IntroGifDone)
                this.ui.Update(delta, !this.intro.Done);
            else
                this.intro.Update(gameTime);

            if (this.intro.Done == false)
                this.intro.UpdateTransition(gameTime);

            //this.fadeTransition.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.White);

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (!uiLoaded || !this.intro.IntroGifDone)
                this.intro.Render(this.spriteBatch);
            else
                this.ui.Render(this.spriteBatch);

            if (!uiLoaded || !this.intro.Done)
                this.intro.RenderTransition(this.spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        private void KeepAracdeOnTop()
        {
            Process arcade = Process.GetCurrentProcess();

            if (!DLLImports.IsWindowFocused(arcade))
                DLLImports.SetWindowFocus(arcade);

            Thread.Sleep(2000);

            while (GraphicsDevice != null) // GraphicsDevice is null if windows is closed
            {
                Thread.Sleep(100);

                if (!GameContainer.GameRunning)
                {
                    if (!DLLImports.IsWindowFocused(arcade))
                        DLLImports.SetWindowFocus(arcade);
                }
            }
        }
    }
}