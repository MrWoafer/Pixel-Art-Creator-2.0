using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UIColourPicker))]
public class ColourPicker : MonoBehaviour
{
    public Color colour
    {
        get
        {
            return uiColourPicker.colour;
        }
    }

    public Color primaryColour
    {
        get
        {
            return colourPreviews[0].colour;
        }
    }
    public Color secondaryColour
    {
        get
        {
            return colourPreviews[1].colour;
        }
    }

    [Header("Events")]
    [SerializeField]
    private UnityEvent onColourChanged = new UnityEvent();

    [Header("References")]
    private ColourPreview[] colourPreviews;
    private int currentColourPreviewIndex;
    private ColourPreview currentColourPreview
    {
        get
        {
            return colourPreviews[currentColourPreviewIndex];
        }
    }
    private UIToggleGroup colourPreviewToggleGroup;
    private UIColourPicker uiColourPicker;

    public int numOfColourPreviews
    {
        get
        {
            return colourPreviews.Length;
        }
    }

    private UIScale rScale;
    private UIScale gScale;
    private UIScale bScale;
    private UIScale aScale;

    private InputSystem inputSystem;
    private Toolbar toolbar;

    private void Awake()
    {
        colourPreviewToggleGroup = transform.Find("Canvas").Find("Colour Preview Toggle Group").GetComponent<UIToggleGroup>();
        colourPreviews = colourPreviewToggleGroup.GetComponentsInChildren<ColourPreview>();
        uiColourPicker = GetComponent<UIColourPicker>();

        rScale = transform.Find("Canvas").Find("RGBA Scales").Find("R Scale").Find("Scale").GetComponent<UIScale>();
        gScale = transform.Find("Canvas").Find("RGBA Scales").Find("G Scale").Find("Scale").GetComponent<UIScale>();
        bScale = transform.Find("Canvas").Find("RGBA Scales").Find("B Scale").Find("Scale").GetComponent<UIScale>();
        aScale = transform.Find("Canvas").Find("RGBA Scales").Find("A Scale").Find("Scale").GetComponent<UIScale>();

        inputSystem = Finder.inputSystem;
        toolbar = Finder.toolbar;
    }

    // Start is called before the first frame update
    void Start()
    {
        rScale.SubscribeToValueChange(ColourChangedByScales);
        gScale.SubscribeToValueChange(ColourChangedByScales);
        bScale.SubscribeToValueChange(ColourChangedByScales);
        aScale.SubscribeToValueChange(ColourChangedByScales);

        foreach (ColourPreview colourPreview in colourPreviews)
        {
            colourPreview.SubscribeToSelect(OnSelectColourPreview);
        }

        inputSystem.SubscribeToGlobalKeyboard(KeyboardShortcut);

        uiColourPicker.SubscribeToColourChange(() => { onColourChanged.Invoke(); });

        SelectColourPreview(0);
    }

    public void SetColour(Color colour)
    {
        uiColourPicker.SetColour(colour);
    }

    public Color GetColour()
    {
        return colour;
    }
    public Color GetColour(int colourPreviewIndex)
    {
        if (colourPreviewIndex < 0 || colourPreviewIndex >= colourPreviews.Length)
        {
            throw new System.IndexOutOfRangeException("colourPreviewIndex out of range: " + colourPreviewIndex);
        }

        return colourPreviews[colourPreviewIndex].colour;
    }

    private void ColourChangedByScales()
    {
        SetColour(new Color(rScale.value / 255f, gScale.value / 255f, bScale.value / 255f, aScale.value / 255f));
    }

    private void SelectColourPreview(int colourPreviewIndex)
    {
        uiColourPicker.colourPreview = colourPreviews[colourPreviewIndex];
        colourPreviewToggleGroup.Press(colourPreviewIndex);
    }

    private void SelectedColourPreview(int colourPreviewIndex)
    {
        if (colourPreviewIndex < 0 || colourPreviewIndex > colourPreviews.Length)
        {
            throw new System.Exception("Index out of range: " + colourPreviewIndex);
        }

        currentColourPreviewIndex = colourPreviewIndex;
        uiColourPicker.colourPreview = colourPreviews[colourPreviewIndex];
        SetColour(colourPreviews[colourPreviewIndex].colour);
    }
    private void SelectedColourPreview(ColourPreview colourPreview)
    {
        for(int i = 0; i < colourPreviews.Length; i++)
        {
            if (colourPreview == colourPreviews[i])
            {
                SelectedColourPreview(i);
                return;
            }
        }
        throw new System.Exception("Couldn't find given colour preview.");
    }

    private void CycleColourPreview(int numOfSteps)
    {
        SelectColourPreview(Functions.Mod(currentColourPreviewIndex + numOfSteps, colourPreviews.Length));
    }

    private void OnSelectColourPreview()
    {
        SelectedColourPreview(colourPreviewToggleGroup.currentToggle.GetComponent<ColourPreview>());
    }

    private void KeyboardShortcut()
    {
        if (toolbar.selectedTool != Tool.Move)
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("selected colour left")))
            {
                CycleColourPreview(-1);
            }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("selected colour right")))
            {
                CycleColourPreview(1);
            }
        }

        if (toolbar.selectedTool == Tool.Pencil || toolbar.selectedTool == Tool.EyeDropper || toolbar.selectedTool == Tool.Fill)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1))
            {
                SelectColourPreview(0);
            }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2))
            {
                SelectColourPreview(1);
            }
        }
    }

    public void SubscribeToColourChange(UnityAction call)
    {
        onColourChanged.AddListener(call);
    }
}
