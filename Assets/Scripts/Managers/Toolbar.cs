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
                foreach (IHasBrushShape tool in AllTools.OfType<IHasBrushShape>())
                {
                    if (tool.brushShape is IHasBrushSize hasBrushSize)
                    {
                        hasBrushSize.brushSize = value;
                    }
                }
                
                onBrushPixelsChanged.Invoke();
                onBrushSizeChanged.Invoke();
            }
        }

        [SerializeField]
        private BrushShape _brushShape;
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
                brushSize = brushSize;

                onBrushPixelsChanged.Invoke();
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

        private InputSystem inputSystem;
        private UIToggleGroup toggleGroup;

        private void Awake()
        {
            inputSystem = Finder.inputSystem;
            toggleGroup = transform.Find("Canvas").Find("Toggle Group").GetComponent<UIToggleGroup>();

            BrushTool = new BrushTool(brushShape);
            PencilTool = new PencilTool();
            RubberTool = new RubberTool(brushShape);
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

            brushShape = new BrushShape.Circle(brushSize);
        }

        void Start()
        {
            inputSystem.SubscribeToGlobalKeyboard(CheckKeyboardShortcuts);
            inputSystem.SubscribeToGlobalMouseScroll(OnMouseScroll);

            toggleGroup.SubscribeToSelectedToggleChange(() =>
            {
                onBrushPixelsChanged.Invoke();
                onToolChanged.Invoke();
            });

            onBrushPixelsChanged.Invoke();
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
                    onBrushPixelsChanged.Invoke();
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
                        onBrushPixelsChanged.Invoke();
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
                    brushShape = new BrushShape.Circle(brushSize);
                    onBrushSizeChanged.Invoke();
                }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha2))
                {
                    brushShape = new BrushShape.Square(brushSize);
                    onBrushSizeChanged.Invoke();
                }
                else if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.Alpha3))
                {
                    brushShape = new BrushShape.Diamond(brushSize);
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
                    brushSize = Math.Clamp(brushSize + (int)inputSystem.mouse.scrollDelta, Config.Tools.minBrushSize, Config.Tools.maxBrushSize);
                }
                if (selectedTool is PencilTool && inputSystem.mouse.scrollDelta > 0)
                {
                    SelectTool("brush");
                    brushSize = 2;
                }
            }
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