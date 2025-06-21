using System.Collections;

using NUnit.Framework;

using PAC.NewUI;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PAC.Tests.UI
{
    public class PixelArtRectTransform_Tests
    {
        private const int UI_ASSET_PPU = 16;

        private Canvas CreateCanvas()
        {
            Canvas canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;
            scaler.referencePixelsPerUnit = UI_ASSET_PPU;

            return canvas;
        }

        private Panel

        [UnityTest]
        public IEnumerator PixelArtRectTransform()
        {
            CreateCanvas();

            yield return new WaitForSeconds(10f);
        }
    }
}
