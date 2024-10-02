using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SFB;
using System.IO;

public class FileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Min(1)]
    private int startingFileWidth = 16;
    [SerializeField]
    [Min(1)]
    private int startingFileHeight = 16;

    public const string defaultFileName = "Untitled Image";

    public List<File> files { get; private set; } = new List<File>();
    public int currentFileIndex { get; private set; } = 0;
    public File currentFile => files[currentFileIndex];
    [HideInInspector]
    public File previousFile = null;

    private DrawingArea drawingArea;
    private AnimationManager animationManager;

    private InputSystem inputSystem;
    private LayerManager layerManager;
    private UIManager uiManager;

    private UnityEvent onFileSwitch = new UnityEvent();
    private UnityEvent onFilesChanged = new UnityEvent();

    private void Awake()
    {
        drawingArea = Finder.drawingArea;
        inputSystem = Finder.inputSystem;
        layerManager = Finder.layerManager;
        animationManager = Finder.animationManager;
        uiManager = Finder.uiManager;
    }

    private void Start()
    {
        inputSystem.SubscribeToGlobalKeyboard(KeyboardShortcut);

        /*AddFile(new File(defaultFileName, startingFileWidth, startingFileHeight));

        /// For testing TileLayer
        currentFile.AddTileLayer();
        TileLayer[] linkedTileLayers = new TileLayer[] { (TileLayer)currentFile.layers[0] };
        File file = File.OpenFile("D:\\Games\\Pixel-Art-Creator-2.0\\Test Images\\Cube.png");
        //((TileLayer)currentFile.layers[0]).AddTile(new Tile(file, IntVector2.zero, linkedTileLayers));
        //((TileLayer)currentFile.layers[0]).AddTile(new Tile(file, new IntVector2(45, 10), linkedTileLayers));
        files[0].AddTile(file, new IntVector2(0, 0), 0);
        files[0].AddTile(file, new IntVector2(45, 10), 0);
        files[0].AddTile(file, new IntVector2(110, 4), 0);

        AddFile(file);
        ///

        SwitchToFile(1);*/
        //AddFile(new File(defaultFileName, 16, 16));
        AddFile(new File(defaultFileName, 64, 64));
        SwitchToFile(0);

        /// Open file from 'Open with' in File Explorer
        string[] args = System.Environment.GetCommandLineArgs();
        if (args.Length >= 2 && !Application.isEditor)
        {
            OpenFile(args[1]);
        }
    }

    public bool AddFile(File file)
    {
        if (file == null)
        {
            return false;
        }
        if (files.Contains(file))
        {
            return false;
        }

        files.Add(file);

        onFilesChanged.Invoke();

        return true;
    }

    /// <summary>
    /// Will close the file if there are no unsaved changes. Otherwise it will open a dialog box asking if you want to save.
    /// </summary>
    public bool TryCloseFile(File file)
    {
        int fileIndex = files.IndexOf(file);

        if (fileIndex == -1)
        {
            return false;
        }

        return TryCloseFile(fileIndex);
    }
    /// <summary>
    /// Will close the file if there are no unsaved changes. Otherwise it will open a dialog box asking if you want to save.
    /// </summary>
    public bool TryCloseFile(int fileIndex)
    {
        if (files[fileIndex].savedSinceLastEdit)
        {
            CloseFile(fileIndex);
            return true;
        }

        uiManager.OpenUnsavedChangesWindow(fileIndex);
        return false;
    }
    /// <summary>
    /// Closes the file, with no checks for unsaved changes.
    /// </summary>
    public bool CloseFile(File file)
    {
        int fileIndex = files.IndexOf(file);

        if (fileIndex == -1)
        {
            return false;
        }

        return CloseFile(fileIndex) != null;
    }
    /// <summary>
    /// Closes the file, with no checks for unsaved changes.
    /// </summary>
    public File CloseFile(int fileIndex)
    {
        if (fileIndex < 0 || fileIndex >= files.Count)
        {
            return null;
        }

        File removedFile = files[fileIndex];

        previousFile = currentFile;

        files.RemoveAt(fileIndex);

        if (files.Count == 0)
        {
            NewFile(removedFile.width, removedFile.height);
        }

        currentFileIndex = Mathf.Min(fileIndex, files.Count - 1);

        SwitchToFile(currentFileIndex);

        return removedFile;
    }

    public void NewFile(int width, int height)
    {
        AddFile(new File(defaultFileName, width, height));
        SwitchToFile(files.Count - 1);
    }

    public void OpenFileDialog()
    {
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Any File ", "png", "jpeg", "jpg", "pac"), new ExtensionFilter("Image File ", "png", "jpeg", "jpg"),
            new ExtensionFilter("Pixel Art Creator File ", "pac") };
        string[] fileNames = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (fileNames.Length > 0)
        {
            OpenFile(fileNames[0]);
        }
    }
    public bool OpenFile(File file)
    {
        if (files.Contains(file))
        {
            SwitchToFile(files.IndexOf(file));
        }
        else
        {
            if (!AddFile(file))
            {
                return false;
            }

            SwitchToFile(files.Count - 1);
        }

        inputSystem.Untarget();
        inputSystem.LockForAFrame();

        return true;
    }
    public bool OpenFile(string filePath)
    {
        if (OpenFile(File.OpenFile(filePath)))
        {
            Debug.Log("Opened file: " + filePath);
            return true;
        }
        return false;
    }

    public void ImportDialog()
    {
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Any File ", "png", "jpeg", "jpg", "pac"), new ExtensionFilter("Image File ", "png", "jpeg", "jpg"),
            new ExtensionFilter("Pixel Art Creator File ", "pac") };
        string[] fileNames = StandaloneFileBrowser.OpenFilePanel("Import", "", extensions, false);
        if (fileNames.Length > 0)
        {
            if (Path.GetExtension(fileNames[0]) == ".png" || Path.GetExtension(fileNames[0]) == ".jpg" || Path.GetExtension(fileNames[0]) == ".jpeg")
            {
                Texture2D tex = Tex2DSprite.LoadFromFile(fileNames[0]);
                if (tex.width != currentFile.width || tex.height != currentFile.height)
                {
                    uiManager.OpenModalWindow("Import", "The size of the imported image must match the size of the file.").AddCloseButton("Okay");
                }
                else
                {
                    currentFile.ImportImage(fileNames[0]);
                }
            }
            else if (Path.GetExtension(fileNames[0]) == ".pac")
            {
                File file = File.OpenPAC(fileNames[0]);
                if (file.width != currentFile.width || file.height != currentFile.height)
                {
                    uiManager.OpenModalWindow("Import", "The size of the imported image must match the size of the file.").AddCloseButton("Okay");
                }
                else
                {
                    uiManager.OpenImportPACWindow(file);
                }
            }
            ReloadFile();
        }
    }

    public void ExportCurrentFrameDialog()
    {
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("PNG ", "png"), new ExtensionFilter("JPEG ", "jpeg", "jpg") };
        string fileName = StandaloneFileBrowser.SaveFilePanel("Export Current Frame", "", currentFile.name, extensions);
        if (fileName != "")
        {
            ExportCurrentFrame(animationManager.currentFrameIndex, fileName);
        }

        inputSystem.Untarget();
        inputSystem.LockForAFrame();
    }
    public bool ExportCurrentFrame(int frameIndex, string filePath)
    {
        return ExportFrame(frameIndex, currentFileIndex, filePath);
    }
    public bool ExportFrame(int frameIndex, int fileIndex, string filePath)
    {
        if (fileIndex < 0 || fileIndex >= files.Count)
        {
            throw new System.Exception("There is no file with index " + fileIndex.ToString());
        }
        return files[fileIndex].ExportFrame(frameIndex, filePath);
    }

    public void ExportCurrentAnimationDialog()
    {
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("PNG ", "png"), new ExtensionFilter("JPEG ", "jpeg", "jpg") };
        string fileName = StandaloneFileBrowser.SaveFilePanel("Export Animation", "", currentFile.name, extensions);
        if (fileName != "")
        {
            ExportCurrentAnimation(fileName);
        }

        inputSystem.Untarget();
        inputSystem.LockForAFrame();
    }
    public bool ExportCurrentAnimation(string filePath)
    {
        return ExportAnimation(currentFileIndex, filePath);
    }
    public bool ExportAnimation(int fileIndex, string filePath)
    {
        if (fileIndex < 0 || fileIndex >= files.Count)
        {
            throw new System.Exception("There is no file with index " + fileIndex.ToString());
        }
        return files[fileIndex].ExportAnimation(filePath);
    }

    public void SaveCurrentFileDialog()
    {
        SaveFileDialog(currentFile);
    }
    public void SaveFileDialog(File file)
    {
        if (file.mostRecentSavePath == null)
        {
            ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Pixel Art Creator File ", "pac") };
            string fileName = StandaloneFileBrowser.SaveFilePanel("Save", "", file.name, extensions);
            if (fileName != "")
            {
                SaveAsFile(file, fileName);
            }

            inputSystem.Untarget();
            inputSystem.LockForAFrame();
        }
        else
        {
            file.SaveAsPAC(file.mostRecentSavePath);
        }
    }

    public void SaveAsCurrentFileDialog()
    {
        SaveAsFileDialog(currentFile);
    }
    public void SaveAsFileDialog(File file)
    {
        ExtensionFilter[] extensions = new ExtensionFilter[] { new ExtensionFilter("Pixel Art Creator File ", "pac") };
        string fileName = StandaloneFileBrowser.SaveFilePanel("Save As", "", file.name, extensions);
        if (fileName != "")
        {
            SaveAsFile(file, fileName);
        }

        inputSystem.Untarget();
        inputSystem.LockForAFrame();
    }

    public void SaveAsCurrentFile(string filePath)
    {
        SaveAsFile(currentFile, filePath);
    }
    public void SaveAsFile(File file, string filePath)
    {
        file.SaveAsPAC(filePath);
    }
    public void SaveAsFile(int fileIndex, string filePath)
    {
        if (fileIndex < 0 || fileIndex >= files.Count)
        {
            throw new System.Exception("There is no file with index " + fileIndex.ToString());
        }
        SaveAsFile(files[fileIndex], filePath);
    }

    public void ReloadFile()
    {
        SwitchToFile(currentFileIndex);
    }
    public void SwitchToFile(int fileIndex)
    {
        if (fileIndex < 0 || fileIndex >= files.Count)
        {
            throw new System.Exception("There is no file with index " + fileIndex.ToString());
        }

        if (currentFile != files[fileIndex])
        {
            previousFile = currentFile;
        }
        
        currentFileIndex = fileIndex;

        onFileSwitch.Invoke();
        onFilesChanged.Invoke();
    }

    private void KeyboardShortcut()
    {
        if (Application.isEditor)
        {
            return;
        }

        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("save")))
        {
            SaveCurrentFileDialog();
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("save as")))
        {
            SaveAsCurrentFileDialog();
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("export")))
        {
            ExportCurrentFrameDialog();
        }
        if (inputSystem.globalKeyboardTarget.OneIsHeldExactly(KeyboardShortcuts.GetShortcutsFor("open")))
        {
            OpenFileDialog();
        }
    }

    public void SubscribeToFileSwitched(UnityAction call)
    {
        onFileSwitch.AddListener(call);
    }
    public void SubscribeToFilesChanged(UnityAction call)
    {
        onFilesChanged.AddListener(call);
    }
}
