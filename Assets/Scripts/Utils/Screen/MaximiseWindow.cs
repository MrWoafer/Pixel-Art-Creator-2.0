using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PAC.Screen
{
    public class MaximiseWindow : MonoBehaviour
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(int hWnd, int nCmdShow);

        void Awake()
        {
            /// Auto maximise the window when the program opens
            /// (the Unity 'Maximized Window' fullscreen mode only works on MacOS)
            if (!Application.isEditor && !UnityEngine.Screen.fullScreen)
            {
                ShowWindowAsync(GetActiveWindow().ToInt32(), 3);
            }
        }
    }
}
