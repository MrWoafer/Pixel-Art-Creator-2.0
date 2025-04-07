using PAC.Drawing;
using PAC.Input;
using PAC.UI.Components.Specialised.ColourPicker;

using UnityEngine;

namespace PAC.Managers
{
    public class Finder
    {
        public static InputSystem inputSystem => GameObject.Find("Input System").GetComponent<InputSystem>();
        public static Mouse mouse => inputSystem.mouse;
        public static Keyboard keyboard => inputSystem.keyboard;

        public static FileManager fileManager => GameObject.Find("File Manager").GetComponent<FileManager>();
        public static FileTabsManager fileTabsManager => GameObject.Find("File Tabs").GetComponent<FileTabsManager>();

        public static LayerManager layerManager => GameObject.Find("Layer Manager").GetComponent<LayerManager>();
        public static AnimationManager animationManager => GameObject.Find("Animation Manager").GetComponent<AnimationManager>();

        public static DrawingArea drawingArea => GameObject.Find("Drawing Area").GetComponent<DrawingArea>();

        public static TilesetManager tilesetManager => GameObject.Find("Tilesets").GetComponent<TilesetManager>();
        public static TileOutlineManager tileOutlineManager => GameObject.Find("Tile Outline Manager").GetComponent<TileOutlineManager>();

        public static GlobalColourPicker colourPicker => GameObject.Find("Colour Picker").GetComponent<GlobalColourPicker>();
        public static Toolbar toolbar => GameObject.Find("Toolbar").GetComponent<Toolbar>();

        public static UIManager uiManager => GameObject.Find("UI Manager").GetComponent<UIManager>();
        public static DialogBoxManager dialogBoxManager => GameObject.Find("UI Manager").GetComponent<DialogBoxManager>();
        public static ThemeManager themeManager => GameObject.Find("UI Manager").GetComponent<ThemeManager>();
        public static GridManager gridManager => GameObject.Find("Grid Manager").GetComponent<GridManager>();

        public static UndoRedoManager undoRedoManager => GameObject.Find("Undo / Redo Manager").GetComponent<UndoRedoManager>();
        public static Clipboard clipboard => GameObject.Find("Undo / Redo Manager").GetComponent<Clipboard>();
        public static ImageEditManager imageEditManager => GameObject.Find("File Manager").GetComponent<ImageEditManager>();
    }
}
