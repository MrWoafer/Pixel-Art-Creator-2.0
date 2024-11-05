using System.Collections.Generic;
using System.IO;
using System.Linq;
using PAC.Json;
using UnityEngine;

namespace PAC.KeyboardShortcuts
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
        /// Saves the current assignment of each shortcut to the disk.
        /// </summary>
        public static void SaveShortcuts()
        {
            JsonObj json = ToJSON();
            System.IO.File.WriteAllText(shortcutsFilePath, json.ToJsonString(true));

            Debug.Log("Keyboard shortcuts saved at: " + shortcutsFilePath);
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

            LoadJSON(JsonObj.Parse(System.IO.File.ReadAllText(shortcutsFilePath)));

            Debug.Log("Keyboard shortcuts loaded from: " + shortcutsFilePath);
        }

        /// <summary>
        /// Returns the list of keyboard shortcuts as a JSON object.
        /// </summary>
        private static JsonObj ToJSON()
        {
            JsonConverterSet converters = new JsonConverterSet(new KeyboardShortcutJsonConverter());

            JsonObj jsonObj = new JsonObj();
            foreach (string key in shortcuts.Keys)
            {
                jsonObj.Add(key, JsonConverter.ToJson(shortcuts[key], converters, false));
            }

            return jsonObj;
        }

        /// <summary>
        /// Clears all keyboard shortcuts then loads them from the JSON object.
        /// </summary>
        private static void LoadJSON(JsonObj jsonObj)
        {
            JsonConverterSet converters = new JsonConverterSet(new KeyboardShortcutJsonConverter());

            shortcuts = new Dictionary<string, List<KeyboardShortcut>>();
            foreach (string key in jsonObj.Keys)
            {
                shortcuts[key] = JsonConverter.FromJson<List<KeyboardShortcut>>(jsonObj[key], converters, false);
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
