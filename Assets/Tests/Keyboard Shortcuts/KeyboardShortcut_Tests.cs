using NUnit.Framework;
using PAC.Json;
using PAC.KeyboardShortcuts;
using UnityEngine;

namespace PAC.Tests
{
    /// <summary>
    /// Tests the KeyboardShortcut and CustomKeyCode classes.
    /// </summary>
    public class KeyboardShortcut_Tests
    {
        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type CustomKeyCode.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void CustomKeyCodeToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new CustomKeyCode.JsonConverter());

            (CustomKeyCode, JsonData)[] testCases =
            {
                (CustomKeyCode.Ctrl, new JsonData.String("Ctrl")),
                (CustomKeyCode._2, new JsonData.String("2")),
                (CustomKeyCode.Plus, new JsonData.String("+")),
                (CustomKeyCode.Shift, new JsonData.String("Shift")),
                (CustomKeyCode.GreaterThan, new JsonData.String(">")),
            };

            foreach ((CustomKeyCode keyCode, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(keyCode, converters, false), expected));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type CustomKeyCode.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void CustomKeyCodeFromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new CustomKeyCode.JsonConverter());

            (CustomKeyCode, JsonData)[] testCases =
            {
                (CustomKeyCode.Ctrl, new JsonData.String("Ctrl")),
                (CustomKeyCode._2, new JsonData.String("2")),
                (CustomKeyCode.Plus, new JsonData.String("+")),
                (CustomKeyCode.Shift, new JsonData.String("Shift")),
                (CustomKeyCode.GreaterThan, new JsonData.String(">")),
            };

            foreach ((CustomKeyCode expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConversion.FromJson<CustomKeyCode>(jsonData, converters, false));
            }
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void KeyboardShortcutToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new KeyboardShortcut.JsonConverter());

            (KeyboardShortcut, JsonData)[] testCases =
            {
                (new KeyboardShortcut(CustomKeyCode.Ctrl, CustomKeyCode.Plus), new JsonData.List(new JsonData.String("Ctrl"), new JsonData.String("+"))),
                (new KeyboardShortcut(KeyCode.G), new JsonData.List(new JsonData.String("G"))),
                (KeyboardShortcut.None, new JsonData.List()),
                (new KeyboardShortcut(KeyCode.Minus, CustomKeyCode._9, CustomKeyCode.Alt, CustomKeyCode.Shift),
                new JsonData.List(new JsonData.String("Alt"), new JsonData.String("Shift"), new JsonData.String("9"), new JsonData.String("-"))),
            };

            foreach ((KeyboardShortcut keyboardShortcut, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(keyboardShortcut, converters, false), expected));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Keyboard Shortcuts")]
        public void KeyboardShortcutFromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new KeyboardShortcut.JsonConverter());

            (KeyboardShortcut, JsonData)[] testCases =
            {
                (new KeyboardShortcut(CustomKeyCode.Ctrl, CustomKeyCode.Plus), new JsonData.List(new JsonData.String("Ctrl"), new JsonData.String("+"))),
                (new KeyboardShortcut(KeyCode.G), new JsonData.List(new JsonData.String("G"))),
                (KeyboardShortcut.None, new JsonData.List()),
                (new KeyboardShortcut(KeyCode.Minus, CustomKeyCode._9, CustomKeyCode.Alt, CustomKeyCode.Shift),
                new JsonData.List(new JsonData.String("Alt"), new JsonData.String("Shift"), new JsonData.String("9"), new JsonData.String("-"))),
            };

            foreach ((KeyboardShortcut expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConversion.FromJson<KeyboardShortcut>(jsonData, converters, false));
            }
        }
    }
}
