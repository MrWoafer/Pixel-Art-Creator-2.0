using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIModalWindow : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject buttonPrefab;

    private Transform canvas;
    private RectTransform shadow;
    private RectTransform background;
    private RectTransform subPanel;
    private Text title;
    private Text message;

    private List<UIButton> buttons = new List<UIButton>();

    private const float SCALE_FACTOR = 100f;

    private UIManager uiManager;

    private void Awake()
    {
        canvas = transform.Find("Canvas");
        shadow = canvas.Find("Shadow").GetComponent<RectTransform>();
        background = shadow.Find("Background").GetComponent<RectTransform>();
        subPanel = background.Find("Sub-Panel").GetComponent<RectTransform>();
        title = background.Find("Title").GetComponent<Text>();
        message = subPanel.Find("Message").GetComponent<Text>();

        uiManager = Finder.uiManager;
    }

    private void UpdateLayoutGroups()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(shadow);
        LayoutRebuilder.ForceRebuildLayoutImmediate(background);
        LayoutRebuilder.ForceRebuildLayoutImmediate(subPanel);
    }

    private void UpdateDisplay()
    {
        UpdateLayoutGroups();
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            UIButton button = buttons[i];

            button.width = background.sizeDelta.x * 0.01f / buttons.Count;

            float x = background.sizeDelta.x / 2f * ((2f * i + 1) / buttons.Count - 1f);
            float y = -background.sizeDelta.y / 2f + button.height * SCALE_FACTOR / 2f;
            button.transform.localPosition = new Vector3(x, y, 0f);
        }
    }

    public UIModalWindow SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            this.title.gameObject.SetActive(false);
        }
        else
        {
            this.title.gameObject.SetActive(true);
            this.title.text = title;
        }

        UpdateDisplay();

        return this;
    }
    public UIModalWindow SetMessage(string message)
    {
        this.message.text = message;

        UpdateDisplay();

        return this;
    }

    public UIModalWindow AddButton(string text, UnityAction onClick)
    {
        UpdateLayoutGroups();

        UIButton button = Instantiate(buttonPrefab, canvas).GetComponent<UIButton>();

        button.SetText(text);
        button.height = 0.6f;
        button.transform.localScale = SCALE_FACTOR * Vector3.one;
        button.SetImages(null, null, null);

        button.SubscribeToClick(onClick);

        buttons.Add(button);

        UpdateButtons();

        return this;
    }
    public UIModalWindow AddCloseButton(string text)
    {
        return AddButton(text, () => { uiManager.CloseModalWindow(this); });
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
