using UnityEngine;

namespace PAC.Extensions
{
    public static class KeyCodeExtensions
    {
        public static KeyCode StrToKeyCode(string str)
        {
            switch (str.ToLower())
            {
                case "a": return KeyCode.A;
                case "b": return KeyCode.B;
                case "c": return KeyCode.C;
                case "d": return KeyCode.D;
                case "e": return KeyCode.E;
                case "f": return KeyCode.F;
                case "g": return KeyCode.G;
                case "h": return KeyCode.H;
                case "i": return KeyCode.I;
                case "j": return KeyCode.J;
                case "k": return KeyCode.K;
                case "l": return KeyCode.L;
                case "m": return KeyCode.M;
                case "n": return KeyCode.N;
                case "o": return KeyCode.O;
                case "p": return KeyCode.P;
                case "q": return KeyCode.Q;
                case "r": return KeyCode.R;
                case "s": return KeyCode.S;
                case "t": return KeyCode.T;
                case "u": return KeyCode.U;
                case "v": return KeyCode.V;
                case "w": return KeyCode.W;
                case "x": return KeyCode.X;
                case "y": return KeyCode.Y;
                case "z": return KeyCode.Z;
                case "0": return KeyCode.Alpha0;
                case "1": return KeyCode.Alpha1;
                case "2": return KeyCode.Alpha2;
                case "3": return KeyCode.Alpha3;
                case "4": return KeyCode.Alpha4;
                case "5": return KeyCode.Alpha5;
                case "6": return KeyCode.Alpha6;
                case "7": return KeyCode.Alpha7;
                case "8": return KeyCode.Alpha8;
                case "9": return KeyCode.Alpha9;
                case "space": return KeyCode.Space;
                case " ": return KeyCode.Space;
                case "backspace": return KeyCode.Backspace;
                case "esc": return KeyCode.Escape;
                case "escape": return KeyCode.Escape;
                case "enter": return KeyCode.Return;
                case "return": return KeyCode.Return;
                case "shift": return KeyCode.LeftShift;
                case "lshift": return KeyCode.LeftShift;
                case "rshift": return KeyCode.RightShift;
                case "ctrl": return KeyCode.LeftControl;
                case "lctrl": return KeyCode.LeftControl;
                case "rctrl": return KeyCode.RightControl;
                case "alt": return KeyCode.LeftAlt;
                case "lalt": return KeyCode.LeftAlt;
                case "ralt": return KeyCode.RightAlt;
                case "+": return KeyCode.Plus;
                case "-": return KeyCode.Minus;
                case "=": return KeyCode.Equals;
                case "_": return KeyCode.Underscore;
                default: return KeyCode.None;
            }
        }

        public static bool IsDigit(KeyCode keyCode)
        {
            return keyCode == KeyCode.Alpha0 || keyCode == KeyCode.Alpha1 || keyCode == KeyCode.Alpha2 || keyCode == KeyCode.Alpha3 || keyCode == KeyCode.Alpha4 || keyCode == KeyCode.Alpha5 ||
                   keyCode == KeyCode.Alpha6 || keyCode == KeyCode.Alpha7 || keyCode == KeyCode.Alpha8 || keyCode == KeyCode.Alpha9;
        }

        public static bool IsAlpha(KeyCode keyCode)
        {
            foreach(char chr in "abcdefghijklmnopqrstuvwxyz")
            {
                if (keyCode == StrToKeyCode(chr.ToString()))
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
