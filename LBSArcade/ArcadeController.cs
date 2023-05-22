using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        public ArcadeController(int playerIndex)
        {
            this.joystickConnected = Joystick.LastConnectedIndex >= playerIndex;
            this.player = playerIndex;
            this.calibrated = false;
            this.calibrating = false;
        }

        public void Update()
        {
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
                return false;

            Calibrate();

            return this.newState.Buttons[button] == ButtonState.Pressed &&
                   this.oldState.Buttons[button] == ButtonState.Released;
        }

        public bool JustPressedDown(ArcadeButton button)
        {
            return JustPressedDown((int)button);
        }

        public bool GetPressed(int button)
        {
            if (!this.joystickConnected)
                return false;

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
