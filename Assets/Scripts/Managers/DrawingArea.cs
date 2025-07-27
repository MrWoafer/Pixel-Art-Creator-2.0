using System.Collections.Generic;
using PAC.Colour;
using PAC.Input;
using PAC.KeyboardShortcuts;
using PAC.Tilesets;
using PAC.UndoRedo;
using UnityEngine;
using PAC.Patterns;

using System;
using PAC.Geometry.Shapes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.ImageEditing;
using PAC.Colour.Compositing;
using PAC.Geometry.Shapes.Extensions;
using PAC.Patterns.Extensions;
using PAC.Geometry;
using PAC.Geometry.Extensions;
using PAC.Extensions.UnityEngine;
using PAC.UI.Components.Specialised.ColourPicker;
using PAC.Image;
using PAC.Image.Layers;
using PAC.Tools;
using PAC.Tools.Interfaces;

namespace PAC.Managers
{
    public class DrawingArea : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float _scrollSpeed = 1f;
        public float scrollSpeed { get => _scrollSpeed; private set => _scrollSpeed = value; }
        [SerializeField]
        private float _zoomScrollSpeed = 1f;
        public float zoomScrollSpeed { get => _zoomScrollSpeed; private set => _zoomScrollSpeed = value; }

        public File file => fileManager.currentFile;
        private Layer selectedLayer => layerManager.selectedLayer;
        private int selectedLayerIndex => layerManager.selectedLayerIndex;
        private int currentFrameIndex => animationManager.currentFrameIndex;

        // Game object references
        private SpriteRenderer drawingSprRen;
        private SpriteRenderer backgroundSprRen;
        private SpriteRenderer previewSprRen;
        private SpriteMask drawingAreaMask;
        private BoxCollider2D collider;

        // Input system references
        private InputTarget inputTarget;
        private Mouse mouse;

        public float pixelsPerUnit => Mathf.Max(file.width, file.height);

        // Manager references
        private InputSystem inputSystem;
        private FileManager fileManager;
        private LayerManager layerManager;
        private AnimationManager animationManager;
        private GlobalColourPicker colourPicker;
        private Toolbar toolbar;
        private ImageEditManager imageEditManager;
        private TileOutlineManager tileOutlineManager;
        private TilesetManager tilesetManager;

        // Mouse variables
        /// <summary>The pixel the mouse is currently on.</summary>
        private IntVector2 mousePixel => WorldPosToPixel(mouse.worldPos);
        private readonly IntVector2 mouseDragInactiveCoords = new IntVector2(-1, -1);
        /// <summary>For tools that involve dragging, these are the pixels the mouse dragged from/to, in order.</summary>
        private List<IntVector2> mouseDragPoints = new List<IntVector2>();
        private IntVector2 previousPixelUsedToolOn = new IntVector2(-1, -1);
        /// <summary>The pixel the mouse was on before the current pixel.</summary>
        private IntVector2 previousMousePixel = new IntVector2(-1, -1);
        private bool mouseOutsideDrawingAreaLastFrame = false;
        private bool leftClickedOn => inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Left;
        private bool rightClickedOn => inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Right;
        private bool middleClickedOn => inputTarget.mouseTarget.buttonTargetedWith == MouseButton.Middle;

        // Keyboard variables
        private bool holdingCtrl => inputTarget.keyboardTarget.IsHeldExactly(CustomKeyCode.Ctrl);

        // View variables
        private Vector3 resetViewPosition;
        private Vector3 resetViewLocalScale;
        /// <summary>The difference between the drawing area's position and the mouse's world position when first starting to move the view.</summary>
        private Vector2 moveOffsetFromMouse;

        // Tool tracker variables
        private Color previousColourUsedForTool;
        private MouseButton previousMouseButtonUsedForTool;
        private Tool previousToolUsed = new NoneTool();
        private bool hasUnclickedSinceUsingTool = true;
        private bool hasUsedKeyboardSinceUsingTool = false;
        private bool deselectedSelectionThisFrame = false;
        private bool finishedUsingTool = true;
        private bool toolCancelled = false;

        // Brush border variables
        private SpriteRenderer brushBorderSprRen;

        // Selection tool variables
        public Texture2D selectionMask
        {
            get => selectionSprRen.sprite.texture;
            private set => selectionSprRen.sprite = value.ToSprite();
        }
        private SpriteRenderer selectionSprRen;
        public bool hasSelection
        {
            get
            {
                for (int x = 0; x < selectionMask.width; x++)
                {
                    for (int y = 0; y < selectionMask.height; y++)
                    {
                        if (selectionMask.GetPixel(x, y).a != 0f)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public IntRect selectionRect
        {
            get
            {
                int minX = -1;
                int minY = -1;
                int maxX = -1;
                int maxY = -1;

                for (int x = 0; x < selectionMask.width; x++)
                {
                    for (int y = 0; y < selectionMask.height; y++)
                    {
                        if (selectionMask.GetPixel(x, y).a != 0f)
                        {
                            if (minX == -1 || x < minX)
                            {
                                minX = x;
                            }
                            if (minY == -1 || y < minY)
                            {
                                minY = y;
                            }
                            if (x > maxX)
                            {
                                maxX = x;
                            }
                            if (y > maxY)
                            {
                                maxY = y;
                            }
                        }
                    }
                }

                return new IntRect(new IntVector2(minX, minY), new IntVector2(maxX, maxY));
            }
        }
        private Texture2D selectionTexture;
        private Texture2D selectionMaskCopy;

        // Moving tiles variables
        private Tile previousTileHoveredOver = null;
        private Tile tileBeingMoved = null;
        private IntVector2 tileBeingMovedOriginalBottomLeft;
        private IntVector2 tileBeingMovedLastValidPosition;

        // Line smoothing variables
        private Geometry.Shapes.Line lineSmoothingPreviousLine = null;
        private bool lineSmoothingPreviousLineWasSmoothed = false;
        private Color lineSmoothingColourUnderLineEnd = Config.Colours.transparent;
        private float lineSmoothingCountdown = 0f;

        // Iso box tool variables
        private bool isoBoxPlacedBase = false;

        void Awake()
        {
            drawingSprRen = transform.Find("Drawing").GetComponent<SpriteRenderer>();
            backgroundSprRen = transform.Find("Background").GetComponent<SpriteRenderer>();
            previewSprRen = transform.Find("Preview").GetComponent<SpriteRenderer>();
            drawingAreaMask = transform.Find("Mask").GetComponent<SpriteMask>();

            inputSystem = Finder.inputSystem;
            layerManager = Finder.layerManager;
            fileManager = Finder.fileManager;
            imageEditManager = Finder.imageEditManager;
            animationManager = Finder.animationManager;
            tilesetManager = Finder.tilesetManager;

            inputTarget = GetComponent<InputTarget>();
            mouse = Finder.mouse;

            collider = GetComponent<BoxCollider2D>();

            colourPicker = Finder.colourPicker;
            toolbar = Finder.toolbar;

            selectionSprRen = transform.Find("Selection").GetComponent<SpriteRenderer>();
            brushBorderSprRen = transform.Find("Brush Border").GetComponent<SpriteRenderer>();
            tileOutlineManager = Finder.tileOutlineManager;

            fileManager.SubscribeToFileSwitched(InitialiseDisplay);
            layerManager.SubscribeToLayerChange(UpdateDrawing);

            resetViewPosition = transform.position;
            resetViewLocalScale = transform.localScale;
        }

        private void Start()
        {
            inputTarget.mouseTarget.SubscribeToStateChange(OnMouseInput);
            inputTarget.mouseTarget.SubscribeToHover(OnMousePixelChanged);
            inputTarget.keyboardTarget.SubscribeToOnInput(OnKeyboardInput);
            inputSystem.globalMouseTarget.SubscribeToScroll(OnMouseScroll);
            inputSystem.SubscribeToGlobalKeyboard(OnGlobalKeyboardInput);

            imageEditManager.SubscribeToEdit(UpdateDrawing);
            imageEditManager.SubscribeToImageSizeChanged(InitialiseDisplay);
            fileManager.SubscribeToFileSwitched(ResetView);

            toolbar.SubscribeToOnToolChanged(OnToolChanged);
            toolbar.SubscribeToOnBrushPixelsChanged(() => UpdateBrushBorder(mousePixel));

            animationManager.SubscribeToOnCurrentFrameIndexChange(UpdateDrawing);
            animationManager.SubscribeToOnKeyFrameDeleted(UpdateDrawing);

            tilesetManager.SubscribeToOnTileIconSelected((tileFile) => OnTileIconSelected(tileFile));

            InitialiseDisplay();

            UpdateBrushBorder(mousePixel);
        }

        // Update is called once per frame
        void Update()
        {
            if (mousePixel != previousMousePixel)
            {
                OnMousePixelChanged();
                previousMousePixel = mousePixel;
            }

            // Use selected tool
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed && !toolCancelled && !layerManager.selectedLayer.locked)
            {
                Color colour = colourPicker.colour;

                bool canUseTool = previousPixelUsedToolOn != mousePixel || previousColourUsedForTool != colour || previousMouseButtonUsedForTool != inputTarget.mouseTarget.buttonTargetedWith ||
                                  previousToolUsed != toolbar.selectedTool || inputTarget.keyboardTarget.inputThisFrame || deselectedSelectionThisFrame || hasUnclickedSinceUsingTool || hasUsedKeyboardSinceUsingTool;
                if (canUseTool)
                {
                    if (file.rect.Contains(mousePixel))
                    {
                        if (mouse.click || mouseOutsideDrawingAreaLastFrame || !toolbar.selectedTool.useMovementInterpolation)
                        {
                            HoldClickTool(toolbar.selectedTool, mousePixel, selectedLayerIndex, currentFrameIndex, colour);
                        }
                        else
                        {
                            // Mouse movement interpolation
                            Line line = new Line(previousPixelUsedToolOn, mousePixel);
                            Color colourUnderLineEnd = selectedLayer.GetPixel(line.end, animationManager.currentFrameIndex);
                            foreach (IntVector2 pixel in line)
                            {
                                HoldClickTool(toolbar.selectedTool, pixel, selectedLayerIndex, currentFrameIndex, colour);
                            }

                            // Pencil line smoothing
                            bool smoothed = false;
                            if (toolbar.selectedTool is PencilTool)
                            {
                                smoothed = Tools.Tools.PencilLineSmoothing(line, lineSmoothingPreviousLine, lineSmoothingPreviousLineWasSmoothed);

                                if (smoothed)
                                {
                                    file.layers[selectedLayerIndex].SetPixel(line.start, currentFrameIndex, lineSmoothingColourUnderLineEnd, AnimFrameRefMode.NewKeyFrame);
                                }
                            }
                            lineSmoothingPreviousLine = line;
                            lineSmoothingPreviousLineWasSmoothed = smoothed;
                            lineSmoothingColourUnderLineEnd = colourUnderLineEnd;
                        }

                        UpdateDrawing();

                        // Update tracker variables
                        previousPixelUsedToolOn = mousePixel;
                        previousColourUsedForTool = colour;
                        previousMouseButtonUsedForTool = inputTarget.mouseTarget.buttonTargetedWith;
                        previousToolUsed = toolbar.selectedTool;

                        hasUnclickedSinceUsingTool = false;
                        hasUsedKeyboardSinceUsingTool = false;
                        deselectedSelectionThisFrame = false;
                    }
                }
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Hover && !layerManager.selectedLayer.locked)
            {
                HoverTool(toolbar.selectedTool, mousePixel, selectedLayerIndex, currentFrameIndex, colourPicker.colour);
            }

            // Moving view
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed && middleClickedOn)
            {
                transform.position = (Vector3)(mouse.worldPos + moveOffsetFromMouse) + Vector3.forward * transform.position.z;
                mouse.SetCursorSprite(CursorState.Grab);
            }

            // Update tracker variables

            if (file.rect.Contains(mousePixel))
            {
                mouseOutsideDrawingAreaLastFrame = false;
            }
            else
            {
                mouseOutsideDrawingAreaLastFrame = true;
            }

            if (inputTarget.mouseTarget.state != MouseTargetState.Pressed && !hasUnclickedSinceUsingTool)
            {
                hasUnclickedSinceUsingTool = true;
                lineSmoothingPreviousLine = null;
            }

            // Stop line smoothing from happening if it has been too long since the last pixel was painted
            if (lineSmoothingPreviousLine is not null)
            {
                lineSmoothingCountdown -= Time.deltaTime;

                if (lineSmoothingCountdown <= 0f)
                {
                    lineSmoothingPreviousLine = null;
                    lineSmoothingCountdown = toolbar.PencilTool.lineSmoothingTime;
                }
            }
        }

        private void InitialiseDisplay()
        {
            drawingSprRen.sprite = file.liveRender.ToSprite();
            backgroundSprRen.sprite = Texture2DCreator.TransparentCheckerboardBackground(file.width, file.height).ToSprite();
            //backgroundSprRen.material.SetFloat("_Width", 2f * file.width);
            //backgroundSprRen.material.SetFloat("_Height", 2f * file.height);
            previewSprRen.sprite = Texture2DExtensions.Transparent(1, 1).ToSprite();
            selectionMask = Texture2DExtensions.Transparent(file.width, file.height);

            collider.size = new Vector2(file.width / pixelsPerUnit, file.height / pixelsPerUnit);
            drawingAreaMask.transform.localScale = new Vector3(file.width / pixelsPerUnit, file.height / pixelsPerUnit, 1f);

            UpdateBrushBorder(mousePixel);

            UpdateDrawing();
            tileOutlineManager.RefreshTileOutlines();
        }

        /// <summary>
        /// Makes any changes to the file visible on the drawing area.
        /// </summary>
        public void UpdateDrawing()
        {
            file.SetLiveRenderFrame(animationManager.currentFrameIndex);
            file.liveRender.Apply();
        }

        private void SetPreview(Texture2D texture, IntVector2 pixel) => SetPreview(texture, pixel.x, pixel.y);
        private void SetPreview(Texture2D texture, int x, int y)
        {
            previewSprRen.sprite = texture.ToSprite();
            previewSprRen.transform.localScale = Vector3.one * Mathf.Max(texture.width, texture.height) / pixelsPerUnit;

            SetPreviewPosition(x, y);
        }

        private void SetPreviewPosition(IntVector2 pixel) => SetPreviewPosition(pixel.x, pixel.y);
        private void SetPreviewPosition(int x, int y)
        {
            previewSprRen.transform.position = PixelsToWorldPos(x + previewSprRen.sprite.texture.width / 2f, y + previewSprRen.sprite.texture.height / 2f);
        }

        private void ClearPreview()
        {
            SetPreview(Texture2DExtensions.Transparent(1, 1), 0, 0);
        }

        private void HideBrushBorder()
        {
            if (brushBorderSprRen.enabled)
            {
                brushBorderSprRen.enabled = false;
            }
        }
        private void ShowBrushBorder()
        {
            if (!brushBorderSprRen.enabled)
            {
                brushBorderSprRen.enabled = true;
            }
        }
        private void UpdateBrushBorder(IntVector2 pixel)
        {
            brushBorderSprRen.sprite = toolbar.brushTexture.ToSprite();

            float scaleFactor = Mathf.Max(toolbar.brushTexture.width, toolbar.brushTexture.height) / pixelsPerUnit;
            brushBorderSprRen.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            brushBorderSprRen.transform.position = PixelsToWorldPos(pixel);
            if (toolbar.brushTexture.width % 2 == 0)
            {
                brushBorderSprRen.transform.localPosition += new Vector3(0.5f / pixelsPerUnit, 0f, 0f);
            }
            if (toolbar.brushTexture.height % 2 == 0)
            {
                brushBorderSprRen.transform.localPosition += new Vector3(0f, 0.5f / pixelsPerUnit, 0f);
            }

            // When the brush is a single pixel, make the border black/white depending on the lightness of the pixel below
            Color outlineColour = Config.Colours.brushOutlineDark;
            if (toolbar.brushPixelsIsSingleCentralPixel && file.liveRender.GetPixel(pixel).a != 0f && ((HSLA)file.liveRender.GetPixel(pixel)).l < 0.5f)
            {
                outlineColour = Config.Colours.brushOutlineLight;
            }
            brushBorderSprRen.GetComponent<Shaders.Outline>().colour = outlineColour;
        }

        /// <summary>
        /// Zooms with the focus point being the centre of the screen.
        /// </summary>
        private void ZoomCentreOfScreen(float amount) => Zoom(amount, Vector2.zero);
        /// <summary>
        /// Zooms the view in/out.
        /// </summary>
        /// <param name="amount">Positive value means zooming in.</param>
        /// <param name="focusPoint">The world point that will not move on-screen when zooming.</param>
        private void Zoom(float amount, Vector2 focusPoint)
        {
            Vector2 zoomPointLocalCoords = transform.InverseTransformPoint(focusPoint);

            transform.localScale = transform.localScale + Vector3.one * amount;
            if (transform.localScale.x < 1f)
            {
                transform.localScale = Vector3.one;
            }

            transform.position = transform.position + (Vector3)(focusPoint - (Vector2)transform.TransformPoint(zoomPointLocalCoords));
        }

        /// <summary>
        /// Scrolls the view horizontally.
        /// </summary>
        /// <param name="amount">Positive means scrolling to the right.</param>
        private void ScrollViewX(float amount) => ScrollView(amount, 0);
        /// <summary>
        /// Scrolls the view vertically.
        /// </summary>
        /// <param name="amount">Positive means scrolling up.</param>
        private void ScrollViewY(float amount) => ScrollView(0f, amount);
        /// <param name="amount">Positive x means scrolling to the right. Positive y means scrolling up.</param>
        private void ScrollView(Vector2 amount) => ScrollView(amount.x, amount.y);
        /// <param name="amountX">Positive means scrolling to the right.</param>
        /// <param name="amountY">Positive means scrolling up.</param>
        private void ScrollView(float amountX, float amountY)
        {
            transform.localPosition = transform.localPosition + new Vector3(-amountX, -amountY, 0f);
        }

        /// <summary>
        /// Resets the position and scale of the drawing area.
        /// </summary>
        private void ResetView()
        {
            transform.position = resetViewPosition;
            transform.localScale = resetViewLocalScale;
        }

        /// <summary>
        /// Called when the drawing area is hovered over/off or clicked on/unclicked.
        /// </summary>
        private void OnMouseInput()
        {
            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                if (finishedUsingTool)
                {
                    mouseDragPoints.Clear();
                }
                mouseDragPoints.Add(mousePixel);

                if (middleClickedOn)
                {
                    moveOffsetFromMouse = (Vector2)transform.position - mouse.worldPos;
                }
                else
                {
                    /*if (toolbar.selectedTool != Tool.EyeDropper && toolbar.selectedTool != Tool.Selection)
                {
                    undoRedo.AddUndoState(new UndoRedoState(UndoRedoAction.Draw, file.layers[layerManager.selectedLayerIndex], layerManager.selectedLayerIndex), fileManager.currentFileIndex);
                }*/

                    /*if (toolbar.selectedTool == Tool.Move && !layerManager.selectedLayer.locked)
                {
                    if (hasSelection)
                    {
                        selectionTexture = Tex2DSprite.ApplyMask(layerManager.selectedLayer[animationManager.currentFrameIndex].texture, selectionMask);
                        DeleteSelection();
                    }
                    else
                    {
                        selectionTexture = Tex2DSprite.Copy(layerManager.selectedLayer[animationManager.currentFrameIndex].texture);
                        layerManager.selectedLayer.ClearFrames();
                    }

                    selectionMaskCopy = Tex2DSprite.Copy(selectionMask);
                    UpdateDrawingMoveTool(WorldPosToPixels(mouse.worldPos));
                }*/

                    if (toolbar.selectedTool is MoveTool && !selectedLayer.locked)
                    {
                        if (selectedLayer.layerType == LayerType.Tile)
                        {
                            tileBeingMoved = ((TileLayer)selectedLayer).PixelToTile(mouseDragPoints[0]);

                            if (tileBeingMoved != null)
                            {
                                tileBeingMovedOriginalBottomLeft = tileBeingMoved.bottomLeft;
                                tileBeingMovedLastValidPosition = tileBeingMoved.bottomLeft;

                                SetPreview(tileBeingMoved.file.liveRender.Applied(), tileBeingMoved.bottomLeft);

                                file.RemoveTile(tileBeingMoved);
                            }
                        }
                    }
                }
            }
            else if (inputTarget.mouseTarget.state == MouseTargetState.Idle && mouse.unclick && !finishedUsingTool && !selectedLayer.locked && !hasUnclickedSinceUsingTool)
            {
                UnclickTool(toolbar.selectedTool, previousPixelUsedToolOn, selectedLayerIndex, currentFrameIndex, colourPicker.colour);
                ClearPreview();
                //mouseDragPoints[0] = mouseDragInactiveCoords;

                if (toolbar.selectedTool.finishMode == MouseTargetDeselectMode.Unclick)
                {
                    finishedUsingTool = true;
                }
                else
                {
                    mouseDragPoints.Add(mousePixel);
                }
            }

            toolCancelled = false;
        }

        /// <summary>
        /// Called when the mouse scrollwheel is used.
        /// </summary>
        private void OnMouseScroll()
        {
            if ((mouse.hoverTarget == null || mouse.hoverTarget == inputTarget) && !mouse.hasHoverTrigger)
            {
                if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("scroll y")))
                {
                    inputSystem.inputTarget.cursorScroll = CursorState.UpDownArrow;
                    ScrollViewY(mouse.scrollDelta * scrollSpeed);
                }
                else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("scroll x")))
                {
                    inputSystem.inputTarget.cursorScroll = CursorState.LeftRightArrow;
                    ScrollViewX(mouse.scrollDelta * scrollSpeed);
                }
                else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("scroll brush size")))
                {
                    inputSystem.inputTarget.cursorScroll = CursorState.CurrentTool;
                }
                else
                {
                    inputSystem.inputTarget.cursorScroll = CursorState.MagnifyingGlass;
                    Zoom(mouse.scrollDelta * zoomScrollSpeed, mouse.worldPos);
                }
            }
        }

        /// <summary>
        /// Called when the mouse moves over a new pixel.
        /// </summary>
        private void OnMousePixelChanged()
        {
            if (file.rect.Contains(mousePixel) && !selectedLayer.locked)
            {
                // This check is so the brush border doesn't display when using pop-up windows.
                if (inputTarget.mouseTarget.state != MouseTargetState.Idle)
                {
                    if (selectedLayer.layerType == LayerType.Tile)
                    {
                        Tile tile = ((TileLayer)selectedLayer).PixelToTile(mousePixel);
                        if (tile != null)
                        {
                            tileOutlineManager.ShowTileOutline(tile);
                        }

                        if (previousTileHoveredOver != null && tile != previousTileHoveredOver)
                        {
                            tileOutlineManager.HideTileOutline(previousTileHoveredOver);
                        }

                        previousTileHoveredOver = tile;
                    }
                    else if (previousTileHoveredOver != null)
                    {
                        tileOutlineManager.HideTileOutline(previousTileHoveredOver);
                        previousTileHoveredOver = null;
                    }

                    ShowBrushBorder();
                }
                else
                {
                    HideBrushBorder();
                }

                UpdateBrushBorder(mousePixel);
            }
            else
            {
                HideBrushBorder();

                if (previousTileHoveredOver != null)
                {
                    tileOutlineManager.HideTileOutline(previousTileHoveredOver);
                    previousTileHoveredOver = null;
                }
            }
        }

        /// <summary>
        /// Called when a key is pressed/unpressed and the drawing area is the current input target.
        /// </summary>
        private void OnKeyboardInput()
        {
            if (inputTarget.keyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("cancel tool")) && toolbar.selectedTool.canBeCancelled)
            {
                toolCancelled = true;
                ClearPreview();
                finishedUsingTool = true;

                if (toolbar.selectedTool is IsometricCuboidTool)
                {
                    isoBoxPlacedBase = false;
                    inputSystem.Untarget();
                }
            }

            hasUsedKeyboardSinceUsingTool = true;
        }

        /// <summary>
        /// Called when a key is pressed/unpressed, regardless of the input target.
        /// </summary>
        private void OnGlobalKeyboardInput()
        {
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("zoom in")))
            {
                Zoom(zoomScrollSpeed, Vector2.zero);
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("zoom out")))
            {
                Zoom(-zoomScrollSpeed, Vector2.zero);
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("reset view")))
            {
                ResetView();
            }
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.KeyboardShortcuts.GetShortcutsFor("clear selection")) && hasSelection)
            {
                DeselectSelection();
            }
            if (inputSystem.globalKeyboardTarget.IsPressed(KeyCode.Delete) && hasSelection)
            {
                DeleteSelection();
            }

            /// To be implemented
            if (toolbar.selectedTool is MoveTool && hasSelection)
            {
                if (inputSystem.globalKeyboardTarget.IsPressed(KeyCode.LeftArrow))
                {

                }
            }
        }

        /// <summary>
        /// Called when a new tool is selected.
        /// </summary>
        private void OnToolChanged()
        {
            mouseDragPoints.Clear();

            if (inputTarget.mouseTarget.state == MouseTargetState.Pressed)
            {
                mouseDragPoints.Add(mousePixel);
                ClearPreview();
            }

            finishedUsingTool = true;

            inputTarget.mouseTarget.deselectMode = toolbar.selectedTool.finishMode;
        }

        private void HoverTool(Tool tool, IntVector2 pixel, int layer, int frame, Color colour)
        {
            if (tool is IsometricCuboidTool)
            {
                if (isoBoxPlacedBase)
                {
                    PreviewShape(new IsometricCuboid(new IsometricRectangle(mouseDragPoints[0], mouseDragPoints[1], false), pixel.y - mouseDragPoints[1].y, rightClickedOn, holdingCtrl), colour);
                }
            }
        }

        private void HoldClickTool(Tool tool, IntVector2 pixel, int layer, int frame, Color colour)
        {
            finishedUsingTool = false;

            if (tool is PencilTool)
            {
                if (leftClickedOn) { Tools.Tools.UsePencil(file, layer, frame, pixel, colour); }
                else if (rightClickedOn) { Tools.Tools.UseRubber(file, layer, frame, pixel); }
            }
            else if (tool is BrushTool)
            {
                if (leftClickedOn) { Tools.Tools.UseBrush(file, layer, frame, pixel, toolbar.brushPixels, colour); }
                else if (rightClickedOn) { Tools.Tools.UseRubber(file, layer, frame, pixel, toolbar.brushPixels); }
            }
            else if (tool is RubberTool)
            {
                if (leftClickedOn || rightClickedOn) { Tools.Tools.UseRubber(file, layer, frame, pixel, toolbar.brushPixels); }
            }
            else if (tool is EyeDropperTool)
            {
                if (leftClickedOn) { colourPicker.SetColour(Tools.Tools.UseEyeDropper(file, layer, frame, pixel)); }
            }
            else if (tool is FillTool)
            {
                if (leftClickedOn) { Tools.Tools.UseFill(file, layer, frame, pixel, colour, toolbar.floodFillDiagonallyAdjacent); }
                else if (rightClickedOn) { Tools.Tools.UseFill(file, layer, frame, pixel, Config.Colours.transparent, toolbar.floodFillDiagonallyAdjacent); }
            }
            else if (tool is LineTool)
            {
                if (leftClickedOn)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToPerfectLine(mouseDragPoints[0], end);
                    }
                    PreviewShape(new Line(mouseDragPoints[0], end), colour);
                }
            }
            else if (tool is ShapeTool && (leftClickedOn || rightClickedOn))
            {
                if (toolbar.ShapeTool.shape == ShapeTool.Shape.Rectangle)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    PreviewShape(new Rectangle(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.Ellipse)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    PreviewShape(new Ellipse(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.RightTriangle)
                {
                    PreviewShape(new RightTriangle(mouseDragPoints[0], pixel, !holdingCtrl, rightClickedOn), colour);
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.Diamond)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    PreviewShape(new Diamond(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.IsometricHexagon)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    PreviewShape(new IsometricHexagon(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);
                }
            }
            else if (tool is IsometricCuboidTool && (leftClickedOn || rightClickedOn))
            {
                if (!isoBoxPlacedBase) { PreviewShape(new IsometricCuboid(new IsometricRectangle(mouseDragPoints[0], pixel, false), 0, rightClickedOn, holdingCtrl), colour); }
                else { PreviewShape(new IsometricCuboid(new IsometricRectangle(mouseDragPoints[0], mouseDragPoints[1], false), pixel.y - mouseDragPoints[1].y, rightClickedOn, holdingCtrl), colour); }
            }
            else if (tool is GradientTool && (leftClickedOn || rightClickedOn))
            {
                Color startColour = leftClickedOn ? colourPicker.primaryColour : colourPicker.secondaryColour;
                Color endColour = leftClickedOn ? colourPicker.secondaryColour : colourPicker.primaryColour;

                IPattern2D<Color> gradient = toolbar.GradientTool.mode switch
                {
                    GradientTool.Mode.Linear => new Patterns.Gradient.Linear((mouseDragPoints[0], startColour), (pixel, endColour)),
                    GradientTool.Mode.Radial => new Patterns.Gradient.Radial((mouseDragPoints[0], startColour), (pixel, endColour)),
                    _ => throw new InvalidOperationException($"Unknown / unimplemented gradient mode: {toolbar.GradientTool.mode}")
                };

                PreviewPattern(gradient);
            }
            else if (tool is SelectionTool && (leftClickedOn || rightClickedOn))
            {
                if (toolbar.selectionMode == SelectionTool.Mode.Draw)
                {
                    SelectionDraw(pixel, rightClickedOn);
                }
                else if (toolbar.selectionMode == SelectionTool.Mode.MagicWand)
                {
                    SelectionMagicWand(pixel, rightClickedOn, holdingCtrl);
                }
                else if (toolbar.selectionMode == SelectionTool.Mode.Rectangle)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    SelectionShape(new Rectangle(new IntRect(mouseDragPoints[0], end), true), rightClickedOn);
                }
                else if (toolbar.selectionMode == SelectionTool.Mode.Ellipse)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    SelectionShape(new Ellipse(new IntRect(mouseDragPoints[0], end), true), rightClickedOn);
                }
            }
            else if (tool is MoveTool)
            {
                if (leftClickedOn) { PreviewMove(pixel); }
            }
        }

        /// <summary>
        /// For tools that only enact changes to the image when you release the mouse (e.g. shape tool), this function enacts those changes.
        /// </summary>
        private void UnclickTool(Tool tool, IntVector2 pixel, int layer, int frame, Color colour)
        {
            if (tool is LineTool && leftClickedOn)
            {
                IntVector2 end = pixel;
                if (holdingCtrl)
                {
                    end = CoordSnapping.SnapToPerfectLine(mouseDragPoints[0], end);
                }
                Tools.Tools.UseShape(file, layer, frame, new Line(mouseDragPoints[0], end), colour);
                UpdateDrawing();
            }
            else if (tool is ShapeTool && (leftClickedOn || rightClickedOn))
            {
                if (toolbar.ShapeTool.shape == ShapeTool.Shape.Rectangle)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    Tools.Tools.UseShape(file, layer, frame, new Rectangle(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);

                    UpdateDrawing();
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.Ellipse)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    Tools.Tools.UseShape(file, layer, frame, new Ellipse(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);

                    UpdateDrawing();
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.RightTriangle)
                {
                    Tools.Tools.UseShape(file, layer, frame, new RightTriangle(mouseDragPoints[0], pixel, !holdingCtrl, rightClickedOn), colour);

                    UpdateDrawing();
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.Diamond)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    Tools.Tools.UseShape(file, layer, frame, new Diamond(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);

                    UpdateDrawing();
                }
                else if (toolbar.ShapeTool.shape == ShapeTool.Shape.IsometricHexagon)
                {
                    IntVector2 end = pixel;
                    if (holdingCtrl)
                    {
                        end = CoordSnapping.SnapToSquare(mouseDragPoints[0], end);
                    }
                    Tools.Tools.UseShape(file, layer, frame, new IsometricHexagon(new IntRect(mouseDragPoints[0], end), rightClickedOn), colour);

                    UpdateDrawing();
                }
            }
            else if (tool is IsometricCuboidTool && (leftClickedOn || rightClickedOn))
            {
                if (!isoBoxPlacedBase)
                {
                    isoBoxPlacedBase = true;
                }
                else
                {
                    Tools.Tools.UseShape(file, layer, frame,
                        new IsometricCuboid(new IsometricRectangle(mouseDragPoints[0], mouseDragPoints[1], false), pixel.y - mouseDragPoints[1].y, rightClickedOn, holdingCtrl), colour
                        );
                    UpdateDrawing();

                    isoBoxPlacedBase = false;
                    finishedUsingTool = true;
                    inputSystem.Untarget();
                }
            }
            else if (tool is MoveTool && leftClickedOn)
            {
                /*if (hasSelection)
            {
                layerManager.selectedLayer.OverlayTexture(animationManager.currentFrameIndex, selectionTexture, pixel - mouseDragPoints[0], AnimFrameRefMode.NewKeyFrame);
            }
            else
            {
                layerManager.selectedLayer.SetTexture(animationManager.currentFrameIndex, Tex2DSprite.Offset(selectionTexture, pixel - mouseDragPoints[0]), AnimFrameRefMode.NewKeyFrame);
            }*/

                if (selectedLayer.layerType == LayerType.Tile && tileBeingMoved != null)
                {
                    tileBeingMoved.bottomLeft = tileBeingMovedLastValidPosition;
                    file.AddTile(tileBeingMoved);
                }

                UpdateDrawing();
            }
            if (tool is GradientTool && (leftClickedOn || rightClickedOn))
            {
                Color startColour = leftClickedOn ? colourPicker.primaryColour : colourPicker.secondaryColour;
                Color endColour = leftClickedOn ? colourPicker.secondaryColour : colourPicker.primaryColour;

                IPattern2D<Color> gradient = toolbar.GradientTool.mode switch
                {
                    GradientTool.Mode.Linear => new Patterns.Gradient.Linear((mouseDragPoints[0], startColour), (pixel, endColour)),
                    GradientTool.Mode.Radial => new Patterns.Gradient.Radial((mouseDragPoints[0], startColour), (pixel, endColour)),
                    _ => throw new InvalidOperationException($"Unknown / unimplemented gradient mode: {toolbar.GradientTool.mode}")
                };

                Tools.Tools.UsePattern(file, layer, frame, gradient);
                UpdateDrawing();
            }
        }

        private void PreviewShape(IShape shape, Color colour) => SetPreview(shape.ToTexture(colour), shape.boundingRect.bottomLeft);
        private void PreviewPattern(IPattern2D<Color> pattern) => SetPreview(pattern.ToTexture(file.rect), IntVector2.zero);
        private void PreviewPattern(IPattern2D<Color32> pattern) => SetPreview(pattern.ToTexture(file.rect), IntVector2.zero);

        private void PreviewMove(IntVector2 pixel)
        {
            /*selectionMask = Tex2DSprite.Offset(selectionMaskCopy, pixel - mouseDragPoints[0]);
            UpdateDrawingMoveTool(pixel);*/

            if (selectedLayer.layerType == LayerType.Tile && tileBeingMoved != null)
            {
                IntRect shiftedRect = file.rect.TranslateClamp(tileBeingMoved.rect + pixel - mouseDragPoints[0]);

                bool validPosition = true;
                foreach (TileLayer tileLayer in tileBeingMoved.tileLayersAppearsOn)
                {
                    foreach (Tile tile in tileLayer.tiles)
                    {
                        if (tile != tileBeingMoved)
                        {
                            validPosition &= !tile.rect.Overlaps(shiftedRect);
                        }
                    }
                }

                if (validPosition)
                {
                    tileBeingMovedLastValidPosition = shiftedRect.bottomLeft;
                    SetPreviewPosition(tileBeingMovedLastValidPosition);
                }
                else
                {
                    /*shiftedRect = file.rect.Clamp(tileBeingMoved.rect + tileBeingMovedLastValidPosition - tileBeingMovedOriginalBottomLeft + pixel - previousPixelUsedToolOn);

                validPosition = true;
                foreach (TileLayer tileLayer in tileBeingMoved.tileLayersAppearsOn)
                {
                    foreach (Tile tile in tileLayer.tiles)
                    {
                        if (tile != tileBeingMoved)
                        {
                            validPosition &= !tile.rect.Overlaps(shiftedRect);
                        }
                    }
                }

                if (validPosition)
                {
                    tileBeingMovedLastValidPosition = shiftedRect.bottomLeft;
                    SetPreviewPosition(tileBeingMovedLastValidPosition);
                }*/

                    HashSet<IntVector2> visitedOffsets = new HashSet<IntVector2>(new List<IntVector2> { IntVector2.zero });
                    Queue<IntVector2> offsetsToVisit = new Queue<IntVector2>(new List<IntVector2> { IntVector2.up, IntVector2.down, IntVector2.left, IntVector2.right });

                    int trials = 0;
                    while (trials <= 10000)
                    {
                        trials++;

                        IntVector2 offset = offsetsToVisit.Dequeue();
                        visitedOffsets.Add(offset);
                        shiftedRect = file.rect.TranslateClamp(tileBeingMoved.rect + pixel - mouseDragPoints[0] + offset);

                        validPosition = true;
                        foreach (TileLayer tileLayer in tileBeingMoved.tileLayersAppearsOn)
                        {
                            foreach (Tile tile in tileLayer.tiles)
                            {
                                if (tile != tileBeingMoved)
                                {
                                    validPosition &= !tile.rect.Overlaps(shiftedRect);
                                }
                            }
                        }

                        if (validPosition)
                        {
                            tileBeingMovedLastValidPosition = shiftedRect.bottomLeft;
                            SetPreviewPosition(tileBeingMovedLastValidPosition);
                            break;
                        }
                        else
                        {
                            foreach (IntVector2 direction in new List<IntVector2> { IntVector2.up, IntVector2.down, IntVector2.left, IntVector2.right })
                            {
                                if (!visitedOffsets.Contains(offset + direction))
                                {
                                    offsetsToVisit.Enqueue(offset + direction);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateDrawingMoveTool(File file, int layer, int frame, IntVector2 mouseCoords)
        {
            if (hasSelection)
            {
                Texture2D bottomLayers = file.RenderLayersBelow(selectedLayerIndex, currentFrameIndex);
                Texture2D topLayers = file.RenderLayersAbove(selectedLayerIndex, currentFrameIndex);
                drawingSprRen.sprite = BlendMode.Normal.Blend(
                    topLayers,
                    BlendMode.Normal.Blend(selectionTexture, bottomLayers, mouseCoords - mouseDragPoints[0])
                    ).ToSprite();
            }
            else
            {
                Texture2D bottomLayers = file.RenderLayersBelow(selectedLayerIndex, currentFrameIndex);
                Texture2D topLayers = file.RenderLayersAbove(selectedLayerIndex, currentFrameIndex);
                drawingSprRen.sprite = BlendMode.Normal.Blend(
                    topLayers,
                    BlendMode.Normal.Blend(selectionTexture, bottomLayers, mouseCoords - mouseDragPoints[0])
                    ).ToSprite();
            }
        }

        private void SelectionShape(I2DShape<IShape> shape, bool erase)
        {
            Texture2D tex = shape.ToTexture(Config.Colours.mask, file.rect);
            if (erase)
            {
                selectionMask = BlendMode.Subtract.Blend(selectionMask, tex);
            }
        }

        private void SelectionMagicWand(IntVector2 pixel, bool erase, bool addToExistingSelection)
        {
            static Texture2D GetMagicWandMask(Texture2D texture, IntVector2 clickPoint)
            {
                Texture2D fillMask = Texture2DExtensions.Transparent(texture.width, texture.height);

                foreach (IntVector2 pixel in FloodFill.GetPixelsToFill(texture, clickPoint, false))
                {
                    fillMask.SetPixel(pixel.x, pixel.y, Config.Colours.mask);
                }

                return fillMask;
            }

            if (erase)
            {
                if (selectionMask.GetPixel(pixel.x, pixel.y).a != 0f)
                {
                    selectionMask = BlendMode.Subtract.Blend(
                        selectionMask,
                        GetMagicWandMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel)
                        );
                }
                else
                {
                    selectionMask = BlendMode.Subtract.Blend(
                        selectionMask,
                        GetMagicWandMask(selectionMask, pixel)
                        );
                }
            }
            else
            {
                if (addToExistingSelection)
                {
                    selectionMask = BlendMode.Normal.Blend(
                        selectionMask,
                        GetMagicWandMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel)
                        );
                }
                else
                {
                    selectionMask = GetMagicWandMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel);
                }
            }
        }

        private void SelectionDraw(IntVector2 pixel, bool erase)
        {
            selectionMask.SetPixel(pixel.x, pixel.y, erase ? Config.Colours.transparent : Config.Colours.mask);
            selectionMask = selectionMask.Applied();
        }

        private void DeselectSelection()
        {
            selectionMask = Texture2DExtensions.Transparent(file.width, file.height);
            deselectedSelectionThisFrame = true;
        }

        public void DeleteSelection()
        {
            List<IntVector2> pixelsToFill = new List<IntVector2>();

            for (int x = 0; x < selectionMask.width; x++)
            {
                for (int y = 0; y < selectionMask.height; y++)
                {
                    if (selectionMask.GetPixel(x, y) == Config.Colours.mask)
                    {
                        pixelsToFill.Add(new IntVector2(x, y));
                    }
                }
            }

            selectedLayer.SetPixels(pixelsToFill.ToArray(), animationManager.currentFrameIndex, Config.Colours.transparent, AnimFrameRefMode.NewKeyFrame);
            UpdateDrawing();
        }

        /// <summary>
        /// Turns the world coord into a pixel coord in the drawing.
        /// </summary>
        public IntVector2 WorldPosToPixels(float x, float y) => WorldPosToPixel(new Vector2(x, y));

        /// <summary>
        /// Turns the world coord into a pixel coord in the drawing.
        /// </summary>
        public IntVector2 WorldPosToPixel(Vector2 worldPos)
        {
            Vector2 pixels = (worldPos - (Vector2)transform.position) / transform.lossyScale * pixelsPerUnit + new Vector2(file.width, file.height) / 2f;

            return new IntVector2(Mathf.FloorToInt(pixels.x), Mathf.FloorToInt(pixels.y));
        }

        /// <summary>
        /// Turns the pixel coordinate in the drawing into a world coordinate (the resulting coord is the centre of the pixel in the world).
        /// </summary>
        public Vector2 PixelsToWorldPos(IntVector2 pixel) => PixelsToWorldPos(pixel.x, pixel.y);

        /// <summary>
        /// Turns the pixel coordinate in the drawing into a world coordinate (the resulting coord is the centre of the pixel in the world).
        /// </summary>
        public Vector2 PixelsToWorldPos(int x, int y)
        {
            return PixelsToWorldPos(x + 0.5f, y + 0.5f);
        }

        /// <summary>
        /// Turns the pixel coordinate in the drawing into a world coordinate.
        /// </summary>
        public Vector2 PixelsToWorldPos(Vector2 pixel) => PixelsToWorldPos(pixel.x, pixel.y);

        /// <summary>
        /// Turns the pixel coordinate in the drawing into a world coordinate.
        /// </summary>
        public Vector2 PixelsToWorldPos(float x, float y)
        {
            Vector2 adjustedPixel = new Vector2(x, y) - new Vector2(file.width, file.height) / 2f;

            return (Vector2)transform.position + adjustedPixel / pixelsPerUnit * transform.lossyScale;
        }

        /// <summary>
        /// Called when a tile icon is selected from the tileset menu.
        /// </summary>
        private void OnTileIconSelected(File tileFile)
        {
            tileBeingMoved = new Tile(tileFile, IntVector2.zero, new TileLayer[] { (TileLayer)file.layers[0] });
            tileBeingMovedOriginalBottomLeft = tileBeingMoved.bottomLeft;
            tileBeingMovedLastValidPosition = tileBeingMoved.bottomLeft;

            SetPreview(tileBeingMoved.file.liveRender.Applied(), tileBeingMoved.bottomLeft);
        }
    }
}
