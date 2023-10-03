using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace LBSArcade
{
    internal class GameContainer
    {
        public static string GamesDirectory;
        public static Vector2 ImageSize = new(300, 400);
        public static int NameLength = 19;
        public static int BigLength = 50;
        public static int BigXOffset = 50;
        public static int BigYOffset = 64;
        public static bool GameRunning = false;
        public static int CloseKey = 0x41; // default: enter


        public string name { get; private set; }

        private static Texture2D backDrop;
        private static Texture2D backDropError;
        private static Vector2 backDropOffset;
        private static Vector2 textOffset;

        private Texture2D texture;
        private Vector2 position;
        private float scale;
        private string developer;
        private string releaseDate;
        private string aboutText;
        private string exeName;
        private string folderName;
        private Process game;
        private uint gameOpenedTime;
        private bool corrupt = false;

        public GameContainer(string name, out bool error, Vector2 position = default, bool corrupt = false)
        {
            LoadAbout(name, out bool infoErr);
            this.texture = LoadTexture(name, out bool loadErr);
            this.position = position;
            this.scale = 1f;
            this.folderName = name;
            this.corrupt = corrupt;
            this.exeName = GamesDirectory + name + "/" + name + ".exe";
            this.name = name;


            LoadTime(out bool timeErr);

            error = infoErr | loadErr | timeErr;

            backDrop ??= Game.Instance.Content.Load<Texture2D>("Backdrop");
            backDropError ??= Game.Instance.Content.Load<Texture2D>("BackdropError");

            if (backDropOffset == default)
                backDropOffset = new Vector2(5, 37);
            if (textOffset == default)
                textOffset = new Vector2(4, -28);
        }
        private GameContainer(Texture2D texture, Vector2 position = default, float scale = 1f, bool corrupt = false)
        {
            this.texture = texture;
            this.position = position;
            this.scale = scale;
            this.corrupt = corrupt;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        public float GetScale()
        {
            return this.scale;
        }

        public Vector2 GetPosition()
        {
            return this.position;
        }

        public string GetShortName(int length)
        {
            return LimitString(this.name, length);
        }

        public void SetCorrupted(bool value)
        {
            this.corrupt = value;
        }

        public void Render(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Draw(
                this.corrupt ? backDropError : backDrop,
                this.position - backDropOffset,
                null,
                Microsoft.Xna.Framework.Color.White,
                0,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0f);

            spriteBatch.DrawString(
                font,
                LimitString(this.name, NameLength),
                this.position + textOffset,
                this.corrupt ? Microsoft.Xna.Framework.Color.White : Microsoft.Xna.Framework.Color.Black
                );

            spriteBatch.Draw(
                this.texture,
                this.position,
                null,
                Microsoft.Xna.Framework.Color.White,
                0,
                Vector2.Zero,
                new Vector2(this.scale, this.scale),
                SpriteEffects.None,
                0f);
        }

        public void RenderBig(SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 position)
        {
            spriteBatch.Draw(
                this.texture,
                position,
                null,
                Microsoft.Xna.Framework.Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);

            spriteBatch.DrawString(
                font,
                LimitString(this.name, BigLength),
                position + new Vector2(this.texture.Width * scale + BigXOffset, BigYOffset * 0),
                Microsoft.Xna.Framework.Color.Black
                );

            string gameOpenedTimeString;
            float minutes = MathF.Floor(this.gameOpenedTime / 60f);
            float hours = MathF.Floor(minutes / 60f);

            if (hours > 0)
                gameOpenedTimeString = hours.ToString() + (hours > 1 ? " hours" : " hour");
            else if (minutes > 0)
                gameOpenedTimeString = minutes.ToString() + (minutes > 1 ? " minutes" : " minute");
            else
                gameOpenedTimeString = this.gameOpenedTime.ToString() + (this.gameOpenedTime > 1 || this.gameOpenedTime == 0 ? " seconds" : " second");

            spriteBatch.DrawString(
                font,
                "Time Played: " + gameOpenedTimeString,
                position + new Vector2(this.texture.Width * scale + BigXOffset, BigYOffset * 1),
                Microsoft.Xna.Framework.Color.Black
                );

            if (corrupt)
                spriteBatch.DrawString(
                font,
                "(This game is corrupted)",
                position + new Vector2(this.texture.Width * scale + BigXOffset, BigYOffset * 2),
                Microsoft.Xna.Framework.Color.Black
                );

            spriteBatch.DrawString(
                font,
                this.aboutText,
                position + new Vector2(this.texture.Width * scale + BigXOffset, this.corrupt ? BigYOffset * 3 : BigYOffset * 2),
                Microsoft.Xna.Framework.Color.Black
                );
        }

        public GameContainer Copy()
        {
            return new(this.texture, this.position, this.scale, this.corrupt);
        }

        public void StartGame(out bool error)
        {
            try
            {
                this.game = new Process();
                this.game.StartInfo.FileName = this.exeName;
                this.game.Start();
                GameRunning = true;

                new Thread(new ThreadStart(KeepGameOnTop)).Start();
                error = false;
            }

            catch (Exception e)
            {
                Logger.Error(e);
                error = true;
            }
        }

        private void KeepGameOnTop()
        {
            DateTime startTime = DateTime.Now;
            uint before = this.gameOpenedTime;
            DLLImports.SetWindowFocus(this.game, true);

            while (this.game.HasExited == false)
            {
                this.gameOpenedTime = (uint)(DateTime.Now - startTime).Seconds;

                
                if (DLLImports.GetAsyncKeyState(CloseKey) < 0 || ArcadeController.Player1.JustPressedDown(ArcadeButton.MenuSelect))
                {
                    this.game.Kill(true);
                    break;
                }

                //if (!DLLImports.IsWindowFocused(this.game))
                    //DLLImports.SetWindowFocus(this.game);
            }
            Game.Instance.RestartIntro();

            this.gameOpenedTime = before + this.gameOpenedTime;
            SaveTime(out bool _);

            DLLImports.SetWindowFocus(Process.GetCurrentProcess());
            GameRunning = false;


            this.game.Dispose();
        }

        private void LoadTime(out bool error)
        {
            try
            {
                uint time;
                string path = GamesDirectory + this.folderName + "/time";

                try
                {
                    if (File.Exists(path))
                        time = Convert.ToUInt32(File.ReadAllText(path));
                    else
                        time = 0;
                }

                catch
                {
                    time = 0;
                    Logger.Error($"Couldn't parse time of game: {this.name}");
                }


                this.gameOpenedTime = time;
                error = false;
            }

            catch (Exception e)
            {
                Logger.Error(e);
                error = true;
            }
        }

        private void SaveTime(out bool error)
        {
            try
            {
                string path = GamesDirectory + this.folderName + "/time";

                if (File.Exists(path))
                    File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.Hidden); // unhide it

                File.WriteAllText(path, this.gameOpenedTime.ToString());
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden); // hide it
                error = false;
            }

            catch (Exception e)
            {
                Logger.Error(e);
                error = true;
            }

        }

        private string LimitString(string s, int length)
        {
            return s.Length > length ? s.Substring(0, length - 3) + "..." : s;
        }

        private void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }
        private string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }

        private void LoadAbout(string name, out bool error)
        {
            try
            {
                this.aboutText = File.ReadAllText(GamesDirectory + name + "/about.txt");
                this.aboutText = SpliceText(this.aboutText, 60);

                //string[] lines = File.ReadAllLines(GamesDirectory + name + "/about.txt");
                error = false;
            }

            catch (Exception e)
            {
                error = true;
                Logger.Error(e);
            }
        }

        private Texture2D LoadTexture(string name, out bool error)
        {
            // No using statement since you can't change a 'using' variable
            try
            {
                FileStream fileStream = new(GamesDirectory + name + "/thumbnail.png", FileMode.Open);
                using Image image = Image.Load(fileStream);

                // If the image is not the right size then resize it
                if (image.Size() != new Size((int)ImageSize.X, (int)ImageSize.Y))
                {
                    fileStream.Dispose();
                    image.Mutate(x => x.Resize((int)ImageSize.X, (int)ImageSize.Y));
                    image.SaveAsPng(GamesDirectory + name + "/thumbnail.png");
                    fileStream = new(GamesDirectory + name + "/thumbnail.png", FileMode.Open);
                }

                Texture2D sprite = Texture2D.FromStream(Game.Instance.GraphicsDevice, fileStream);

                fileStream.Dispose();

                error = false;

                return sprite;
            }

            catch (Exception e)
            {
                Logger.Error(e);
                error = true;
                return null;
            }
        }
    }
}
