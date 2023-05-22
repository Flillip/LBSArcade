using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LBSArcade
{
    internal class UI
    {
        private List<string> corruptGames;
        private Konami konami;
        private Background background;
        private SpriteFont smallFont;
        private SpriteFont largeFont;
        private GameContainer[] games;
        private GameContainer[] lerpCopy;
        private Texture2D border;
        private Texture2D corruptedBackdrop;
        private Vector2 selectedGamePosition;
        private Vector2 pointerPos;
        private Vector2 pointerPosLerpCopy;
        private Vector2 imageSize;
        private Vector2 borderOffset;
        private Vector2 corruptedBackdropSize;
        private Vector2 noGamesTextOffset;
        private float spacing, distanceBetweenGames;
        private float gameAnimationTimeElapsed;
        private float gameAnimationLerpDuration;
        private float cursorAnimationTimeElapsed;
        private float cursorAnimationLerpDuration;
        private float borderOppacity;
        private float borderFadeSpeed;
        private float focusedGameScale;
        private string gamesDirectory;
        private int gamesPointer, moveDir;
        private bool shouldMovePointer, shouldMove;
        private bool gameJustCorrupted;
        private float justCorruptedTimer;
        private float justCorruptedTimerMax;

#if DEBUG
        private readonly Keys Left = Keys.Left;
        private readonly Keys Right = Keys.Right;
        private readonly Keys Select = Keys.Enter;
#endif

#if RELEASE
        private readonly Keys Left = Keys.D1;
        private readonly Keys Right = Keys.D3;
        private readonly Keys Select = Keys.D2;
#endif

        public UI()
        {
            this.gamesPointer = 0;
            this.shouldMove = false;
            this.shouldMovePointer = false;
            this.gameAnimationTimeElapsed = 0;
            this.cursorAnimationTimeElapsed = 0;
            this.borderOppacity = 0;
            this.konami = new();
            this.corruptGames = new List<string>();
            this.gameJustCorrupted = false;
            this.justCorruptedTimer = 0f;

            this.focusedGameScale = Settings.GetData<float>(nameof(this.focusedGameScale));
            this.gamesDirectory = Settings.GetData<string>(nameof(this.gamesDirectory));
            GameContainer.GamesDirectory = this.gamesDirectory;
            this.spacing = Settings.GetData<float>(nameof(this.spacing));
            this.imageSize = Settings.GetVector2(nameof(this.imageSize)) * this.focusedGameScale;
            this.gameAnimationLerpDuration = Settings.GetData<float>(nameof(this.gameAnimationLerpDuration));
            this.cursorAnimationLerpDuration = Settings.GetData<float>(nameof(this.cursorAnimationLerpDuration));
            this.borderFadeSpeed = Settings.GetData<float>(nameof(this.borderFadeSpeed));
            this.selectedGamePosition = Settings.GetVector2(nameof(this.selectedGamePosition));
            this.justCorruptedTimerMax = Settings.GetData<float>(nameof(this.justCorruptedTimerMax));
            this.noGamesTextOffset = Settings.GetVector2(nameof(this.noGamesTextOffset));
        }

        public void LoadUI()
        {
            // load fonts
            this.smallFont = Game.Instance.Content.Load<SpriteFont>("SmallFont");
            this.largeFont = Game.Instance.Content.Load<SpriteFont>("LargeFont");

            // create corrupted backdrop
            this.corruptedBackdropSize = new((int)(Game.ScreenSize.X / 3f), (int)(Game.ScreenSize.Y / 3f));
            this.corruptedBackdrop = new Texture2D(Game.Instance.GraphicsDevice, (int)this.corruptedBackdropSize.X, (int)this.corruptedBackdropSize.Y);
            Color[] colorData = new Color[(int)(this.corruptedBackdropSize.X * this.corruptedBackdropSize.Y)];

            // fill corrupted backgrop
            Color errorColor = Settings.GetColor(nameof(errorColor));
            Array.Fill(colorData, errorColor);
            this.corruptedBackdrop.SetData(colorData);

            // load border
            this.border = Game.Instance.Content.Load<Texture2D>("Border");
            this.borderOffset = Settings.GetVector2(nameof(this.borderOffset));

            // load waves
            int amountOfWaves = Settings.GetData<int>(nameof(amountOfWaves));
            string[] waves = new string[amountOfWaves];
            string[] wavesFlipped = new string[amountOfWaves];

            for (int i = 0; i < amountOfWaves; i++)
            {
                waves[i] = File.ReadAllText("./Content/Wave" + (i + 1) + ".txt");
                wavesFlipped[i] = File.ReadAllText("./Content/Wave" + (i + 1) + "flip.txt");
            }

            Texture2D waveBackground = Game.Instance.Content.Load<Texture2D>("WaveBackground");

            // load pallete
            Color pallete2 = Settings.GetColor(nameof(pallete2));
            Color pallete3 = Settings.GetColor(nameof(pallete3));
            Color pallete4 = Settings.GetColor(nameof(pallete4));
            Color pallete5 = Settings.GetColor(nameof(pallete5));

            this.background = new Background(waves, wavesFlipped, new Color[] { pallete2, pallete3, pallete4, pallete5 },
                                                                                waveBackground, 500f, new(0, -400));

            GameContainer.ImageSize = this.imageSize;
            CreateGameContainers();
        }

        /// <summary>
        /// Parses the game folder and creates GameContainers.
        /// </summary>
        private void CreateGameContainers()
        {
            this.games = ParseImageFolder(out bool _);

            if (this.games.Length == 0)
                return;

            float scale = 1f;
            int yPos = 750;
            int height = 300;
            float padding = 50 * scale;

            for (int i = 0; i < this.games.Length; i++)
            {
                this.games[i].SetPosition(new(i * (height * scale) * spacing + padding, yPos));
                this.games[i].SetScale(scale / this.focusedGameScale);
            }

            this.pointerPos = this.games[this.gamesPointer].GetPosition();
            this.lerpCopy = CopyGameContainerArray(this.games);

            if (this.games.Length > 1)
                this.distanceBetweenGames = this.games[1].GetPosition().X - this.games[0].GetPosition().X;
        }

        public void Update(float delta, bool lockCursor)
        {
            this.background.Update(delta);

            if (this.games.Length == 0 || GameContainer.GameRunning || lockCursor)
                return;

            this.konami.Update(delta);

            if (this.gameJustCorrupted)
                UpdateCorrupted(delta);

            if (!this.shouldMove && !this.shouldMovePointer)
            {

                if ((ArcadeController.Player1.JustPressedDown(ArcadeButton.MenuLeft) || Keyboard.GetKeyDown(this.Left)) && this.gamesPointer > 0)
                    MovePointer(-1);

                else if ((ArcadeController.Player1.JustPressedDown(ArcadeButton.MenuRight) || Keyboard.GetKeyDown(this.Right)) && this.gamesPointer < this.games.Length - 1)
                    MovePointer(1);

                else if (ArcadeController.Player1.JustPressedDown(ArcadeButton.MenuSelect) || Keyboard.GetKeyDown(this.Select))
                    // Off load this to a new Task as to not block UI
                    new Task(StartGame).Start();

                this.lerpCopy = CopyGameContainerArray(this.games);
            }

            if (this.shouldMovePointer)
                AnimatePointer(delta);

            if (this.shouldMove)
                MoveGames(delta);

            this.borderOppacity += this.borderFadeSpeed * delta;
        }

        /// <summary>
        /// Handle Corrupted Timer
        /// </summary>
        /// <param name="delta"></param>
        private void UpdateCorrupted(float delta)
        {
            this.justCorruptedTimer += delta;

            if (this.justCorruptedTimer > this.justCorruptedTimerMax)
            {
                this.justCorruptedTimer = 0;
                this.gameJustCorrupted = false;
            }
        }

        /// <summary>
        /// Animate the gamecontainers
        /// </summary>
        /// <param name="delta"></param>
        private void MoveGames(float delta)
        {
            this.gameAnimationTimeElapsed += delta;

            for (int i = 0; i < this.games.Length; i++)
            {
                Vector2 gamePos = this.lerpCopy[i].GetPosition();
                float dirDistance = this.distanceBetweenGames * this.moveDir;
                float lerped = MathHelper.Lerp(gamePos.X, gamePos.X - dirDistance, this.gameAnimationTimeElapsed / this.gameAnimationLerpDuration);
                this.games[i].SetPosition(new(lerped, gamePos.Y));
            }

            if (this.gameAnimationTimeElapsed > this.gameAnimationLerpDuration)
            {
                this.shouldMove = false;
                this.gameAnimationTimeElapsed = 0;
            }
        }

        /// <summary>
        /// Animate the pointer
        /// </summary>
        /// <param name="delta"></param>
        private void AnimatePointer(float delta)
        {
            float offset = this.distanceBetweenGames * this.moveDir;
            // check if the next move will put the pointer outside of the screen.
            // if not move it.
            if ((this.pointerPos.X + this.border.Width + offset < Game.ScreenSize.X &&
                this.pointerPos.X + offset > 0) || this.cursorAnimationTimeElapsed != 0)
                MoveCursor(delta);
            else // if it does appear outisde. Move the games.
            {
                this.shouldMove = true;
                this.shouldMovePointer = false;
            }

            //this.shouldMovePointer = false;
        }

        /// <summary>
        /// Move pointer
        /// </summary>
        /// <param name="value"></param>
        private void MovePointer(int value)
        {
            this.gamesPointer += value;
            this.moveDir = value;
            this.shouldMovePointer = true;
            this.pointerPosLerpCopy = this.pointerPos;
            this.gameJustCorrupted = false;
            this.justCorruptedTimer = 0;
            this.borderOppacity = 0;
        }

        /// <summary>
        /// Starts a game
        /// </summary>
        private void StartGame()
        {
            GameContainer game = this.games[this.gamesPointer];
            game.StartGame(out bool error);

            if (error)
            {
                Logger.Error("Error starting " + game.name);
                this.corruptGames.Add(game.name);
                CreateGameContainers(); // Recreate them since it crashed
                this.gameJustCorrupted = true;
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            if (this.games.Length == 0)
            {
                DrawNoGames(spriteBatch);
                return;
            }

            DrawGames(spriteBatch);
            DrawBorder(spriteBatch);
            DrawBigGame(spriteBatch);
            DrawTime(spriteBatch);

            if (this.konami.Success)
                DrawKonami(spriteBatch);

            if (gameJustCorrupted)
                DrawCorrupted(spriteBatch);
        }

        private void DrawNoGames(SpriteBatch spriteBatch)
        {
            string text = "Try adding some games...";
            Vector2 size = this.largeFont.MeasureString(text);
            spriteBatch.DrawString(this.largeFont, text, (Game.ScreenSize / 2f) - (size / 2f) - noGamesTextOffset, Color.Black);
        }

        private void DrawGames(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this.games.Length; i++)
            {
                if (this.games[i].GetPosition().X < Game.ScreenSize.X)
                    this.games[i].Render(spriteBatch, this.smallFont);
            }
        }

        private void DrawBigGame(SpriteBatch spriteBatch)
        {
            this.games[this.gamesPointer].RenderBig(spriteBatch, this.largeFont, 1f, this.selectedGamePosition);
        }

        private void DrawBorder(SpriteBatch spriteBatch)
        {
            float oppacity = (MathF.Cos(this.borderOppacity) + 1f) / 2f;//MathF.Abs(MathF.Cos(this.borderOppacity));
            spriteBatch.Draw(this.border, this.pointerPos - this.borderOffset, Color.White * oppacity);
        }

        private void DrawTime(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.largeFont,
                            DateTime.Now.ToString("HH:mm:ss"),
                            new Vector2(Game.ScreenSize.X - this.selectedGamePosition.X * 2, this.selectedGamePosition.Y),
                            Color.Black
                            );
        }

        private void DrawKonami(SpriteBatch spriteBatch)
        {
            string text = "KONAMI CODE";
            Vector2 size = this.largeFont.MeasureString(text);

            spriteBatch.DrawString(this.largeFont,
                            text,
                            (Game.ScreenSize / 2f) - (size / 2f) - noGamesTextOffset,
                            Color.Black
                            );
        }

        private void DrawCorrupted(SpriteBatch spriteBatch)
        {
            Vector2 pos = (Game.ScreenSize / 2f) - (this.corruptedBackdropSize / 2f);
            spriteBatch.Draw(this.corruptedBackdrop, pos, Color.White);

            string temp = " is corrupted";
            string text = this.games[this.gamesPointer].GetShortName(18);

            temp = temp.PadLeft(text.Length / 2 - (temp.Length / 2) + temp.Length, ' ');
            text += temp;

            Vector2 textSize = this.largeFont.MeasureString(text);
            spriteBatch.DrawString(this.largeFont, text, pos + (pos / 2f - (textSize / 2f)), Color.White);
        }

        private void MoveCursor(float delta)
        {
            this.cursorAnimationTimeElapsed += delta;

            float dirDistance = this.distanceBetweenGames * this.moveDir;
            float lerped = MathHelper.Lerp(this.pointerPosLerpCopy.X, this.pointerPosLerpCopy.X + dirDistance, this.cursorAnimationTimeElapsed / this.cursorAnimationLerpDuration);
            this.pointerPos.X = lerped;

            if (this.cursorAnimationTimeElapsed > this.cursorAnimationLerpDuration)
            {
                this.pointerPos.X = this.pointerPosLerpCopy.X + dirDistance;
                this.shouldMovePointer = false;
                this.cursorAnimationTimeElapsed = 0;
            }
        }

        private GameContainer[] CopyGameContainerArray(GameContainer[] array)
        {
            GameContainer[] copy = new GameContainer[array.Length];

            for (int i = 0; i < array.Length; i++)
                copy[i] = array[i].Copy();

            return copy;
        }

        private GameContainer[] ParseImageFolder(out bool error)
        {
            try
            {
                if (!Directory.Exists(this.gamesDirectory))
                    throw new DirectoryNotFoundException(this.gamesDirectory);

                string[] files = Directory.GetDirectories(this.gamesDirectory)
                                    .Select(name => name.Replace(this.gamesDirectory, ""))
                                    .ToArray();

                error = false;

                return files.Select(name =>
                {
                    GameContainer gc = new(name, out bool error);
                    if (error)
                        this.corruptGames.Add(gc.name);

                    foreach (string corrupt in this.corruptGames)
                        if (gc.name == corrupt)
                            gc.SetCorrupted(true);
                    return gc;
                }).Where(c => c != null).ToArray();
            }

            catch (Exception e)
            {
                error = true;
                Logger.Error(e);
                return Array.Empty<GameContainer>();
            }
        }
    }
}
