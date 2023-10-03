using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;

namespace LBSArcade
{
    public class ArcadeController
    {
        public static ArcadeController Player1 { get; private set; } = new ArcadeController(0);
        public static ArcadeController Player2 { get; private set; } = new ArcadeController(1);

        private static int triggerZone = 30000;

        private int player;
        private bool joystickConnected;
        private bool calibrated;
        private bool calibrating;
        private JoystickState oldState;
        private JoystickState newState;
        private bool updateKeyboard = false;

        public ArcadeController(int playerIndex)
        {
            this.joystickConnected = Joystick.LastConnectedIndex >= playerIndex;
            this.player = playerIndex;
            this.calibrated = false;
            this.calibrating = false;
        }

        public void ShouldUpdateKeyboard(bool value) => this.updateKeyboard = value;

        public void Update()
        {
            if (this.updateKeyboard)
                Keyboard.GetState();

            if (this.joystickConnected)
                this.oldState = this.newState;

            this.newState = Joystick.GetState(this.player);

            this.joystickConnected = this.newState.IsConnected;

            if (!this.joystickConnected)
            {
                Player1.calibrated = false;
                Player2.calibrated = false;
            }
        }

        public bool JustPressedDown(int button)
        {
            if (!this.joystickConnected)
                return GetJustPressedKeyboard((ArcadeButton)button);

            Calibrate();

            return this.newState.Buttons[button] == ButtonState.Pressed &&
                   this.oldState.Buttons[button] == ButtonState.Released;
        }

        public bool JustPressedDown(ArcadeButton button)
        {
            return JustPressedDown((int)button);
        }

        private bool GetJustPressedKeyboard(ArcadeButton button)
        {
            return button switch
            {
                ArcadeButton.Green  => this == Player1 ? Keyboard.GetKeyDown(Keys.R) : Keyboard.GetKeyDown(Keys.U),
                ArcadeButton.Yellow => this == Player1 ? Keyboard.GetKeyDown(Keys.F) : Keyboard.GetKeyDown(Keys.J),
                ArcadeButton.Blue   => this == Player1 ? Keyboard.GetKeyDown(Keys.T) : Keyboard.GetKeyDown(Keys.I),
                ArcadeButton.Red    => this == Player1 ? Keyboard.GetKeyDown(Keys.G) : Keyboard.GetKeyDown(Keys.K),
                ArcadeButton.Top    => this == Player1 ? Keyboard.GetKeyDown(Keys.Y) : Keyboard.GetKeyDown(Keys.O),
                ArcadeButton.Bottom => this == Player1 ? Keyboard.GetKeyDown(Keys.H) : Keyboard.GetKeyDown(Keys.L),

                ArcadeButton.MenuLeft   => this == Player1 && Keyboard.GetKeyDown(Keys.Left), // if this is not false
                ArcadeButton.MenuSelect => this == Player1 && Keyboard.GetKeyDown(Keys.Enter), // it may cause an error
                ArcadeButton.MenuRight  => this == Player1 && Keyboard.GetKeyDown(Keys.Right), // with the calibrate code
                _ => false
            };
        }

        private bool GetPressedKeyboard(ArcadeButton button)
        {
            return button switch
            {
                ArcadeButton.Green  => this == Player1 ? Keyboard.IsPressed(Keys.R) : Keyboard.IsPressed(Keys.U),
                ArcadeButton.Yellow => this == Player1 ? Keyboard.IsPressed(Keys.F) : Keyboard.IsPressed(Keys.J),
                ArcadeButton.Blue   => this == Player1 ? Keyboard.IsPressed(Keys.T) : Keyboard.IsPressed(Keys.I),
                ArcadeButton.Red    => this == Player1 ? Keyboard.IsPressed(Keys.G) : Keyboard.IsPressed(Keys.K),
                ArcadeButton.Top    => this == Player1 ? Keyboard.IsPressed(Keys.Y) : Keyboard.IsPressed(Keys.O),
                ArcadeButton.Bottom => this == Player1 ? Keyboard.IsPressed(Keys.H) : Keyboard.IsPressed(Keys.L),

                ArcadeButton.MenuLeft   => this == Player1 && Keyboard.IsPressed(Keys.Left),
                ArcadeButton.MenuSelect => this == Player1 && Keyboard.IsPressed(Keys.Enter),
                ArcadeButton.MenuRight  => this == Player1 && Keyboard.IsPressed(Keys.Right),
                _ => false
            };
        }

        public bool GetPressed(int button)
        {
            if (!this.joystickConnected)
                return GetPressedKeyboard((ArcadeButton)button);

            Calibrate();

            return this.newState.Buttons[button] == ButtonState.Pressed;
        }

        public bool GetPressed(ArcadeButton button)
        {
            return GetPressed((int)button);
        }

        public Vector2 GetJoystick()
        {
            if (!this.joystickConnected)
                return Vector2.Zero;

            Vector2 result = new(0, 0);
            int[] axes = this.newState.Axes;

            result.X = axes[0] > triggerZone ? 1 : axes[0] < -triggerZone ? -1 : 0;
            result.Y = axes[1] > triggerZone ? -1 : axes[1] < -triggerZone ? 1 : 0;

            return result;
        }

        public Vector2 GetJoystickJustPressed()
        {
            if (!this.joystickConnected)
                return Vector2.Zero;

            Vector2 result = new(0, 0);
            int[] axes = this.newState.Axes;
            int[] oldAxes = this.oldState.Axes;

            result.X = axes[0] > triggerZone && oldAxes[0] < triggerZone ? 1 : axes[0] < -triggerZone && oldAxes[0] > -triggerZone ? -1 : 0;
            result.Y = axes[1] > triggerZone && oldAxes[1] < triggerZone ? 1 : axes[1] < -triggerZone && oldAxes[1] > -triggerZone ? -1 : 0;

            return result;
        }

        private void Calibrate()
        {
            if (this.calibrated || this.calibrating) return;

            Player1.calibrating = true;
            Player2.calibrating = true;

            bool player2Left = Player2.GetPressed(ArcadeButton.MenuLeft);
            bool player2Select = Player2.GetPressed(ArcadeButton.MenuSelect);
            bool player2Right = Player2.GetPressed(ArcadeButton.MenuRight);

            bool isPlayer1 = this == Player1;

            // if player 2 is pressing the menu buttons then we know
            // that player 2 is supposed to be player 1.
            if ((player2Left || player2Select || player2Right) && isPlayer1)
            {
                int tempPlayer = Player1.player;
                Player1.player = Player2.player;
                Player2.player = tempPlayer;

                Player1.Update();
                Player2.Update();

                Player1.calibrated = true;
                Player2.calibrated = true;
            }

            Player1.calibrating = false;
            Player2.calibrating = false;
        }
    }

    public enum ArcadeButton
    {
        Green = 0,
        Blue = 1,
        Yellow = 3,
        Red = 4,
        Top = 2,
        Bottom = 5,
        MenuLeft = 6,
        MenuSelect = 7,
        MenuRight = 9
    }
}
