using System.Collections.Generic;
using System.Linq;
using PAC.Input;
using PAC.Geometry.Shapes;

using UnityEngine;
using UnityEngine.Events;
using PAC.Geometry.Shapes.Extensions;
using PAC.Geometry;
using PAC.Extensions.UnityEngine;
using PAC.UI.Components.General;
using PAC.UI.Components.Specialised;
using PAC.Tools;
using System;
using PAC.Tools.Interfaces;

namespace PAC.Managers
{
    /// <summary>
    /// Handles selecting tools, brush size, etc.
    /// </summary>
    public class Toolbar : MonoBehaviour
    {
        public Tool selectedTool { get => usingGlobalEyeDropper ? GlobalEyeDropperTool : ParseTool(toggleGroup.currentToggle.toggleName); }
        public Tool previousTool { get; private set; } = new NoneTool();

        private bool usingGlobalEyeDropper = false;

        [SerializeField]
        private int _brushSize = 1;
        /// <summary>
        /// The global brush size for all tools.
        /// </summary>
        /// <remarks>
        /// Do not edit the tools' brush sizes directly. Only edit them via this property (which will then update them). Editing the tools' brush sizes directly will not update this or
        /// other tools.
        /// </remarks>
        public int brushSize
        {
            get => _brushSize;
            set
            {
                if (brushSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(brushSize)} must be positive.");
                }
                if (brushSize < Config.Tools.minBrushSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(brushSize)} cannot be less than {nameof(Config.Tools.minBrushSize)}.");
                }
                if (brushSize > Config.Tools.maxBrushSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(brushSize)} cannot be more than {nameof(Config.Tools.maxBrushSize)}.");
                }
                if (value == brushSize)
                {
                    return;
                }

                _brushSize = value;
                foreach (IHasBrushSize tool in AllTools.OfType<IHasBrushSize>())
                {
                    tool.brushSize = value;
                }
                
                UpdateBrushBorder();
                onBrushSizeChanged.Invoke();
            }
        }

        [SerializeField]
        private BrushShape _brushShape = new BrushShape.Circle();
        /// <summary>
        /// The global brush shape for all tools.
        /// </summary>
        /// <remarks>
        /// Do not edit the tools' brush shapes directly. Only edit them via this property (which will then update them). Editing the tools' brush shapes directly will not update this or
        /// other tools.
        /// </remarks>
        public BrushShape brushShape
        {
            get => _brushShape;
            set
            {
                _brushShape = value;
                foreach (IHasSettableBrushShape tool in AllTools.OfType<IHasSettableBrushShape>())
                {
                    tool.brushShape = value;
                }

                UpdateBrushBorder();
            }
        }

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

        public BrushTool BrushTool { get; private set; }
        public PencilTool PencilTool { get; private set; }
        public RubberTool RubberTool { get; private set; }
        public EyeDropperTool EyeDropperTool { get; private set; }
        public GlobalEyeDropperTool GlobalEyeDropperTool { get; private set; }
        public FillTool FillTool { get; private set; }
        public ShapeTool ShapeTool { get; private set; }
        public LineTool LineTool { get; private set; }
        public IsometricCuboidTool IsometricCuboidTool { get; private set; }
        public GradientTool GradientTool { get; private set; }
        public MoveTool MoveTool { get; private set; }
        public SelectionTool SelectionTool { get; private set; }

        private Tool[] AllTools;

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

            BrushTool = new BrushTool(brushSize, brushShape);
            PencilTool = new PencilTool();
            RubberTool = new RubberTool(brushSize, brushShape);
            EyeDropperTool = new EyeDropperTool();
            GlobalEyeDropperTool = new GlobalEyeDropperTool();
            FillTool = new FillTool(false);
            ShapeTool = new ShapeTool(ShapeTool.Shape.Rectangle);
            LineTool = new LineTool();
            IsometricCuboidTool = new IsometricCuboidTool();
            GradientTool = new GradientTool(GradientTool.Mode.Linear);
            MoveTool = new MoveTool();
            SelectionTool = new SelectionTool(SelectionTool.Mode.Rectangle);

            AllTools = new Tool[]
            {
                BrushTool,
                PencilTool,
                RubberTool,
                EyeDropperTool,
                GlobalEyeDropperTool,
                FillTool,
                ShapeTool,
                LineTool,
                IsometricCuboidTool,
                GradientTool,
                MoveTool,
                SelectionTool,
            }; 
        }

        void Start()
        {
            inputSystem.SubscribeToGlobalKeyboard(CheckKeyboardShortcuts);
            inputSystem.SubscribeToGlobalMouseScroll(OnMouseScroll);

            toggleGroup.SubscribeToSelectedToggleChange(() =>
            {
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

        private void CheckKeyboardShortcuts()
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("pencil"))) { SelectTool(PencilTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("brush"))) { SelectTool(BrushTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("rubber"))) { SelectTool(RubberTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("eye dropper"))) { SelectTool(EyeDropperTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("fill"))) { SelectTool(FillTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("shape"))) { SelectTool(ShapeTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("line"))) { SelectTool(LineTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("iso box"))) { SelectTool(IsometricCuboidTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("gradient"))) { SelectTool(GradientTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("move"))) { SelectTool(MoveTool); }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("selection"))) { SelectTool(SelectionTool); }
            else if (selectedTool is ShapeTool)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { ShapeTool.shape = ShapeTool.Shape.Rectangle; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { ShapeTool.shape = ShapeTool.Shape.Ellipse; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3)) { ShapeTool.shape = ShapeTool.Shape.RightTriangle; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha4)) { ShapeTool.shape = ShapeTool.Shape.Diamond; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha5)) { ShapeTool.shape = ShapeTool.Shape.IsometricHexagon; }
            }
            else if (selectedTool is SelectionTool)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { SelectionTool.mode = SelectionTool.Mode.Draw; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { SelectionTool.mode = SelectionTool.Mode.MagicWand; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3)) { SelectionTool.mode = SelectionTool.Mode.Rectangle; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha4)) { SelectionTool.mode = SelectionTool.Mode.Ellipse; }
            }
            else if (selectedTool is BrushTool || selectedTool is RubberTool)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1))
                {
                    brushShape = new BrushShape.Circle();
                    onBrushSizeChanged.Invoke();
                }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2))
                {
                    brushShape = new BrushShape.Square();
                    onBrushSizeChanged.Invoke();
                }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3))
                {
                    brushShape = new BrushShape.Diamond();
                    onBrushSizeChanged.Invoke();
                }
            }
            else if (selectedTool is GradientTool)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha1)) { GradientTool.mode = GradientTool.Mode.Linear; }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2)) { GradientTool.mode = GradientTool.Mode.Radial; }
            }
            else if (selectedTool is GlobalEyeDropperTool)
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
            previousTool = GlobalEyeDropperTool;
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
                if (selectedTool is BrushTool || selectedTool is RubberTool)
                {
                    brushSize = brushSize + (int)inputSystem.mouse.scrollDelta;
                }
                if (selectedTool is PencilTool && inputSystem.mouse.scrollDelta > 0)
                {
                    SelectTool("brush");
                    brushSize = 2;
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
            if (selectedTool is NoneTool || selectedTool is MoveTool)
            {
                brushTexture = Texture2DExtensions.Transparent(1, 1);
            }
            else if (selectedTool is RubberTool || selectedTool is BrushTool)
            {
                if (brushShape is BrushShape.Square)
                {
                    brushTexture = Texture2DExtensions.Solid(brushSize * 2 - 1, brushSize * 2 - 1, Config.Colours.mask);
                }
                else if (brushShape is BrushShape.Circle)
                {
                    brushTexture = new Ellipse(new IntRect(IntVector2.zero, IntVector2.one * (brushSize * 2 - 2)), true).ToTexture(Config.Colours.mask);
                }
                else if (brushShape is BrushShape.Diamond)
                {
                    brushTexture = new Diamond(new IntRect(IntVector2.zero, IntVector2.one * (brushSize * 2 - 2)), true).ToTexture(Config.Colours.mask);
                }
                else if (brushShape is BrushShape.Custom)
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
                brushTexture = Texture2DExtensions.Solid(1, 1, Config.Colours.mask).Applied();
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
            customBrushTexture = Texture2DExtensions.Transparent(brushShape.width, brushShape.height);

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

        public Tool ParseTool(string toolName)
        {
            foreach (Tool tool in AllTools)
            {
                if (tool.name == toolName)
                {
                    return tool;
                }
            }
            throw new FormatException($"Could not parse tool name: {toolName}");
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