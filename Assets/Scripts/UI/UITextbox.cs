using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum UITextboxAnchorPoint
{
    Left = 3,
    Centre = 4,
    Right = 5
}

[RequireComponent(typeof(InputTarget), typeof(Collider2D))]
[AddComponentMenu("Custom UI/UI Textbox")]
public class UITextbox : MonoBehaviour
{
    [Header("Size")]
    [Min(0f)]
    public float width = 1f;
    [Min(0f)]
    public float height = 1f;
    public UITextboxAnchorPoint anchorPoint = UITextboxAnchorPoint.Centre;

    [Header("Text")]
    [SerializeField]
    private string _text = "";
    public string text
    {
        get
        {
            return _text;
        }
    }
    [SerializeField]
    private string _prefix = "";
    public string prefix
    {
        get
        {
            return _prefix;
        }
    }
    [SerializeField]
    private string _suffix = "";
    public string suffix
    {
        get
        {
            return _suffix;
        }
    }
    public bool allowLetters = true;
    public bool allowNumbers = true;
    public bool allowSpaces = true;
    public bool allowPunctuation = true;
    [SerializeField]
    private Color textColour = Color.black;
    [SerializeField]
    private Color textHoverColour = Color.black;
    [SerializeField]
    private Color textPressedColour = Color.black;
    [SerializeField]
    private Color textSelectedColour = Color.black;

    [Header("Background")]
    public Color backgroundColour = Color.white;
    public Color backgroundHoverColour = Color.white;
    public Color backgroundPressedColour = Color.white;
    public Color backgroundSelectedColour = Color.white;

    [Header("Behaviour")]
    [SerializeField]
    private UnityEvent onInput;
    [SerializeField]
    private UnityEvent onFinished;

    private string allowedCharacters
    {
        get
        {
            string str = "";
            if (allowLetters)
            {
                str += "abcdefghijklmnopqrstuvwxyz";
            }
            if (allowNumbers)
            {
                str += "0123456789";
            }
            if (allowSpaces)
            {
                str += " ";
            }
            if (allowPunctuation)
            {
                str += ",.;:<>-_/\\?!*+=";
            }
            return str;
        }
    }

    private Image background;
    private BoxCollider2D collider;
    private Text textBox;
    private Canvas canvas;
    private Image cursor;

    private InputTarget inputTarget;

    private void Awake()
    {
        inputTarget = GetComponent<InputTarget>();

        GetReferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        inputTarget.mouseTarget.SubscribeToStateChange(MouseTargetInput);
        inputTarget.keyboardTarget.SubscribeToOnInput(KeyboardTargetInput);
        inputTarget.SubscribeToUntarget(() => onFinished.Invoke());

        UpdateDisplay();
    }

    private void GetReferences()
    {
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
        background = transform.Find("Canvas").Find("Background").GetComponent<Image>();
        collider = GetComponent<BoxCollider2D>();
        textBox = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        cursor = transform.Find("Canvas").Find("Cursor").GetComponent<Image>();
    }

    private void OnValidate()
    {
        GetReferences();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        UpdateSize();
        UpdateAnchor();
        UpdateText();
        Idle();
        textBox.color = textColour;
    }

    private void UpdateText()
    {
        textBox.text = prefix + text + suffix;
    }

    private void UpdateSize()
    {
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector3(width, height, 1f);
        background.rectTransform.sizeDelta = new Vector3(width, height, 1f);
        collider.size = new Vector2(width, height);

        textBox.rectTransform.sizeDelta = new Vector2(width * 100f, height * 100f);
    }

    public void UpdateAnchor()
    {
        Vector3 offset = Vector3.zero;
        if (anchorPoint == UITextboxAnchorPoint.Centre)
        {
            offset = Vector3.zero;
        }
        else if (anchorPoint == UITextboxAnchorPoint.Left)
        {
            offset = new Vector3(width / 2f, 0f, 0f);
        }
        else if (anchorPoint == UITextboxAnchorPoint.Right)
        {
            offset = new Vector3(-width / 2f, 0f, 0f);
        }

        canvas.transform.localPosition = offset;
        collider.offset = offset;
    }

    private void Idle()
    {
        background.color = backgroundColour;
        textBox.color = textColour;
    }

    private void Hover()
    {
        background.color = backgroundHoverColour;
        textBox.color = textHoverColour;
    }

    private void Press()
    {
        background.color = backgroundPressedColour;
        textBox.color = textPressedColour;
    }

    private void Select()
    {
        background.color = backgroundSelectedColour;
        textBox.color = textSelectedColour;
    }

    private void MouseTargetInput()
    {
        if (inputTarget.mouseTarget.selected)
        {
            Select();
        }
        else if (inputTarget.mouseTarget.state == MouseTargetState.Idle)
        {
            Idle();
        }
        else if (inputTarget.mouseTarget.state == MouseTargetState.Hover)
        {
            Hover();
        }
        else if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
        {
            Press();
        }
    }

    private void KeyboardTargetInput()
    {
        bool inputMade = false;

        if (inputTarget.keyboardTarget.IsPressed(KeyCode.Return) || inputTarget.keyboardTarget.IsPressed(KeyCode.Escape))
        {
            inputTarget.GetUntargeted();
            onFinished.Invoke();
        }
        else if (inputTarget.keyboardTarget.IsPressed(KeyCode.Backspace))
        {
            if (text.Length > 0)
            {
                _text = text.Remove(text.Length - 1);
                inputMade = true;
            }
        }
        else
        {
            foreach(char chr in allowedCharacters)
            {
                if (inputTarget.keyboardTarget.IsPressed(KeyCodeFunctions.StrToKeyCode(chr.ToString())))
                {
                    _text += chr;
                    inputMade = true;
                }
            }
        }

        UpdateText();

        if (inputMade)
        {
            onInput.Invoke();
        }
    }

    public void SetText(string text)
    {
        _text = text;
        UpdateText();
    }
    public void SetPrefix(string prefix)
    {
        _prefix = prefix;
        UpdateText();
    }
    public void SetSuffix(string suffix)
    {
        _suffix = suffix;
        UpdateText();
    }

    public void SubscribeToInputEvent(UnityAction call)
    {
        onInput.AddListener(call);
    }
    public void SubscribeToFinishEvent(UnityAction call)
    {
        onFinished.AddListener(call);
    }
}
