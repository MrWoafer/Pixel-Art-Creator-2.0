using System.Collections.Generic;
using PAC.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PAC.Keyboard_Shortcuts
{
    /// <summary>
    /// Handles the window where you can view / change keyboard shortcuts.
    /// </summary>
    public class KeyboardShortcutOptionsManager : MonoBehaviour
    {
        /// <summary>
        /// A category for keyboard shortcuts, including the actions included in this category.
        /// </summary>
        private class KeyboardShortcutCategory
        {
            /// <summary>The name of the category.</summary>
            public string categoryName;
            /// <summary>The actions whose shortcuts will be displayed in this category.</summary>
            public List<KeyboardShortcutOption> shortcutOptions;

            /// <param name="categoryName">The name of the category.</param>
            /// <param name="shortcutOptions">The actions whose shortcuts will be displayed in this category.</param>
            public KeyboardShortcutCategory(string categoryName, params KeyboardShortcutOption[] shortcutOptions)
            {
                this.categoryName = categoryName;
                this.shortcutOptions = new List<KeyboardShortcutOption>(shortcutOptions);
            }
        }

        /// <summary>
        /// A single action that will have its shortcut displayed.
        /// </summary>
        private struct KeyboardShortcutOption
        {
            /// <summary>The name of the action's key in the KeyboardShortcuts object.</summary>
            public string referenceName;
            /// <summary>The name of the action to display in the window.</summary>
            public string displayName;

            /// <param name="referenceName">The name of the action's key in the KeyboardShortcuts object.</param>
            /// <param name="displayName">The name of the action to display in the window.</param>
            public KeyboardShortcutOption(string referenceName, string displayName)
            {
                this.referenceName = referenceName;
                this.displayName = displayName;
            }
        }

        [Header("Settings")]
        [Min(0f)]
        [Tooltip("The vertical gap between each shortcut.")]
        private float optionSpacing = 1f;
        [Min(0f)]
        [Tooltip("The vertical gap between the top/bottom shortcut and the top/bottom of the window.")]
        private float optionTopBottomPadding = 0.5f;

        [Header("References")]
        [SerializeField]
        private UITabManager tabManager;
        [SerializeField]
        private GameObject categoryButtonPrefab;
        [SerializeField]
        private UIViewport categoriesViewport;
        [SerializeField]
        private UIToggleGroup categoriesToggleGroup;
        [SerializeField]
        private GameObject optionPrefab;
        [SerializeField]
        private UIViewport optionsViewport;
        [SerializeField]
        private UIScrollbar optionsScrollbar;

        /// <summary>The categories, and their shortcuts, to displayed in the window.</summary>
        private KeyboardShortcutCategory[] editableShortcuts = new KeyboardShortcutCategory[]
        {
            new KeyboardShortcutCategory("Animation", new KeyboardShortcutOption("play / pause", "Play / Pause"), new KeyboardShortcutOption("previous frame", "Previous Frame"),
                new KeyboardShortcutOption("next frame", "Next Frame"), new KeyboardShortcutOption("first frame", "First Frame"), new KeyboardShortcutOption("last frame", "Last Frame"),
                new KeyboardShortcutOption("scroll in/out timeline", "Scroll In/Out")),

            new KeyboardShortcutCategory("Colour", new KeyboardShortcutOption("selected colour left", "Selected Colour Left"), new KeyboardShortcutOption("selected colour right", "Selected Colour Right")),

            new KeyboardShortcutCategory("File", new KeyboardShortcutOption("save", "Save"), new KeyboardShortcutOption("save as", "Save As"), new KeyboardShortcutOption("export", "Export"),
                new KeyboardShortcutOption("open", "Open")),

            new KeyboardShortcutCategory("Navigation", new KeyboardShortcutOption("zoom in", "Zoom In"), new KeyboardShortcutOption("zoom out", "Zoom Out"),
                new KeyboardShortcutOption("reset view", "Reset View"), new KeyboardShortcutOption("scroll x", "Scroll X"), new KeyboardShortcutOption("scroll y", "Scroll Y")),

            new KeyboardShortcutCategory("Tools", new KeyboardShortcutOption("brush", "Brush"), new KeyboardShortcutOption("pencil", "Pencil"), new KeyboardShortcutOption("rubber", "Rubber"),
                new KeyboardShortcutOption("eye dropper", "Eye Dropper"), new KeyboardShortcutOption("fill", "Fill"), new KeyboardShortcutOption("shape", "Shape"),
                new KeyboardShortcutOption("line", "Line"), new KeyboardShortcutOption("iso box", "Isometric Box"), new KeyboardShortcutOption("gradient", "Gradient"),
                new KeyboardShortcutOption("move", "Move"), new KeyboardShortcutOption("selection", "Selection"), new KeyboardShortcutOption("cancel tool", "Cancel Tool"),
                new KeyboardShortcutOption("clear selection", "Clear Selection"), new KeyboardShortcutOption("scroll brush size", "Scroll Brush Size"))
        };

        private void Start()
        {
            // Instantiate all the UI elements in the keyboard shortcuts window

            for (int i = 0; i < editableShortcuts.Length; i++)
            {
                // Make the category button

                KeyboardShortcutCategory category = editableShortcuts[i];

                UIToggleButton button = Instantiate(categoryButtonPrefab, categoriesViewport.scrollingArea).GetComponent<UIToggleButton>();
                button.transform.localPosition = new Vector3(0f, -i * button.height, 0f);
            
                button.SetText(category.categoryName);

                categoriesToggleGroup.Add(button);

                int x = i;
                button.SubscribeToLeftClick(() => tabManager.SelectTab(x));
                button.SubscribeToLeftClick(() => optionsScrollbar.SetScrollAmount(1f));

                // Create the tab object for this category to use in the tab system
            
                GameObject tab = new GameObject("Tab - " + category.categoryName);
                tab.transform.parent = optionsViewport.scrollingArea;
                tab.transform.localPosition = Vector3.zero;
                tabManager.AddTab(tab);

                tab.AddComponent<RectTransform>();
                float baseRadius = category.shortcutOptions.Count / 2 * optionSpacing + optionTopBottomPadding;
                if (category.shortcutOptions.Count % 2 == 0)
                {
                    tab.GetComponent<RectTransform>().sizeDelta = new Vector2(1f, 2f * (baseRadius - optionSpacing / 2f));
                }
                else
                {
                    tab.GetComponent<RectTransform>().sizeDelta = new Vector2(1f, 2f * baseRadius);
                }

                // Make the ui elements for each shortcut in this category

                float offsetY = category.shortcutOptions.Count % 2 == 0 ? category.shortcutOptions.Count / 2 * optionSpacing - optionSpacing / 2f : category.shortcutOptions.Count / 2 * optionSpacing;
                for (int j = 0; j < category.shortcutOptions.Count; j++)
                {
                    KeyboardShortcutOption option = category.shortcutOptions[j];
                    GameObject optionObj = Instantiate(optionPrefab, tab.transform);
                    optionObj.transform.localPosition = new Vector3(0f, -j * optionSpacing + offsetY, 0f);
                    optionObj.transform.Find("Text").GetComponent<Text>().text = option.displayName;

                    UIKeyboardShortcut uiKeyboardShortcut = optionObj.transform.Find("Keyboard Shortcut").GetComponent<UIKeyboardShortcut>();
                    uiKeyboardShortcut.shortcut = KeyboardShortcuts.GetShortcutsFor(option.referenceName)[0];
                    uiKeyboardShortcut.SubscribeToOnShortcutSet((shortcut) => KeyboardShortcuts.SetShortcut(option.referenceName, shortcut));
                    uiKeyboardShortcut.SubscribeToOnShortcutSet((shortcut) => KeyboardShortcuts.SaveShortcuts());
                }
            }

            categoriesToggleGroup.Press(0);
        }
    }
}
