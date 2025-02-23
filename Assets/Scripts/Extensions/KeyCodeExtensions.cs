using UnityEngine;

namespace PAC.Extensions
{
    public static class KeyCodeExtensions
    {
        public static KeyCode Parse(string str) => str.ToLower() switch
        {
            "a" => KeyCode.A,
            "b" => KeyCode.B,
            "c" => KeyCode.C,
            "d" => KeyCode.D,
            "e" => KeyCode.E,
            "f" => KeyCode.F,
            "g" => KeyCode.G,
            "h" => KeyCode.H,
            "i" => KeyCode.I,
            "j" => KeyCode.J,
            "k" => KeyCode.K,
            "l" => KeyCode.L,
            "m" => KeyCode.M,
            "n" => KeyCode.N,
            "o" => KeyCode.O,
            "p" => KeyCode.P,
            "q" => KeyCode.Q,
            "r" => KeyCode.R,
            "s" => KeyCode.S,
            "t" => KeyCode.T,
            "u" => KeyCode.U,
            "v" => KeyCode.V,
            "w" => KeyCode.W,
            "x" => KeyCode.X,
            "y" => KeyCode.Y,
            "z" => KeyCode.Z,
            "0" => KeyCode.Alpha0,
            "1" => KeyCode.Alpha1,
            "2" => KeyCode.Alpha2,
            "3" => KeyCode.Alpha3,
            "4" => KeyCode.Alpha4,
            "5" => KeyCode.Alpha5,
            "6" => KeyCode.Alpha6,
            "7" => KeyCode.Alpha7,
            "8" => KeyCode.Alpha8,
            "9" => KeyCode.Alpha9,
            "space" or " " => KeyCode.Space,
            "backspace" => KeyCode.Backspace,
            "esc" or "escape" => KeyCode.Escape,
            "enter" or "return" => KeyCode.Return,
            "shift" or "lshift" => KeyCode.LeftShift,
            "rshift" => KeyCode.RightShift,
            "ctrl" or "lctrl" => KeyCode.LeftControl,
            "rctrl" => KeyCode.RightControl,
            "alt" or "lalt" => KeyCode.LeftAlt,
            "ralt" => KeyCode.RightAlt,
            "+" => KeyCode.Plus,
            "-" => KeyCode.Minus,
            "=" => KeyCode.Equals,
            "_" => KeyCode.Underscore,
            _ => KeyCode.None,
        };

        public static bool IsDigit(KeyCode keyCode) =>
            keyCode == KeyCode.Alpha0 ||
            keyCode == KeyCode.Alpha1 ||
            keyCode == KeyCode.Alpha2 ||
            keyCode == KeyCode.Alpha3 ||
            keyCode == KeyCode.Alpha4 ||
            keyCode == KeyCode.Alpha5 ||
            keyCode == KeyCode.Alpha6 ||
            keyCode == KeyCode.Alpha7 ||
            keyCode == KeyCode.Alpha8 ||
            keyCode == KeyCode.Alpha9;

        public static bool IsAlpha(KeyCode keyCode)
        {
            foreach(char chr in "abcdefghijklmnopqrstuvwxyz")
            {
                if (keyCode == Parse(chr.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAlphanumeric(KeyCode keyCode)
        {
            return IsDigit(keyCode) || IsAlpha(keyCode);
        }
    }
}
