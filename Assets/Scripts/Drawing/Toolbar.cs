using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.ImageEditing;
using PAC.Input;
using PAC.KeyboardShortcuts;
using PAC.Geometry.Shapes;
using PAC.UI;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Drawing
{
    public enum Shape
    {
        Rectangle = 0,
        Ellipse = 1,
        RightTriangle = 2,
        Diamond = 3
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
        Custom = -1,
        Circle = 0,
        Square = 1,
        Diamond = 2
    }

    public enum GradientMode
    {
        Linear = 0,
        Radial = 1,
    }


    /// <summary>
    /// Handles selecting tools, brush size, etc.
    /// </summary>
    public class Toolbar : MonoBehaviour
    {
        [SerializeField]
        private int _brushSize = 1;
        public int brushSize
        {
            get => _brushSize;
            private set
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
        public float lineSmoothingTime => _lineSmoothingTime;

        public Tool selectedTool { get => usingGlobalEyeDropper ? Tool.GlobalEyeDropper : Tool.StringToTool(toggleGroup.currentToggle.toggleName); }
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

        public bool floodFillDiagonallyAdjacent = false;

        [Header("Events")]
        [SerializeField]
        /// <summary>Event invoked when selected tool changes.</summary>
        private UnityEvent onToolChanged = new UnityEvent();
        [SerializeField]
        /// <summary>Event invoked when brush size changes.</summary>
        private UnityEvent onBrushSizeChanged = new UnityEvent();
        [SerializeField]
        /// <summary>Event invoked when brush pixels change.</summary>
        private UnityEvent onBrushPixelsChanged = new UnityEvent();

        /// <summary>The pixels, given relative to the position of the mouse, that will be affected by the current brush.</summary>
        public IntVector2[] brushPixels { get; private set; } = new IntVector2[0];
        public Texture2D brushTexture { get; private set; }
        public int brushPixelsWidth { get; private set; } = 0;
        public int brushPixelsHeight { get; private set; } = 0;
        public bool brushPixelsIsEmpty => brushPixels.Length == 0;
        public bool brushPixelsIsSingleCentralPixel => brushPixels.Length == 1 && brushPixels[0] == IntVector2.zero;

        private Texture2D customBrushTexture = null;

        private InputSystem inputSystem;
        private UIToggleGroup toggleGroup;

        private void Awake()
        {
            inputSystem = Finder.inputSystem;
            toggleGroup = transform.Find("Canvas").Find("Toggle Group").GetComponent<UIToggleGroup>();
        }

        void Start()
        {
            inputSystem.SubscribeToGlobalKeyboard(CheckKeyboardShortcuts);
            inputSystem.SubscribeToGlobalMouseScroll(OnMouseScroll);

            toggleGroup.SubscribeToSelectedToggleChange(() => {
                UpdateBrushBorder();
                onToolChanged.Invoke();
            });

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

        public bool SetBrushSize(int brushSize)
        {
            if (brushSize < 0)
            {
                return false;
            }
            if (brushSize > Config.Tools.maxBrushSize)
            {
                return false;
            }
            if (this.brushSize == brushSize)
            {
                return false;
            }

            this.brushSize = Mathf.Clamp(brushSize, 1, Config.Tools.maxBrushSize);
            onBrushSizeChanged.Invoke();

            return true;
        }

        private void CheckKeyboardShortcuts()
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("pencil"))) { SelectTool(Tool.Pencil); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("brush"))) { SelectTool(Tool.Brush); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("rubber"))) { SelectTool(Tool.Rubber); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("eye dropper"))) { SelectTool(Tool.EyeDropper); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("fill"))) { SelectTool(Tool.Fill); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("shape"))) { SelectTool(Tool.Shape); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("line"))) { SelectTool(Tool.Line); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("iso box"))) { SelectTool(Tool.IsoBox); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("gradient"))) { SelectTool(Tool.Gradient); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("move"))) { SelectTool(Tool.Move); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("selection"))) { SelectTool(Tool.Selection); }
            else if (selectedTool == Tool.Shape)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { shapeToolShape = Shape.Rectangle; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { shapeToolShape = Shape.Ellipse; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3)) { shapeToolShape = Shape.RightTriangle; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha4)) { shapeToolShape = Shape.Diamond; }
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
                if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("cancel tool"))) { DeselectGlobalEyeDropper(); }
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

        /// <summary>
        /// Called when the mouse scrolls.
        /// </summary>
        private void OnMouseScroll()
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("scroll brush size")))
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

        /// <summary>
        /// Update the outline of the pixels the brush will affect.
        /// </summary>
        private void UpdateBrushBorder()
        {
            // Get the brush texture
            brushTexture = new Texture2D(1, 1);
            if (selectedTool == Tool.None || selectedTool == Tool.Move)
            {
                brushTexture = Texture2DCreator.Transparent(1, 1);
            }
            else if (selectedTool == Tool.Rubber || selectedTool == Tool.Brush)
            {
                if (brushShape == BrushShape.Square)
                {
                    brushTexture = Texture2DCreator.Solid(brushSize * 2 - 1, brushSize * 2 - 1, Config.Colours.mask);
                }
                else if (brushShape == BrushShape.Circle)
                {
                    brushTexture = new Ellipse(new IntRect(IntVector2.zero, IntVector2.one * (brushSize * 2 - 2)), true).ToTexture(Config.Colours.mask);
                }
                else if (brushShape == BrushShape.Diamond)
                {
                    brushTexture = new Diamond(new IntRect(IntVector2.zero, IntVector2.one * (brushSize * 2 - 2)), true).ToTexture(Config.Colours.mask);
                }
                else if (brushShape == BrushShape.Custom)
                {
                    if (customBrushTexture == null)
                    {
                        throw new System.Exception("Custom brush shape has not yet been assigned.");
                    }

                    brushTexture = customBrushTexture.DeepCopy();
                }
                else
                {
                    throw new System.Exception("Unknown / unimplemented brush shape: " + brushShape);
                }
            }
            else
            {
                brushTexture = Texture2DCreator.Solid(1, 1, Config.Colours.mask).Applied();
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

        /// <summary>
        /// Turns the given texture into a brush shape by taking any pixels with non-zero alpha.
        /// </summary>
        public void LoadCustomBrush(Texture2D brushShape)
        {
            customBrushTexture = Texture2DCreator.Transparent(brushShape.width, brushShape.height);

            for (int x = 0; x < brushShape.width; x++)
            {
                for (int y = 0; y < brushShape.height; y++)
                {
                    if (brushShape.GetPixel(x, y).a != 0f)
                    {
                        customBrushTexture.SetPixel(x, y, Config.Colours.mask);
                    }
                }
            }

            customBrushTexture.Apply();
        }

        /// <summary>
        /// Event invoked when selected tool changes.
        /// </summary>
        public void SubscribeToOnToolChanged(UnityAction call)
        {
            onToolChanged.AddListener(call);
        }
        /// <summary>
        /// Event invoked when brush size changes.
        /// </summary>
        public void SubscribeToOnBrushSizeChanged(UnityAction call)
        {
            onBrushSizeChanged.AddListener(call);
        }
        /// <summary>
        /// Event invoked when brush pixels change.
        /// </summary>
        public void SubscribeToOnBrushPixelsChanged(UnityAction call)
        {
            onBrushPixelsChanged.AddListener(call);
        }
    }
}