using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

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
        if (!Application.isEditor && !Screen.fullScreen)
        {
            ShowWindowAsync(GetActiveWindow().ToInt32(), 3);
        }
    }
}
