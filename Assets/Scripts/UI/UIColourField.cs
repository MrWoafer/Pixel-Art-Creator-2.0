using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIColourField : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Color startingColour = Color.red;
    public Color colour
    {
        get
        {
            return Application.isPlaying ? colourPicker.colour : startingColour;
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

    public bool colourPickerOpen { get; private set; } = false;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onColourChanged = new UnityEvent();
    [SerializeField]
    private UnityEvent onColourPickerOpened = new UnityEvent();
    [SerializeField]
    private UnityEvent onColourPickerClosed = new UnityEvent();

    private UIColourPicker colourPicker;
    private UIButton button;
    private Image background;
    private Transform outerOutline;
    private Transform innerOutline;

    private Vector3 colourPickerPosition;

    private bool beenRunningAFrame = false;

    private void Awake()
    {
        GetReferences();
        colourPicker.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        colourPicker.SubscribeToColourChange(UpdateColour);
        colourPicker.SubscribeToClose(CloseColourPicker);
        button.SubscribeToClick(OpenColourPicker);

        colourPickerPosition = colourPicker.transform.position;

        SetColour(startingColour);
        CloseColourPicker();
    }

    private void Update()
    {
        if (!beenRunningAFrame)
        {
            beenRunningAFrame = true;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying || beenRunningAFrame)
        {
            GetReferences();
            UpdateDisplay();
        }
    }

    private void GetReferences()
    {
        colourPicker = transform.Find("Colour Picker Panel").GetComponent<UIColourPicker>();

        button = transform.Find("Button").GetComponent<UIButton>();
        background = button.transform.Find("Canvas").Find("Background").GetComponent<Image>();
        outerOutline = button.transform.Find("Canvas").Find("Outline").Find("Outer Outline");
        innerOutline = button.transform.Find("Canvas").Find("Outline").Find("Inner Outline");
    }

    private void UpdateDisplay()
    {
        GetReferences();

        UpdateSize();
        UpdateColourNoEvent();
    }

    private void UpdateSize()
    {
        outerOutline.GetComponent<RectTransform>().localScale = new Vector3(1f + outlineThickness * 2f,
            (button.transform.localScale.y / button.transform.localScale.x + outlineThickness * 2f) * button.transform.localScale.x / button.transform.localScale.y, 1f);

        innerOutline.GetComponent<RectTransform>().localScale = new Vector3(1f + outlineThickness,
            (button.transform.localScale.y / button.transform.localScale.x + outlineThickness) * button.transform.localScale.x / button.transform.localScale.y, 1f);
    }

    private void UpdateColour()
    {
        UpdateColourNoEvent();

        onColourChanged.Invoke();
    }
    private void UpdateColourNoEvent()
    {
        button.backgroundColour = colour;
        button.backgroundHoverColour = colour;
        button.backgroundPressedColour = colour;
        background.color = colour;
    }

    public void SetColour(Color colour)
    {
        colourPicker.SetColour(colour);
    }

    public void OpenColourPicker()
    {
        colourPicker.transform.position = colourPickerPosition;
        colourPickerOpen = true;

        onColourPickerOpened.Invoke();
    }

    public void CloseColourPicker()
    {
        colourPicker.transform.position = new Vector3(-10000f, 0f, 0f);
        colourPickerOpen = false;

        onColourPickerClosed.Invoke();
    }

    public void SubscribeToColourChange(UnityAction call)
    {
        onColourChanged.AddListener(call);
    }
    public void SubscribeToColourPickerOpen(UnityAction call)
    {
        onColourPickerOpened.AddListener(call);
    }
    public void SubscribeToColourPickerClose(UnityAction call)
    {
        onColourPickerClosed.AddListener(call);
    }
}
