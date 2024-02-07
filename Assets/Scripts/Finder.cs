using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder
{
    public static InputSystem inputSystem
    {
        get
        {
            return GameObject.Find("Input System").GetComponent<InputSystem>();
        }
    }
    public static Mouse mouse
    {
        get
        {
            return inputSystem.mouse;
        }
    }
    public static Keyboard keyboard
    {
        get
        {
            return inputSystem.keyboard;
        }
    }

    public static UndoRedoManager undoRedoManager
    {
        get
        {
            return GameObject.Find("Undo / Redo Manager").GetComponent<UndoRedoManager>();
        }
    }

    public static FileManager fileManager
    {
        get
        {
            return GameObject.Find("File Manager").GetComponent<FileManager>();
        }
    }

    public static LayerManager layerManager
    {
        get
        {
            return GameObject.Find("Layer Manager").GetComponent<LayerManager>();
        }
    }

    public static DrawingArea drawingArea
    {
        get
        {
            return GameObject.Find("Drawing Area").GetComponent<DrawingArea>();
        }
    }

    public static ColourPicker colourPicker
    {
        get
        {
            return GameObject.Find("Colour Picker").GetComponent<ColourPicker>();
        }
    }

    public static Toolbar toolbar
    {
        get
        {
            return GameObject.Find("Toolbar").GetComponent<Toolbar>();
        }
    }

    public static ImageEditManager imageEditManager
    {
        get
        {
            return GameObject.Find("File Manager").GetComponent<ImageEditManager>();
        }
    }

    public static UIManager uiManager
    {
        get
        {
            return GameObject.Find("UI Manager").GetComponent<UIManager>();
        }
    }

    public static ThemeManager themeManager
    {
        get
        {
            return GameObject.Find("UI Manager").GetComponent<ThemeManager>();
        }
    }

    public static Clipboard clipboard
    {
        get
        {
            return GameObject.Find("Undo / Redo Manager").GetComponent<Clipboard>();
        }
    }

    public static FileTabsManager fileTabsManager
    {
        get
        {
            return GameObject.Find("File Tabs").GetComponent<FileTabsManager>();
        }
    }

    public static AnimationManager animationManager
    {
        get
        {
            return GameObject.Find("Animation Manager").GetComponent<AnimationManager>();
        }
    }

    public static GridManager gridManager
    {
        get
        {
            return GameObject.Find("Grid Manager").GetComponent<GridManager>();
        }
    }

    public static TileOutlineManager tileOutlineManager
    {
        get
        {
            return GameObject.Find("Tile Outline Manager").GetComponent<TileOutlineManager>();
        }
    }
}
