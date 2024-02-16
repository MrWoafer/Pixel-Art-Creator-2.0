using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardShortutOptionsManager : MonoBehaviour
{
    private class KeyboardShortcutCategory
    {
        public string categoryName;
        public List<KeyboardShortcutOption> shortcutOptions;

        public KeyboardShortcutCategory(string categoryName, params KeyboardShortcutOption[] shortcutOptions)
        {
            this.categoryName = categoryName;
            this.shortcutOptions = new List<KeyboardShortcutOption>(shortcutOptions);
        }
    }

    private struct KeyboardShortcutOption
    {
        public string referenceName;
        public string displayName;

        public KeyboardShortcutOption(string referenceName, string displayName)
        {
            this.referenceName = referenceName;
            this.displayName = displayName;
        }
    }

    [Header("Settings")]
    [Min(0f)]
    private float optionSpacing = 1f;
    [Min(0f)]
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

    private KeyboardShortcutCategory[] editableShortcuts = new KeyboardShortcutCategory[]
    {
        new KeyboardShortcutCategory("Tools", new KeyboardShortcutOption("brush", "Brush"), new KeyboardShortcutOption("pencil", "Pencil"), new KeyboardShortcutOption("rubber", "Rubber"),
            new KeyboardShortcutOption("eye dropper", "Eye Dropper"), new KeyboardShortcutOption("fill", "Fill"), new KeyboardShortcutOption("shape", "Shape"),
            new KeyboardShortcutOption("line", "Line"), new KeyboardShortcutOption("gradient", "Gradient"), new KeyboardShortcutOption("move", "Move"),
            new KeyboardShortcutOption("selection", "Selection"), new KeyboardShortcutOption("cancel tool", "Cancel Tool")),
        new KeyboardShortcutCategory("Navigation", new KeyboardShortcutOption("zoom in", "Zoom In"), new KeyboardShortcutOption("zoom out", "Zoom Out"))
    };

    private void Start()
    {
        for (int i = 0; i < editableShortcuts.Length; i++)
        {
            KeyboardShortcutCategory category = editableShortcuts[i];

            UIToggleButton button = Instantiate(categoryButtonPrefab, categoriesViewport.scrollingArea).GetComponent<UIToggleButton>();
            button.transform.localPosition = new Vector3(0f, -i * button.height, 0f);
            
            button.SetText(category.categoryName);

            categoriesToggleGroup.Add(button);

            int x = i;
            button.SubscribeToLeftClick(() => tabManager.SelectTab(x));
            button.SubscribeToLeftClick(() => optionsScrollbar.SetScrollAmount(1f));

            GameObject tab = Instantiate(new GameObject("Tab - " + category.categoryName), optionsViewport.scrollingArea);
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

            float offsetY = category.shortcutOptions.Count % 2 == 0 ? category.shortcutOptions.Count / 2 * optionSpacing - optionSpacing / 2f : category.shortcutOptions.Count / 2 * optionSpacing;
            for (int j = 0; j < category.shortcutOptions.Count; j++)
            {
                KeyboardShortcutOption option = category.shortcutOptions[j];
                GameObject optionObj = Instantiate(optionPrefab, tab.transform);
                optionObj.transform.localPosition = new Vector3(0f, -j * optionSpacing + offsetY, 0f);
                optionObj.transform.Find("Text").GetComponent<Text>().text = option.displayName;

                UIKeyboardShortcut uiKeyboardShortcut = optionObj.transform.Find("Keyboard Shortcut").GetComponent<UIKeyboardShortcut>();
                uiKeyboardShortcut.shortcut = KeyboardShortcuts.GetShortcutsFor(option.referenceName)[0];
                uiKeyboardShortcut.SubscribeToOnShortcutSet((shortcut) => KeyboardShortcuts.SetShortcutFor(option.referenceName, shortcut));
                uiKeyboardShortcut.SubscribeToOnShortcutSet((shortcut) => KeyboardShortcuts.SaveShortcuts());
            }
        }

        categoriesToggleGroup.Press(0);
    }
}
