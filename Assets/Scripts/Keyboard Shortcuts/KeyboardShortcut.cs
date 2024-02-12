using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyboardShortcut : JSONable
{
    public List<CustomKeyCode> keyCodes = new List<CustomKeyCode>();

    public KeyboardShortcut(params CustomKeyCode[] keyCodes)
    {
        this.keyCodes = new List<CustomKeyCode>(keyCodes);
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
    /// Returns true the frame this shortcut is released.
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

    public CustomKeyCode[] ToArray()
    {
        return keyCodes.ToArray();
    }
    public List<CustomKeyCode> ToList()
    {
        return new List<CustomKeyCode>(keyCodes);
    }

    public JSON ToJSON()
    {
        JSON json = new JSON();

        json.Add("keyCodes", from keyCode in keyCodes select keyCode.displayName);

        return json;
    }

    public KeyboardShortcut FromJSON(JSON json)
    {
        throw new System.NotImplementedException();
    }
}
