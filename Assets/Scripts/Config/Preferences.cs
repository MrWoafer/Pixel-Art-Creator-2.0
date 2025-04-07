using System;

using UnityEngine;

namespace PAC
{
    public static class Preferences
    {
        public class Preference<T>
        {
            public string displayName { get; private set; }
            private Func<T> getter;
            private Action<T> setter;

            /// <summary>Gets the value of the preference.</summary>
            public static implicit operator T(Preference<T> pref) => pref.Get();
            /// <summary>Gets the value of the preference. Can also be done via casting.</summary>
            public T Get() => getter.Invoke();
            /// <summary>Change the value of the preference and save it.</summary>
            public void Set(T value) => setter.Invoke(value);

            public Preference(string displayName, Func<T> getter, Action<T> setter)
            {
                this.displayName = displayName;
                this.getter = getter;
                this.setter = setter;
            }
        }

        public static Preference<int> CreatePreference(string displayName, string playerPrefsKey, int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return new Preference<int>(displayName,
                () => PlayerPrefs.GetInt(playerPrefsKey, defaultValue),
                (value) =>
                {
                    if (value < minValue)
                    {
                        throw new ArgumentException("Value (" + value + ") cannot be less than " + minValue);
                    }
                    if (value > maxValue)
                    {
                        throw new ArgumentException("Value (" + value + ") cannot be more than " + maxValue);
                    }
                    PlayerPrefs.SetInt(playerPrefsKey, value);
                }
            );
        }

        public static Preference<Color> CreatePreference(string displayName, string playerPrefsKey, Color defaultValue)
        {
            return new Preference<Color>(displayName,
                () => new Color(
                PlayerPrefs.GetFloat(playerPrefsKey + " r", defaultValue.r),
                PlayerPrefs.GetFloat(playerPrefsKey + " g", defaultValue.g),
                PlayerPrefs.GetFloat(playerPrefsKey + " b", defaultValue.b),
                PlayerPrefs.GetFloat(playerPrefsKey + " a", defaultValue.a)
                ),
                (colour) =>
                {
                    PlayerPrefs.SetFloat(playerPrefsKey + " r", colour.r);
                    PlayerPrefs.SetFloat(playerPrefsKey + " g", colour.g);
                    PlayerPrefs.SetFloat(playerPrefsKey + " b", colour.b);
                    PlayerPrefs.SetFloat(playerPrefsKey + " a", colour.a);
                }
            );
        }

        public static Preference<Color32> CreatePreference(string displayName, string playerPrefsKey, Color32 defaultValue)
        {
            return new Preference<Color32>(displayName,
                () => new Color32(
                (byte)PlayerPrefs.GetInt(playerPrefsKey + " r", defaultValue.r),
                (byte)PlayerPrefs.GetInt(playerPrefsKey + " g", defaultValue.g),
                (byte)PlayerPrefs.GetInt(playerPrefsKey + " b", defaultValue.b),
                (byte)PlayerPrefs.GetInt(playerPrefsKey + " a", defaultValue.a)
                ),
                (colour) =>
                {
                    PlayerPrefs.SetInt(playerPrefsKey + " r", colour.r);
                    PlayerPrefs.SetInt(playerPrefsKey + " g", colour.g);
                    PlayerPrefs.SetInt(playerPrefsKey + " b", colour.b);
                    PlayerPrefs.SetInt(playerPrefsKey + " a", colour.a);
                }
            );
        }

        public static readonly Preference<int> startupFileWidth = CreatePreference("Startup File Width", "startup file width", 32, 1, 1024);
        public static readonly Preference<int> startupFileHeight = CreatePreference("Startup File Height", "startup file height", 32, 1, 1024);

        public static readonly Preference<int> startupBrushSize = CreatePreference("Startup Brush Size", "startup brush size", 1, Config.Tools.minBrushSize, Config.Tools.maxBrushSize);

        public static readonly Preference<int> startupAnimationFramerate = CreatePreference("Startup Animation Framerate", "startup animation framerate", 2, 1, 60);

        /// <summary>This is the colour that will be in the bottom left / top right of the checkerboard.</summary>
        public static readonly Preference<Color32> transparentCheckerboardColour1 =
            CreatePreference("Transparent Checkerboard Colour 1", "transparent checkerboard colour 1", new Color32(224, 224, 224, 255));
        /// <summary>This is the colour that will be in the bottom right / top left of the checkerboard.</summary>
        public static readonly Preference<Color32> transparentCheckerboardColour2 =
            CreatePreference("Transparent Checkerboard Colour 2", "transparent checkerboard colour 2", new Color32(190, 190, 190, 255));

        public static readonly Preference<Color> startupPrimaryColour =
            CreatePreference("Startup Primary Colour", "startup primary colour", new Color(0f, 0f, 0f, 1f));
        public static readonly Preference<Color> startupSecondaryColour =
            CreatePreference("Startup Secondary Colour", "startup secondary colour", new Color(1f, 1f, 1f, 1f));
    }
}
