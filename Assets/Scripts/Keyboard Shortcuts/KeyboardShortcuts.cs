using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class KeyboardShortcuts : MonoBehaviour
{
    /// Essentially using a singleton pattern.
    /// This whole class is essentially a static class, except I want to use the MonoBehaviour.Start() method to load saved shortcuts at startup.
    private static KeyboardShortcuts main;

    private Dictionary<string, List<KeyboardShortcut>> _shortcuts = new Dictionary<string, List<KeyboardShortcut>>();
    public static Dictionary<string, List<KeyboardShortcut>> shortcuts
    {
        get => main._shortcuts;
        private set
        {
            main._shortcuts = value;
        }
    }

    private static string shortcutsFilePath => Path.Combine(Application.persistentDataPath, "KeyboardShortcuts.json");

    private void Awake()
    {
        main = GameObject.Find("Keyboard Shortcuts").GetComponent<KeyboardShortcuts>();
    }

    private void Start()
    {
        /// Load saved keyboard shortcuts
        LoadShortcuts();
    }

    public static void LoadShortcuts()
    {
        if (!Path.IsPathFullyQualified(shortcutsFilePath))
        {
            throw new System.Exception("shortcutsFilePath not fully qualified: " + shortcutsFilePath);
        }
        if (!System.IO.File.Exists(shortcutsFilePath))
        {
            throw new System.Exception("shortcutsFilePath doesn't exist: " + shortcutsFilePath);
        }
        if (Path.GetExtension(shortcutsFilePath) != ".json")
        {
            throw new System.Exception("The file is not a JSON file. File extension: " + Path.GetExtension(shortcutsFilePath));
        }

        JSON json = JSON.Parse(System.IO.File.ReadAllText(shortcutsFilePath));
        LoadJSON(json);
    }

    public static void SaveShortcuts()
    {
        JSON json = ToJSON();
        System.IO.File.WriteAllText(shortcutsFilePath, json.ToString());

        Debug.Log("Keyboard shortcuts saved at: " + shortcutsFilePath);
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

    public static void LoadJSON(JSON json)
    {
        shortcuts = new Dictionary<string, List<KeyboardShortcut>>();

        JSON shortcutsJSON = JSON.Parse(json["shortcuts"]);

        /// Loop through each key for keyboard shortcuts - e.g. "pencil"
        foreach (string key in shortcutsJSON.Keys)
        {
            /// Loop through each keyboard shortcut
            foreach (JSON shortcutJSON in JSON.SplitArray(shortcutsJSON[key]).Select(x => JSON.Parse(x)))
            {
                /// Get each key code in the keyboard shortcut
                string[] keyCodeStrings = JSON.SplitArray(shortcutJSON["keyCodes"]).Select(x => JSON.StripQuotationMarks(x)).ToArray();

                /// Convert each key code from a string to the actual CustomKeyCode object
                CustomKeyCode[] keyCodes = keyCodeStrings.Select(x => CustomKeyCode.FromString(x)).ToArray();

                AddShortcut(key, new KeyboardShortcut(keyCodes));
            }
        }
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
        shortcuts[actionName] = new List<KeyboardShortcut>();
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
