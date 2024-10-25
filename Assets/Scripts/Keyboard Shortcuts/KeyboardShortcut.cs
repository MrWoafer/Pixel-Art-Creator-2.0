using System;
using System.Collections.Generic;
using System.Linq;
using PAC.JSON;

namespace PAC.KeyboardShortcuts
{
    /// <summary>
    /// A class to represent a single keyboard shortcut.
    /// </summary>
    public class KeyboardShortcut : IJSONable<KeyboardShortcut>
    {
        /// <summary>The keycodes in this shortcut, kept in the order they would be read.</summary>
        public List<CustomKeyCode> keyCodes { get; private set; } = new List<CustomKeyCode>();

        /// <summary>The empty keyboard shortcut - i.e. no keycodes.</summary>
        public static KeyboardShortcut None => new KeyboardShortcut();

        /// <summary>
        /// Creates a shortcut from the given keycodes. Sorts the keycodes into the order they would be read.
        /// </summary>
        public KeyboardShortcut(params CustomKeyCode[] keyCodes)
        {
            this.keyCodes = new List<CustomKeyCode>();
            foreach (CustomKeyCode keyCode in keyCodes)
            {
                Add(keyCode);
            }
            Sort();
        }

        /// <summary>
        /// Checks if the shortcuts have the same set of keycodes.
        /// </summary>
        public static bool operator ==(KeyboardShortcut shortcut1, KeyboardShortcut shortcut2)
        {
            return shortcut1.keyCodes.ToHashSet().SetEquals(shortcut2.keyCodes.ToHashSet());
        }
        /// <summary>
        /// Checks if the shortcuts do not have the same set of keycodes.
        /// </summary>
        public static bool operator !=(KeyboardShortcut shortcut1, KeyboardShortcut shortcut2)
        {
            return !(shortcut1 == shortcut2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj.GetType() == typeof(KeyboardShortcut))
            {
                return this == (KeyboardShortcut)obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (keyCodes.Count == 0)
            {
                throw new System.Exception("CustomKeyCodes should not have 0 KeyCodes.");
            }
            if (keyCodes.Count == 1)
            {
                return keyCodes[0].GetHashCode();
            }
            if (keyCodes.Count == 2)
            {
                return HashCode.Combine(keyCodes[0], keyCodes[1]);
            }
            if (keyCodes.Count == 3)
            {
                return HashCode.Combine(keyCodes[0], keyCodes[1], keyCodes[2]);
            }
            if (keyCodes.Count == 4)
            {
                return HashCode.Combine(keyCodes[0], keyCodes[1], keyCodes[2], keyCodes[3]);
            }
            if (keyCodes.Count == 5)
            {
                return HashCode.Combine(keyCodes[0], keyCodes[1], keyCodes[2], keyCodes[3], keyCodes[4]);
            }
            throw new System.Exception("KeyboardShortcuts should not have more than 5 CustomKeyCodes. CustomKeyCodes count: " + keyCodes.Count);
        }

        /// <summary>
        /// Adds the keycode to the shortcut. Returns false if it was already in the shortcut; otherwise returns true.
        /// </summary>
        public bool Add(CustomKeyCode keyCode)
        {
            if (Contains(keyCode))
            {
                return false;
            }
            keyCodes.Add(keyCode);
            Sort();
            return true;
        }

        /// <summary>
        /// Removes the keycode from the shortcut. Returns false if the keycode already wasn't in the shortcut; otherwise returns true.
        /// </summary>
        public bool Remove(CustomKeyCode keyCode)
        {
            return keyCodes.Remove(keyCode);
        }

        /// <summary>
        /// Returns true if the keycode is in the shortcut.
        /// </summary>
        public bool Contains(CustomKeyCode keyCode)
        {
            return keyCodes.Contains(keyCode);
        }

        /// <summary>
        /// Returns true while all keys are being held down.
        /// </summary>
        public bool GetKeys()
        {
            foreach (CustomKeyCode keyCode in keyCodes)
            {
                if (!keyCode.GetKey())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true the frame the final key is pressed.
        /// </summary>
        public bool GetKeysDown()
        {
            int keysDown = 0;
            foreach (CustomKeyCode keyCode in keyCodes)
            {
                if (keyCode.GetKeyDown())
                {
                    keysDown++;
                }
                else if (!keyCode.GetKey())
                {
                    return false;
                }
            }
            return keysDown > 0;
        }

        /// <summary>
        /// Returns true the frame (any key of) this shortcut is released.
        /// </summary>
        public bool GetKeysUp()
        {
            int keysUp = 0;
            foreach (CustomKeyCode keyCode in keyCodes)
            {
                if (keyCode.GetKeyUp())
                {
                    keysUp++;
                }
                else if (!keyCode.GetKey())
                {
                    return false;
                }
            }
            return keysUp > 0;
        }

        /// <summary>
        /// Sorts the keycodes into the order they would be read:
        /// Ctrl Alt Shift A-Z 0-9 other
        /// </summary>
        private void Sort()
        {
            keyCodes.Sort(delegate (CustomKeyCode keyCode1, CustomKeyCode keyCode2) { return CompareKeyCodes(keyCode1, keyCode2); });
        }
        private int CompareKeyCodes(CustomKeyCode keyCode1, CustomKeyCode keyCode2)
        {
            if (keyCode1 == keyCode2)
            {
                return 0;
            }

            if (keyCode1 == CustomKeyCode.Ctrl)
            {
                return -1;
            }
            if (keyCode2 == CustomKeyCode.Ctrl)
            {
                return 1;
            }
            if (keyCode1 == CustomKeyCode.Alt)
            {
                return -1;
            }
            if (keyCode2 == CustomKeyCode.Alt)
            {
                return 1;
            }
            if (keyCode1 == CustomKeyCode.Shift)
            {
                return -1;
            }
            if (keyCode2 == CustomKeyCode.Shift)
            {
                return 1;
            }

            if (CustomKeyCode.alphabet.Contains(keyCode1))
            {
                if (!CustomKeyCode.alphabet.Contains(keyCode2))
                {
                    return -1;
                }
                return keyCode1.ToString().CompareTo(keyCode2.ToString());
            }
            else if(CustomKeyCode.alphabet.Contains(keyCode2))
            {
                return 1;
            }

            if (CustomKeyCode.digits.Contains(keyCode1))
            {
                if (!CustomKeyCode.digits.Contains(keyCode2))
                {
                    return -1;
                }
                return keyCode1.ToString().CompareTo(keyCode2.ToString());
            }
            else if (CustomKeyCode.digits.Contains(keyCode2))
            {
                return 1;
            }

            return keyCode1.ToString().CompareTo(keyCode2.ToString());
        }

        public override string ToString()
        {
            return string.Join(" ", keyCodes);
        }

        /// <summary>
        /// Returns an array of the keycodes in this shortcut, in the order they would be read.
        /// </summary>
        public CustomKeyCode[] ToArray()
        {
            return keyCodes.ToArray();
        }

        public JSON.JSON ToJSON()
        {
            JSON.JSON json = new JSON.JSON();

            json.Add("keyCodes", from keyCode in keyCodes select keyCode.displayName, false);

            return json;
        }
    }
}
