using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImageEditManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private UnityEvent onEdit = new UnityEvent();
    [SerializeField]
    private UnityEvent onImageSizeChanged = new UnityEvent();

    private FileManager fileManager;
    private LayerManager layerManager;
    private UIManager uiManager;

    private void Awake()
    {
        fileManager = Finder.fileManager;
        layerManager = Finder.layerManager;
        uiManager = Finder.uiManager;
    }

    public void FlipSelectedLayers(FlipDirection direction)
    {
        foreach (Layer selectedLayer in layerManager.selectedLayers)
        {
            selectedLayer.Flip(direction);
        }
        onEdit.Invoke();
    }
    public void FlipSelectedLayersX() => FlipSelectedLayers(FlipDirection.X);
    public void FlipSelectedLayersY() => FlipSelectedLayers(FlipDirection.Y);

    public void FlipFile(FlipDirection direction)
    {
        fileManager.currentFile.Flip(direction);
        onEdit.Invoke();
    }
    public void FlipFileX() => FlipFile(FlipDirection.X);
    public void FlipFileY() => FlipFile(FlipDirection.Y);

    public void RotateFile(RotationAngle angle)
    {
        fileManager.currentFile.Rotate(angle);
        onEdit.Invoke();

        if (fileManager.currentFile.width != fileManager.currentFile.height && (angle == RotationAngle._90 || angle == RotationAngle.Minus90))
        {
            onImageSizeChanged.Invoke();
        }
    }
    public void RotateSelectedLayers(RotationAngle angle)
    {
        if (layerManager.selectedLayers.Length == fileManager.currentFile.layers.Count)
        {
            fileManager.currentFile.Rotate(angle);

            if (fileManager.currentFile.width != fileManager.currentFile.height)
            {
                onImageSizeChanged.Invoke();
            }
            onEdit.Invoke();
        }
        else
        {
            if (fileManager.currentFile.width == fileManager.currentFile.height)
            {
                foreach (Layer selectedLayer in layerManager.selectedLayers)
                {
                    selectedLayer.Rotate(angle);
                }
                onEdit.Invoke();
            }
            else
            {
                UIModalWindow modalWindow = uiManager.OpenModalWindow("Rotate", "For non-square images, all layers must be rotated at once.");
                modalWindow.AddCloseButton("Okay");
            }
        }
    }
    public void RotateSelectedLayers90() => RotateSelectedLayers(RotationAngle._90);
    public void RotateSelectedLayersMinus90() => RotateSelectedLayers(RotationAngle.Minus90);
    public void RotateSelectedLayers180() => RotateSelectedLayers(RotationAngle._180);

    public void ExtendFile(int left, int right, int up, int down)
    {
        fileManager.currentFile.Extend(left, right, up, down);
        onEdit.Invoke();

        if (left != 0 || right != 0 || up != 0 || down != 0)
        {
            onImageSizeChanged.Invoke();
        }
    }
    public void ExtendFileLeft(int amount) => ExtendFile(amount, 0, 0, 0);
    public void ExtendFileRight(int amount) => ExtendFile(0, amount, 0, 0);
    public void ExtendFileUp(int amount) => ExtendFile(0, 0, amount, 0);
    public void ExtendFileDown(int amount) => ExtendFile(0, 0, 0, amount);

    public void ScaleFile(float scaleFactor)
    {
        fileManager.currentFile.Scale(scaleFactor);
        onEdit.Invoke();

        if (scaleFactor != 1f)
        {
            onImageSizeChanged.Invoke();
        }
    }
    public void ScaleFile(float xScaleFactor, float yScaleFactor)
    {
        fileManager.currentFile.Scale(xScaleFactor, yScaleFactor);
        onEdit.Invoke();

        if (xScaleFactor != 1f || yScaleFactor != 1f)
        {
            onImageSizeChanged.Invoke();
        }
    }
    public void ScaleFile(int newWidth, int newHeight)
    {
        int oldWidth = fileManager.currentFile.width;
        int oldHeight = fileManager.currentFile.height;

        fileManager.currentFile.Scale(newWidth, newHeight);
        onEdit.Invoke();

        if (newWidth != oldWidth || newHeight != oldHeight)
        {
            onImageSizeChanged.Invoke();
        }
    }

    public void SubscribeToEdit(UnityAction call)
    {
        onEdit.AddListener(call);
    }
    public void SubscribeToImageSizeChanged(UnityAction call)
    {
        onImageSizeChanged.AddListener(call);
    }
}
