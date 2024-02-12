using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class CustomKeyCode : IEnumerable
{
    public string displayName { get; private set; } = "";
    private List<KeyCode> _keyCodes = new List<KeyCode>();
    public List<KeyCode> keyCodes { get => _keyCodes; private set => _keyCodes = value; }

    public static readonly CustomKeyCode Shift = new CustomKeyCode("Shift", KeyCode.LeftShift, KeyCode.RightShift);
    public static readonly CustomKeyCode Alt = new CustomKeyCode("Alt", KeyCode.LeftAlt, KeyCode.RightAlt);
    public static readonly CustomKeyCode Ctrl = new CustomKeyCode("Ctrl", KeyCode.LeftControl, KeyCode.RightControl);
    public static readonly CustomKeyCode[] allKeyCodes = Functions.ConcatArrays((from x in (KeyCode[])System.Enum.GetValues(typeof(KeyCode)) select (CustomKeyCode)x).ToArray(),
        new CustomKeyCode[] { Shift, Alt, Ctrl });

    private CustomKeyCode(string displayName, params KeyCode[] keyCodes)
    {
        this.displayName = displayName;
        this.keyCodes = new List<KeyCode>(keyCodes);
    }

    public static implicit operator CustomKeyCode(KeyCode keyCode)
    {
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
        else if (keyCodes.Count == 1)
        {
            return keyCodes[0].GetHashCode();
        }
        else if (keyCodes.Count == 2)
        {
            return HashCode.Combine(keyCodes[0], keyCodes[1]);
        }
        else
        {
            throw new System.Exception("CustomKeyCodes should not have more than 2 KeyCodes. KeyCode count: " + keyCodes.Count);
        }
    }

    public override string ToString()
    {
        return displayName;
    }

    public CustomKeyCode FromString(string displayName)
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

    public bool Contains(KeyCode keyCode)
    {
        return keyCodes.Contains(keyCode);
    }

    public IEnumerator GetEnumerator()
    {
        return keyCodes.GetEnumerator();
    }
}
