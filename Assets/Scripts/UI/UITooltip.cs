using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Multiline]
    private string _text = "";
    public string text
    {
        get
        {
            return textbox.text;
        }
        set
        {
            SetText(value);
        }
    }
    public Vector2 padding = Vector2.zero;

    public float globalWidth
    {
        get
        {
            return background.sizeDelta.x * background.transform.lossyScale.x;
        }
    }
    public float globalHeight
    {
        get
        {
            return background.sizeDelta.y * background.transform.lossyScale.y;
        }
    }

    public float localWidth
    {
        get
        {
            return background.sizeDelta.x * background.transform.localScale.x;
        }
    }
    public float localHeight
    {
        get
        {
            return background.sizeDelta.y * background.transform.localScale.y;
        }
    }

    private RectTransform background;
    private Text textbox;
    private RectTransform textboxRectTransform;

    public void Awake()
    {
        GetReferences();
        SetText(_text);
    }

    // Update is called once per frame
    void Update()
    {
        if (background.sizeDelta != textboxRectTransform.sizeDelta)
        {
            background.sizeDelta = textbox.GetComponent<RectTransform>().sizeDelta + padding;
        }
    }

    private void GetReferences()
    {
        background = transform.Find("Canvas").Find("Background").GetComponent<RectTransform>();
        textbox = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        textboxRectTransform = textbox.GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        GetReferences();

        SetText(_text);
        background.sizeDelta = textbox.GetComponent<RectTransform>().sizeDelta + padding;
    }

    public void SetText(string text)
    {
        _text = text;
        textbox.text = text;
    }

    public bool GoesOffScreen()
    {
        return GoesOffLeftOfScreen() || GoesOffRightOfScreen() || GoesOffBottomOfScreen() || GoesOffTopOfScreen();
    }
    public bool GoesOffLeftOfScreen()
    {
        return background.position.x - background.lossyScale.x * textboxRectTransform.sizeDelta.x / 2f < -ScreenInfo.screenWorldWidth / 2f;
    }
    public bool GoesOffRightOfScreen()
    {
        return background.position.x + background.lossyScale.x * textboxRectTransform.sizeDelta.x / 2f > ScreenInfo.screenWorldWidth / 2f;
    }
    public bool GoesOffBottomOfScreen()
    {
        return background.position.y - background.lossyScale.y * textboxRectTransform.sizeDelta.y / 2f < -ScreenInfo.screenWorldHeight / 2f;
    }
    public bool GoesOffTopOfScreen()
    {
        return background.position.y + background.lossyScale.y * textboxRectTransform.sizeDelta.y / 2f > ScreenInfo.screenWorldHeight / 2f;
    }
}
