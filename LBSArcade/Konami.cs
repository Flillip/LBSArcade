using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace LBSArcade
{
    internal class Konami
    {
        private const float WaitTime = 1f;
        private JoystickKeys[] joyKeys = new JoystickKeys[]
        {
            JoystickKeys.Up,
            JoystickKeys.Up,
            JoystickKeys.Down,
            JoystickKeys.Down,
            JoystickKeys.Left,
            JoystickKeys.Right,
            JoystickKeys.Left,
            JoystickKeys.Right,
            JoystickKeys.A,
            JoystickKeys.B
        };

#if DEBUG
        private Keys[] keys = new Keys[]
        {
            Keys.Up,
            Keys.Up,
            Keys.Down,
            Keys.Down,
            Keys.Left,
            Keys.Right,
            Keys.Left,
            Keys.Right,
            Keys.B,
            Keys.A
        };
#endif

#if RELEASE
        private Keys[] keys = new Keys[]
        {
            Keys.W,
            Keys.W,
            Keys.S,
            Keys.S,
            Keys.A,
            Keys.D,
            Keys.A,
            Keys.D,
            Keys.D1,
            Keys.D3
        };
#endif

        public bool Success { get; private set; }

        private float timer = 0f;
        private int index = 0;
        private int sleepAmount;

        public Konami()
        {
            this.sleepAmount = Settings.GetData<int>("konamiTimer");
        }

        private bool CheckJoyStick(JoystickKeys key)
        {
            switch (key)
            {
                case JoystickKeys.Up:
                    return ArcadeController.Player1.GetJoystickJustPressed().Y == -1;
                case JoystickKeys.Down:
                    return ArcadeController.Player1.GetJoystickJustPressed().Y == 1;
                case JoystickKeys.Left:
                    return ArcadeController.Player1.GetJoystickJustPressed().X == -1;
                case JoystickKeys.Right:
                    return ArcadeController.Player1.GetJoystickJustPressed().X == 1;
                case JoystickKeys.A:
                    return ArcadeController.Player1.JustPressedDown(ArcadeButton.Yellow);
                case JoystickKeys.B:
                    return ArcadeController.Player1.JustPressedDown(ArcadeButton.Red);
                case JoystickKeys.C:
                    return ArcadeController.Player1.JustPressedDown(ArcadeButton.Bottom);
                default:
                    return false;
            }
        }

        public void Update(float delta)
        {
            Keys[] keysPressed = Keyboard.AnyKeyDown();

            if (Keyboard.GetKeyDown(keys[index]) || CheckJoyStick(joyKeys[index]))
            {
                int[] axes = Joystick.GetState(0).Axes;
                Logger.Debug(joyKeys[index]);

                index++;

                if (index == keys.Length)
                {
                    this.Success = true;
                    timer = 0f;
                    index = 0;
                    Logger.Debug("SUCCESS!");

                    new Thread(() =>
                    {
                        Thread.Sleep(this.sleepAmount);
                        this.Success = false;
                    }).Start();
                }

                else
                {
                    timer = WaitTime;
                    Logger.Debug("Right key pressed: " + keys[index - 1]);
                }
            }

            else if (keysPressed.Length > 0)
            {
                timer = 0;
                index = 0;

                Logger.Debug("Wrong keys pressed");
            }

            if (timer > 0)
            {
                timer -= delta;

                if (timer <= 0)
                {
                    timer = 0;
                    index = 0;
                    Logger.Debug("Giving up out of time");
                }
            }
        }
    }

    internal enum JoystickKeys
    {
        Up, Down, Left, Right, A, B, C, D, E, F
    }
}
