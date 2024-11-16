using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PAC.DataStructures;
using PAC.Layers;
using PAC.Tilesets;
using UnityEngine;
using UnityEngine.Events;
using PAC.Json;
using System.Runtime.Serialization;

namespace PAC.Files
{
    /// <summary>
    /// A class to represent a single Pixel Art Creator file.
    /// </summary>
    public class File
    {
        public string name { get; set; } = "";

        public string mostRecentSavePath { get; private set; } = null;
        private bool _savedSinceLastEdit = true;
        public bool savedSinceLastEdit
        {
            get => _savedSinceLastEdit;
            private set
            {
                _savedSinceLastEdit = value;
                onSavedSinceEditChanged.Invoke();
            }
        }

        public int width { get; private set; }
        public int height { get; private set; }
        public IntRect rect => new IntRect(IntVector2.zero, new IntVector2(width - 1, height - 1));

        public List<Layer> layers { get; private set; } = new List<Layer>();
        /// <summary>The lowest layer (in terms of display order, so highest index) that is visible.</summary>
        private int lowestVisibleLayer
        {
            get
            {
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    if (layers[i].visible)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
        /// <summary>The highest layer (in terms of display order, so lowest index) that is visible.</summary>
        private int highestVisibleLayer
        {
            get
            {
                for (int i = 0; i <= layers.Count - 1; i++)
                {
                    if (layers[i].visible)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        // Used to keep track of what number to name newly-created layers - e.g. the first normal layer is 'Layer 1', the next is 'Layer 2', etc.
        /// <summary>The number that will be given to the next normal layer created.</summary>
        private int newNormalLayerNameNum = 1;
        /// <summary>The number that will be given to the next tile layer created.</summary>
        private int newTileLayerNameNum = 1;

        /// <summary>The number of frames the animation lasts for.</summary>
        public int numOfFrames { get; set; } = 10;
        /// <summary>The frame indices at which some layer has a key frame.</summary>
        public int[] keyFrameIndices => GetKeyFrameIndices();

        public Texture2D liveRender { get; private set; }
        private int liveRenderFrame = 0;

        private List<Tileset> tilesets = new List<Tileset>();
        private int tilesetIndex = 0;
        /// <summary>All the tile objects currently in the file.</summary>
        public Tile[] tiles => GetTiles();

        // Events

        /// <summary>
        /// Called when some pixels have been changed on a layer.
        /// Passes the pixels, layers and frames that were affected.
        /// </summary>
        private UnityEvent<IntVector2[], Layer[], int[]> onPixelsChanged = new UnityEvent<IntVector2[], Layer[], int[]>();
        /// <summary>Used to bypass the Layer.onPixelChanged event callback when doing operations on multiple layers.</summary>
        // This is to help performance - so that we can rerender the liveRender once after all layers have been changed instead of after each one.
        private bool ignoreOnLayerPixelsChanged = false;
        /// <summary>Called when a tile is added to the file.</summary>
        private UnityEvent onTileAdded = new UnityEvent();
        /// <summary>Called when a tile is removed from the file.</summary>
        private UnityEvent onTileRemoved = new UnityEvent();
        /// <summary>Called when the savedSinceLastEdit variable is changed.</summary>
        private UnityEvent onSavedSinceEditChanged = new UnityEvent();

        /// <summary>
        /// Creates a blank file.
        /// </summary>
        public File(string name, int width, int height)
        {
            this.name = name;
            this.width = width;
            this.height = height;

            liveRender = new Texture2D(width, height);

            AddNormalLayer();

            savedSinceLastEdit = true;
        }

        /// <summary>
        /// Creates a deep copy of the File.
        /// </summary>
        public File(File file) : this(file.name, file.width, file.height)
        {
            mostRecentSavePath = file.mostRecentSavePath;
            numOfFrames = file.numOfFrames;
            newNormalLayerNameNum = file.newNormalLayerNameNum;
            newTileLayerNameNum = file.newTileLayerNameNum;
            savedSinceLastEdit = file.savedSinceLastEdit;

            ClearLayers();
            foreach (Layer layer in file.layers)
            {
                AddLayer(layer.DeepCopy(), layers.Count);
            }

            RerenderLiveRender();
        }

        /// <summary>
        /// Creates a deep copy of the File.
        /// </summary>
        public File DeepCopy()
        {
            return new File(this);
        }

        /// <summary>
        /// Opens the file according to its file extension.
        /// </summary>
        public static File OpenFile(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("filePath doesn't exist: " + filePath);
            }

            if (Path.GetExtension(filePath) == ".png")
            {
                return OpenPNG(filePath);
            }
            else if (Path.GetExtension(filePath) == ".jpg" || Path.GetExtension(filePath) == ".jpeg")
            {
                return OpenJPEG(filePath);
            }
            else if (Path.GetExtension(filePath) == "." + Config.Files.pacFileExtension)
            {
                return OpenPAC(filePath);
            }
            else
            {
                throw new System.Exception("Unknown / unimplemented file extension: " + Path.GetExtension(filePath));
            }
        }

        /// <summary>
        /// Opens the PNG file. Throws an error if the file is not a PNG file.
        /// </summary>
        public static File OpenPNG(string filePath)
        {
            if (Path.GetExtension(filePath) != ".png")
            {
                throw new System.Exception("The file is not a PNG file. File extension: " + Path.GetExtension(filePath));
            }
            return OpenImage(filePath);
        }
        /// <summary>
        /// Opens the JPEG/JPG file. Throws an error if the file is not a JPEG/JPG file.
        /// </summary>
        public static File OpenJPEG(string filePath)
        {
            if (Path.GetExtension(filePath) != ".jpeg" && Path.GetExtension(filePath) != ".jpg")
            {
                throw new System.Exception("The file is not a JPEG / JPG file. File extension: " + Path.GetExtension(filePath));
            }
            return OpenImage(filePath);
        }
        /// <summary>
        /// Opens the file at the file path if it is an image file type, e.g. PNG or JPEG.
        /// </summary>
        public static File OpenImage(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("filePath doesn't exist: " + filePath);
            }

            Texture2D texture = Tex2DSprite.LoadFromFile(filePath);
            if (texture == null)
            {
                throw new System.Exception("Loaded texture is null.");
            }

            File file = new File(Path.GetFileNameWithoutExtension(filePath), texture.width, texture.height);
            file.ReplaceLayer(0, new NormalLayer("Layer 1", texture));

            file.savedSinceLastEdit = true;

            return file;
        }

        /// <summary>
        /// Opens the PAC file. Throws an error if the file is not a PAC file.
        /// </summary>
        public static File OpenPAC(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("filePath doesn't exist: " + filePath);
            }
            if (Path.GetExtension(filePath) != "." + Config.Files.pacFileExtension)
            {
                throw new System.Exception("The file is not a PAC file. File extension: " + Path.GetExtension(filePath));
            }

            JsonData.Object json;
            using (StreamReader stream = new StreamReader(filePath))
            {
                json = JsonData.Object.Parse(stream, true);
            }

            SemanticVersion fileFormatVersion = JsonConversion.FromJson<SemanticVersion>(json["file format version"], new JsonConversion.JsonConverterSet(new SemanticVersion.JsonConverter()), false);
            json.Remove("file format version");

            File file = JsonConversion.FromJson<File>(JsonData.Parse(System.IO.File.ReadAllText(filePath)), new JsonConversion.JsonConverterSet(new JsonConverter(fileFormatVersion)), false);
            
            file.savedSinceLastEdit = true;

            return file;
        }

        /// <summary>
        /// Adds the image at the file path as a new layer.
        /// </summary>
        /// <param name="filePath"></param>
        public void ImportImage(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.Exception("filePath doesn't exist: " + filePath);
            }

            Texture2D texture = Tex2DSprite.LoadFromFile(filePath);
            if (texture == null)
            {
                throw new System.Exception("Loaded texture is null.");
            }

            if (texture.width != width || texture.height != height)
            {
                throw new System.Exception("Image dimensions (" + texture.width + ", " + texture.height + ") do not match file dimensions (" + width + ", " + height + ")");
            }

            AddLayer(new NormalLayer(Path.GetFileNameWithoutExtension(filePath), texture));
        }

        /// <summary>
        /// Saves the file as a PAC file at the location it was most recently saved at. Throws an error if the file hasn't been saved before.
        /// </summary>
        public void SavePAC()
        {
            if (mostRecentSavePath == null)
            {
                throw new System.Exception("File has not been saved before. Please use SaveAsPAC() for the first time a file is saved.");
            }

            SaveAsPAC(mostRecentSavePath);
        }
        /// <summary>
        /// Saves the file as a PAC file at the given file path. Throws an error if the file is not a PAC file.
        /// </summary>
        public void SaveAsPAC(string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (Path.GetExtension(filePath) != "." + Config.Files.pacFileExtension)
            {
                throw new System.Exception("The file is not a PAC file. File extension: " + Path.GetExtension(filePath));
            }

            JsonData.Object json = (JsonData.Object)JsonConversion.ToJson(this, new JsonConversion.JsonConverterSet(new JsonConverter(Config.Files.fileFormatVersion)), false);
            json = json.Prepend(new JsonData.Object {
                { "file format version", JsonConversion.ToJson(Config.Files.fileFormatVersion, new JsonConversion.JsonConverterSet(new SemanticVersion.JsonConverter()), false) }
            });

            System.IO.File.WriteAllText(filePath, json.ToJsonString(true));

            mostRecentSavePath = filePath;
            savedSinceLastEdit = true;

            Debug.Log("Saved file at: " + filePath);
        }

        /// <summary>
        /// Exports the frame to the file path, according to the file extension.
        /// </summary>
        public bool ExportFrame(int frame, string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }

            if (Path.GetExtension(filePath) == ".png")
            {
                ExportFramePNG(frame, filePath);
            }
            else if (Path.GetExtension(filePath) == ".jpeg" || Path.GetExtension(filePath) == ".jpg")
            {
                ExportFrameJPEG(frame, filePath);
            }
            else if (Path.GetExtension(filePath) == ".ico")
            {
                ExportFrameICO(frame, filePath);
            }
            else
            {
                throw new System.Exception("Unknown / unimplemented file extension: " + Path.GetExtension(filePath));
            }

            Debug.Log("Exported frame at: " + filePath);

            return true;
        }

        /// <summary>
        /// Exports each frame of the animation and puts them in a folder. The file path specifies the folder name and location, and the file extension to export each frame as.
        /// </summary>
        public bool ExportAnimation(string filePath)
        {
            string folderPath = Path.ChangeExtension(filePath, null);

            Directory.CreateDirectory(folderPath);

            foreach (int keyFrame in keyFrameIndices)
            {
                string newFilePath = folderPath + "\\" + (keyFrame + 1) + Path.GetExtension(filePath);
                ExportFrame(keyFrame, newFilePath);
            }

            Debug.Log("Exported animation at: " + folderPath);

            return true;
        }

        /// <summary>
        /// Exports the frame to the PNG file. Throws an error if the file is not a PNG file.
        /// </summary>
        private void ExportFramePNG(int frame, string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (Path.GetExtension(filePath) != ".png")
            {
                throw new System.Exception("The file is not a PNG file. File extension: " + Path.GetExtension(filePath));
            }

            Texture2D texture = Render(frame);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(filePath, bytes);
        }
        /// <summary>
        /// Exports the frame to the JPEG/JPG file. Throws an error if the file is not a JPEG/JPG file.
        /// </summary>
        private void ExportFrameJPEG(int frame, string filePath)
        {
            if (!Path.IsPathFullyQualified(filePath))
            {
                throw new System.Exception("filePath not fully qualified: " + filePath);
            }
            if (Path.GetExtension(filePath) != ".jpeg" && Path.GetExtension(filePath) != ".jpg")
            {
                throw new System.Exception("The file is not a JPEG/JPG file. File extension: " + Path.GetExtension(filePath));
            }

            Texture2D texture = Render(frame);
            texture.Apply();

            byte[] bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes(filePath, bytes);
        }
        /// <summary>
        /// Not yet implemented. Exports the frame to the ICO file. Throws an error if the file is not an ICO file.
        /// </summary>
        private bool ExportFrameICO(int frame, string filePath)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns true if and only if all pixels on all layers are completely transparent.
        /// </summary>
        public bool IsBlank()
        {
            foreach (Layer layer in layers)
            {
                if (!layer.IsBlank())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the index of the layer within the file.
        /// </summary>
        /// <returns>The index of the layer, or -1 if the layer is not in the file.</returns>
        public int IndexOfLayer(Layer layer)
        {
            return layers.IndexOf(layer);
        }

        /// <summary>
        /// Adds the given layer on top of all existing layers.
        /// </summary>
        public void AddLayer(Layer layer) => AddLayer(layer, 0);
        /// <summary>
        /// Adds the given layer at the given index. Throws an error if the layer is already in the file.
        /// </summary>
        public void AddLayer(Layer layer, int index)
        {
            if (layers.Contains(layer))
            {
                throw new System.Exception("File already contains layer.");
            }

            layers.Insert(index, layer);

            layer.SubscribeToOnPixelsChanged((pixels, frames) => OnLayerPixelsChanged(pixels, new Layer[] { layer }, frames));
            layer.SubscribeToOnVisibilityChanged(RerenderLiveRender);
            layer.SubscribeToOnBlendModeChanged(RerenderLiveRender);

            RerenderLiveRender();
            savedSinceLastEdit = false;
        }
        /// <summary>
        /// Adds the layers on top of all existing layers, retaining the order they have in the array - i.e. first layer in the array is the one on top, etc.
        /// </summary>
        public void AddLayers(params Layer[] layers)
        {
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                AddLayer(layers[i]);
            }
        }
        /// <summary>
        /// Adds deep copies of the layers on top of all existing layers, retaining the order they have in the array - i.e. first layer in the array is the one on top, etc.
        /// </summary>
        public void ImportLayers(params Layer[] layers)
        {
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                AddLayer(layers[i].DeepCopy());
            }
        }
        /// <summary>
        /// Adds a blank normal layer on top of all existing layers.
        /// </summary>
        public void AddNormalLayer() => AddNormalLayer(0);
        /// <summary>
        /// Adds a blank normal layer at the given index.
        /// </summary>
        public void AddNormalLayer(int index) => AddNormalLayer(Tex2DSprite.BlankTexture(width, height), index);
        /// <summary>
        /// Adds a normal layer at the given index with the given texture.
        /// </summary>
        public void AddNormalLayer(Texture2D texture, int index = 0)
        {
            if (texture.width != width || texture.height != height)
            {
                throw new System.Exception("Texture dimensions must match file dimensions: Texture (" + texture.width + ", " + texture.height + "); File (" + width + ", " + height + ")");
            }

            Layer layer = new NormalLayer("Layer " + newNormalLayerNameNum, texture);
            newNormalLayerNameNum++;

            AddLayer(layer, index);
        }

        /// <summary>
        /// Adds a blank tile layer on top of all existing layers.
        /// </summary>
        public void AddTileLayer() => AddTileLayer(0);
        /// <summary>
        /// Adds a blank tile layer at the given index.
        /// </summary>
        public void AddTileLayer(int index)
        {
            TileLayer tileLayer = new TileLayer("Tile Layer " + newTileLayerNameNum, width, height);
            newTileLayerNameNum++;

            AddLayer(tileLayer, index);
        }

        /// <summary>
        /// Removes the layer at the given index.
        /// </summary>
        public void RemoveLayer(int layer) => RemoveLayers(new int[] { layer });
        /// <summary>
        /// Removes the layer from the file. Throws an error if the layer is not in the file.
        /// </summary>
        public void RemoveLayer(Layer layer) => RemoveLayers(new Layer[] { layer });
        /// <summary>
        /// Removes the layers at the given indices.
        /// </summary>
        public void RemoveLayers(int[] layers)
        {
            Array.Sort(layers);
            for (int i = 0; i < layers.Length; i++)
            {
                // Since we are deleting elements, the indices no longer point to the element they originally did. So, we shift the indices accordingly. This is why we sorted the array.
                int layer = layers[i] - i;
                if (layer < 0 || layer >= this.layers.Count)
                {
                    throw new System.IndexOutOfRangeException("Index out of range: " + layers[i]);
                }

                this.layers.RemoveAt(layer);
            }

            if (this.layers.Count == 0)
            {
                AddNormalLayer();
            }

            savedSinceLastEdit = false;

            RerenderLiveRender();
        }
        /// <summary>
        /// Removes the given layers from the file. Throws an error if a layer is not in the file.
        /// </summary>
        public void RemoveLayers(Layer[] layers)
        {
            int[] layerIndices = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerIndices[i] = IndexOfLayer(layers[i]);
                if (layerIndices[i] == -1)
                {
                    throw new SystemException("Layer is not the file: " + layers[i].name);
                }
            }
            RemoveLayers(layerIndices);
        }
        /// <summary>
        /// Removes all layers, then adds a blank normal layer.
        /// </summary>
        public void RemoveAllLayers()
        {
            RemoveLayers(Enumerable.Range(0, layers.Count).ToArray());
        }
        /// <summary>
        /// Removes all layers and does not add a new one.
        /// </summary>
        private void ClearLayers()
        {
            layers.Clear();
        }

        /// <summary>
        /// Replaces the layer at the given index with the given layer. Throws an error if the layer to replace with is already in the file.
        /// </summary>
        public void ReplaceLayer(int layerToReplace, Layer layerToReplaceWith)
        {
            if (layerToReplace < 0 || layerToReplace >= layers.Count)
            {
                throw new System.IndexOutOfRangeException("Index out of range: " + layerToReplace);
            }
            if (layers.Contains(layerToReplaceWith))
            {
                throw new System.Exception("File already contains layerToReplaceWith.");
            }
            if (layers[layerToReplace] == layerToReplaceWith)
            {
                return;
            }

            AddLayer(layerToReplaceWith, layerToReplace);
            RemoveLayer(layerToReplace + 1);

            savedSinceLastEdit = false;

            RerenderLiveRender();
        }
        /// <summary>
        /// Replaces the layer with the given layer. Throws an error if the layer to replace is not in the file, or if the layer to replace with is already in the file.
        /// </summary>
        public void ReplaceLayer(Layer layerToReplace, Layer layerToReplaceWith)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == layerToReplace)
                {
                    ReplaceLayer(i, layerToReplaceWith);
                    return;
                }
            }
            throw new System.Exception("layerToReplace is not in the file.");
        }

        /// <summary>
        /// Moves the layer at indexToMove to indexToMoveTo.
        /// </summary>
        public void MoveLayer(int layerToMove, int indexToMoveTo)
        {
            if (layerToMove < 0 || layerToMove >= layers.Count)
            {
                throw new System.IndexOutOfRangeException("indexToMove out of range: " + layerToMove);
            }
            if (indexToMoveTo < 0 || indexToMoveTo > layers.Count)
            {
                throw new System.IndexOutOfRangeException("indexToMoveTo out of range: " + layerToMove);
            }

            // To counteract indices being shifted when we delete the layer at indexToMove
            if (layerToMove < indexToMoveTo)
            {
                indexToMoveTo--;
            }

            Layer layer = layers[layerToMove];
            layers.RemoveAt(layerToMove);
            layers.Insert(indexToMoveTo, layer);

            RerenderLiveRender();
            savedSinceLastEdit = false;
        }
        /// <summary>
        /// Moves the layer to indexToMoveTo. Throws an error if the layer is not in the file.
        /// </summary>
        public void MoveLayer(Layer layerToMove, int indexToMoveTo)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == layerToMove)
                {
                    MoveLayer(i, indexToMoveTo);
                    return;
                }
            }
            throw new System.Exception("layerToReplace is not in the file.");
        }

        /// <summary>
        /// Renders all layers down to a Texture2D.
        /// Does not apply the texture.
        /// </summary>
        public Texture2D Render(int frame)
        {
            return RenderLayers(0, layers.Count - 1, frame, true);
        }

        /// <summary>
        /// Renders all the layers with index between lowestLayer and highestLayer (inclusive iff inclusive == true) down to a Texture2D.
        /// highestLayer and lowestLayer are automatically clamped to be in the valid range of layers.
        /// Does not apply the texture.
        /// </summary>
        /// <param name="highestLayer">The higher layer (so lower index).</param>
        /// <param name="lowestLayer">The lower layer (so higher index).</param>
        public Texture2D RenderLayers(int highestLayer, int lowestLayer, int frame, bool inclusive = true)
        {
            highestLayer = Mathf.Clamp(highestLayer, 0, layers.Count - 1);
            lowestLayer = Mathf.Clamp(lowestLayer, 0, layers.Count - 1);
            if (lowestLayer < highestLayer)
            {
                throw new System.Exception("lowestlayer cannot be above highestLayer: " + lowestLayer + " > " + highestLayer);
            }

            if (!inclusive && (highestLayer == lowestLayer || highestLayer == lowestLayer - 1))
            {
                return Tex2DSprite.BlankTexture(width, height);
            }

            // Get the indices of the highest / lowest visible layers so that we don't waste time rendering some invisible layers.

            int highestVisibleLayer = -1;
            for (int i = highestLayer + (inclusive ? 0 : 1); i <= lowestLayer - (inclusive ? 0 : 1); i++)
            {
                if (layers[i].visible)
                {
                    highestVisibleLayer = i;
                    break;
                }
            }

            int lowestVisibleLayer = -1;
            for (int i = lowestLayer - (inclusive ? 0 : 1); i >= highestLayer + (inclusive ? 0 : 1); i--)
            {
                if (layers[i].visible)
                {
                    lowestVisibleLayer = i;
                    break;
                }
            }

            if (highestVisibleLayer == -1)
            {
                return Tex2DSprite.BlankTexture(width, height);
            }

            return RenderLayers(Functions.Range(highestVisibleLayer, lowestVisibleLayer), frame);
        }

        /// <summary>
        /// Renders the layers at the given layer indices down to a Texture2D.
        /// Does not apply the texture.
        /// </summary>
        /// <param name="layerIndices">The layer indices in the order you want them to be rendered, from highest layer (so lowest index) to lowest.</param>
        public Texture2D RenderLayers(IEnumerable<int> layerIndices, int frame)
        {
            if (layerIndices.Count() == 0)
            {
                return Tex2DSprite.BlankTexture(width, height);
            }

            Texture2D tex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, RenderPixel(x, y, layerIndices, frame));
                }
            }

            return tex;
        }

        /// <summary>
        /// Renders all layers that appear above the given layer (includes the layer iff inclusive == true) down to a Texture2D. Returns a blank texture if there are none.
        /// Does not apply the texture.
        /// </summary>
        public Texture2D RenderLayersAbove(int layer, int frame, bool inclusive = false)
        {
            if (layer == 0 && !inclusive)
            {
                return Tex2DSprite.BlankTexture(width, height);
            }
            return RenderLayers(0, layer - (inclusive ? 0 : 1), frame);
        }

        /// <summary>
        /// Renders all layers that appear strictly beneath the given layer (includes the layer iff inclusive == true) down to a Texture2D. Returns a blank texture if there are none.
        /// Does not apply the texture.
        /// </summary>
        public Texture2D RenderLayersBelow(int layer, int frame, bool inclusive = false)
        {
            if (layer == layers.Count - 1 && !inclusive)
            {
                return Tex2DSprite.BlankTexture(width, height);
            }
            return RenderLayers(layer + (inclusive ? 0 : 1), layers.Count - 1, frame);
        }

        /// <summary>
        /// Renders the colour of the given pixel.
        /// </summary>
        public Color RenderPixel(IntVector2 pixel, int frame) => RenderPixel(pixel.x, pixel.y, frame);
        /// <summary>
        /// Renders the colour of the given pixel.
        /// </summary>
        public Color RenderPixel(int x, int y, int frame) => RenderPixel(x, y, 0, layers.Count - 1, frame);
        /// <summary>
        /// Renders the colour of the pixel on the layers between highestLayer and lowestLayer (inclusive iff inclusive == true).
        /// highestLayer and lowestLayer are automatically clamped to be in the valid range of layers.
        /// </summary>
        /// <param name="highestLayer">The higher layer (so lower index).</param>
        /// <param name="lowestLayer">The lower layer (so higher index).</param>
        /// </summary>
        public Color RenderPixel(IntVector2 pixel, int highestLayer, int lowestLayer, int frame, bool inclusive = true) => RenderPixel(pixel.x, pixel.y, highestLayer, lowestLayer, frame, inclusive);
        /// <summary>
        /// Renders the colour of pixel (x, y) on the layers between highestLayer and lowestLayer (inclusive iff inclusive == true).
        /// highestLayer and lowestLayer are automatically clamped to be in the valid range of layers.
        /// </summary>
        /// <param name="highestLayer">The higher layer (so lower index).</param>
        /// <param name="lowestLayer">The lower layer (so higher index).</param>
        /// </summary>
        public Color RenderPixel(int x, int y, int highestLayer, int lowestLayer, int frame, bool inclusive = true)
        {
            highestLayer = Mathf.Clamp(highestLayer, 0, layers.Count - 1);
            lowestLayer = Mathf.Clamp(lowestLayer, 0, layers.Count - 1);

            if (lowestLayer < highestLayer)
            {
                throw new System.Exception("lowestlayer cannot be above highestLayer: " + lowestLayer + " > " + highestLayer);
            }

            if (!inclusive && (highestLayer == lowestLayer || highestLayer == lowestLayer - 1))
            {
                throw new System.Exception("inclusive == false and there are no layers strictly between the highest layer and the lowest layer. highestLayer: " + highestLayer + "; lowestLayer: " + lowestLayer);
            }

            return RenderPixel(x, y, Functions.Range(highestLayer + (inclusive ? 0 : 1), lowestLayer - (inclusive ? 0 : 1)), frame);
        }
        /// <summary>
        /// Renders the colour of the pixel on the layers at the given layer indices. Throws an error if there are no layer indices.
        /// </summary>
        public Color RenderPixel(IntVector2 pixel, IEnumerable<int> layerIndices, int frame) => RenderPixel(pixel.x, pixel.y, layerIndices, frame);
        /// <summary>
        /// Renders the colour of pixel (x, y) on the layers at the given layer indices. Throws an error if there are no layer indices.
        /// </summary>
        public Color RenderPixel(int x, int y, IEnumerable<int> layerIndices, int frame)
        {
            if (layerIndices.Count() == 0)
            {
                throw new System.Exception("layerIndices cannot be empty.");
            }

            layerIndices = layerIndices.Reverse();

            Color pixelColour = new Color(0f, 0f, 0f, 0f);
            foreach (int i in layerIndices)
            {
                if (layers[i].visible)
                {
                    Color layerPixelColour = layers[i].GetPixel(x, y, frame);
                    pixelColour = layers[i].blendMode.Blend(layerPixelColour, pixelColour);
                }
            }

            return pixelColour;
        }

        /// <summary>
        /// Rerenders the whole live render.
        /// </summary>
        private void RerenderLiveRender()
        {
            Color[] pixels = new Color[rect.area];
            IEnumerable<int> layerIndices = Functions.Range(0, layers.Count() - 1);

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[index] = RenderPixel(x, y, layerIndices, liveRenderFrame);
                    index++;
                }
            }
            liveRender.SetPixels(pixels);
        }
        /// <summary>
        /// Rerenders the section of the live render within the given rect.
        /// </summary>
        private void RerenderLiveRender(IntRect rect)
        {
            for (int x = rect.bottomLeft.x; x <= rect.topRight.x; x++)
            {
                for (int y = rect.bottomLeft.y; y <= rect.topRight.y; y++)
                {
                    liveRender.SetPixel(x, y, RenderPixel(x, y, liveRenderFrame));
                }
            }
        }
        /// <summary>
        /// Rerenders the given pixels of the live render.
        /// </summary>
        private void RerenderLiveRender(IntVector2[] pixels)
        {
            foreach (IntVector2 pixel in pixels)
            {
                liveRender.SetPixel(pixel, RenderPixel(pixel, liveRenderFrame));
            }
        }

        /// <summary>
        /// Sets the frame that the live render will be for.
        /// </summary>
        public void SetLiveRenderFrame(int frame)
        {
            foreach (Tile tile in tiles)
            {
                tile.file.SetLiveRenderFrame(frame);
            }

            if (frame != liveRenderFrame)
            {
                liveRenderFrame = frame;
                RerenderLiveRender();
            }
        }

        /// <summary>
        /// Flips the file.
        /// </summary>
        public void Flip(FlipDirection direction)
        {
            ignoreOnLayerPixelsChanged = true;
            foreach (Layer layer in layers)
            {
                layer.Flip(direction);
            }
            ignoreOnLayerPixelsChanged = false;

            RerenderLiveRender();
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(rect.points, layers.ToArray(), keyFrameIndices);
        }

        /// <summary>
        /// Rotates the file. Rotation is clockwise.
        /// </summary>
        public void Rotate(RotationAngle angle)
        {
            if (angle == RotationAngle._0)
            {
                return;
            }

            ignoreOnLayerPixelsChanged = true;
            foreach (Layer layer in layers)
            {
                layer.Rotate(angle);
            }
            ignoreOnLayerPixelsChanged = false;

            width = layers[0].width;
            height = layers[0].height;

            if (width != height && (angle == RotationAngle._90 || angle == RotationAngle.Minus90))
            {
                liveRender.Reinitialize(width, height);
            }
            RerenderLiveRender();
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(rect.points, layers.ToArray(), keyFrameIndices);
        }

        /// <summary>
        /// Extends the dimensions of the file in each direction by the given amounts.
        /// </summary>
        public void Extend(int left, int right, int up, int down)
        {
            ignoreOnLayerPixelsChanged = true;
            foreach (Layer layer in layers)
            {
                layer.Extend(left, right, up, down);
            }
            ignoreOnLayerPixelsChanged = false;

            width += left + right;
            height += up + down;

            liveRender.Reinitialize(width, height);
            RerenderLiveRender();
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(rect.points, layers.ToArray(), keyFrameIndices);
        }

        /// <summary>
        /// Resizes the dimensions of the file by the scale factor.
        /// </summary>
        public void Scale(float scaleFactor) => Scale(scaleFactor, scaleFactor);
        /// <summary>
        /// Resizes the dimensions of the file by the scale factors.
        /// </summary>
        public void Scale(float xScaleFactor, float yScaleFactor)
        {
            ignoreOnLayerPixelsChanged = true;
            foreach (Layer layer in layers)
            {
                layer.Scale(xScaleFactor, yScaleFactor);
            }
            ignoreOnLayerPixelsChanged = false;

            width = layers[0].width;
            height = layers[0].height;

            if (xScaleFactor != 1f || yScaleFactor != 1f)
            {
                liveRender.Reinitialize(width, height);
            }
            RerenderLiveRender();
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(rect.points, layers.ToArray(), keyFrameIndices);
        }
        /// <summary>
        /// Resizes the file to the new width and height.
        /// </summary>
        public void Scale(int newWidth, int newHeight)
        {
            ignoreOnLayerPixelsChanged = true;
            foreach (Layer layer in layers)
            {
                layer.Scale(newWidth, newHeight);
            }
            ignoreOnLayerPixelsChanged = false;

            int oldWidth = width;
            int oldHeight = height;

            width = newWidth;
            height = newHeight;

            if (oldWidth != newWidth || oldHeight != newHeight)
            {
                liveRender.Reinitialize(width, height);
            }
            RerenderLiveRender();
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(rect.points, layers.ToArray(), keyFrameIndices);
        }

        /// <summary>
        /// Gets the frame indices at which some layer has a key frame.
        /// </summary>
        private int[] GetKeyFrameIndices()
        {
            List<int> indices = new List<int>();
            foreach (Layer layer in layers)
            {
                foreach (int keyFrameIndex in layer.keyFrameIndices)
                {
                    if (!indices.Contains(keyFrameIndex))
                    {
                        indices.Add(keyFrameIndex);
                    }
                }
            }
            indices.Sort();
            return indices.ToArray();
        }

        /// <summary>
        /// Gets all the tile objects currently in the file.
        /// </summary>
        private Tile[] GetTiles()
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Layer layer in layers)
            {
                if (layer.layerType == LayerType.Tile)
                {
                    foreach (Tile tile in ((TileLayer)layer).tiles)
                    {
                        if (!tiles.Contains(tile))
                        {
                            tiles.Add(tile);
                        }
                    }
                }
            }
            return tiles.ToArray();
        }

        /// <summary>
        /// Adds the given file as a tile.
        /// </summary>
        public void AddTile(File file, IntVector2 bottomLeft, int lowestLayer)
        {
            if (file == this)
            {
                throw new System.Exception("Cannot add a tile whose file is the file it's being added to.");
            }
            if (layers[lowestLayer].layerType != LayerType.Tile)
            {
                throw new System.Exception("Starting layer must be a tile layer.");
            }

            TileLayer[] linkedTileLayers = new TileLayer[file.layers.Count];
            int tileLayerCount = 0;
            for (int i = lowestLayer; i >= 0 && tileLayerCount < file.layers.Count; i--)
            {
                if (layers[i].layerType == LayerType.Tile)
                {
                    tileLayerCount++;
                    linkedTileLayers[^tileLayerCount] = (TileLayer)layers[i];
                }
            }
            if (tileLayerCount < file.layers.Count)
            {
                throw new System.Exception("There are not enough tile layers to add the tile at this layer.");
            }

            AddTile(new Tile(file, bottomLeft, linkedTileLayers));
        }
        /// <summary>
        /// Adds the tile to the file.
        /// </summary>
        public void AddTile(Tile tile)
        {
            if (tile.file == this)
            {
                throw new System.Exception("Cannot add a tile whose file is the file it's being added to.");
            }
            foreach (TileLayer tileLayer in tile.tileLayersAppearsOn)
            {
                if (!layers.Contains(tileLayer))
                {
                    throw new System.Exception("Tile is linked to a tile layer that is not in this file.");
                }
            }

            foreach (TileLayer tileLayer in tile.tileLayersAppearsOn)
            {
                tileLayer.AddTile(tile);
            }

            tile.file.SetLiveRenderFrame(liveRenderFrame);
            RerenderLiveRender(tile.rect);

            onTileAdded.Invoke();
        }

        /// <summary>
        /// Removes the tile. Throws an error if the tile is not in the file.
        /// </summary>
        public void RemoveTile(Tile tile)
        {
            if (!ContainsTile(tile))
            {
                throw new System.Exception("Tile is not in the file.");
            }

            bool success = true;
            foreach (TileLayer tileLayer in tile.tileLayersAppearsOn)
            {
                tileLayer.RemoveTile(tile);
            }

            RerenderLiveRender(tile.rect);

            onTileRemoved.Invoke();
        }

        public bool ContainsTile(Tile tile)
        {
            return tiles.Contains(tile);
        }

        // Event callbacks

        private void OnLayerPixelsChanged(IntVector2[] pixels, Layer[] layer, int[] frames)
        {
            if (ignoreOnLayerPixelsChanged)
            {
                return;
            }

            RerenderLiveRender(pixels);
            savedSinceLastEdit = false;

            onPixelsChanged.Invoke(pixels, layer, frames);
        }

        // Events

        /// <summary>
        /// Event invoked when some pixels have been changed on a layer.
        /// Passes the pixels, layers and frames that were affected.
        /// </summary>
        public void SubscribeToOnPixelsChanged(UnityAction<IntVector2[], Layer[], int[]> call)
        {
            onPixelsChanged.AddListener(call);
        }
        /// <summary>
        /// Event invoked when a tile is added to the file.
        /// </summary>
        public void SubscribeToOnTileAdded(UnityAction call)
        {
            onTileAdded.AddListener(call);
        }
        /// <summary>
        /// Event invoked when a tile is removed from the file.
        /// </summary>
        public void SubscribeToOnTileRemoved(UnityAction call)
        {
            onTileRemoved.AddListener(call);
        }
        /// <summary>
        /// Event invoked when the a change to file is made or when the file is saved.
        /// </summary>
        public void SubscribeToOnSavedSinceEditChanged(UnityAction call)
        {
            onSavedSinceEditChanged.AddListener(call);
        }

        public class JsonConverter : JsonConversion.JsonConverter<File, JsonData.Object>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            public override JsonData.Object ToJson(File file)
            {
                return new JsonData.Object
                    {
                        { "name", file.name },
                        { "most recent save path", file.mostRecentSavePath },
                        { "width", file.width },
                        { "height", file.height },
                        { "new normal layer name num", file.newNormalLayerNameNum },
                        { "new tile layer name num", file.newTileLayerNameNum },
                        { "num of frames", file.numOfFrames },
                        { "layers", JsonConversion.ToJson(file.layers, Layer.GetJsonConverterSet(Config.Files.fileFormatVersion), false) }
                    };
            }

            public override File FromJson(JsonData.Object jsonData)
            {
                if (fromJsonFileFormatVersion > Config.Files.fileFormatVersion)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is ahead of the current version " + Config.Files.fileFormatVersion);
                }
                if (fromJsonFileFormatVersion.major < Config.Files.fileFormatVersion.major)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is out of date with the current version " + Config.Files.fileFormatVersion);
                }

                string name = JsonConversion.FromJson<string>(jsonData["name"]);
                int width = JsonConversion.FromJson<int>(jsonData["width"]);
                int height = JsonConversion.FromJson<int>(jsonData["height"]);

                File file = new File(name, width, height);

                file.mostRecentSavePath = JsonConversion.FromJson<string>(jsonData["most recent save path"]);

                file.newNormalLayerNameNum = JsonConversion.FromJson<int>(jsonData["new normal layer name num"]);
                file.newTileLayerNameNum = JsonConversion.FromJson<int>(jsonData["new tile layer name num"]);

                file.numOfFrames = JsonConversion.FromJson<int>(jsonData["num of frames"]);
                file.layers = JsonConversion.FromJson<List<Layer>>(jsonData["layers"], new JsonConversion.JsonConverterSet(new Layer.JsonConverter(fromJsonFileFormatVersion)), false);

                file.RerenderLiveRender();
                return file;
            }
        }
    }
}
