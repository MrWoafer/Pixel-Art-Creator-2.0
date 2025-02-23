using UnityEngine;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for Unity's <see cref="KeyCode"/>.
    /// </summary>
    public static class KeyCodeExtensions
    {
        /// <summary>
        /// Parses the given <see cref="string"/> as a Unity <see cref="KeyCode"/>.
        /// </summary>
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
    }
}
