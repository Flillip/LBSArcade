using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LBSArcade;

namespace ControllerTester
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D black;
        private Texture2D white;
        private Texture2D blue;
        private Texture2D red;
        private Texture2D green;
        private Texture2D yellow;
        private Texture2D blackClicked;
        private Texture2D whiteClicked;
        private Texture2D blueClicked;
        private Texture2D redClicked;
        private Texture2D greenClicked;
        private Texture2D yellowClicked;
        private Texture2D center;
        private Texture2D left;
        private Texture2D up;
        private Texture2D right;
        private Texture2D down; 
        private Texture2D upRight;
        private Texture2D upLeft;
        private Texture2D downLeft;
        private Texture2D downRight;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.black         = Content.Load<Texture2D>("black");
            this.white         = Content.Load<Texture2D>("white");
            this.blue          = Content.Load<Texture2D>("blue");
            this.red           = Content.Load<Texture2D>("red");
            this.green         = Content.Load<Texture2D>("green");
            this.yellow        = Content.Load<Texture2D>("yellow");
            this.blackClicked  = Content.Load<Texture2D>("black-clicked");
            this.whiteClicked  = Content.Load<Texture2D>("white-clicked");
            this.blueClicked   = Content.Load<Texture2D>("blue-clicked");
            this.redClicked    = Content.Load<Texture2D>("red-clicked");
            this.greenClicked  = Content.Load<Texture2D>("green-clicked");
            this.yellowClicked = Content.Load<Texture2D>("yellow-clicked");
            this.center        = Content.Load<Texture2D>("center");
            this.left          = Content.Load<Texture2D>("left");
            this.up            = Content.Load<Texture2D>("up");
            this.right         = Content.Load<Texture2D>("right");
            this.down          = Content.Load<Texture2D>("down");
            this.upLeft        = Content.Load<Texture2D>("up-left");
            this.upRight       = Content.Load<Texture2D>("up-right");
            this.downLeft      = Content.Load<Texture2D>("down-left");
            this.downRight     = Content.Load<Texture2D>("down-right");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ArcadeController.Player1.Update();
            ArcadeController.Player2.Update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            Vector2 joy0 = ArcadeController.Player1.GetJoystick();
            Vector2 joy1 = ArcadeController.Player2.GetJoystick();
            Texture2D joy0Tex = GetJoystickTexture(joy0);
            Texture2D joy1Tex = GetJoystickTexture(joy1);

            Texture2D yellow0Btn = GetButtonTexture(ArcadeButton.Yellow, this.yellow, this.yellowClicked, 0);
            Texture2D green0Btn  = GetButtonTexture(ArcadeButton.Green,  this.green,  this.greenClicked,  0);
            Texture2D red0Btn    = GetButtonTexture(ArcadeButton.Red,    this.red,    this.redClicked,    0);
            Texture2D blue0Btn   = GetButtonTexture(ArcadeButton.Blue,   this.blue,   this.blueClicked,   0);
            Texture2D top0Btn    = GetButtonTexture(ArcadeButton.Top,    this.white,  this.whiteClicked,  0);
            Texture2D bottom0Btn = GetButtonTexture(ArcadeButton.Bottom, this.white,  this.whiteClicked,  0);

            Texture2D yellow1Btn = GetButtonTexture(ArcadeButton.Yellow, this.yellow, this.yellowClicked, 1);
            Texture2D green1Btn  = GetButtonTexture(ArcadeButton.Green,  this.green,  this.greenClicked,  1);
            Texture2D red1Btn    = GetButtonTexture(ArcadeButton.Red,    this.red,    this.redClicked,    1);
            Texture2D blue1Btn   = GetButtonTexture(ArcadeButton.Blue,   this.blue,   this.blueClicked,   1);
            Texture2D top1Btn    = GetButtonTexture(ArcadeButton.Top,    this.black,  this.blackClicked,  1);
            Texture2D bottom1Btn = GetButtonTexture(ArcadeButton.Bottom, this.black,  this.blackClicked,  1);

            Texture2D menuLeft   = GetButtonTexture(ArcadeButton.MenuLeft,   this.white, this.whiteClicked, 0);
            Texture2D menuSelect = GetButtonTexture(ArcadeButton.MenuSelect, this.black, this.blackClicked, 0);
            Texture2D menuRight  = GetButtonTexture(ArcadeButton.MenuRight,  this.white, this.whiteClicked, 0);

            float buttonScale = 0.2f;
            float scaled = 256f * buttonScale;
            float player1 = 400f;

            DrawTexture(joy0Tex,    new Vector2(100, 240), 0.3f);
            DrawTexture(yellow0Btn, new Vector2(175, 270), buttonScale);
            DrawTexture(green0Btn,  new Vector2(175, 270 - scaled), buttonScale);
            DrawTexture(red0Btn,    new Vector2(175 + scaled, 270 - scaled / 2f), buttonScale);
            DrawTexture(blue0Btn,   new Vector2(175 + scaled, 270 - scaled / 2f - scaled), buttonScale);
            DrawTexture(bottom0Btn, new Vector2(175 + scaled * 2, 270 - scaled / 2f - scaled / 2), buttonScale);
            DrawTexture(top0Btn,    new Vector2(175 + scaled * 2, 270 - scaled / 2f - scaled / 2 - scaled), buttonScale);


            DrawTexture(joy1Tex,    new Vector2(player1 + 100, 240), 0.3f);
            DrawTexture(yellow1Btn, new Vector2(player1 + 175, 270), buttonScale);
            DrawTexture(green1Btn,  new Vector2(player1 + 175, 270 - scaled), buttonScale);
            DrawTexture(red1Btn,    new Vector2(player1 + 175 + scaled, 270 - scaled / 2f), buttonScale);
            DrawTexture(blue1Btn,   new Vector2(player1 + 175 + scaled, 270 - scaled / 2f - scaled), buttonScale);
            DrawTexture(bottom1Btn, new Vector2(player1 + 175 + scaled * 2, 270 - scaled / 2f - scaled / 2), buttonScale);
            DrawTexture(top1Btn,    new Vector2(player1 + 175 + scaled * 2, 270 - scaled / 2f - scaled / 2 - scaled), buttonScale);

    
            DrawTexture(menuLeft, new Vector2(400-scaled, 100f), buttonScale);
            DrawTexture(menuSelect, new Vector2(400, 100f), buttonScale);
            DrawTexture(menuRight, new Vector2(400+scaled, 100f), buttonScale);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D GetButtonTexture(ArcadeButton button, Texture2D normal, Texture2D clicked, int player)
        {
            bool pressed;
            if (player == 0)
                pressed = ArcadeController.Player1.GetPressed(button);
            else
                pressed = ArcadeController.Player2.GetPressed(button);

            return pressed ? clicked : normal;
        }

        private Texture2D GetJoystickTexture(Vector2 joy)
        {
            Texture2D joyTex = this.center;

            if (joy.X == 1f && joy.Y == 1f)
                joyTex = this.upRight;
            else if (joy.X == -1f && joy.Y == 1f)
                joyTex = this.upLeft;
            else if (joy.X == 1f && joy.Y == -1f)
                joyTex = this.downRight;
            else if (joy.X == -1f && joy.Y == -1f)
                joyTex = this.downLeft;
            else if (joy.X == 1f)
                joyTex = this.right;
            else if (joy.X == -1f)
                joyTex = this.left;
            else if (joy.Y == 1)
                joyTex = this.up;
            else if (joy.Y == -1f)
                joyTex = this.down;
            return joyTex;
        }

        private void DrawTexture(Texture2D texture, Vector2 pos, float scale = 1.0f)
        {
            spriteBatch.Draw(
                texture,
                pos,
                null,
                Color.White,
                0f,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}