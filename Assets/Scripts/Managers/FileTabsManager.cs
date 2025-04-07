using System.Collections.Generic;

using PAC.Files;
using PAC.UI.Components.General;
using PAC.UI.Components.Specialised;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Managers
{
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

        private List<FileTab> fileTiles = new List<FileTab>();

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
                previouslySelectedFile = toggleGroup.currentToggle.GetComponent<FileTab>().file;
            }

            ClearFileTiles();

            for (int i = 0; i < fileManager.files.Count; i++)
            {
                AddFileTile(fileManager.files[i]);
            }

            toggleGroup.Press(fileManager.currentFileIndex);
            viewport.RefreshViewport();
        }

        private void ClearFileTiles()
        {
            foreach (FileTab fileTile in fileTiles)
            {
                DestroyImmediate(fileTile.gameObject);
            }
            fileTiles = new List<FileTab>();
            toggleGroup.Clear();
        }

        private void AddFileTile(File file)
        {
            FileTab fileTile = Instantiate(fileTilePrefab, viewport.scrollingArea).GetComponent<FileTab>();
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
}
