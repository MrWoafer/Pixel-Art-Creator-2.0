using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clipboard : MonoBehaviour
{
    [Header("Clipboard")]
    [SerializeField]
    private Texture2D copiedTexture;
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
            copiedTexture = Tex2DSprite.ChangeRect(Tex2DSprite.ApplyMask(layerManager.selectedLayer[animationManager.currentFrameIndex].texture, drawingArea.selectionMask), drawingArea.selectionRect);
            copiedTexturePos = new IntVector2(drawingArea.selectionRect.bottomLeft);

            Debug.Log("Copied.");
        }
    }

    public void Paste()
    {
        if (copiedTexture)
        {
            IntVector2 bottomLeft = new IntVector2(copiedTexturePos);
            bottomLeft -= IntVector2.Max(new IntVector2(copiedTexture.width + bottomLeft.x - fileManager.currentFile.width, copiedTexture.height + bottomLeft.y - fileManager.currentFile.height),
                IntVector2.zero);
            bottomLeft = IntVector2.Max(bottomLeft, IntVector2.zero);

            layerManager.AddLayer(Tex2DSprite.Overlay(copiedTexture, Tex2DSprite.BlankTexture(fileManager.currentFile.width, fileManager.currentFile.height), bottomLeft));

            Debug.Log("Pasted.");
        }
    }
}
