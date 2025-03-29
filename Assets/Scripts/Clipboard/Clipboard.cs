using PAC.Animation;
using PAC.Colour.Compositing;
using PAC.DataStructures;
using PAC.Drawing;
using PAC.Extensions;
using PAC.Files;
using PAC.ImageEditing;
using PAC.Layers;

using UnityEngine;

namespace PAC.Clipboard
{
    /// <summary>
    /// A class to handle copy/paste functionality.
    /// Not yet properly implemented.
    /// </summary>
    public class Clipboard : MonoBehaviour
    {
        [Header("Clipboard")]
        [SerializeField]
        /// <summary>The texture last copied to the clipboard.</summary>
        private Texture2D copiedTexture;
        /// <summary>
        /// The coord of the bottom left of the last texture copied to clipboard, with respect to the file it was copied from.
        /// </summary>
        private IntVector2 copiedTexturePos;

        private FileManager fileManager;
        private LayerManager layerManager;
        private DrawingArea drawingArea;
        private AnimationManager animationManager;

        private void Awake()
        {
            fileManager = Finder.fileManager;
            layerManager = Finder.layerManager;
            drawingArea = Finder.drawingArea;
            animationManager = Finder.animationManager;
        }

        public void Cut()
        {
            Copy();
            drawingArea.DeleteSelection();
        }

        public void Copy()
        {
            if (drawingArea.hasSelection)
            {
                copiedTexture = BlendMode.Multiply.Blend(
                    layerManager.selectedLayer[animationManager.currentFrameIndex].texture,
                    drawingArea.selectionMask
                    ).ExtendCrop(drawingArea.selectionRect);
                copiedTexturePos = drawingArea.selectionRect.bottomLeft;

                Debug.Log("Copied.");
            }
        }

        public void Paste()
        {
            if (copiedTexture)
            {
                IntVector2 bottomLeft = copiedTexturePos;
                bottomLeft -= IntVector2.Max(new IntVector2(copiedTexture.width + bottomLeft.x - fileManager.currentFile.width, copiedTexture.height + bottomLeft.y - fileManager.currentFile.height),
                    IntVector2.zero);
                bottomLeft = IntVector2.Max(bottomLeft, IntVector2.zero);

                layerManager.AddLayer(
                    BlendMode.Normal.Blend(
                        copiedTexture,
                        Texture2DCreator.Transparent(fileManager.currentFile.width, fileManager.currentFile.height),
                        bottomLeft
                        )
                    );

                Debug.Log("Pasted.");
            }
        }
    }
}
