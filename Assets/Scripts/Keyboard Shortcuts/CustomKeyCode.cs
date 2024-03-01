using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// A class to supersede Unity's KeyCode enum to allow keycodes to refer to multiple keys - e.g. Shift instead of separated into LeftShift and RightShift.
/// </summary>
public class CustomKeyCode : IEnumerable
{
    /// <summary>What will be returned from ToString().</summary>
    public string displayName { get; private set; } = "";
    private List<KeyCode> _keyCodes = new List<KeyCode>();
    /// <summary>The Unity KeyCodes comprising this keycode.</summary>
    public List<KeyCode> keyCodes { get => _keyCodes; private set => _keyCodes = value; }

    /// Define combined keycodes
    public static readonly CustomKeyCode Shift = new CustomKeyCode("Shift", KeyCode.LeftShift, KeyCode.RightShift);
    public static readonly CustomKeyCode Alt = new CustomKeyCode("Alt", KeyCode.LeftAlt, KeyCode.RightAlt);
    public static readonly CustomKeyCode Ctrl = new CustomKeyCode("Ctrl", KeyCode.LeftControl, KeyCode.RightControl);

    public static readonly CustomKeyCode Plus = new CustomKeyCode("+", KeyCode.Plus, KeyCode.Equals, KeyCode.KeypadPlus);
    public static readonly CustomKeyCode Minus = new CustomKeyCode("-", KeyCode.Minus, KeyCode.KeypadMinus);

    /// <summary>The keycode for 0.</summary>
    public static readonly CustomKeyCode _0 = new CustomKeyCode("0", KeyCode.Alpha0, KeyCode.Keypad0);
    /// <summary>The keycode for 1.</summary>
    public static readonly CustomKeyCode _1 = new CustomKeyCode("1", KeyCode.Alpha1, KeyCode.Keypad1);
    /// <summary>The keycode for 2.</summary>
    public static readonly CustomKeyCode _2 = new CustomKeyCode("2", KeyCode.Alpha2, KeyCode.Keypad2);
    /// <summary>The keycode for 3.</summary>
    public static readonly CustomKeyCode _3 = new CustomKeyCode("3", KeyCode.Alpha3, KeyCode.Keypad3);
    /// <summary>The keycode for 4.</summary>
    public static readonly CustomKeyCode _4 = new CustomKeyCode("4", KeyCode.Alpha4, KeyCode.Keypad4);
    /// <summary>The keycode for 5.</summary>
    public static readonly CustomKeyCode _5 = new CustomKeyCode("5", KeyCode.Alpha5, KeyCode.Keypad5);
    /// <summary>The keycode for 6.</summary>
    public static readonly CustomKeyCode _6 = new CustomKeyCode("6", KeyCode.Alpha6, KeyCode.Keypad6);
    /// <summary>The keycode for 7.</summary>
    public static readonly CustomKeyCode _7 = new CustomKeyCode("7", KeyCode.Alpha7, KeyCode.Keypad7);
    /// <summary>The keycode for 8.</summary>
    public static readonly CustomKeyCode _8 = new CustomKeyCode("8", KeyCode.Alpha8, KeyCode.Keypad8);
    /// <summary>The keycode for 9.</summary>
    public static readonly CustomKeyCode _9 = new CustomKeyCode("9", KeyCode.Alpha9, KeyCode.Keypad9);

    public static readonly CustomKeyCode GreaterThan = new CustomKeyCode(">", KeyCode.Period, KeyCode.Greater);
    public static readonly CustomKeyCode LessThan = new CustomKeyCode("<", KeyCode.Comma, KeyCode.Less);

    /// <summary>All the defined keycodes that combine multiple Unity KeyCodes.</summary>
    public static readonly CustomKeyCode[] combinedKeyCodes = new CustomKeyCode[] { Shift, Alt, Ctrl, Plus, Minus, _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, GreaterThan, LessThan };
    /// <summary>All Unity KeyCodes and all combined keycodes.</summary>
    public static readonly CustomKeyCode[] allKeyCodes = Functions.ConcatArrays((from x in (KeyCode[])System.Enum.GetValues(typeof(KeyCode)) select (CustomKeyCode)x).ToArray(), combinedKeyCodes);

    /// <summary>The keycodes A-Z.</summary>
    public static readonly CustomKeyCode[] alphabet = new CustomKeyCode[] { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
    KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z };
    /// <summary>The keycodes 0-9.</summary>
    public static readonly CustomKeyCode[] digits = new CustomKeyCode[] { _0, _1, _2, _3, _4, _5, _6, _7, _8, _9 };

    // private so that no new keycodes can be created - only the pre-defined ones. So it's sort of like an enum but with methods.
    private CustomKeyCode(string displayName, params KeyCode[] keyCodes)
    {
        this.displayName = displayName;

        if (keyCodes.Length == 0)
        {
            throw new System.Exception("CustomKeyCodes should not have 0 KeyCodes.");
        }
        if (keyCodes.Length > 5)
        {
            throw new System.Exception("CustomKeyCodes should not have more than 5 KeyCodes. KeyCode count: " + keyCodes.Length);
        }
        this.keyCodes = new List<KeyCode>(keyCodes);
    }

    // Implicitly cast Unity KeyCode to CustomKeyCode to allow ease of working with the two forms and so I don't have to redefine all ~400 Unity KeyCodes as CustomKeyCodes.
    public static implicit operator CustomKeyCode(KeyCode keyCode)
    {
        foreach (CustomKeyCode combinedKeyCode in combinedKeyCodes)
        {
            if (combinedKeyCode.Contains(keyCode))
            {
                return combinedKeyCode;
            }
        }
        return new CustomKeyCode(keyCode.ToString(), keyCode);
    }

    public static bool operator ==(CustomKeyCode keyCode1, CustomKeyCode keyCode2)
    {
        return keyCode1.keyCodes.SequenceEqual(keyCode2.keyCodes);
    }
    public static bool operator !=(CustomKeyCode keyCode1, CustomKeyCode keyCode2)
    {
        return !(keyCode1 == keyCode2);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        else if (obj.GetType() == typeof(CustomKeyCode))
        {
            return this == (CustomKeyCode)obj;
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
        throw new System.Exception("CustomKeyCodes should not have more than 5 KeyCodes. KeyCode count: " + keyCodes.Count);
    }

    public override string ToString()
    {
        return displayName;
    }

    /// <summary>
    /// Converts a string into the keycode with that display name. Returns null if there isn't one.
    /// </summary>
    public static CustomKeyCode FromString(string displayName)
    {
        foreach (CustomKeyCode keyCode in allKeyCodes)
        {
            if (keyCode.displayName == displayName)
            {
                return keyCode;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns true while this key is being held down.
    /// </summary>
    public bool GetKey()
    {
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKey(keyCode))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true the frame this key is pressed.
    /// </summary>
    public bool GetKeyDown()
    {
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true the frame this key is released.
    /// </summary>
    public bool GetKeyUp()
    {
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyUp(keyCode))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if the Unity KeyCode forms part of this CustomKeyCode.
    /// </summary>
    public bool Contains(KeyCode keyCode)
    {
        return keyCodes.Contains(keyCode);
    }

    public IEnumerator GetEnumerator()
    {
        return keyCodes.GetEnumerator();
    }
}
