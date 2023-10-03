using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace LBSArcade
{
    internal static class Keyboard
    {
        public static KeyboardState CurrentKeyState { get; private set; }
        public static KeyboardState PreviousKeyState { get; private set; }

        public static void GetState()
        {
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static bool IsPressed(Keys key)
        {
            return CurrentKeyState.IsKeyDown(key);
        }

        public static bool GetKeyDown(Keys key)
        {
            return CurrentKeyState.IsKeyDown(key) && !PreviousKeyState.IsKeyDown(key);
        }

        public static Keys[] AnyKeyDown()
        {
            Keys[] current = CurrentKeyState.GetPressedKeys();
            Keys[] prev = PreviousKeyState.GetPressedKeys();

            return current.Union(prev).ToArray();
            /*int length = current.Length + prev.Length;

            List<Keys> distinct = new List<Keys>();

            for (int i = 0; i < current.Length; i++)
            {
                bool duplicate = false;

                for (int j = 0; j < current.Length; j++)
                {
                    if (current[i] == current[j])
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate == false)
                    distinct.Add(current[i]);
            }

            return distinct.ToArray();*/

        }
    }
}
