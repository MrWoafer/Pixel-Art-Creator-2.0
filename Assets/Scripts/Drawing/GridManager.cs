using System.Collections.Generic;
using PAC.Files;
using PAC.Utils;
using UnityEngine;

namespace PAC.Drawing
{
    /// <summary>
    /// Handles drawing a grid over the image when the grid is enabled.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool startOn = true;
        [Min(0f)]
        public float lineThickness = 0.25f;

        [Header("References")]
        [SerializeField]
        private GameObject gridLinePrefab;

        public int width { get; private set; } = 0;
        public int height { get; private set; } = 0;
        public int xOffset { get; private set; } = 0;
        public int yOffset { get; private set; } = 0;

        public bool on { get; private set; } = true;

        private List<Transform> gridLines = new List<Transform>();

        private FileManager fileManager;

        private void Awake()
        {
            fileManager = Finder.fileManager;
        }

        private void Start()
        {
            fileManager.SubscribeToFileSwitched(DisplayGrid);

            SetOnOff(startOn);
        }

        private void DisplayGrid()
        {
            ClearGrid();

            if (!on)
            {
                return;
            }

            if (width > 0 && height > 0)
            {
                int numOfVerticalLines = Mathf.CeilToInt(fileManager.currentFile.width / (float)width);
                if (Functions.Mod(xOffset, width) == 0 || Functions.Mod(fileManager.currentFile.width, width) != 0)
                {
                    numOfVerticalLines -= 1;
                }

                int numOfHorizontalLines = Mathf.CeilToInt(fileManager.currentFile.height / (float)height);
                if (Functions.Mod(yOffset, height) == 0 || Functions.Mod(fileManager.currentFile.height, height) != 0)
                {
                    numOfHorizontalLines -= 1;
                }

                int maxFileWidthHeight = Mathf.Max(fileManager.currentFile.width, fileManager.currentFile.height);

                for (int i = 1; i <= numOfVerticalLines; i++)
                {
                    Transform gridLine = Instantiate(gridLinePrefab, transform).GetComponent<Transform>();
                    gridLine.transform.localScale = new Vector3(lineThickness, (float)fileManager.currentFile.height / maxFileWidthHeight, 1f);
                    gridLines.Add(gridLine);

                    if (Functions.Mod(xOffset, width) == 0)
                    {
                        gridLine.localPosition = new Vector3(i * (float)width / fileManager.currentFile.width - 0.5f, 0f, 0f);
                    }
                    else
                    {
                        gridLine.localPosition = new Vector3((i - 1f) * (float)width / fileManager.currentFile.width - 0.5f + Functions.Mod(xOffset, width) / (float)fileManager.currentFile.width, 0f, 0f);
                    }
                }

                for (int i = 1; i <= numOfHorizontalLines; i++)
                {
                    Transform gridLine = Instantiate(gridLinePrefab, transform).GetComponent<Transform>();
                    gridLine.transform.localScale = new Vector3((float)fileManager.currentFile.width / maxFileWidthHeight, lineThickness, 1f);
                    gridLines.Add(gridLine);

                    if (Functions.Mod(yOffset, height) == 0)
                    {
                        gridLine.localPosition = new Vector3(0f, i * (float)height / fileManager.currentFile.height - 0.5f, 0f);
                    }
                    else
                    {
                        gridLine.localPosition = new Vector3(0f, (i - 1f) * (float)height / fileManager.currentFile.height - 0.5f + Functions.Mod(yOffset, height) / (float)fileManager.currentFile.height, 0f);
                    }
                }
            }
        }

        private void ClearGrid()
        {
            foreach (Transform gridLine in gridLines)
            {
                Destroy(gridLine.gameObject);
            }

            gridLines = new List<Transform>();
        }

        public void SetGrid(int width, int height, int xOffset, int yOffset)
        {
            Debug.Log("Setting grid with dimensions: " + width + "x" + height + "; and offset: (" + xOffset + ", " + yOffset + ")");

            this.width = width;
            this.height = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;

            DisplayGrid();
        }

        public void SetOnOff(bool on)
        {
            if (on)
            {
                this.on = true;
                DisplayGrid();
            }
            else
            {
                this.on = false;
                ClearGrid();
            }
        }

        public void SetOnOffNoDisplayUpdate(bool on)
        {
            this.on = on;
        }
    }
}
