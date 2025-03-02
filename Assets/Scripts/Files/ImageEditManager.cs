using PAC.Geometry;
using PAC.Geometry.Axes;
using PAC.Layers;
using PAC.UI;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Files
{
    public class ImageEditManager : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onEdit = new UnityEvent();
        [SerializeField]
        private UnityEvent onImageSizeChanged = new UnityEvent();

        private FileManager fileManager;
        private LayerManager layerManager;
        private DialogBoxManager dialogBoxManager;

        private void Awake()
        {
            fileManager = Finder.fileManager;
            layerManager = Finder.layerManager;
            dialogBoxManager = Finder.dialogBoxManager;
        }

        public void FlipSelectedLayers(CardinalOrdinalAxis axis)
        {
            foreach (Layer selectedLayer in layerManager.selectedLayers)
            {
                selectedLayer.Flip(axis);
            }
            onEdit.Invoke();
        }
        public void FlipSelectedLayersX() => FlipSelectedLayers(CardinalAxis.Vertical);
        public void FlipSelectedLayersY() => FlipSelectedLayers(CardinalAxis.Horizontal);

        public void FlipFile(CardinalOrdinalAxis axis)
        {
            fileManager.currentFile.Flip(axis);
            onEdit.Invoke();
        }
        public void FlipFileX() => FlipFile(CardinalAxis.Vertical);
        public void FlipFileY() => FlipFile(CardinalAxis.Horizontal);

        public void RotateFile(QuadrantalAngle angle)
        {
            fileManager.currentFile.Rotate(angle);
            onEdit.Invoke();

            if (fileManager.currentFile.width != fileManager.currentFile.height && (angle == QuadrantalAngle.Clockwise90 || angle == QuadrantalAngle.Anticlockwise90))
            {
                onImageSizeChanged.Invoke();
            }
        }
        public void RotateSelectedLayers(QuadrantalAngle angle)
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
                    UIModalWindow modalWindow = dialogBoxManager.OpenModalWindow("Rotate", "For non-square images, all layers must be rotated at once.");
                    modalWindow.AddCloseButton("Okay");
                }
            }
        }
        public void RotateSelectedLayers90() => RotateSelectedLayers(QuadrantalAngle.Clockwise90);
        public void RotateSelectedLayersMinus90() => RotateSelectedLayers(QuadrantalAngle.Anticlockwise90);
        public void RotateSelectedLayers180() => RotateSelectedLayers(QuadrantalAngle._180);

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
}
