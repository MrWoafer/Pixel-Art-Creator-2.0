using System;

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
        /// <exception cref="ArgumentException"><paramref name="str"/> couldn't been parsed as a <see cref="KeyCode"/>.</exception>
        public static KeyCode Parse(string str)
        {
            if (TryParse(str, out KeyCode parsed))
            {
                return parsed;
            }
            throw new ArgumentException($"Couldn't parse the given string as a {nameof(KeyCode)}: {str}");
        }
        /// <summary>
        /// Tries parsing the given <see cref="string"/> as a Unity <see cref="KeyCode"/>, returning whether the parsing was successful.
        /// </summary>
        /// <returns>
        /// Whether the parsing was successful.
        /// </returns>
        /// <param name="parsed">The result of the parsing, if successful. If unsuccessful, it will be <see cref="KeyCode.None"/>.</param>
        public static bool TryParse(string str, out KeyCode parsed)
        {
            bool succeeded;
            (parsed, succeeded) = str.ToLower() switch
            {
                "a" => (KeyCode.A, true),
                "b" => (KeyCode.B, true),
                "c" => (KeyCode.C, true),
                "d" => (KeyCode.D, true),
                "e" => (KeyCode.E, true),
                "f" => (KeyCode.F, true),
                "g" => (KeyCode.G, true),
                "h" => (KeyCode.H, true),
                "i" => (KeyCode.I, true),
                "j" => (KeyCode.J, true),
                "k" => (KeyCode.K, true),
                "l" => (KeyCode.L, true),
                "m" => (KeyCode.M, true),
                "n" => (KeyCode.N, true),
                "o" => (KeyCode.O, true),
                "p" => (KeyCode.P, true),
                "q" => (KeyCode.Q, true),
                "r" => (KeyCode.R, true),
                "s" => (KeyCode.S, true),
                "t" => (KeyCode.T, true),
                "u" => (KeyCode.U, true),
                "v" => (KeyCode.V, true),
                "w" => (KeyCode.W, true),
                "x" => (KeyCode.X, true),
                "y" => (KeyCode.Y, true),
                "z" => (KeyCode.Z, true),
                "0" => (KeyCode.Alpha0, true),
                "1" => (KeyCode.Alpha1, true),
                "2" => (KeyCode.Alpha2, true),
                "3" => (KeyCode.Alpha3, true),
                "4" => (KeyCode.Alpha4, true),
                "5" => (KeyCode.Alpha5, true),
                "6" => (KeyCode.Alpha6, true),
                "7" => (KeyCode.Alpha7, true),
                "8" => (KeyCode.Alpha8, true),
                "9" => (KeyCode.Alpha9, true),
                "space" or " " => (KeyCode.Space, true),
                "backspace" => (KeyCode.Backspace, true),
                "esc" or "escape" => (KeyCode.Escape, true),
                "enter" or "return" => (KeyCode.Return, true),
                "shift" or "lshift" => (KeyCode.LeftShift, true),
                "rshift" => (KeyCode.RightShift, true),
                "ctrl" or "lctrl" => (KeyCode.LeftControl, true),
                "rctrl" => (KeyCode.RightControl, true),
                "alt" or "lalt" => (KeyCode.LeftAlt, true),
                "ralt" => (KeyCode.RightAlt, true),
                "+" => (KeyCode.Plus, true),
                "-" => (KeyCode.Minus, true),
                "=" => (KeyCode.Equals, true),
                "_" => (KeyCode.Underscore, true),
                _ => (KeyCode.None, false)
            };

            return succeeded;
        }
    }
}
