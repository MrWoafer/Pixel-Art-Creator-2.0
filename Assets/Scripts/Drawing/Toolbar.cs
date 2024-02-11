using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Shape
{
    Rectangle = 0,
    Ellipse = 1,
    Triangle = 2
}

public enum SelectionMode
{
    Draw = 0,
    MagicWand = 1,
    Rectangle = 10,
    Ellipse = 11
}

public enum BrushShape
{
    Circle = 0,
    Square = 1,
    Diamond = 2
}

public enum GradientMode
{
    Linear = 0,
    Radial = 1,
}

public class Toolbar : MonoBehaviour
{
    [Header("Tool Options")]
    [SerializeField]
    private int _maxBrushSize = 10;
    public int maxBrushSize { get => _maxBrushSize; }

    [SerializeField]
    private int _brushSize = 1;
    public int brushSize
    {
        get => _brushSize;
        set
        {
            _brushSize = value;
            UpdateBrushBorder();
        }
    }

    [SerializeField]
    [Min(0f)]
    private float _lineSmoothingTime = 0.2f;
    /// <summary>
    /// The amount of time you have to draw a new pixel in for an old one to be potentially smoothed.
    /// </summary>
    public float lineSmoothingTime { get => _lineSmoothingTime; }

    public Tool selectedTool { get => usingGlobalEyeDropper ? Tool.GlobalEyeDropper : Tool.ToolNameToTool(toggleGroup.currentToggle.toggleName); }
    public Tool previousTool { get; private set; } = Tool.None;

    private bool usingGlobalEyeDropper = false;

    [SerializeField]
    private BrushShape _brushShape = BrushShape.Circle;
    public BrushShape brushShape
    {
        get => _brushShape;
        set
        {
            _brushShape = value;
            UpdateBrushBorder();
        }
    }

    [SerializeField]
    private Shape _shapeToolShape = Shape.Rectangle;
    public Shape shapeToolShape
    {
        get => _shapeToolShape;
        private set
        {
            _shapeToolShape = value;
            UpdateBrushBorder();
        }
    }

    [SerializeField]
    private GradientMode _gradientMode = GradientMode.Linear;
    public GradientMode gradientMode
    {
        get => _gradientMode;
        private set
        {
            _gradientMode = value;
            UpdateBrushBorder();
        }
    }

    [SerializeField]
    private SelectionMode _selectionMode = SelectionMode.Rectangle;
    public SelectionMode selectionMode
    {
        get => _selectionMode;
        private set
        {
            _selectionMode = value;
            UpdateBrushBorder();
        }
    }

    /// <summary>The pixels, given relative to the position of the mouse, that will be affected by the current brush.</summary>
    public IntVector2[] brushPixels { get; private set; } = new IntVector2[0];
    public Texture2D brushTexture { get; private set; }
    public int brushPixelsWidth { get; private set; } = 0;
    public int brushPixelsHeight { get; private set; } = 0;
    public bool brushPixelsIsEmpty { get => brushPixels.Length == 0; }
    public bool brushPixelsIsSingleCentralPixel { get => brushPixels.Length == 1 && brushPixels[0] == IntVector2.zero; }

    private InputSystem inputSystem;
    private UIToggleGroup toggleGroup;

    private UnityEvent onToolChanged = new UnityEvent();
    private UnityEvent onBrushSizeChanged = new UnityEvent();
    private UnityEvent onBrushPixelsChanged = new UnityEvent();

    private void Awake()
    {
        inputSystem = Finder.inputSystem;
        toggleGroup = transform.Find("Canvas").Find("Toggle Group").GetComponent<UIToggleGroup>();
    }

    void Start()
    {
        inputSystem.SubscribeToGlobalKeyboard(KeyboardShortcut);
        inputSystem.SubscribeToGlobalMouseScroll(MouseScroll);

        toggleGroup.SubscribeToSelectedToggleChange(onToolChanged.Invoke);

        UpdateBrushBorder();
    }

    private bool SelectTool(Tool tool) => SelectTool(tool.name);
    private bool SelectTool(string toolName)
    {
        toolName = toolName.ToLower();
        foreach (UIToggleButton toggle in toggleGroup.toggles)
        {
            if (toggle.toggleName == toolName)
            {
                previousTool = selectedTool;
                toggleGroup.Press(toggle);
                UpdateBrushBorder();
                onToolChanged.Invoke();
                return true;
            }
        }
        foreach (UIToolButton toolButton in GetComponentsInChildren<UIToolButton>())
        {
            foreach (UIButton button in toolButton.buttons)
            {
                if (button.name.ToLower() == toolName)
                {
                    previousTool = selectedTool;
                    button.Press();
                    UpdateBrushBorder();
                    onToolChanged.Invoke();
                    return true;
                }
            }
        }
        return false;
    }

    private bool SetBrushSize(int brushSize)
    {
        if (brushSize < 0)
        {
            return false;
        }
        if (brushSize > maxBrushSize)
        {
            return false;
        }
        if (this.brushSize == brushSize)
        {
            return false;
        }

        this.brushSize = Mathf.Clamp(brushSize, 1, maxBrushSize);
        onBrushSizeChanged.Invoke();

        return true;
    }

    private void KeyboardShortcut()
    {
        if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.W) || inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.P)) { SelectTool(Tool.Pencil); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.B)) { SelectTool(Tool.Brush); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.R)) { SelectTool(Tool.Rubber); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.E)) { SelectTool(Tool.EyeDropper); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.F)) { SelectTool(Tool.Fill); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.S)) { SelectTool(Tool.Shape); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.L)) { SelectTool(Tool.Line); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.G)) { SelectTool(Tool.Gradient); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.M)) { SelectTool(Tool.Move); }
        else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Q)) { SelectTool(Tool.Selection); }
        else if (selectedTool == Tool.Shape)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { shapeToolShape = Shape.Rectangle; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { shapeToolShape = Shape.Ellipse; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3)) { shapeToolShape = Shape.Triangle; }
        }
        else if (selectedTool == Tool.Selection)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { selectionMode = SelectionMode.Draw; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { selectionMode = SelectionMode.MagicWand; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3)) { selectionMode = SelectionMode.Rectangle; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha4)) { selectionMode = SelectionMode.Ellipse; }
        }
        else if (selectedTool == Tool.Brush || selectedTool == Tool.Rubber)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1))
            {
                brushShape = BrushShape.Circle;
                onBrushSizeChanged.Invoke();
            }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2))
            {
                brushShape = BrushShape.Square;
                onBrushSizeChanged.Invoke();
            }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3))
            {
                brushShape = BrushShape.Diamond;
                onBrushSizeChanged.Invoke();
            }
        }
        else if (selectedTool == Tool.Gradient)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { gradientMode = GradientMode.Linear; }
            else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { gradientMode = GradientMode.Radial; }
        }
        else if (selectedTool == Tool.GlobalEyeDropper)
        {
            if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Escape)) { DeselectGlobalEyeDropper(); }
        }

        inputSystem.mouse.SetCursorSprite(inputSystem.mouse.cursorState);
    }

    public void SelectGlobalEyeDropper()
    {
        previousTool = selectedTool;
        usingGlobalEyeDropper = true;
        inputSystem.mouse.lockAllUIInteractions = true;

        onToolChanged.Invoke();
        inputSystem.mouse.SetCursorSprite(CursorState.EyeDropper);
    }

    public void DeselectGlobalEyeDropper()
    {
        previousTool = Tool.GlobalEyeDropper;
        usingGlobalEyeDropper = false;
        inputSystem.mouse.lockAllUIInteractions = false;

        onToolChanged.Invoke();
        inputSystem.mouse.SetCursorSprite(inputSystem.mouse.cursorState);
    }

    private void MouseScroll()
    {
        if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.LeftShift) || inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.RightShift))
        {
            if (selectedTool == Tool.Brush || selectedTool == Tool.Rubber)
            {
                SetBrushSize(brushSize + (int)inputSystem.mouse.scrollDelta);
            }
            if (selectedTool == Tool.Pencil && inputSystem.mouse.scrollDelta > 0)
            {
                SelectTool("brush");
                SetBrushSize(2);
            }
        }
    }

    private void UpdateBrushBorder()
    {
        // Get the brush texture
        brushTexture = new Texture2D(1, 1);
        if (selectedTool == Tool.None || selectedTool == Tool.Move)
        {
            brushTexture = Tex2DSprite.BlankTexture(1, 1);
        }
        else if (selectedTool == Tool.Rubber || selectedTool == Tool.Brush)
        {
            if (brushShape == BrushShape.Square)
            {
                brushTexture = Tex2DSprite.SolidTexture(brushSize * 2 - 1, brushSize * 2 - 1, Color.white);
            }
            else if (brushShape == BrushShape.Circle)
            {
                brushTexture = Shapes.Circle(brushSize * 2 - 1, brushSize * 2 - 1, IntVector2.zero, IntVector2.one * (brushSize * 2 - 2), Color.white, true, false);
            }
            else if (brushShape == BrushShape.Diamond)
            {
                brushTexture = Shapes.Diamond(brushSize * 2 - 1, brushSize * 2 - 1, IntVector2.zero, IntVector2.one * (brushSize * 2 - 2), Color.white, true);
            }
            else
            {
                throw new System.Exception("Unknown / unimplemented brush shape: " + brushShape);
            }
        }
        else
        {
            brushTexture = Tex2DSprite.SolidTexture(1, 1, Color.white);
            brushTexture.Apply();
        }

        // Get array of non-transparent pixels in the brush texture
        List<IntVector2> pixelsCovered = new List<IntVector2>();
        for (int x = 0; x < brushTexture.width; x++)
        {
            for (int y = 0; y < brushTexture.height; y++)
            {
                if (brushTexture.GetPixel(x, y).a != 0)
                {
                    pixelsCovered.Add(new IntVector2(x - (brushTexture.width - 1) / 2, y - (brushTexture.height - 1) / 2));
                }
            }
        }
        brushPixels = pixelsCovered.ToArray();

        if (brushPixels.Length == 0)
        {
            brushPixelsWidth = 0;
            brushPixelsHeight = 0;
        }
        else
        {
            brushPixelsWidth = brushPixels.Max(pixel => pixel.x) - brushPixels.Min(pixel => pixel.x) + 1;
            brushPixelsHeight = brushPixels.Max(pixel => pixel.y) - brushPixels.Min(pixel => pixel.y) + 1;
        }

        onBrushPixelsChanged.Invoke();
    }

    public void SubscribeToToolChanged(UnityAction call)
    {
        onToolChanged.AddListener(call);
    }
    public void SubscribeToBrushSizeChanged(UnityAction call)
    {
        onBrushSizeChanged.AddListener(call);
    }
    public void SubscribeToBrushPixelsChanged(UnityAction call)
    {
        onBrushPixelsChanged.AddListener(call);
    }
}
