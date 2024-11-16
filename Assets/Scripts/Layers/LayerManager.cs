using System.Collections.Generic;
using System.Linq;
using PAC.Animation;
using PAC.Colour;
using PAC.Files;
using PAC.UI;
using PAC.UndoRedo;
using PAC.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace PAC.Layers
{
    public class LayerManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Vector2 layerTileStartCoords = new Vector2(0f, 0.25f);
        [SerializeField]
        [Min(0f)]
        private float yIncrement = 0.5f;

        [Header("References")]
        [SerializeField]
        private GameObject layerTilePrefab;
        public Sprite layerTypeSprite;
        public Sprite tileLayerTypeSprite;

        private List<LayerTile> layerTiles = new List<LayerTile>();
        private Dictionary<File, float> fileScrollbarScrollAmount = new Dictionary<File, float>();

        private UIToggleGroup toggleGroup;
        private FileManager fileManager;
        private UndoRedoManager undoRedoManager;
        private ImageEditManager imageEditManager;
        private AnimationManager animationManager;
        private DialogBoxManager dialogBoxManager;

        private UIViewport viewport;
        private UIScrollbar scrollbar;

        private UnityEvent onLayersChanged = new UnityEvent();
        private UnityEvent onVisibilityChange = new UnityEvent();

        /// <summary>
        /// The index of the last layer that was selected.
        /// </summary>
        public int selectedLayerIndex
        {
            get
            {
                return toggleGroup.currentToggleIndex;
            }
        }
        /// <summary>
        /// The last layer that was selected.
        /// </summary>
        public Layer selectedLayer
        {
            get
            {
                return layerTiles[selectedLayerIndex].layer;
            }
        }

        /// <summary>
        /// The indices of all selected layers, in increasing order.
        /// </summary>
        public int[] selectedLayerIndices
        {
            get
            {
                return toggleGroup.selectedIndices;
            }
        }
        /// <summary>
        /// The selected layers, in order (highest layer first etc).
        /// </summary>
        public Layer[] selectedLayers
        {
            get
            {
                return (from index in selectedLayerIndices select layerTiles[index].layer).ToArray();
            }
        }

        private void Awake()
        {
            toggleGroup = transform.Find("Toggle Group").GetComponent<UIToggleGroup>();
            fileManager = Finder.fileManager;
            undoRedoManager = Finder.undoRedoManager;
            imageEditManager = Finder.imageEditManager;
            animationManager = Finder.animationManager;
            dialogBoxManager = Finder.dialogBoxManager;

            viewport = transform.Find("Viewport").GetComponent<UIViewport>();
            scrollbar = transform.Find("Scrollbar").GetComponent<UIScrollbar>();

            fileManager.SubscribeToFileSwitched(OnFileSwitched);
        }

        private void Start()
        {
            undoRedoManager.SubscribeToUndoOrRedo(RedisplayLayerTiles);
            imageEditManager.SubscribeToEdit(OnLayersChanged);
        }

        public void OnLayersChanged()
        {
            RedisplayLayerTiles();
            onLayersChanged.Invoke();
        }

        public void OnFileSwitched()
        {
            OnLayersChanged();

            if (fileManager.previousFile != null)
            {
                fileScrollbarScrollAmount[fileManager.previousFile] = scrollbar.scrollAmount;
            }
            if (!fileScrollbarScrollAmount.ContainsKey(fileManager.currentFile))
            {
                fileScrollbarScrollAmount[fileManager.currentFile] = 1f;
            }
            scrollbar.SetScrollAmount(fileScrollbarScrollAmount[fileManager.currentFile]);
        }

        private void RedisplayLayerTiles()
        {
            int[] previouslySelectedIndices = new int[0];
            Layer[] previouslySelectedLayers = new Layer[0];
            if (toggleGroup.currentToggle != null)
            {
                previouslySelectedIndices = toggleGroup.selectedIndices;
                previouslySelectedLayers = (from index in previouslySelectedIndices select layerTiles[index].layer).ToArray();
            }

            ClearLayerTiles();

            foreach (Layer layer in fileManager.currentFile.layers)
            {
                AddLayerTile(layer);
            }

            List<int> indicesToSelect = new List<int>();
            if (previouslySelectedLayers.Length != 0)
            {
                for (int i = 0; i < layerTiles.Count; i++)
                {
                    if (previouslySelectedLayers.Contains(layerTiles[i].layer))
                    {
                        indicesToSelect.Add(i);
                    }
                }

                if (indicesToSelect.Count == 0)
                {
                    indicesToSelect.Add(Mathf.Clamp(previouslySelectedIndices[0], 0, toggleGroup.Count - 1));
                }
            }
            else
            {
                indicesToSelect.Add(0);
            }

            foreach (int index in indicesToSelect)
            {
                toggleGroup.CtrlPress(index);
            }
            viewport.RefreshViewport();
        }

        private void ClearLayerTiles()
        {
            foreach(LayerTile layerTile in layerTiles)
            {
                DestroyImmediate(layerTile.gameObject);
            }
            layerTiles = new List<LayerTile>();
            toggleGroup.Clear();
        }

        private void AddLayerTile(Layer layer)
        {
            LayerTile layerTile = GameObject.Instantiate(layerTilePrefab, viewport.scrollingArea).GetComponent<LayerTile>();
            layerTile.SetLayer(layer);
            layerTile.transform.localPosition = layerTileStartCoords - new Vector2(0f, yIncrement * layerTiles.Count);

            layerTiles.Add(layerTile);
            toggleGroup.Add(layerTile.tileToggle);

            layerTile.SubscribeToVisibilityChange(OnVisibilityChange);

            int i = layerTiles.Count - 1;
            layerTile.SubscribeToRightClick(() => { dialogBoxManager.OpenLayerPropertiesWindow(i); });
        }

        public void AddLayer()
        {
            int previouslySelectLayerIndex = selectedLayerIndex;
            fileManager.currentFile.AddNormalLayer(selectedLayerIndex);
            OnLayersChanged();
            toggleGroup.Press(previouslySelectLayerIndex);
        }
        public void AddLayer(Texture2D texture)
        {
            int previouslySelectLayerIndex = selectedLayerIndex;
            fileManager.currentFile.AddNormalLayer(texture, selectedLayerIndex);
            OnLayersChanged();
            toggleGroup.Press(previouslySelectLayerIndex);
        }

        public void AddTileLayer()
        {
            int previouslySelectLayerIndex = selectedLayerIndex;
            fileManager.currentFile.AddTileLayer(selectedLayerIndex);
            OnLayersChanged();
            toggleGroup.Press(previouslySelectLayerIndex);
        }

        public void RemoveSelectedLayers()
        {
            fileManager.currentFile.RemoveLayers(selectedLayers);

            OnLayersChanged();
        }

        public void DuplicateSelectedLayers()
        {
            Layer[] previouslySelectLayers = Functions.CopyArray(selectedLayers);

            for (int i = layerTiles.Count - 1; i >= 0; i--)
            {
                if (previouslySelectLayers.Contains(layerTiles[i].layer))
                {
                    Layer duplicate = layerTiles[i].layer.DeepCopy();
                    duplicate.name += " - Copy";

                    fileManager.currentFile.AddLayer(duplicate, i);
                }
            }
        
            OnLayersChanged();
        }

        public void FlattenSelectedLayers()
        {
            foreach (Layer layer in selectedLayers)
            {
                if (layer.layerType != LayerType.Normal)
                {
                    dialogBoxManager.OpenModalWindow("Flatten", "You can only flatten normal layers.").AddCloseButton("Okay");
                    return;
                }
            }

            if (selectedLayers.Length > 1)
            {
                int[] keyFrames = new int[0];
                foreach (Layer layer in selectedLayers)
                {
                    keyFrames = Functions.ConcatArrays(keyFrames, layer.keyFrameIndices);
                }

                foreach (int keyFrame in keyFrames)
                {
                    Texture2D tex = selectedLayers[^1][keyFrame].texture;

                    for (int i = selectedLayers.Length - 2; i >= 0; i--)
                    {
                        tex = Tex2DSprite.Overlay(selectedLayers[i][keyFrame].texture, tex);
                    }
                    ((NormalLayer)selectedLayers[^1]).SetTexture(keyFrame, tex, AnimFrameRefMode.NewKeyFrame);
                }

                fileManager.currentFile.RemoveLayers(selectedLayers[0..^1]);

                OnLayersChanged();
            }
            else
            {
                dialogBoxManager.OpenModalWindow("Flatten", "Please select at least 2 layers to flatten.").AddCloseButton("Okay");
            }
        }

        public void MoveSelectedLayersUp()
        {
            bool movedPreviousSelectedLayer;

            if (selectedLayerIndices[0] == 0)
            {
                movedPreviousSelectedLayer = false;
            }
            else
            {
                fileManager.currentFile.MoveLayer(selectedLayerIndices[0], selectedLayerIndices[0] - 1);
                movedPreviousSelectedLayer = true;
            }
        
            for (int i = 1; i < selectedLayerIndices.Length; i++)
            {
                if (selectedLayerIndices[i] > selectedLayerIndices[i - 1] + 1 || movedPreviousSelectedLayer)
                {
                    fileManager.currentFile.MoveLayer(selectedLayerIndices[i], selectedLayerIndices[i] - 1);
                    movedPreviousSelectedLayer = true;
                }
            }

            OnLayersChanged();
        }

        public void MoveSelectedLayersDown()
        {
            bool movedPreviousSelectedLayer;

            if (selectedLayerIndices[^1] == layerTiles.Count - 1)
            {
                movedPreviousSelectedLayer = false;
            }
            else
            {
                fileManager.currentFile.MoveLayer(selectedLayerIndices[^1], selectedLayerIndices[^1] + 2);
                movedPreviousSelectedLayer = true;
            }

            for (int i = selectedLayerIndices.Length - 2; i >= 0; i--)
            {
                if (selectedLayerIndices[i] < selectedLayerIndices[i + 1] - 1 || movedPreviousSelectedLayer)
                {
                    fileManager.currentFile.MoveLayer(selectedLayerIndices[i], selectedLayerIndices[i] + 2);
                    movedPreviousSelectedLayer = true;
                }
            }

            OnLayersChanged();
        }

        public void HideAllBut(Layer layer)
        {
            bool allOthersInvisible = true;
            foreach (LayerTile layerTile in layerTiles)
            {
                if (layerTile.layer != layer && layerTile.layer.visible)
                {
                    allOthersInvisible = false;
                }
            }

            layer.visible = !allOthersInvisible;
            foreach (LayerTile layerTile in layerTiles)
            {
                if (layerTile.layer != layer)
                {
                    layerTile.layer.visible = allOthersInvisible;
                }
            }

            OnLayersChanged();
        }

        public float WorldYCoordOfLayerTile(int layerTileIndex)
        {
            if (layerTileIndex < 0 || layerTileIndex >= layerTiles.Count)
            {
                throw new System.Exception("layerTileIndex out of range: " + layerTileIndex);
            }

            return layerTiles[layerTileIndex].transform.position.y;
        }

        public void SetLayerName(int layerIndex, string layerName)
        {
            if (layerIndex < 0 || layerIndex >= layerTiles.Count)
            {
                throw new System.IndexOutOfRangeException("Layer index out of range: " + layerIndex);
            }

            layerTiles[layerIndex].layer.name = layerName;

            RedisplayLayerTiles();
            onLayersChanged.Invoke();
        }

        public void SetLayerOpacity(int layerIndex, float opacity)
        {
            if (layerIndex < 0 || layerIndex >= layerTiles.Count)
            {
                throw new System.IndexOutOfRangeException("Layer index out of range: " + layerIndex);
            }

            if (opacity < 0f || opacity > 1f)
            {
                throw new System.ArgumentOutOfRangeException("Opacity must be between 0 and 1 inclusive: " + opacity);
            }

            layerTiles[layerIndex].layer.opacity = opacity;

            onLayersChanged.Invoke();
        }

        public void SetLayerBlendMode(int layerIndex, BlendMode blendMode)
        {
            if (layerIndex < 0 || layerIndex >= layerTiles.Count)
            {
                throw new System.IndexOutOfRangeException("Layer index out of range: " + layerIndex);
            }

            layerTiles[layerIndex].layer.blendMode = blendMode;

            onLayersChanged.Invoke();
        }

        private void OnVisibilityChange()
        {
            onLayersChanged.Invoke();
            onVisibilityChange.Invoke();
        }

        public void SubscribeToLayerChange(UnityAction call)
        {
            onLayersChanged.AddListener(call);
        }
        public void SubscribeToVisibilityChange(UnityAction call)
        {
            onVisibilityChange.AddListener(call);
        }
    }
}
