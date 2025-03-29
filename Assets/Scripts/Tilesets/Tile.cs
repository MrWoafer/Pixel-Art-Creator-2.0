using System.Collections.Generic;
using System.Linq;

using PAC.Files;
using PAC.Geometry;
using PAC.Layers;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Tilesets
{
    public class Tile
    {
        public File file { get; private set; }

        /// <summary>
        /// Takes in the tile layer (that this tile must be on) and returns the layer it is associated with in the tile's file.
        /// </summary>
        private Dictionary<TileLayer, Layer> tileLayersToLayersInTile = new Dictionary<TileLayer, Layer>();
        /// <summary>
        /// Takes in the layer (that must be in the tile's file) and returns the tile layer (that the tile is on) that the layer is associated with.
        /// </summary>
        private Dictionary<Layer, TileLayer> layersInTileToTileLayers = new Dictionary<Layer, TileLayer>();
        public TileLayer[] tileLayersAppearsOn { get => tileLayersToLayersInTile.Keys.ToArray(); }

        public int width { get => file.width; }
        public int height { get => file.height; }

        private IntVector2 _bottomLeft;
        public IntVector2 bottomLeft
        {
            get => _bottomLeft;
            set
            {
                IntRect oldRect = rect;
                _bottomLeft = value;
                onRectChanged.Invoke(oldRect);
            }
        }
        public IntVector2 bottomRight
        {
            get => bottomLeft + new IntVector2(width - 1, 0);
            set
            {
                bottomLeft = new IntVector2(value.x - width + 1, value.y);
            }
        }
        public IntVector2 topLeft
        {
            get => bottomLeft + new IntVector2(0, height - 1);
            set
            {
                bottomLeft = new IntVector2(value.x, value.y - height + 1);
            }
        }
        public IntVector2 topRight
        {
            get => bottomLeft + new IntVector2(width - 1, height - 1);
            set
            {
                bottomLeft = new IntVector2(value.x - width + 1, value.y - height + 1);
            }
        }
        public Vector2 centre { get => (Vector2)(bottomLeft + topRight + IntVector2.one) / 2f; }
        public IntRect rect { get => new IntRect(bottomLeft, topRight); }

        /// <summary>
        /// Called when the tile's rect is changed (e.g. moved / rotated / resized).
        /// Passes the previous rect.
        /// </summary>
        private UnityEvent<IntRect> onRectChanged = new UnityEvent<IntRect>();

        /// <param name="linkedTileLayers">
        /// The tile layers that the tile is on that are associated with each of the layers in the tile's file.
        /// Given in the same order as the layers in the tile's file.
        /// </param>
        public Tile(File file, IntVector2 bottomLeft, TileLayer[] linkedTileLayers)
        {
            if (file.layers.Count != linkedTileLayers.Length)
            {
                throw new System.Exception("There must be exactly as many linked tile layers as layers in the file.");
            }
            for (int i = 0; i < linkedTileLayers.Length - 2; i++)
            {
                for (int j = i + 1; j < linkedTileLayers.Length - 1; j++)
                {
                    if (linkedTileLayers[i] == linkedTileLayers[j])
                    {
                        throw new System.Exception("The linked tile layers cannot contain duplicates.");
                    }
                }
            }

            this.file = file;
            this.bottomLeft = bottomLeft;

            for (int i = 0; i < file.layers.Count; i++)
            {
                tileLayersToLayersInTile[linkedTileLayers[i]] = file.layers[i];
                layersInTileToTileLayers[file.layers[i]] = linkedTileLayers[i];
            }
        }

        /// <summary>
        /// Takes in the tile layer (that this tile must be on) and returns the layer it is associated with in the tile's file.
        /// </summary>
        public Layer TileLayerToLayerInTile(TileLayer tileLayer)
        {
            if (!tileLayersToLayersInTile.ContainsKey(tileLayer))
            {
                throw new System.Exception("This tile is not on the given tile layer.");
            }
            return tileLayersToLayersInTile[tileLayer];
        }
        /// <summary>
        /// Takes in the layer (that must be in the tile's file) and returns the tile layer (that the tile is on) that the layer is associated with.
        /// </summary>
        public TileLayer LayerInTileToTileLayer(Layer layer)
        {
            if (!layersInTileToTileLayers.ContainsKey(layer))
            {
                throw new System.Exception("This layer is not in the tile's file.");
            }
            return layersInTileToTileLayers[layer];
        }

        // Events

        public void SubscribeToOnMoved(UnityAction<IntRect> call)
        {
            onRectChanged.AddListener(call);
        }
    }
}
