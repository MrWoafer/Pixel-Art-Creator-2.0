using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColourPreview : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Color _colour = Color.red;
    public Color colour
    {
        get
        {
            if (_colour.a == 0f)
            {
                return new Color(0f, 0f, 0f, 0f);
            }
            return _colour;
        }
        set
        {
            _colour = value;
        }
    }
    [SerializeField]
    [Min(0f)]
    private float _outlineThickness = 0.1f;
    public float outlineThickness
    {
        get
        {
            return _outlineThickness;
        }
    }
    [SerializeField]
    private bool useRainbowOutline = true;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onSelect = new UnityEvent();
    [SerializeField]
    private UnityEvent onToggle = new UnityEvent();

    private UIToggleButton toggle;
    private Image toggleBackground;
    private RainbowOutline rainbowOutline;
    private Transform outerOutline;
    private Transform innerOutline;

    private void Awake()
    {
        GetReferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        toggle.SubscribeToLeftClick(OnInput);
        SetColour(colour);
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            UpdateDisplay();
        }
    }

    private void OnValidate()
    {
        GetReferences();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        GetReferences();
        UpdateSize();
        toggleBackground.color = colour;
    }

    private void GetReferences()
    {
        toggle = GetComponent<UIToggleButton>();
        toggleBackground = toggle.transform.Find("Canvas").Find("Background").GetComponent<Image>();
        rainbowOutline = transform.Find("Canvas").Find("Outline").Find("Rainbow").GetComponent<RainbowOutline>();
        outerOutline = transform.Find("Canvas").Find("Outline").Find("Outer Outline");
        innerOutline = transform.Find("Canvas").Find("Outline").Find("Inner Outline");
    }

    private void UpdateSize()
    {
        rainbowOutline.transform.localScale = new Vector3(1f + outlineThickness * 2f,
            (transform.localScale.y / transform.localScale.x + outlineThickness * 2f) * transform.localScale.x / transform.localScale.y, 1f);

        rainbowOutline.thickness = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y) / 2f;
        rainbowOutline.outlineEnabled = toggle.on && useRainbowOutline;

        outerOutline.GetComponent<RectTransform>().localScale = rainbowOutline.transform.localScale;

        innerOutline.GetComponent<RectTransform>().localScale = new Vector3(1f + outlineThickness,
            (transform.localScale.y / transform.localScale.x + outlineThickness) * transform.localScale.x / transform.localScale.y, 1f);
    }

    public void SetColour(Color colour)
    {
        _colour = colour;
        toggle.offBackgroundColour = colour;
        toggle.onBackgroundColour = colour;
        toggle.pressedBackgroundColour = colour;
        toggleBackground.color = colour;
    }

    private void OnInput()
    {
        rainbowOutline.outlineEnabled = toggle.on && useRainbowOutline;
        if (toggle.on)
        {
            onSelect.Invoke();
        }
        onToggle.Invoke();
    }

    public void SubscribeToSelect(UnityAction call)
    {
        onSelect.AddListener(call);
    }
    public void SubscribeToToggle(UnityAction call)
    {
        onToggle.AddListener(call);
    }
}
