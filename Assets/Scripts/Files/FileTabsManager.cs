using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FileTabsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float scrollSpeed = 1f;
    [SerializeField]
    private Vector2 fileTileStartCoords = new Vector2(0f, 0f);
    [SerializeField]
    [Min(0f)]
    private float xIncrement = 0.5f;

    [Header("References")]
    [SerializeField]
    private GameObject fileTilePrefab;

    private UIButton scrollLeft;
    private UIButton scrollRight;
    private UIViewport viewport;
    private UIToggleGroup toggleGroup;
    private FileManager fileManager;
    private UIManager uiManager;

    private List<FileTile> fileTiles = new List<FileTile>();

    public int selectedFileIndex
    {
        get
        {
            return toggleGroup.currentToggleIndex;
        }
    }
    public File selectedFile
    {
        get
        {
            return fileTiles[selectedFileIndex].file;
        }
    }

    private UnityEvent onFilesChanged = new UnityEvent();

    private void Awake()
    {
        scrollLeft = transform.Find("Scroll Left").GetComponent<UIButton>();
        scrollRight = transform.Find("Scroll Right").GetComponent<UIButton>();
        viewport = transform.Find("Viewport").GetComponent<UIViewport>();
        toggleGroup = transform.Find("Toggle Group").GetComponent<UIToggleGroup>();

        fileManager = Finder.fileManager;
        uiManager = Finder.uiManager;

        fileManager.SubscribeToFilesChanged(OnFilesChanged);
    }

    private void Start()
    {
        toggleGroup.Press(fileManager.currentFileIndex);
    }

    void Update()
    {
        if (scrollLeft.isPressed)
        {
            viewport.SetScrollAmount(viewport.scrollAmount - scrollSpeed * Time.deltaTime);
        }
        else if (scrollRight.isPressed)
        {
            viewport.SetScrollAmount(viewport.scrollAmount + scrollSpeed * Time.deltaTime);
        }
    }

    public void OnFilesChanged()
    {
        RedisplayFileTiles();
        onFilesChanged.Invoke();
    }

    private void RedisplayFileTiles()
    {
        int previouslySelectedIndex = -1;
        File previouslySelectedFile = null;
        if (toggleGroup.currentToggle != null)
        {
            previouslySelectedIndex = toggleGroup.currentToggleIndex;
            previouslySelectedFile = toggleGroup.currentToggle.GetComponent<FileTile>().file;
        }

        ClearFileTiles();

        for (int i = 0; i < fileManager.files.Count; i++)
        {
            AddFileTile(fileManager.files[i]);
        }

        int indexToSelect = 0;
        if (previouslySelectedIndex != -1)
        {
            bool foundPreviouslySelectedLayer = false;
            for (int i = 0; i < fileTiles.Count; i++)
            {
                if (fileTiles[i].file == previouslySelectedFile)
                {
                    indexToSelect = i;
                    foundPreviouslySelectedLayer = true;
                    break;
                }
            }

            if (!foundPreviouslySelectedLayer)
            {
                indexToSelect = Mathf.Clamp(previouslySelectedIndex, 0, toggleGroup.Count - 1);
            }
        }

        toggleGroup.Press(indexToSelect);
        viewport.RefreshViewport();
    }

    private void ClearFileTiles()
    {
        foreach (FileTile fileTile in fileTiles)
        {
            DestroyImmediate(fileTile.gameObject);
        }
        fileTiles = new List<FileTile>();
        toggleGroup.Clear();
    }

    private void AddFileTile(File file)
    {
        FileTile fileTile = GameObject.Instantiate(fileTilePrefab, viewport.scrollingArea).GetComponent<FileTile>();
        fileTile.SetFile(file);
        fileTile.transform.localPosition = fileTileStartCoords + new Vector2(xIncrement * fileTiles.Count, 0f);

        fileTiles.Add(fileTile);
        toggleGroup.Add(fileTile.tileToggle);

        fileTile.SubscribeToSelect(SelectFile);
        fileTile.SubscribeToClose(CloseFile);
    }

    public void SelectFile()
    {
        fileManager.SwitchToFile(toggleGroup.currentToggleIndex);
    }

    public void CloseFile()
    {
        for (int i = 0; i < fileTiles.Count; i++)
        {
            if (fileTiles[i].closed)
            {
                fileTiles[i].closed = false;
                fileManager.TryCloseFile(i);
                break;
            }
        }
        
        OnFilesChanged();
    }

    public void SubscribeToOnFilesChanged(UnityAction call)
    {
        onFilesChanged.AddListener(call);
    }
}
