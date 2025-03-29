using PAC.Geometry;

using UnityEngine;

namespace PAC.Screen
{
    public static class ScreenInfo
    {
        /// <summary>
        /// Returns the width of the screen in world coords.
        /// </summary>
        public static float screenWorldWidth
        {
            get
            {
                return Camera.main.orthographicSize * 2f * UnityEngine.Screen.width / UnityEngine.Screen.height;
            }
        }

        /// <summary>
        /// Returns the height of the screen in world coords.
        /// </summary>
        public static float screenWorldHeight
        {
            get
            {
                return Camera.main.orthographicSize * 2f;
            }
        }

        /// <summary>
        /// Returns the colour of the screen pixel at the given coords. Best results when called after end of frame, using 'yield return new WaitForEndOfFrame()'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetScreenPixelColour(IntVector2 coords)
        {
            return GetScreenPixelColour(coords.x, coords.y);
        }
        /// <summary>
        /// Returns the colour of the screen pixel at coords (x, y). Best results when called after end of frame, using 'yield return new WaitForEndOfFrame()'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Color GetScreenPixelColour(int x, int y)
        {
            Texture2D tex = new Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), 0, 0);
            tex.Apply();

            return tex.GetPixel(x, y);
        }
    }
}
