using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PAC.Keyboard_Shortcuts
{
    /// <summary>
    /// A class for storing, loading and accessing keyboard shortcuts for defined actions.
    /// </summary>
    public class KeyboardShortcuts : MonoBehaviour
    {
        /// Essentially using a singleton pattern.
        /// This whole class is essentially a static class, except I want to use the MonoBehaviour.Start() method to load saved shortcuts at startup.
        private static KeyboardShortcuts main;

        private Dictionary<string, List<KeyboardShortcut>> _shortcuts = new Dictionary<string, List<KeyboardShortcut>>();
        /// <summary>A key is an action name. A value is a list of the keyboard shortcuts for that action.</summary>
        public static Dictionary<string, List<KeyboardShortcut>> shortcuts
        {
            get => main._shortcuts;
            private set
            {
                main._shortcuts = value;
            }
        }

        /// <summary>The file path where shortcuts are saved to / loaded from.</summary>
        private static string shortcutsFilePath => Path.Combine(Application.persistentDataPath, "KeyboardShortcuts.json");

        private void Awake()
        {
            main = GameObject.Find("Keyboard Shortcuts").GetComponent<KeyboardShortcuts>();

            LoadShortcuts();
        }

        /// <summary>
        /// Loads saved shortcuts from the disk.
        /// </summary>
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

            JSON.JSON json = JSON.JSON.Parse(System.IO.File.ReadAllText(shortcutsFilePath));
            LoadJSON(json);

            Debug.Log("Keyboard shortcuts loaded from: " + shortcutsFilePath);
        }

        /// <summary>
        /// Saves the current assignment of each shortcut to the disk.
        /// </summary>
        public static void SaveShortcuts()
        {
            JSON.JSON json = ToJSON();
            System.IO.File.WriteAllText(shortcutsFilePath, json.ToString());

            Debug.Log("Keyboard shortcuts saved at: " + shortcutsFilePath);
        }

        /// <summary>
        /// Returns the list of keyboard shortcuts as a JSON object.
        /// </summary>
        public static JSON.JSON ToJSON()
        {
            JSON.JSON json = new JSON.JSON();

            JSON.JSON shortcutsJSON = new JSON.JSON();
            foreach (string key in shortcuts.Keys)
            {
                shortcutsJSON.Add(key, shortcuts[key]);
            }

            json.Add("shortcuts", shortcutsJSON);

            return json;
        }

        /// <summary>
        /// Clears all keyboard shortcuts then loads them from the JSON object.
        /// </summary>
        private static void LoadJSON(JSON.JSON json)
        {
            shortcuts = new Dictionary<string, List<KeyboardShortcut>>();

            JSON.JSON shortcutsJSON = JSON.JSON.Parse(json["shortcuts"]);

            /// Loop through each key for keyboard shortcuts - e.g. "pencil"
            foreach (string key in shortcutsJSON.Keys)
            {
                /// Loop through each keyboard shortcut
                foreach (JSON.JSON shortcutJSON in JSON.JSON.SplitArray(shortcutsJSON[key]).Select(x => JSON.JSON.Parse(x)))
                {
                    /// Get each key code in the keyboard shortcut
                    string[] keyCodeStrings = JSON.JSON.SplitArray(shortcutJSON["keyCodes"]).Select(x => JSON.JSON.StripQuotationMarks(x)).ToArray();

                    /// Convert each key code from a string to the actual CustomKeyCode object
                    CustomKeyCode[] keyCodes = keyCodeStrings.Select(x => CustomKeyCode.FromString(x)).ToArray();

                    AddShortcut(key, new KeyboardShortcut(keyCodes));
                }
            }
        }

        /// <summary>
        /// Combines the given keycodes into a single shortcut and adds it for the given action.
        /// </summary>
        public static void AddShortcut(string actionName, params CustomKeyCode[] keyCodes) => AddShortcut(actionName, new KeyboardShortcut(keyCodes));
        /// <summary>
        /// Adds the given shortcut for the given action.
        /// </summary>
        public static void AddShortcut(string actionName, KeyboardShortcut shortcut)
        {
            if (!shortcuts.ContainsKey(actionName))
            {
                shortcuts[actionName] = new List<KeyboardShortcut>();
            }
            shortcuts[actionName].Add(shortcut);
        }

        /// <summary>
        /// Removes any existing shortcuts for the action then adds the given shortcut.
        /// </summary>
        public static void SetShortcut(string actionName, KeyboardShortcut shortcut)
        {
            shortcuts[actionName] = new List<KeyboardShortcut>();
            AddShortcut(actionName, shortcut);
        }

        /// <summary>
        /// Removes all shortcuts for the action.
        /// </summary>
        public static void ClearShortcutsFor(string actionName)
        {
            if (!shortcuts.ContainsKey(actionName))
            {
                throw new System.Exception("There are no shortcuts for: " + actionName);
            }
            shortcuts[actionName] = new List<KeyboardShortcut>();
        }

        /// <summary>
        /// Returns all shortcuts for the action.
        /// </summary>
        public static List<KeyboardShortcut> GetShortcutsFor(string actionName)
        {
            if (!shortcuts.ContainsKey(actionName))
            {
                throw new System.Exception("There are no shortcuts for: " + actionName);
            }
            return shortcuts[actionName];
        }

        /// <summary>
        /// Returns true if the action has a shortcut.
        /// </summary>
        public static bool ContainsShortcutFor(string actionName)
        {
            return shortcuts.ContainsKey(actionName) & shortcuts[actionName].Count != 0;
        }

        /// <summary>
        /// Returns true if the action is defined for having keyboard shortcuts.
        /// </summary>
        public static bool ContainsKey(string actionName)
        {
            return shortcuts.ContainsKey(actionName);
        }
    }
}
