using NUnit.Framework;
using PAC.Json;
using PAC.KeyboardShortcuts;
using UnityEngine;

namespace PAC.Tests
{
    public class KeyboardShortcutTests
    {
        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type CustomKeyCode.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void CustomKeyCodeToJson()
        {
            JsonConverterSet converters = new JsonConverterSet(new CustomKeyCodeJsonConverter());

            (CustomKeyCode, JsonData)[] testCases =
            {
                (CustomKeyCode.Ctrl, new JsonString("Ctrl")),
                (CustomKeyCode._2, new JsonString("2")),
                (CustomKeyCode.Plus, new JsonString("+")),
                (CustomKeyCode.Shift, new JsonString("Shift")),
                (CustomKeyCode.GreaterThan, new JsonString(">")),
            };

            foreach ((CustomKeyCode keyCode, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(keyCode, converters, false), expected));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type CustomKeyCode.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void CustomKeyCodeFromJson()
        {
            JsonConverterSet converters = new JsonConverterSet(new CustomKeyCodeJsonConverter());

            (CustomKeyCode, JsonData)[] testCases =
            {
                (CustomKeyCode.Ctrl, new JsonString("Ctrl")),
                (CustomKeyCode._2, new JsonString("2")),
                (CustomKeyCode.Plus, new JsonString("+")),
                (CustomKeyCode.Shift, new JsonString("Shift")),
                (CustomKeyCode.GreaterThan, new JsonString(">")),
            };

            foreach ((CustomKeyCode expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConverter.FromJson<CustomKeyCode>(jsonData, converters, false));
            }
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void KeyboardShortcutToJson()
        {
            JsonConverterSet converters = new JsonConverterSet(new KeyboardShortcutJsonConverter());

            (KeyboardShortcut, JsonData)[] testCases =
            {
                (new KeyboardShortcut(CustomKeyCode.Ctrl, CustomKeyCode.Plus), new JsonList(new JsonString("Ctrl"), new JsonString("+"))),
                (new KeyboardShortcut(KeyCode.G), new JsonList(new JsonString("G"))),
                (KeyboardShortcut.None, new JsonList()),
                (new KeyboardShortcut(KeyCode.Minus, CustomKeyCode._9, CustomKeyCode.Alt, CustomKeyCode.Shift),
                new JsonList(new JsonString("Alt"), new JsonString("Shift"), new JsonString("9"), new JsonString("-"))),
            };

            foreach ((KeyboardShortcut keyboardShortcut, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(keyboardShortcut, converters, false), expected));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void KeyboardShortcutFromJson()
        {
            JsonConverterSet converters = new JsonConverterSet(new KeyboardShortcutJsonConverter());

            (KeyboardShortcut, JsonData)[] testCases =
            {
                (new KeyboardShortcut(CustomKeyCode.Ctrl, CustomKeyCode.Plus), new JsonList(new JsonString("Ctrl"), new JsonString("+"))),
                (new KeyboardShortcut(KeyCode.G), new JsonList(new JsonString("G"))),
                (KeyboardShortcut.None, new JsonList()),
                (new KeyboardShortcut(KeyCode.Minus, CustomKeyCode._9, CustomKeyCode.Alt, CustomKeyCode.Shift),
                new JsonList(new JsonString("Alt"), new JsonString("Shift"), new JsonString("9"), new JsonString("-"))),
            };

            foreach ((KeyboardShortcut expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConverter.FromJson<KeyboardShortcut>(jsonData, converters, false));
            }
        }
    }
}
