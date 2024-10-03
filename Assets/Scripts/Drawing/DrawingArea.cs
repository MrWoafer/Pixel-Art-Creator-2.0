using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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
    private UndoRedoManager undoRedoManager;
    private ImageEditManager imageEditManager;
    private TileOutlineManager tileOutlineManager;
    private TilesetManager tilesetManager;

    // Mouse variables
    /// <summary>The pixel the mouse is currently on.</summary>
    private IntVector2 mousePixel => WorldPosToPixel(mouse.worldPos);
    private readonly IntVector2 mouseDragInactiveCoords = new IntVector2(-1, -1);
    /// <summary>For tools that involve dragging, this are the pixels the mouse started dragged from/to, in order.</summary>
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
    private Tool previousToolUsed = Tool.None;
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
        private set => selectionSprRen.sprite = Tex2DSprite.Tex2DToSprite(value);
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
                    if (selectionMask.GetPixel(x, y) != Color.clear)
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
                    if (selectionMask.GetPixel(x, y) != Color.clear)
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
    private IntVector2[] lineSmoothingPreviousLine = new IntVector2[0];
    private Color lineSmoothingColourUnderLineEnd = Color.clear;
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
        undoRedoManager = Finder.undoRedoManager;
        imageEditManager = Finder.imageEditManager;
        animationManager = Finder.animationManager;
        tilesetManager = Finder.tilesetManager;

        inputTarget = GetComponent<InputTarget>();
        mouse = Finder.mouse;

        collider = GetComponent<BoxCollider2D>();

        colourPicker = Finder.colourPicker;
        toolbar = Finder.toolbar;
        undoRedoManager = Finder.undoRedoManager;

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

        undoRedoManager.SubscribeToUndoOrRedo(UpdateDrawing);
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
                        ClickTool(toolbar.selectedTool, mousePixel, selectedLayerIndex, currentFrameIndex, colour);
                    }
                    else
                    {
                        // Mouse movement interpolation
                        IntVector2[] line = Shapes.LineCoords(previousPixelUsedToolOn, mousePixel);
                        Color colourUnderLineEnd = selectedLayer.GetPixel(line[^1], animationManager.currentFrameIndex);
                        foreach (IntVector2 pixel in line)
                        {
                            ClickTool(toolbar.selectedTool, pixel, selectedLayerIndex, currentFrameIndex, colour);
                        }

                        bool smoothed = false;
                        if (toolbar.selectedTool == Tool.Pencil)
                        {
                            smoothed = Tools.PencilLineSmoothing(file, selectedLayerIndex, currentFrameIndex, line, lineSmoothingPreviousLine, lineSmoothingColourUnderLineEnd);
                        }
                        lineSmoothingPreviousLine = smoothed ? (from x in line where x != line[0] select x).ToArray() : line;
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
            transform.position = Functions.Vector2ToVector3(mouse.worldPos + moveOffsetFromMouse) + Vector3.forward * transform.position.z;
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
            lineSmoothingPreviousLine = new IntVector2[0];
        }

        // Stop line smoothing from happening if it has been too long since the last pixel was painted
        if (lineSmoothingPreviousLine.Length != 0)
        {
            lineSmoothingCountdown -= Time.deltaTime;

            if (lineSmoothingCountdown <= 0f)
            {
                lineSmoothingPreviousLine = new IntVector2[0];
                lineSmoothingCountdown = toolbar.lineSmoothingTime;
            }
        }
    }

    private void InitialiseDisplay()
    {
        drawingSprRen.sprite = Tex2DSprite.Tex2DToSprite(file.liveRender);
        backgroundSprRen.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.CheckerboardBackground(file.width, file.height));
        previewSprRen.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.BlankTexture(1, 1));
        selectionMask = Tex2DSprite.BlankTexture(file.width, file.height);

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
        previewSprRen.sprite = Tex2DSprite.Tex2DToSprite(texture);
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
        SetPreview(Tex2DSprite.BlankTexture(1, 1), 0, 0);
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
        brushBorderSprRen.sprite = Tex2DSprite.Tex2DToSprite(toolbar.brushTexture);
        brushBorderSprRen.transform.localScale = new Vector3(toolbar.brushPixelsWidth / pixelsPerUnit, toolbar.brushPixelsHeight / pixelsPerUnit, 1f);
        brushBorderSprRen.transform.position = PixelsToWorldPos(pixel);

        // When the brush is a single pixel, make the border black/white depending on the lightness of the pixel below
        Color outlineColour = Color.black;
        if (toolbar.brushPixelsIsSingleCentralPixel && file.liveRender.GetPixel(pixel).a != 0f && new HSL(file.liveRender.GetPixel(pixel)).l < 0.5f)
        {
            outlineColour = Color.white;
        }
        brushBorderSprRen.GetComponent<Outline>().colour = outlineColour;
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

        transform.position = transform.position + Functions.Vector2ToVector3(focusPoint - Functions.Vector3ToVector2(transform.TransformPoint(zoomPointLocalCoords)));
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
                moveOffsetFromMouse = Functions.Vector3ToVector2(transform.position) - mouse.worldPos;
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

                if (toolbar.selectedTool == Tool.Move && !selectedLayer.locked)
                {
                    if (selectedLayer.layerType == LayerType.Tile)
                    {
                        tileBeingMoved = ((TileLayer)selectedLayer).PixelToTile(mouseDragPoints[0]);

                        if (tileBeingMoved != null)
                        {
                            tileBeingMovedOriginalBottomLeft = tileBeingMoved.bottomLeft;
                            tileBeingMovedLastValidPosition = tileBeingMoved.bottomLeft;

                            tileBeingMoved.file.liveRender.Apply();
                            SetPreview(tileBeingMoved.file.liveRender, tileBeingMoved.bottomLeft);

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
            if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("scroll y")))
            {
                inputSystem.inputTarget.cursorScroll = CursorState.UpDownArrow;
                ScrollViewY(mouse.scrollDelta * scrollSpeed);
            }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("scroll x")))
            {
                inputSystem.inputTarget.cursorScroll = CursorState.LeftRightArrow;
                ScrollViewX(mouse.scrollDelta * scrollSpeed);
            }
            else if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("scroll brush size")))
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
        if (inputTarget.keyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("cancel tool")) && toolbar.selectedTool.canBeCancelled)
        {
            toolCancelled = true;
            ClearPreview();
            finishedUsingTool = true;

            if (toolbar.selectedTool == Tool.IsoBox)
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
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("zoom in")))
        {
            Zoom(zoomScrollSpeed, Vector2.zero);
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("zoom out")))
        {
            Zoom(-zoomScrollSpeed, Vector2.zero);
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("reset view")))
        {
            ResetView();
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("clear selection")) && hasSelection)
        {
            DeselectSelection();
        }
        if (inputSystem.globalKeyboardTarget.IsPressed(KeyCode.Delete) && hasSelection)
        {
            undoRedoManager.AddUndoState(new UndoRedoState(UndoRedoAction.Draw, file.layers[selectedLayerIndex], selectedLayerIndex), fileManager.currentFileIndex);
            DeleteSelection();
        }

        /// To be implemented
        if (toolbar.selectedTool == Tool.Move && hasSelection)
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
        if (tool == Tool.IsoBox)
        {
            if (isoBoxPlacedBase) { PreviewIsoBox(mouseDragPoints[0], mouseDragPoints[1], pixel, colour, rightClickedOn); }
        }
    }

    private void ClickTool(Tool tool, IntVector2 pixel, int layer, int frame, Color colour)
    {
        finishedUsingTool = false;

        if (tool == Tool.Pencil)
        {
            if (leftClickedOn) { Tools.UsePencil(file, layer, frame, pixel, colour); }
            else if (rightClickedOn) { Tools.UseRubber(file, layer, frame, pixel); }
        }
        else if (tool == Tool.Brush)
        {
            if (leftClickedOn) { Tools.UseBrush(file, layer, frame, pixel, toolbar.brushPixels, colour); }
            else if (rightClickedOn) { Tools.UseRubber(file, layer, frame, pixel, toolbar.brushPixels); }
        }
        else if (tool == Tool.Rubber)
        {
            if (leftClickedOn || rightClickedOn) { Tools.UseRubber(file, layer, frame, pixel, toolbar.brushPixels); }
        }
        else if (tool == Tool.EyeDropper)
        {
            if (leftClickedOn) { colourPicker.SetColour(Tools.UseEyeDropper(file, layer, frame, pixel)); }
        }
        else if (tool == Tool.Fill)
        {
            if (leftClickedOn) { Tools.UseFill(file, layer, frame, pixel, colour); }
            else if (rightClickedOn) { Tools.UseFill(file, layer, frame, pixel, Color.clear); }
        }
        else if (tool == Tool.Line)
        {
            if (leftClickedOn) { PreviewLine(mouseDragPoints[0], pixel, colour); }
        }
        else if (tool == Tool.Shape && (leftClickedOn || rightClickedOn))
        {
            if (toolbar.shapeToolShape == Shape.Rectangle)
            {
                if (holdingCtrl) { PreviewSquare(mouseDragPoints[0], pixel, colour, rightClickedOn); }
                else { PreviewRectangle(mouseDragPoints[0], pixel, colour, rightClickedOn); }
            }
            else if (toolbar.shapeToolShape == Shape.Ellipse)
            {
                if (holdingCtrl) { PreviewCircle(mouseDragPoints[0], pixel, colour, rightClickedOn); }
                else { PreviewEllipse(mouseDragPoints[0], pixel, colour, rightClickedOn); }
            }
            else if (toolbar.shapeToolShape == Shape.Triangle)
            {
                PreviewTriangle(mouseDragPoints[0], pixel, colour, !holdingCtrl, rightClickedOn);
            }
        }
        else if (tool == Tool.IsoBox && (leftClickedOn || rightClickedOn))
        {
            if (!isoBoxPlacedBase) { PreviewIsoRectangle(mouseDragPoints[0], pixel, colour, false); }
            else { PreviewIsoBox(mouseDragPoints[0], mouseDragPoints[1], pixel, colour, rightClickedOn); }
        }
        else if (tool == Tool.Gradient && (leftClickedOn || rightClickedOn))
        {
            Color startColour = leftClickedOn ? colourPicker.primaryColour : colourPicker.secondaryColour;
            Color endColour = leftClickedOn ? colourPicker.secondaryColour : colourPicker.primaryColour;
            PreviewGradient(mouseDragPoints[0], pixel, startColour, endColour, toolbar.gradientMode);
        }
        else if (tool == Tool.Selection && (leftClickedOn || rightClickedOn))
        {
            if (toolbar.selectionMode == SelectionMode.Draw)
            {
                SelectionDraw(pixel, rightClickedOn);
            }
            else if (toolbar.selectionMode == SelectionMode.MagicWand)
            {
                SelectionMagicWand(pixel, rightClickedOn, holdingCtrl);
            }
            else if (toolbar.selectionMode == SelectionMode.Rectangle)
            {
                if (holdingCtrl) { SelectionSquare(mouseDragPoints[0], pixel, rightClickedOn); }
                else { SelectionRectangle(mouseDragPoints[0], pixel, rightClickedOn); }
            }
            else if (toolbar.selectionMode == SelectionMode.Ellipse)
            {
                if (holdingCtrl) { SelectionCircle(mouseDragPoints[0], pixel, rightClickedOn); }
                else { SelectionEllipse(mouseDragPoints[0], pixel, rightClickedOn); }
            }
        }
        else if (tool == Tool.Move)
        {
            if (leftClickedOn) { PreviewMove(pixel); }
        }
    }

    /// <summary>
    /// For tools that only enact changes to the image when you release the mouse (e.g. shape tool), this function enacts those changes.
    /// </summary>
    private void UnclickTool(Tool tool, IntVector2 pixel, int layer, int frame, Color colour)
    {
        if (tool == Tool.Line && leftClickedOn)
        {
            Tools.UseLine(file, layer, frame, mouseDragPoints[0], pixel, colour);
            UpdateDrawing();
        }
        else if (tool == Tool.Shape && (leftClickedOn || rightClickedOn))
        {
            if (toolbar.shapeToolShape == Shape.Rectangle)
            {
                if (holdingCtrl) { Tools.UseSquare(file, layer, frame, mouseDragPoints[0], pixel, colour, rightClickedOn, true); }
                else { Tools.UseRectangle(file, layer, frame, mouseDragPoints[0], pixel, colour, rightClickedOn); }
                UpdateDrawing();
            }
            else if (toolbar.shapeToolShape == Shape.Ellipse)
            {
                if (holdingCtrl) { Tools.UseCircle(file, layer, frame, mouseDragPoints[0], pixel, colour, rightClickedOn, true); }
                else { Tools.UseEllipse(file, layer, frame, mouseDragPoints[0], pixel, colour, rightClickedOn); }
                UpdateDrawing();
            }
            else if (toolbar.shapeToolShape == Shape.Triangle)
            {
                Tools.UseRightTriangle(file, layer, frame, mouseDragPoints[0], pixel, colour, !holdingCtrl, rightClickedOn);
                UpdateDrawing();
            }
        }
        else if (tool == Tool.IsoBox && (leftClickedOn || rightClickedOn))
        {
            if (!isoBoxPlacedBase)
            {
                isoBoxPlacedBase = true;
            }
            else
            {
                Tools.UseIsoBox(file, layer, frame, mouseDragPoints[0], mouseDragPoints[1], pixel, colour, rightClickedOn);
                UpdateDrawing();

                isoBoxPlacedBase = false;
                finishedUsingTool = true;
                inputSystem.Untarget();
            }
        }
        else if (tool == Tool.Move && leftClickedOn)
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
        if (tool == Tool.Gradient && (leftClickedOn || rightClickedOn))
        {
            Color startColour = leftClickedOn ? colourPicker.primaryColour : colourPicker.secondaryColour;
            Color endColour = leftClickedOn ? colourPicker.secondaryColour : colourPicker.primaryColour;
            Tools.UseGradient(file, layer, frame, mouseDragPoints[0], pixel, startColour, endColour, toolbar.gradientMode);
            UpdateDrawing();
        }
    }

    private void PreviewLine(IntVector2 start, IntVector2 end, Color colour)
    {
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.Line(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewSquare(IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        end = Shapes.SnapEndCoordToSquare(file.width, file.height, start, end, true);
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.Square(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, filled, true);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewRectangle(IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.Rectangle(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, filled);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewCircle(IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        end = Shapes.SnapEndCoordToSquare(file.width, file.height, start, end, true);
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.Circle(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, filled, true);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewEllipse(IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.Ellipse(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, filled);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewTriangle(IntVector2 start, IntVector2 end, Color colour, bool rightAngleOnBottom, bool filled)
    {
        IntRect rect = new IntRect(start, end);
        Texture2D tex = Shapes.RightTriangle(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, rightAngleOnBottom, filled);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewIsoRectangle(IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        IntRect rect = file.rect;
        Texture2D tex = Shapes.IsoRectangle(rect.width, rect.height, start - rect.bottomLeft, end - rect.bottomLeft, colour, filled);

        SetPreview(tex, rect.bottomLeft);
    }

    private void PreviewIsoBox(IntVector2 baseStart, IntVector2 baseEnd, IntVector2 heightEnd, Color colour, bool filled)
    {
        IntRect rect = file.rect;
        Texture2D tex = Shapes.IsoBox(rect.width, rect.height, baseStart - rect.bottomLeft, baseEnd - rect.bottomLeft, heightEnd - rect.bottomLeft, colour, filled);

        SetPreview(tex, rect.bottomLeft);
    }

    public void PreviewGradient(IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode)
    {
        Texture2D tex = Shapes.Gradient(file.width, file.height, start, end, startColour, endColour, gradientMode);

        SetPreview(tex, 0, 0);
    }
    private void PreviewGradientLinear(IntVector2 start, IntVector2 end, Color startColour, Color endColour) => PreviewGradient(start, end, startColour, endColour, GradientMode.Linear);
    private void PreviewGradientRadial(IntVector2 start, IntVector2 end, Color startColour, Color endColour) => PreviewGradient(start, end, startColour, endColour, GradientMode.Radial);

    private void PreviewMove(IntVector2 pixel)
    {
        /*selectionMask = Tex2DSprite.Offset(selectionMaskCopy, pixel - mouseDragPoints[0]);
            UpdateDrawingMoveTool(pixel);*/

        if (selectedLayer.layerType == LayerType.Tile && tileBeingMoved != null)
        {
            IntRect shiftedRect = file.rect.Clamp(tileBeingMoved.rect + pixel - mouseDragPoints[0]);

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
                    shiftedRect = file.rect.Clamp(tileBeingMoved.rect + pixel - mouseDragPoints[0] + offset);

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
            drawingSprRen.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(topLayers, Tex2DSprite.Overlay(selectionTexture, bottomLayers, mouseCoords - mouseDragPoints[0])));
        }
        else
        {
            Texture2D bottomLayers = file.RenderLayersBelow(selectedLayerIndex, currentFrameIndex);
            Texture2D topLayers = file.RenderLayersAbove(selectedLayerIndex, currentFrameIndex);
            drawingSprRen.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(topLayers, Tex2DSprite.Overlay(selectionTexture, bottomLayers, mouseCoords - mouseDragPoints[0])));
        }
    }

    private void SelectionSquare(IntVector2 start, IntVector2 end, bool erase)
    {
        if (erase)
        {
            selectionMask = Tex2DSprite.Subtract(selectionMask, Shapes.Square(file.width, file.height, start, end, Color.white, true, true));
        }
        else
        {
            selectionMask = Shapes.Square(file.width, file.height, start, end, Color.white, true, true);
        }
    }
    private void SelectionRectangle(IntVector2 start, IntVector2 end, bool erase)
    {
        if (erase)
        {
            selectionMask = Tex2DSprite.Subtract(selectionMask, Shapes.Rectangle(file.width, file.height, start, end, Color.white, true));
        }
        else
        {
            selectionMask = Shapes.Rectangle(file.width, file.height, start, end, Color.white, true);
        }
    }
    private void SelectionCircle(IntVector2 start, IntVector2 end, bool erase)
    {
        if (erase)
        {
            selectionMask = Tex2DSprite.Subtract(selectionMask, Shapes.Circle(file.width, file.height, start, end, Color.white, true, true));
        }
        else
        {
            selectionMask = Shapes.Circle(file.width, file.height, start, end, Color.white, true, true);
        }
    }
    private void SelectionEllipse(IntVector2 start, IntVector2 end, bool erase)
    {
        if (erase)
        {
            selectionMask = Tex2DSprite.Subtract(selectionMask, Shapes.Ellipse(file.width, file.height, start, end, Color.white, true));
        }
        else
        {
            selectionMask = Shapes.Ellipse(file.width, file.height, start, end, Color.white, true);
        }
    }

    private void SelectionMagicWand(IntVector2 pixel, bool erase, bool addToExistingSelection)
    {
        if (erase)
        {
            if (selectionMask.GetPixel(pixel.x, pixel.y) == Color.clear)
            {
                selectionMask = Tex2DSprite.Subtract(selectionMask, Tex2DSprite.GetFillMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel));
            }
            else
            {
                selectionMask = Tex2DSprite.Subtract(selectionMask, Tex2DSprite.GetFillMask(selectionMask, pixel));
            }
        }
        else
        {
            if (addToExistingSelection)
            {
                selectionMask = Tex2DSprite.Overlay(selectionMask, Tex2DSprite.GetFillMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel));
            }
            else
            {
                selectionMask = Tex2DSprite.GetFillMask(selectedLayer[animationManager.currentFrameIndex].texture, pixel);
            }
        }
    }

    private void SelectionDraw(IntVector2 pixel, bool erase)
    {
        selectionMask.SetPixel(pixel.x, pixel.y, erase ? Color.clear : Color.white);
        selectionMask.Apply();
        selectionMask = selectionMask;
    }

    private void DeselectSelection()
    {
        selectionMask = Tex2DSprite.BlankTexture(file.width, file.height);
        deselectedSelectionThisFrame = true;
    }

    public void DeleteSelection()
    {
        List<IntVector2> pixelsToFill = new List<IntVector2>();

        for (int x = 0; x < selectionMask.width; x++)
        {
            for (int y = 0; y < selectionMask.height; y++)
            {
                if (selectionMask.GetPixel(x, y) == Color.white)
                {
                    pixelsToFill.Add(new IntVector2(x, y));
                }
            }
        }

        selectedLayer.SetPixels(pixelsToFill.ToArray(), animationManager.currentFrameIndex, Color.clear, AnimFrameRefMode.NewKeyFrame);
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
        Vector2 pixels = (worldPos - Functions.Vector3ToVector2(transform.position)) / transform.lossyScale * pixelsPerUnit + new Vector2(file.width, file.height) / 2f;

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

        tileBeingMoved.file.liveRender.Apply();
        SetPreview(tileBeingMoved.file.liveRender, tileBeingMoved.bottomLeft);
    }
}
