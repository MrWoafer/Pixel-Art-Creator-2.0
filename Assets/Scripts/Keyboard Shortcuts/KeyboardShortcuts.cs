using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardShortcuts : MonoBehaviour
{
    /// Essentially using a singleton pattern.
    /// This whole class is essentially a static class, except I want to use the MonoBehaviour.Start() method to load saved shortcuts at startup.
    private static KeyboardShortcuts main;

    private Dictionary<string, List<KeyboardShortcut>> _shortcuts = new Dictionary<string, List<KeyboardShortcut>>();
    private static Dictionary<string, List<KeyboardShortcut>> shortcuts => main._shortcuts;

    private void Awake()
    {
        main = GameObject.Find("Keyboard Shortcuts").GetComponent<KeyboardShortcuts>();
    }

    private void Start()
    {
        /// Load saved keyboard shortcuts
        //LoadShortcuts();
        AddShortcut("pencil", KeyCode.W);
        AddShortcut("pencil", KeyCode.P);
        AddShortcut("rubber", KeyCode.R);
        AddShortcut("eye dropper", CustomKeyCode.Shift, KeyCode.E);

        Debug.Log(ToJSON().ToString());
    }

    public static void LoadShortcuts()
    {
        throw new System.NotImplementedException();
    }

    public static void SaveShortcuts()
    {
        throw new System.NotImplementedException();
    }

    public static JSON ToJSON()
    {
        JSON json = new JSON();

        JSON shortcutsJSON = new JSON();
        foreach (string key in shortcuts.Keys)
        {
            shortcutsJSON.Add(key, shortcuts[key]);
        }

        json.Add("shortcuts", shortcutsJSON);

        return json;
    }

    public static void LoadJSON()
    {
        throw new System.NotImplementedException();
    }

    public static void AddShortcut(string actionName, params CustomKeyCode[] keyCodes) => AddShortcut(actionName, new KeyboardShortcut(keyCodes));
    public static void AddShortcut(string actionName, KeyboardShortcut shortcut)
    {
        if (!shortcuts.ContainsKey(actionName))
        {
            shortcuts[actionName] = new List<KeyboardShortcut>();
        }
        shortcuts[actionName].Add(shortcut);
    }

    public static void ClearShortcutsFor(string actionName, KeyboardShortcut shortcut)
    {
        if (!shortcuts.ContainsKey(actionName))
        {
            throw new System.Exception("There are no shortcuts for: " + actionName);
        }
        shortcuts.Remove(actionName);
    }

    public static List<KeyboardShortcut> GetShortcutsFor(string actionName)
    {
        if (!shortcuts.ContainsKey(actionName))
        {
            throw new System.Exception("There are no shortcuts for: " + actionName);
        }
        return shortcuts[actionName];
    }

    public static bool ContainsShortcutFor(string actionName)
    {
        return shortcuts.ContainsKey(actionName);
    }
}
