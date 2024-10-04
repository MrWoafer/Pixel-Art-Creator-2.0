using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SFB;

public class UIManager : MonoBehaviour
{
    [Header("UI Animation")]
    [SerializeField]
    private float popUpDuration = 0.3f;
    [SerializeField]
    private AnimationCurve popUpOpenScaleCurve;
    [SerializeField]
    private AnimationCurve popUpCloseScaleCurve;

    [Header("New File Window")]
    [SerializeField]
    private GameObject newFileWindow;
    [SerializeField]
    private UINumberField newFileWidthField;
    [SerializeField]
    private UINumberField newFileHeightField;
    [SerializeField]
    private UIScrollbar newFileSizeScrollbar;

    [Header("Extend / Crop Window")]
    [SerializeField]
    private GameObject extendCropWindow;
    [SerializeField]
    private Image extendCropPreview;
    [SerializeField]
    private UINumberField extendCropLeftField;
    [SerializeField]
    private UINumberField extendCropRightField;
    [SerializeField]
    private UINumberField extendCropTopField;
    [SerializeField]
    private UINumberField extendCropBottomField;

    [Header("Scale Window")]
    [SerializeField]
    private GameObject scaleWindow;
    [SerializeField]
    private Image scalePreview;
    [SerializeField]
    private UINumberField scaleWidthField;
    [SerializeField]
    private UINumberField scaleHeightField;
    [SerializeField]
    private UINumberField scaleXScalarField;
    [SerializeField]
    private UINumberField scaleYScalarField;

    [Header("Grid Window")]
    [SerializeField]
    private GameObject gridWindow;
    [SerializeField]
    private Image gridPreview;
    [SerializeField]
    private UINumberField gridWidthField;
    [SerializeField]
    private UINumberField gridHeightField;
    [SerializeField]
    private UINumberField gridOffsetXField;
    [SerializeField]
    private UINumberField gridOffsetYField;
    [SerializeField]
    private UIToggleButton gridEnableButton;
    private GridManager gridPreviewGridManager;

    [Header("Outline Window")]
    [SerializeField]
    private GameObject outlineWindow;
    [SerializeField]
    private Image outlinePreview;
    [SerializeField]
    private UIToggleButton outlineOutsideToggle;
    [SerializeField]
    private UIColourField outlineColourField;
    [SerializeField]
    private UIToggleButton outlineTopLeftToggle;
    [SerializeField]
    private UIToggleButton outlineTopMiddleToggle;
    [SerializeField]
    private UIToggleButton outlineTopRightToggle;
    [SerializeField]
    private UIToggleButton outlineMiddleLeftToggle;
    [SerializeField]
    private UIToggleButton outlineMiddleRightToggle;
    [SerializeField]
    private UIToggleButton outlineBottomLeftToggle;
    [SerializeField]
    private UIToggleButton outlineBottomMiddleToggle;
    [SerializeField]
    private UIToggleButton outlineBottomRightToggle;

    [Header("Replace Colour Window")]
    [SerializeField]
    private GameObject replaceColourWindow;
    [SerializeField]
    private Image replaceColourPreview;
    [SerializeField]
    private UIColourField replaceColourToReplaceField;
    [SerializeField]
    private UIColourField replaceColourReplaceWithField;

    [Header("Import PAC Window")]
    [SerializeField]
    private GameObject importPACWindow;
    [SerializeField]
    private Image importPACPreview;
    [SerializeField]
    private UIViewport importPACLayersViewport;
    [SerializeField]
    private UIScrollbar importPACScrollbar;
    [SerializeField]
    private UIToggleGroup importPACLayersToggleGroup;
    private File importPACFile;

    [Header("Layer Properties")]
    [SerializeField]
    private GameObject layerPropertiesWindow;
    [SerializeField]
    private UITextbox layerNameTextbox;
    [SerializeField]
    private UINumberField layerOpacityField;
    [SerializeField]
    private UIDropdownChoice layerBlendModeDropdown;
    private int layerPropertiesLayerIndex;

    [Header("Brush Settings")]
    [SerializeField]
    private GameObject brushSettingsWindow;
    [SerializeField]
    private UIToggleGroup brushSettingsShapeToggleGroup;
    [SerializeField]
    private UINumberField brushSettingsSizeField;

    [Header("Keyboard Shortcuts")]
    [SerializeField]
    private GameObject keyboardShortcutsWindow;

    [Header("Unsaved Changes Properties")]
    private UIModalWindow unsavedChangesModalWindow;
    private int unsavedChangesFileIndex;

    [Header("Modal Window")]
    [SerializeField]
    private Transform modalWindowsParentObj;
    [SerializeField]
    private GameObject modalWindowPrefab;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject uiToggleButtonPrefab;

    public UIElement selectedUIElement { get; private set; }

    private bool untargetedThisFrame = false;

    private FileManager fileManager;
    private LayerManager layerManager;
    private ImageEditManager imageEditManager;
    private AnimationManager animationManager;
    private GridManager gridManager;
    private DrawingArea drawingArea;
    private Toolbar toolbar;

    private BlurPanel blurPanel;

    private bool updatedScaleSettingsThisFrame = false;

    private bool beenRunningAFrame = false;
    private bool openedWindow = false;

    private void Awake()
    {
        fileManager = Finder.fileManager;
        layerManager = Finder.layerManager;
        imageEditManager = Finder.imageEditManager;
        animationManager = Finder.animationManager;
        gridManager = Finder.gridManager;
        drawingArea = Finder.drawingArea;
        toolbar = Finder.toolbar;

        blurPanel = GameObject.Find("Blur Panel").GetComponent<BlurPanel>();

        gridPreviewGridManager = gridPreview.GetComponent<GridManager>();

        newFileWindow.SetActive(true);
        extendCropWindow.SetActive(true);
        scaleWindow.SetActive(true);
        gridWindow.SetActive(true);
        outlineWindow.SetActive(true);
        replaceColourWindow.SetActive(true);
        importPACWindow.SetActive(true);
        layerPropertiesWindow.SetActive(true);
        brushSettingsWindow.SetActive(true);
        keyboardShortcutsWindow.SetActive(true);
    }

    private void Start()
    {
        blurPanel.EnableDisable(false);
        newFileWindow.SetActive(false);

        extendCropLeftField.SubscribeToValueChanged(UpdateExtendCropPreview);
        extendCropRightField.SubscribeToValueChanged(UpdateExtendCropPreview);
        extendCropTopField.SubscribeToValueChanged(UpdateExtendCropPreview);
        extendCropBottomField.SubscribeToValueChanged(UpdateExtendCropPreview);

        scaleWidthField.SubscribeToValueChanged(UpdateScaleWidthHeight);
        scaleHeightField.SubscribeToValueChanged(UpdateScaleWidthHeight);
        scaleXScalarField.SubscribeToValueChanged(UpdateScaleScalars);
        scaleYScalarField.SubscribeToValueChanged(UpdateScaleScalars);

        gridWidthField.SubscribeToValueChanged(UpdateGridPreview);
        gridHeightField.SubscribeToValueChanged(UpdateGridPreview);
        gridOffsetXField.SubscribeToValueChanged(UpdateGridPreview);
        gridOffsetYField.SubscribeToValueChanged(UpdateGridPreview);
        gridEnableButton.SubscribeToLeftClick(UpdateGridPreview);

        outlineOutsideToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineColourField.SubscribeToColourChange(UpdateOutlinePreview);
        outlineTopLeftToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineTopMiddleToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineTopRightToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineMiddleLeftToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineMiddleRightToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineBottomLeftToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineBottomMiddleToggle.SubscribeToLeftClick(UpdateOutlinePreview);
        outlineBottomRightToggle.SubscribeToLeftClick(UpdateOutlinePreview);

        replaceColourToReplaceField.SubscribeToColourChange(UpdateReplaceColourPreview);
        replaceColourReplaceWithField.SubscribeToColourChange(UpdateReplaceColourPreview);

        layerNameTextbox.SubscribeToFinishEvent(UpdateLayerPropertiesPreview);
        layerOpacityField.SubscribeToValueChanged(UpdateLayerPropertiesPreview);
        layerBlendModeDropdown.SubscribeToOptionChanged(UpdateLayerPropertiesPreview);

        brushSettingsShapeToggleGroup.SubscribeToSelectedToggleChange(UpdateBrushSettingsShape);
        brushSettingsSizeField.SubscribeToValueChanged(() => toolbar.SetBrushSize((int)brushSettingsSizeField.value));
    }

    private void Update()
    {
        if (updatedScaleSettingsThisFrame)
        {
            updatedScaleSettingsThisFrame = false;
        }

        if (!beenRunningAFrame)
        {
            newFileWindow.SetActive(false);
            extendCropWindow.SetActive(false);
            scaleWindow.SetActive(false);
            gridWindow.SetActive(false);
            outlineWindow.SetActive(false);
            replaceColourWindow.SetActive(false);
            importPACWindow.SetActive(false);
            layerPropertiesWindow.SetActive(false);
            brushSettingsWindow.SetActive(false);
            keyboardShortcutsWindow.SetActive(false);

            beenRunningAFrame = true;
        }
    }

    private void LateUpdate()
    {
        if (untargetedThisFrame)
        {
            untargetedThisFrame = false;
        }
    }

    public bool TryTarget(UIElement uiElement)
    {
        if (selectedUIElement)
        {
            return false;
        }

        Target(uiElement);
        return true;
    }

    private void Target(UIElement uiElement)
    {
        selectedUIElement = uiElement;

        //Debug.Log("Targeted UI Element: " + uiElement.elementName);
    }

    public bool TryUntarget(UIElement uiElement)
    {
        if (selectedUIElement != uiElement)
        {
            return false;
        }

        selectedUIElement = null;
        untargetedThisFrame = true;

        //Debug.Log("Untargeted UI Element: " + uiElement.elementName);

        return true;
    }

    public bool CanTargetInputTarget(InputTarget inputTarget)
    {
        return !untargetedThisFrame && (selectedUIElement == null || (inputTarget.uiElement != null && inputTarget.uiElement == selectedUIElement));
    }

    public void OpenDialogBox(GameObject dialogBox)
    {
        blurPanel.EnableDisable(true);
        dialogBox.SetActive(true);

        dialogBox.transform.localScale = Vector3.zero;
        LeanTween.scale(dialogBox, Vector3.one, popUpDuration).setEase(popUpOpenScaleCurve);
    }
    public void CloseDialogBox(GameObject dialogBox) => CloseDialogBox(dialogBox, null);
    public void CloseDialogBox(GameObject dialogBox, Action onComplete)
    {
        LeanTween.scale(dialogBox, Vector3.zero, popUpDuration).setEase(popUpCloseScaleCurve).setOnComplete(() =>
        {
            dialogBox.SetActive(false);
            blurPanel.EnableDisable(false);
            onComplete?.Invoke();
        });
    }

    public void OpenNewFileWindow()
    {
        OpenDialogBox(newFileWindow);

        newFileSizeScrollbar.scrollAmount = 1f;
    }
    public void CloseNewFileWindow()
    {
        CloseDialogBox(newFileWindow);
    }

    public void ConfirmNewFileWindow()
    {
        int width = (int)newFileWidthField.value;
        int height = (int)newFileHeightField.value;

        fileManager.NewFile(width, height);

        CloseNewFileWindow();
    }

    public void SetNewFileWidth(int width)
    {
        newFileWidthField.value = width;
    }
    public void SetNewFileHeight(int height)
    {
        newFileHeightField.value = height;
    }

    public void OpenExtendCropWindow()
    {
        OpenDialogBox(extendCropWindow);

        extendCropLeftField.value = 0f;
        extendCropRightField.value = 0f;
        extendCropTopField.value = 0f;
        extendCropBottomField.value = 0f;

        UpdateExtendCropPreview();
    }
    public void CloseExtendCropWindow()
    {
        CloseDialogBox(extendCropWindow);
    }

    private void UpdateExtendCropPreview()
    {
        int left = (int)extendCropLeftField.value;
        int right = (int)extendCropRightField.value;
        int up = (int)extendCropTopField.value;
        int down = (int)extendCropBottomField.value;

        extendCropLeftField.min = 1f - (fileManager.currentFile.width + right);
        extendCropRightField.min = 1f - (fileManager.currentFile.width + left);
        extendCropTopField.min = 1f - (fileManager.currentFile.height + down);
        extendCropBottomField.min = 1f - (fileManager.currentFile.height + up);

        Texture2D render = fileManager.currentFile.Render(animationManager.currentFrameIndex);
        render.Apply();

        extendCropPreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(Tex2DSprite.Extend(render, left, right, up, down), 2f),
            Tex2DSprite.CheckerboardBackground(fileManager.currentFile.width + left + right, fileManager.currentFile.height + up + down)));
    }

    public void ConfirmExtendCropWindow()
    {
        int left = (int)extendCropLeftField.value;
        int right = (int)extendCropRightField.value;
        int up = (int)extendCropTopField.value;
        int down = (int)extendCropBottomField.value;

        imageEditManager.ExtendFile(left, right, up, down);

        CloseExtendCropWindow();
    }

    public void OpenScaleWindow()
    {
        OpenDialogBox(scaleWindow);

        updatedScaleSettingsThisFrame = true;
        scaleWidthField.value = fileManager.currentFile.width;
        scaleHeightField.value = fileManager.currentFile.height;
        updatedScaleSettingsThisFrame = false;

        UpdateScaleWidthHeight();
    }
    public void CloseScaleWindow()
    {
        CloseDialogBox(scaleWindow);
    }

    private void UpdateScaleWidthHeight()
    {
        if (!updatedScaleSettingsThisFrame)
        {
            updatedScaleSettingsThisFrame = true;

            int width = (int)scaleWidthField.value;
            int height = (int)scaleHeightField.value;

            scaleXScalarField.min = 1f / fileManager.currentFile.width;
            scaleYScalarField.min = 1f / fileManager.currentFile.height;

            scaleXScalarField.value = (float)width / fileManager.currentFile.width;
            scaleYScalarField.value = (float)height / fileManager.currentFile.height;

            UpdateScalePreview();
        }
    }

    private void UpdateScaleScalars()
    {
        if (!updatedScaleSettingsThisFrame)
        {
            updatedScaleSettingsThisFrame = true;

            float xScalar = scaleXScalarField.value;
            float yScalar = scaleYScalarField.value;

            scaleWidthField.value = Mathf.Round(xScalar * fileManager.currentFile.width);
            scaleHeightField.value = Mathf.Round(yScalar * fileManager.currentFile.height);

            UpdateScalePreview();
        }
    }

    private void UpdateScalePreview()
    {
        int width = (int)scaleWidthField.value;
        int height = (int)scaleHeightField.value;

        Texture2D render = fileManager.currentFile.Render(animationManager.currentFrameIndex);
        render.Apply();

        scalePreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(Tex2DSprite.Scale(render, width, height), 2f), Tex2DSprite.CheckerboardBackground(width, height)));
    }

    public void ConfirmScaleWindow()
    {
        int width = (int)scaleWidthField.value;
        int height = (int)scaleHeightField.value;

        imageEditManager.ScaleFile(width, height);

        CloseScaleWindow();
    }

    public void OpenGridWindow()
    {
        OpenDialogBox(gridWindow);

        gridWidthField.value = gridManager.width;
        gridHeightField.value = gridManager.height;
        gridOffsetXField.value = gridManager.xOffset;
        gridOffsetYField.value = gridManager.yOffset;

        gridWidthField.max = fileManager.currentFile.width;
        gridHeightField.max = fileManager.currentFile.height;

        UpdateGridPreview();
    }
    public void CloseGridWindow()
    {
        CloseDialogBox(gridWindow);
    }

    public void ConfirmGridWindow()
    {
        int width = (int)gridWidthField.value;
        int height = (int)gridHeightField.value;
        int xOffset = (int)gridOffsetXField.value;
        int yOffset = (int)gridOffsetYField.value;

        gridManager.SetGrid(width, height, xOffset, yOffset);

        CloseGridWindow();
    }

    private void UpdateGridPreview()
    {
        int width = (int)gridWidthField.value;
        int height = (int)gridHeightField.value;
        int xOffset = (int)gridOffsetXField.value;
        int yOffset = (int)gridOffsetYField.value;
        bool on = gridEnableButton.on;

        Texture2D render = fileManager.currentFile.Render(animationManager.currentFrameIndex);
        render.Apply();

        gridPreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(render, 2f), Tex2DSprite.CheckerboardBackground(fileManager.currentFile.width,
            fileManager.currentFile.height)));

        gridPreviewGridManager.SetOnOffNoDisplayUpdate(on);
        gridPreviewGridManager.SetGrid(width, height, xOffset, yOffset);
    }

    public void OpenOutlineWindow()
    {
        OpenDialogBox(outlineWindow);

        UpdateOutlinePreview();
    }
    public void CloseOutlineWindow()
    {
        outlineColourField.CloseColourPicker();

        CloseDialogBox(outlineWindow);
    }

    public void ConfirmOutlineWindow()
    {
        bool outside = outlineOutsideToggle.on;
        OutlineSideFill sideFill = new OutlineSideFill(outlineTopLeftToggle.on, outlineTopMiddleToggle.on, outlineTopRightToggle.on, outlineMiddleLeftToggle.on, outlineMiddleRightToggle.on,
            outlineBottomLeftToggle.on, outlineBottomMiddleToggle.on, outlineBottomRightToggle.on);

        foreach (Layer layer in layerManager.selectedLayers)
        {
            ((NormalLayer)layer).SetTexture(animationManager.currentFrameIndex, Tex2DSprite.Outline(layer[animationManager.currentFrameIndex].texture, outlineColourField.colour, outside, sideFill),
                AnimFrameRefMode.NewKeyFrame);
        }
        drawingArea.UpdateDrawing();

        CloseOutlineWindow();
    }

    private void UpdateOutlinePreview()
    {
        bool outside = outlineOutsideToggle.on;
        OutlineSideFill sideFill = new OutlineSideFill(outlineTopLeftToggle.on, outlineTopMiddleToggle.on, outlineTopRightToggle.on, outlineMiddleLeftToggle.on, outlineMiddleRightToggle.on,
            outlineBottomLeftToggle.on, outlineBottomMiddleToggle.on, outlineBottomRightToggle.on);

        File fileCopy = new File(fileManager.currentFile);
        foreach (int layer in layerManager.selectedLayerIndices)
        {
            ((NormalLayer)fileCopy.layers[layer]).SetTexture(animationManager.currentFrameIndex, Tex2DSprite.Outline(fileCopy.layers[layer][animationManager.currentFrameIndex].texture, outlineColourField.colour,
                outside, sideFill), AnimFrameRefMode.NewKeyFrame);
        }

        Texture2D render = fileCopy.Render(animationManager.currentFrameIndex);
        render.Apply();

        outlinePreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(render, 2f), Tex2DSprite.CheckerboardBackground(fileManager.currentFile.width,
            fileManager.currentFile.height)));
    }

    public void OpenReplaceColourWindow()
    {
        OpenDialogBox(replaceColourWindow);

        UpdateReplaceColourPreview();
    }
    public void CloseReplaceColourWindow()
    {
        replaceColourReplaceWithField.CloseColourPicker();
        replaceColourToReplaceField.CloseColourPicker();

        CloseDialogBox(replaceColourWindow);
    }

    public void ConfirmReplaceColourWindow()
    {
        Color toReplace = replaceColourToReplaceField.colour;
        Color replaceWith = replaceColourReplaceWithField.colour;

        foreach (Layer layer in layerManager.selectedLayers)
        {
            ((NormalLayer)layer).SetTexture(animationManager.currentFrameIndex, Tex2DSprite.ReplaceColour(layer[animationManager.currentFrameIndex].texture, toReplace, replaceWith),
                AnimFrameRefMode.NewKeyFrame);
        }
        drawingArea.UpdateDrawing();

        CloseReplaceColourWindow();
    }

    private void UpdateReplaceColourPreview()
    {
        Color toReplace = replaceColourToReplaceField.colour;
        Color replaceWith = replaceColourReplaceWithField.colour;

        File fileCopy = new File(fileManager.currentFile);
        foreach (int layer in layerManager.selectedLayerIndices)
        {
            ((NormalLayer)fileCopy.layers[layer]).SetTexture(animationManager.currentFrameIndex, Tex2DSprite.ReplaceColour(fileCopy.layers[layer][animationManager.currentFrameIndex].texture, toReplace,
                replaceWith), AnimFrameRefMode.NewKeyFrame);
        }

        Texture2D render = fileCopy.Render(animationManager.currentFrameIndex);
        render.Apply();

        replaceColourPreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(render, 2f), Tex2DSprite.CheckerboardBackground(fileManager.currentFile.width,
            fileManager.currentFile.height)));
    }

    public void OpenImportPACWindow(File file)
    {
        importPACFile = file;

        OpenDialogBox(importPACWindow);

        importPACLayersToggleGroup.DestroyToggles();

        for (int i = 0; i < file.layers.Count; i++)
        {
            UIToggleButton button = Instantiate(uiToggleButtonPrefab, importPACLayersViewport.scrollingArea).GetComponent<UIToggleButton>();

            button.SetText(file.layers[i].name);
            button.SetTextAlignment(TextAnchor.MiddleLeft);
            button.height = 0.6f;
            button.width = 3f;
            button.SetImages(null, null, null, null);

            button.transform.localPosition = new Vector3(0f, -i * button.height, 0f);

            button.SubscribeToLeftClick(UpdateImportPACPreview);

            importPACLayersToggleGroup.Add(button);
        }

        for (int i = 0; i < importPACLayersToggleGroup.Count; i++)
        {
            importPACLayersToggleGroup.Press(i);
        }

        importPACLayersViewport.RefreshViewport();
        importPACScrollbar.SetScrollAmount(1f);
        UpdateImportPACPreview();
    }

    public void CloseImportPACWindow()
    {
        CloseDialogBox(importPACWindow);
    }

    public void ConfirmImportPACWindow()
    {
        int[] selectedLayerIndices = importPACLayersToggleGroup.selectedIndices;
        for (int i = selectedLayerIndices.Length - 1; i >= 0; i--)
        {
            fileManager.currentFile.AddLayer(importPACFile.layers[selectedLayerIndices[i]]);
            layerManager.OnLayersChanged();
        }

        CloseImportPACWindow();
    }

    public void UpdateImportPACPreview()
    {
        importPACPreview.sprite = Tex2DSprite.Tex2DToSprite(Tex2DSprite.Overlay(Tex2DSprite.Scale(importPACFile.RenderLayers(importPACLayersToggleGroup.selectedIndices,
            animationManager.currentFrameIndex), 2f), Tex2DSprite.CheckerboardBackground(fileManager.currentFile.width, fileManager.currentFile.height)));
    }

    public void OpenLayerPropertiesWindow(int layerIndex)
    {
        openedWindow = false;

        layerPropertiesLayerIndex = layerIndex;

        OpenDialogBox(layerPropertiesWindow);

        layerNameTextbox.SetText(fileManager.currentFile.layers[layerIndex].name);
        layerOpacityField.value = fileManager.currentFile.layers[layerIndex].opacity * 255f;
        layerBlendModeDropdown.Select(fileManager.currentFile.layers[layerIndex].blendMode.ToString().ToLower());

        openedWindow = true;
    }
    public void CloseLayerPropertiesWindow()
    {
        CloseDialogBox(layerPropertiesWindow, () => openedWindow = false);
    }

    private void UpdateLayerPropertiesPreview()
    {
        if (!openedWindow)
        {
            return;
        }

        string layerName = layerNameTextbox.text;
        float opacity = layerOpacityField.value / 255f;
        BlendMode blendMode = BlendMode.StringToBlendMode(layerBlendModeDropdown.selectedOption);

        layerManager.SetLayerOpacity(layerPropertiesLayerIndex, opacity);
        layerManager.SetLayerBlendMode(layerPropertiesLayerIndex, blendMode);
        layerManager.SetLayerName(layerPropertiesLayerIndex, layerName);
    }

    public void OpenBrushSettingsWindow()
    {
        OpenDialogBox(brushSettingsWindow);

        brushSettingsShapeToggleGroup.Press((int)toolbar.brushShape);
        brushSettingsSizeField.max = toolbar.maxBrushSize;
        brushSettingsSizeField.value = toolbar.brushSize;
    }
    public void CloseBrushSettingsWindow()
    {
        CloseDialogBox(brushSettingsWindow);
    }

    public void UpdateBrushSettingsShape()
    {
        string brushShape = brushSettingsShapeToggleGroup.selectedToggles[0].toggleName;
        if (brushShape == "circle")
        {
            toolbar.brushShape = BrushShape.Circle;
        }
        else if (brushShape == "square")
        {
            toolbar.brushShape = BrushShape.Square;
        }
        else if (brushShape == "diamond")
        {
            toolbar.brushShape = BrushShape.Diamond;
        }
        else if (brushShape == "open")
        {
            ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Image File ", "png", "jpeg", "jpg") };
            string[] fileNames = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (fileNames.Length > 0)
            {
                toolbar.LoadCustomBrush(Tex2DSprite.LoadFromFile(fileNames[0]));
            }

            toolbar.brushShape = BrushShape.Custom;
        }
        else
        {
            throw new System.Exception("Unknown / unimplemented brush shape: " + brushShape);
        }
    }

    public void OpenKeyboardShortcutsWindow()
    {
        OpenDialogBox(keyboardShortcutsWindow);
    }
    public void CloseKeyboardShortcutsWindow()
    {
        CloseDialogBox(keyboardShortcutsWindow);
    }

    public void OpenUnsavedChangesWindow(int fileIndex)
    {
        unsavedChangesFileIndex = fileIndex;

        unsavedChangesModalWindow = OpenModalWindow("Unsaved Changes", "There are unsaved changes.\n\nWould you like to save?");
        unsavedChangesModalWindow.AddButton("Yes", UnsavedChangesYes);
        unsavedChangesModalWindow.AddButton("No", UnsavedChangesNo);
        unsavedChangesModalWindow.AddCloseButton("Cancel");
    }

    public void UnsavedChangesYes()
    {
        fileManager.SaveFileDialog(fileManager.files[unsavedChangesFileIndex]);
        fileManager.CloseFile(unsavedChangesFileIndex);

        CloseModalWindow(unsavedChangesModalWindow);
    }
    public void UnsavedChangesNo()
    {
        fileManager.CloseFile(unsavedChangesFileIndex);

        CloseModalWindow(unsavedChangesModalWindow);
    }

    public UIModalWindow OpenModalWindow()
    {
        UIModalWindow modalWindow = Instantiate(modalWindowPrefab, modalWindowsParentObj).GetComponent<UIModalWindow>();

        blurPanel.EnableDisable(true);

        modalWindow.transform.localScale = Vector3.zero;
        LeanTween.scale(modalWindow.gameObject, Vector3.one, popUpDuration).setEase(popUpOpenScaleCurve);

        return modalWindow;
    }
    public UIModalWindow OpenModalWindow(string title, string message)
    {
        UIModalWindow modalWindow = OpenModalWindow();

        modalWindow.SetTitle(title);
        modalWindow.SetMessage(message);

        return modalWindow;
    }
    public UIModalWindow OpenModalWindow(string title, string message, string[] buttonTexts, UnityAction[] buttonOnClicks)
    {
        if (buttonTexts.Length != buttonOnClicks.Length)
        {
            throw new System.Exception("Number of button texts (" + buttonTexts.Length + ") does not equal number of button onClicks (" + buttonOnClicks.Length + ")");
        }

        UIModalWindow modalWindow = OpenModalWindow(title, message);

        for (int i = 0; i < buttonTexts.Length; i++)
        {
            modalWindow.AddButton(buttonTexts[i], buttonOnClicks[i]);
        }

        return modalWindow;
    }

    public void CloseModalWindow(UIModalWindow modalWindow)
    {
        LeanTween.scale(modalWindow.gameObject, Vector3.zero, popUpDuration).setEase(popUpCloseScaleCurve).setOnComplete(() =>
        {
            modalWindow.Close();
            blurPanel.EnableDisable(false);
        });
    }
}
