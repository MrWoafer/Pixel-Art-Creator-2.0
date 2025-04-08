using System;
using System.Collections.Generic;

using PAC.Drawing;
using PAC.Managers;
using PAC.UI.Components;

using UnityEngine;
using UnityEngine.Events;

using GradientMode = PAC.Managers.GradientMode;

namespace PAC.Input
{
    public enum CursorState
    {
        CurrentTool = -1,
        Unspecified = 0,
        Normal = 1,
        Hover = 2,
        Press = 3,
        Grab = 12,
        Invisible = 4,
        EyeDropper = 5,
        Text = 6,
        CrossArrows = 7,
        UpDownArrow = 8,
        LeftRightArrow = 9,
        MagnifyingGlass = 10,
        MagicWand = 11,
        LinearGradient = 30,
        RadialGradient = 31,
        SelectionRectangle = 100,
        SelectionEllipse = 101
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public class Mouse : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        [Min(0f)]
        private float _scrollCursorDisplayTime = 0.5f;
        public float scrollCursorDisplayTime => _scrollCursorDisplayTime;

        [Header("Display")]
        [SerializeField]
        private Texture2D cursorSpriteNormal;
        [SerializeField]
        private Vector2 cursorHotSpotNormal;
        [SerializeField]
        private Texture2D cursorSpriteHover;
        [SerializeField]
        private Vector2 cursorHotSpotHover;
        [SerializeField]
        private Texture2D cursorSpritePress;
        [SerializeField]
        private Vector2 cursorHotSpotPress;
        [SerializeField]
        private Texture2D cursorSpriteGrab;
        [SerializeField]
        private Vector2 cursorHotSpotGrab;
        [SerializeField]
        private Texture2D cursorSpriteEyeDropper;
        [SerializeField]
        private Vector2 cursorHotSpotEyeDropper;
        [SerializeField]
        private Texture2D cursorSpriteText;
        [SerializeField]
        private Vector2 cursorHotSpotText;
        [SerializeField]
        private Texture2D cursorSpritePencil;
        [SerializeField]
        private Vector2 cursorHotSpotPencil;
        [SerializeField]
        private Texture2D cursorSpriteRubber;
        [SerializeField]
        private Vector2 cursorHotSpotRubber;
        [SerializeField]
        private Texture2D cursorSpriteBrush;
        [SerializeField]
        private Vector2 cursorHotSpotBrush;
        [SerializeField]
        private Texture2D cursorSpriteFill;
        [SerializeField]
        private Vector2 cursorHotSpotFill;
        [SerializeField]
        private Texture2D cursorSpriteRectangle;
        [SerializeField]
        private Vector2 cursorHotSpotRectangle;
        [SerializeField]
        private Texture2D cursorSpriteEllipse;
        [SerializeField]
        private Vector2 cursorHotSpotEllipse;
        [SerializeField]
        private Texture2D cursorSpriteTriangle;
        [SerializeField]
        private Vector2 cursorHotSpotTriangle;
        [SerializeField]
        private Texture2D cursorSpriteTriangleUpsideDown;
        [SerializeField]
        private Vector2 cursorHotSpotTriangleUpsideDown;
        [SerializeField]
        private Texture2D cursorSpriteDiamond;
        [SerializeField]
        private Vector2 cursorHotSpotDiamond;
        [SerializeField]
        private Texture2D cursorSpriteCrossArrows;
        [SerializeField]
        private Vector2 cursorHotSpotCrossArrows;
        [SerializeField]
        private Texture2D cursorSpriteUpDownArrow;
        [SerializeField]
        private Vector2 cursorHotSpotUpDownArrow;
        [SerializeField]
        private Texture2D cursorSpriteLeftRightArrow;
        [SerializeField]
        private Vector2 cursorHotSpotLeftRightArrow;
        [SerializeField]
        private Texture2D cursorSpriteMagnifyingGlass;
        [SerializeField]
        private Vector2 cursorHotSpotMagnifyingGlass;
        [SerializeField]
        private Texture2D cursorSpriteMagicWand;
        [SerializeField]
        private Vector2 cursorHotSpotMagicWand;
        [SerializeField]
        private Texture2D cursorSpriteSelectionRectangle;
        [SerializeField]
        private Vector2 cursorHotSpotSelectionRectangle;
        [SerializeField]
        private Texture2D cursorSpriteSelectionEllipse;
        [SerializeField]
        private Vector2 cursorHotSpotSelectionEllipse;
        [SerializeField]
        private Texture2D cursorSpriteLinearGradient;
        [SerializeField]
        private Vector2 cursorHotSpotLinearGradient;
        [SerializeField]
        private Texture2D cursorSpriteRadialGradient;
        [SerializeField]
        private Vector2 cursorHotSpotRadialGradient;
        [SerializeField]
        private Texture2D cursorSpriteIsoBox;
        [SerializeField]
        private Vector2 cursorHotSpotIsoBox;

        public bool leftClick => UnityEngine.Input.GetMouseButtonDown(0);
        public bool leftUnclick => UnityEngine.Input.GetMouseButtonUp(0);
        public bool leftHold => UnityEngine.Input.GetMouseButton(0);

        public bool rightClick => UnityEngine.Input.GetMouseButtonDown(1);
        public bool rightUnclick => UnityEngine.Input.GetMouseButtonUp(1);
        public bool rightHold => UnityEngine.Input.GetMouseButton(1);

        public bool middleClick => UnityEngine.Input.GetMouseButtonDown(2);
        public bool middleUnclick => UnityEngine.Input.GetMouseButtonUp(2);
        public bool middleHold => UnityEngine.Input.GetMouseButton(2);

        public bool click => leftClick || rightClick || middleClick;
        public bool unclick => leftUnclick || rightUnclick || middleUnclick;
        public bool held => leftHold || rightHold || middleHold;
        
        public bool nothingClick => !leftClick && !rightClick && !middleClick;
        public bool nothingHeld => !leftHold && !rightHold && !middleHold;

        public int numButtonsHeld
        {
            get
            {
                int num = 0;
                if (leftHold)
                {
                    num++;
                }
                if (rightHold)
                {
                    num++;
                }
                if (middleHold)
                {
                    num++;
                }
                return num;
            }
        }

        public Vector2 worldPos => Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

        public float scrollDelta => UnityEngine.Input.mouseScrollDelta.y;

        public InputTarget hoverTarget { get; private set; }
        private InputTarget previousHoverTarget;
        private List<InputTarget> hoverTriggers = new List<InputTarget>();
        private List<InputTarget> previousHoverTriggers = new List<InputTarget>();
        public bool hasHoverTrigger => hoverTriggers.Count > 0;

        [HideInInspector]
        public bool canInteract = false;
        [HideInInspector]
        public bool lockAllUIInteractions = false;

        private InputSystem inputSystem;
        private UIManager uiManager;

        private Toolbar toolbar;

        public CursorState cursorState { get; private set; } = CursorState.Normal;
        private const int CURSOR_RESOLUTION = 16;
        private const int CURSOR_SCALED_RESOLUTION = 256;

        private UnityEvent onScroll = new UnityEvent();
        private UnityEvent onClick = new UnityEvent();
        private UnityEvent onLeftClick = new UnityEvent();
        private UnityEvent onRightClick = new UnityEvent();
        private UnityEvent onMiddleClick = new UnityEvent();
        private UnityEvent onUnclick = new UnityEvent();
        private UnityEvent onLeftUnclick = new UnityEvent();
        private UnityEvent onRightUnclick = new UnityEvent();
        private UnityEvent onMiddleUnclick = new UnityEvent();

        private float scrollCursorTimer = 0f;

        // Start is called before the first frame update
        void Start()
        {
            inputSystem = Finder.inputSystem;
            uiManager = Finder.uiManager;
            toolbar = Finder.toolbar;

            SetCursorState(CursorState.Normal);
        }

        // Update is called once per frame
        void Update()
        {
            if (!lockAllUIInteractions)
            {
                if (scrollCursorTimer > 0f)
                {
                    scrollCursorTimer -= Time.deltaTime;
                    if (scrollCursorTimer <= 0f)
                    {
                        SetCursorSprite(cursorState);
                    }
                }

                bool untargetedThisFrame = false;
                if (inputSystem.hasInputTarget && inputSystem.inputTarget.mouseInputEnabled)
                {
                    MouseTargetDeselectMode deselectMode = inputSystem.inputTarget.mouseTarget.deselectMode;
                    if ((deselectMode == MouseTargetDeselectMode.Unclick && IsUnclicked(inputSystem.inputTarget.mouseTarget.buttonTargetedWith)) ||
                        (deselectMode == MouseTargetDeselectMode.ClickAgain && IsClicked(inputSystem.inputTarget.mouseTarget.buttonTargetedWith)))
                    {
                        InputTarget target = inputSystem.Untarget();
                        if (target)
                        {
                            untargetedThisFrame = true;
                        }
                    }
                    else if (IsUnclicked(inputSystem.inputTarget.mouseTarget.buttonTargetedWith))
                    {
                        /*if (hoverTarget && !hoverTarget.mouseTarget.selected)
                    {
                        canInteract = !inputSystem.inputTarget.disableMouseWhenSelected;
                        hoverTarget.mouseTarget.Select();
                        SetCursorState(hoverTarget.cursorSelected);
                    }*/
                    
                        if (!inputSystem.inputTarget.mouseTarget.selected)
                        {
                            canInteract = !inputSystem.inputTarget.disableMouseWhenSelected;
                            inputSystem.inputTarget.mouseTarget.Select();
                            SetCursorState(inputSystem.inputTarget.cursorSelected);
                        }

                        inputSystem.inputTarget.mouseTarget.Idle();
                    }
                }

                hoverTarget = null;
                hoverTriggers = new List<InputTarget>();

                if (canInteract)
                {
                    Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

                    if (hits.Length > 0)
                    {
                        foreach (Collider2D hit in hits)
                        {
                            InputTarget target = hit.transform.GetComponent<InputTarget>();

                            if (target && uiManager.CanTargetInputTarget(target))
                            {
                                UIElement uiElement = target.uiElement;

                                if ((!target.viewport || target.viewport.collider.OverlapPoint(worldPos)) && (!uiElement || !uiElement.viewport || uiElement.viewport.collider.OverlapPoint(worldPos)))
                                {
                                    if (target.mouseInputEnabled)
                                    {
                                        if (target.isHoverTrigger)
                                        {
                                            hoverTriggers.Add(target);

                                            if (target.mouseTarget.state != MouseTargetState.Hover)
                                            {
                                                target.mouseTarget.Hover();
                                            }
                                        }

                                        if (!hoverTarget)
                                        {
                                            hoverTarget = target;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (hoverTarget)
                    {
                        bool click = (hoverTarget.mouseTarget.allowLeftClick && leftClick) || (hoverTarget.mouseTarget.allowRightClick && rightClick) ||
                                     (hoverTarget.mouseTarget.allowMiddleClick && middleClick);

                        if (click && hoverTarget.mouseTarget.state != MouseTargetState.Pressed && !untargetedThisFrame)
                        {
                            SetCursorState(hoverTarget.cursorPress);
                            if (inputSystem.Target(hoverTarget))
                            {
                                if (hoverTarget.mouseTarget.allowLeftClick && leftClick)
                                {
                                    hoverTarget.mouseTarget.buttonTargetedWith = MouseButton.Left;
                                }
                                else if (hoverTarget.mouseTarget.allowRightClick && rightClick)
                                {
                                    hoverTarget.mouseTarget.buttonTargetedWith = MouseButton.Right;
                                }
                                else if (hoverTarget.mouseTarget.allowMiddleClick && middleClick)
                                {
                                    hoverTarget.mouseTarget.buttonTargetedWith = MouseButton.Middle;
                                }

                                hoverTarget.mouseTarget.Press();
                            }
                        }
                        else if (hoverTarget.mouseTarget.state != MouseTargetState.Hover)
                        {
                            hoverTarget.mouseTarget.Hover();
                            SetCursorState(hoverTarget.cursorHover);
                        }
                    }

                    if (scrollDelta != 0f)
                    {
                        onScroll.Invoke();
                        if (inputSystem.inputTarget != null && inputSystem.inputTarget.allowScroll)
                        {
                            inputSystem.inputTarget.mouseTarget.Scroll();
                            SetCursorSprite(inputSystem.inputTarget.mouseTarget.cursorScroll);
                            scrollCursorTimer = scrollCursorDisplayTime;
                        }
                    }

                    if (previousHoverTarget != hoverTarget)
                    {
                        if (hoverTarget == null)
                        {
                            SetCursorState(CursorState.Unspecified);
                        }
                        if (previousHoverTarget != null)
                        {
                            previousHoverTarget.mouseTarget.Idle();
                        }
                    }

                    foreach (InputTarget hoverTrigger in previousHoverTriggers)
                    {
                        if (!hoverTriggers.Contains(hoverTrigger))
                        {
                            hoverTrigger.mouseTarget.Idle();
                        }
                    }

                    previousHoverTarget = hoverTarget;
                    previousHoverTriggers = new List<InputTarget>(hoverTriggers);
                }
            }

            if (click)
            {
                onClick.Invoke();
            }
            if (leftClick)
            {
                onLeftClick.Invoke();
            }
            if (rightClick)
            {
                onRightClick.Invoke();
            }
            if (middleClick)
            {
                onMiddleClick.Invoke();
            }
            if (unclick)
            {
                onUnclick.Invoke();
            }
            if (leftUnclick)
            {
                onLeftUnclick.Invoke();
            }
            if (rightUnclick)
            {
                onRightUnclick.Invoke();
            }
            if (middleUnclick)
            {
                onMiddleUnclick.Invoke();
            }
        }

        public void SetCursorState(CursorState cursorState)
        {
            this.cursorState = cursorState;
            SetCursorSprite(cursorState);
        }
        public void SetCursorSprite(CursorState cursorState)
        {
            if (cursorState == CursorState.Invisible)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;

                if (cursorState == CursorState.Unspecified)
                {
                    SetCursor(cursorSpriteNormal, cursorHotSpotNormal);
                }
                else if (cursorState == CursorState.CurrentTool)
                {
                    if (toolbar.selectedTool == Tool.Pencil) { SetCursor(cursorSpritePencil, cursorHotSpotPencil); }
                    else if (toolbar.selectedTool == Tool.Brush) { SetCursor(cursorSpriteBrush, cursorHotSpotBrush); }
                    else if (toolbar.selectedTool == Tool.Rubber) { SetCursor(cursorSpriteRubber, cursorHotSpotRubber); }
                    else if (toolbar.selectedTool == Tool.EyeDropper) { SetCursor(cursorSpriteEyeDropper, cursorHotSpotEyeDropper); }
                    else if (toolbar.selectedTool == Tool.Fill) { SetCursor(cursorSpriteFill, cursorHotSpotFill); }
                    else if (toolbar.selectedTool == Tool.Shape)
                    {
                        Tuple<Texture2D, Vector2> shapeCursor = ShapeCursor(toolbar.shapeToolShape);
                        SetCursor(shapeCursor.Item1, shapeCursor.Item2);
                    }
                    else if (toolbar.selectedTool == Tool.IsoBox) { SetCursor(cursorSpriteIsoBox, cursorHotSpotIsoBox); }
                    else if (toolbar.selectedTool == Tool.Move) { SetCursor(cursorSpriteCrossArrows, cursorHotSpotCrossArrows); }
                    else if (toolbar.selectedTool == Tool.Selection)
                    {
                        Tuple<Texture2D, Vector2> selectionCursor = SelectionCursor(toolbar.selectionMode);
                        SetCursor(selectionCursor.Item1, selectionCursor.Item2);
                    }
                    else if (toolbar.selectedTool == Tool.Gradient)
                    {
                        Tuple<Texture2D, Vector2> gradientCursor = GradientCursor(toolbar.gradientMode);
                        SetCursor(gradientCursor.Item1, gradientCursor.Item2);
                    }
                    else { SetCursor(cursorSpriteNormal, cursorHotSpotNormal); }
                }
                else if (cursorState == CursorState.Normal) { SetCursor(cursorSpriteNormal, cursorHotSpotNormal); }
                else if (cursorState == CursorState.Hover) { SetCursor(cursorSpriteHover, cursorHotSpotHover); }
                else if (cursorState == CursorState.Press) { SetCursor(cursorSpritePress, cursorHotSpotPress); }
                else if (cursorState == CursorState.Grab) { SetCursor(cursorSpriteGrab, cursorHotSpotGrab); }
                else if (cursorState == CursorState.EyeDropper) { SetCursor(cursorSpriteEyeDropper, cursorHotSpotEyeDropper); }
                else if (cursorState == CursorState.Text) { SetCursor(cursorSpriteText, cursorHotSpotText); }
                else if (cursorState == CursorState.CrossArrows) { SetCursor(cursorSpriteCrossArrows, cursorHotSpotCrossArrows); }
                else if (cursorState == CursorState.UpDownArrow) { SetCursor(cursorSpriteUpDownArrow, cursorHotSpotUpDownArrow); }
                else if (cursorState == CursorState.LeftRightArrow) { SetCursor(cursorSpriteLeftRightArrow, cursorHotSpotLeftRightArrow); }
                else if (cursorState == CursorState.MagnifyingGlass) { SetCursor(cursorSpriteMagnifyingGlass, cursorHotSpotMagnifyingGlass); }
                else
                {
                    throw new System.Exception("Unknown / unimplemented cursor state: " + cursorState.ToString());
                }

                scrollCursorTimer = 0f;
            }
        }

        private void SetCursor(Texture2D cursor, Vector2 hotSpot)
        {
            Cursor.SetCursor(cursor, hotSpot * (float)CURSOR_SCALED_RESOLUTION / CURSOR_RESOLUTION, CursorMode.Auto);
        }

        private Tuple<Texture2D, Vector2> ShapeCursor(Shape shape)
        {
            if (shape == Shape.Rectangle)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteRectangle, cursorHotSpotRectangle);
            }
            else if (shape == Shape.Ellipse)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteEllipse, cursorHotSpotEllipse);
            }
            else if (shape == Shape.RightTriangle)
            {
                if (inputSystem.globalKeyboardTarget.IsHeldExactly(KeyCode.LeftControl))
                {
                    return new Tuple<Texture2D, Vector2>(cursorSpriteTriangleUpsideDown, cursorHotSpotTriangleUpsideDown);
                }
                else
                {
                    return new Tuple<Texture2D, Vector2>(cursorSpriteTriangle, cursorHotSpotTriangle);
                }
            }
            else if (shape == Shape.Diamond)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteDiamond, cursorHotSpotDiamond);
            }
            else
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteNormal, cursorHotSpotNormal);
            }
        }

        private Tuple<Texture2D, Vector2> SelectionCursor(SelectionMode selectionMode)
        {
            if (selectionMode == SelectionMode.Draw)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpritePencil, cursorHotSpotPencil);
            }
            else if (selectionMode == SelectionMode.MagicWand)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteMagicWand, cursorHotSpotMagicWand);
            }
            else if (selectionMode == SelectionMode.Rectangle)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteSelectionRectangle, cursorHotSpotSelectionRectangle);
            }
            else if (selectionMode == SelectionMode.Ellipse)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteSelectionEllipse, cursorHotSpotSelectionEllipse);
            }
            else
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteNormal, cursorHotSpotNormal);
            }
        }

        private Tuple<Texture2D, Vector2> GradientCursor(GradientMode gradientMode)
        {
            if (gradientMode == GradientMode.Linear)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteLinearGradient, cursorHotSpotLinearGradient);
            }
            else if (gradientMode == GradientMode.Radial)
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteRadialGradient, cursorHotSpotRadialGradient);
            }
            else
            {
                return new Tuple<Texture2D, Vector2>(cursorSpriteNormal, cursorHotSpotNormal);
            }
        }

        public bool IsClicked(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left: return leftClick;
                case MouseButton.Right: return rightClick;
                case MouseButton.Middle: return middleClick;
                default: throw new System.Exception("Unknown / unimplemented mouse button: " + mouseButton);
            }
        }
        public bool IsUnclicked(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left: return leftUnclick;
                case MouseButton.Right: return rightUnclick;
                case MouseButton.Middle: return middleUnclick;
                default: throw new System.Exception("Unknown / unimplemented mouse button: " + mouseButton);
            }
        }
        public bool IsHeld(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left: return leftHold;
                case MouseButton.Right: return rightHold;
                case MouseButton.Middle: return middleHold;
                default: throw new System.Exception("Unknown / unimplemented mouse button: " + mouseButton);
            }
        }

        public void SubscribeToScroll(UnityAction call)
        {
            onScroll.AddListener(call);
        }

        public void SubscribeToClick(UnityAction call)
        {
            onClick.AddListener(call);
        }
        public void SubscribeToLeftClick(UnityAction call)
        {
            onLeftClick.AddListener(call);
        }
        public void SubscribeToRightClick(UnityAction call)
        {
            onRightClick.AddListener(call);
        }
        public void SubscribeToMiddleClick(UnityAction call)
        {
            onMiddleClick.AddListener(call);
        }
        public void SubscribeToUnclick(UnityAction call)
        {
            onUnclick.AddListener(call);
        }
        public void SubscribeToLeftUnclick(UnityAction call)
        {
            onLeftUnclick.AddListener(call);
        }
        public void SubscribeToRightUnclick(UnityAction call)
        {
            onRightUnclick.AddListener(call);
        }
        public void SubscribeToMiddleUnclick(UnityAction call)
        {
            onMiddleUnclick.AddListener(call);
        }
    }
}