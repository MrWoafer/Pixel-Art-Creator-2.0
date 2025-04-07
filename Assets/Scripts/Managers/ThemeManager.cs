using System.Collections.Generic;

using PAC.UI.Themes;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Themes
{
    public class ThemeManager : MonoBehaviour
    {
        [Header("Themes")]
        [SerializeField]
        private Theme _currentTheme;
        public Theme currentTheme
        {
            get => _currentTheme;
            private set => _currentTheme = value;
        }
        [SerializeField]
        private List<Theme> _themes;
        public List<Theme> themes
        {
            get => _themes;
            private set => _themes = value;
        }

        private UnityEvent onThemeChanged = new UnityEvent();

        private bool beenRunningForAFrame = false;

        private void Start()
        {
            SetTheme(currentTheme.themeName);
        }

        private void Update()
        {
            if (!beenRunningForAFrame)
            {
                beenRunningForAFrame = true;
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && beenRunningForAFrame)
            {
                SetTheme(currentTheme);
            }
        }

        public void SetTheme(Theme theme)
        {
            currentTheme = theme;
            currentTheme?.SubscribeToOnChanged(() => onThemeChanged.Invoke());
            onThemeChanged.Invoke();
        }
        public void SetTheme(string themeName)
        {
            themeName = themeName.ToLower();
            foreach (Theme theme in themes)
            {
                if (theme.themeName.ToLower() == themeName)
                {
                    SetTheme(theme);
                    return;
                }
            }

            throw new System.Exception("Couldn't find theme: " + themeName);
        }

        public void SubscribeToThemeChanged(UnityAction call)
        {
            onThemeChanged.AddListener(call);
        }
    }
}
