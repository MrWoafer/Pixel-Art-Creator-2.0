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

            public T Get() => getter.Invoke();
            public void Set(T value) => setter.Invoke(value);

            public Preference(string displayName, Func<T> getter, Action<T> setter)
            {
                this.displayName = displayName;
                this.getter = getter;
                this.setter = setter;
            }
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

        public static readonly Preference<int> startupBrushSize = new Preference<int>("Startup Brush Size",
            () => PlayerPrefs.GetInt("startup brush size", 1),
            (size) =>
            {
                if (size < Config.Tools.minBrushSize)
                {
                    throw new ArgumentException("Size (" + size + ") is less than the max brush size (" + Config.Tools.minBrushSize + ")");
                }
                if (size > Config.Tools.maxBrushSize)
                {
                    throw new ArgumentException("Size (" + size + ") is larger than the max brush size (" + Config.Tools.maxBrushSize + ")");
                }
                PlayerPrefs.SetInt("startup brush size", size);
            }
            );

        /// <summary>This is the colour that will be in the bottom left / top right of the checkerboard.</summary>
        public static readonly Preference<Color32> transparentCheckerboardColour1 =
            CreatePreference("Transparent Checkerboard Colour 1", "transparent checkerboard colour 1 ", new Color32(224, 224, 224, 255));
        /// <summary>This is the colour that will be in the bottom right / top left of the checkerboard.</summary>
        public static readonly Preference<Color32> transparentCheckerboardColour2 =
            CreatePreference("Transparent Checkerboard Colour 2", "transparent checkerboard colour 2 ", new Color32(190, 190, 190, 255));
    }
}
