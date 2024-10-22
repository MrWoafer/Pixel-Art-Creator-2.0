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

            public Preference(string displayName, Func<T> getter, Action<T> setter)
            {
                this.displayName = displayName;
                this.getter = getter;
                this.setter = setter;
            }

            public T Get() => getter.Invoke();
            public void Set(T value) => setter.Invoke(value);
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
    }
}
