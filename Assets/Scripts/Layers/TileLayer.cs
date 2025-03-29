using System;
using System.Collections.Generic;
using System.Linq;

using PAC.Animation;
using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Geometry;
using PAC.Geometry.Axes;
using PAC.Geometry.Extensions;
using PAC.ImageEditing;
using PAC.Json;
using PAC.Tilesets;

using UnityEngine;

namespace PAC.Layers
{
    /// <summary>
    /// A class to represent a tile layer - one for placing and editing tileset tiles.
    /// </summary>
    public class TileLayer : Layer
    {
        public override LayerType layerType => LayerType.Tile;

        /// <summary>The tiles on this layer.</summary>
        public List<Tile> tiles { get; private set; } = new List<Tile>();

        /// <summary>
        /// Keeps track of which tile each pixel falls within.
        /// Only stores a pixel as a key if it falls within a tile.
        /// </summary>
        private Dictionary<IntVector2, Tile> pixelToTile = new Dictionary<IntVector2, Tile>();

        /// <summary>Used to bypass the OnTilePixelsChanged() event callback when doing operations on multiple tiles.</summary>
        // This is to help performance - so that we can rerender the key frames once after all tiles have been changed instead of after each one.
        private bool ignoreOnTilePixelsChanged = false;

        public TileLayer(string name, int width, int height) : base(name, Texture2DCreator.Transparent(width, height)) { }

        /// <summary>
        /// Creates a deep copy of the TileLayer.
        /// </summary>
        public TileLayer(TileLayer layer) : base(layer)
        {
            // This doesn't create a deep copy!
            tiles = layer.tiles;
        }

        public override Layer DeepCopy()
        {
            return new TileLayer(this);
        }

        /// <summary>
        /// Adds the tile to the layer.
        /// </summary>
        public void AddTile(Tile tile)
        {
            // Validation checks

            if (ContainsTile(tile))
            {
                throw new System.Exception("Tile has already been added to the layer.");
            }
            if (!tile.tileLayersAppearsOn.Contains(this))
            {
                throw new System.Exception("Tile is not linked to this tile layer.");
            }

            // Add the tile

            // Subscribe to changes on the associate layer in the file's tile, but only if we haven't already done so because of another tile linked to the same file.
            bool alreadyTileWithThisLinkedLayer = false;
            foreach (Tile existingTile in tiles)
            {
                if (existingTile.TileLayerToLayerInTile(this) == tile.TileLayerToLayerInTile(this))
                {
                    alreadyTileWithThisLinkedLayer = true;
                    break;
                }
            }
            if (!alreadyTileWithThisLinkedLayer)
            {
                tile.TileLayerToLayerInTile(this).SubscribeToOnPixelsChanged((pixels, frames) => OnTilePixelsChanged(pixels, tile.TileLayerToLayerInTile(this), frames));
            }

            tile.SubscribeToOnMoved((previousBottomLeft) => OnTileRectChanged(tile, previousBottomLeft));

            tiles.Add(tile);
            foreach (int keyframe in tile.TileLayerToLayerInTile(this).keyFrameIndices)
            {
                AddKeyFrame(keyframe);
            }
            foreach (IntVector2 pixel in tile.rect)
            {
                pixelToTile[pixel] = tile;
            }

            RerenderKeyFrames(tile.rect);
        }

        /// <summary>
        /// Removes the tile from the layer. Throws an error if the tile is not in the layer.
        /// </summary>
        public void RemoveTile(Tile tile)
        {
            if (!tiles.Contains(tile))
            {
                throw new System.Exception("Tile is not in this layer.");
            }

            tiles.Remove(tile);

            foreach (IntVector2 pixel in tile.rect)
            {
                if (pixelToTile[pixel] == tile)
                {
                    pixelToTile.Remove(pixel);
                }
            }

            RerenderKeyFrames(tile.rect);
        }

        /// <summary>
        /// Removes all tiles.
        /// </summary>
        public void ClearTiles()
        {
            tiles.Clear();
            RerenderKeyFrames();
        }

        /// <summary>
        /// Returns true if the tile appears on this layer.
        /// </summary>
        public bool ContainsTile(Tile tile)
        {
            return tiles.Contains(tile);
        }

        protected override IEnumerable<IntVector2> SetPixelsNoEvent(IEnumerable<IntVector2> pixels, int frame, Color colour, AnimFrameRefMode frameRefMode)
        {
            HashSet<IntVector2> pixelsFilled = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in pixels)
            {
                Tile tile = PixelToTileNoValidation(pixel.x, pixel.y);
                if (tile != null)
                {
                    tile.TileLayerToLayerInTile(this).SetPixel(new IntVector2(pixel.x, pixel.y) - tile.bottomLeft, frame, colour, frameRefMode);

                    if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
                    {
                        AddKeyFrame(frame);
                    }

                    foreach (Tile otherTile in tiles)
                    {
                        if (otherTile.file == tile.file)
                        {
                            IntVector2 pixelMappedToTile = new IntVector2(pixel.x, pixel.y) - tile.bottomLeft + otherTile.bottomLeft;
                            GetKeyFrame(frame).texture.SetPixel(pixelMappedToTile, colour * new Color(1f, 1f, 1f, tile.TileLayerToLayerInTile(this).opacity));
                            if (!pixelsFilled.Contains(pixelMappedToTile))
                            {
                                pixelsFilled.Add(pixelMappedToTile);
                            }
                        }
                    }
                }
            }
            return pixelsFilled.ToArray();
        }

        public override Color GetPixel(IntVector2 pixel, int frame, bool useLayerOpacity = true)
        {
            if (pixel.x < 0 || pixel.y < 0 || pixel.x >= width || pixel.y >= height)
            {
                throw new System.Exception("Pixel (" + pixel.x + ", " + pixel.y + ") outside of layer dimensions " + width + "x" + height);
            }

            return GetKeyFrame(frame).texture.GetPixel(pixel.x, pixel.y) * new Color(1f, 1f, 1f, useLayerOpacity ? opacity : 1f);
        }

        /// <summary>
        /// Gets the tile that the pixel (x, y) lands in, or null if there isn't one.
        /// </summary>
        public Tile PixelToTile(int x, int y) => PixelToTile(new IntVector2(x, y));
        /// <summary>
        /// Gets the tile that the pixel lands in, or null if there isn't one.
        /// </summary>
        public Tile PixelToTile(IntVector2 pixel)
        {
            if (!rect.Contains(pixel))
            {
                throw new System.Exception("Pixel (" + pixel.x + ", " + pixel.y + ") outside of layer dimensions " + width + "x" + height);
            }

            return PixelToTileNoValidation(pixel);
        }
        /// <summary>
        /// Gets the tile that the pixel (x, y) lands in, or null if there isn't one. Doesn't check if the pixel is within the layer.
        /// </summary>
        private Tile PixelToTileNoValidation(int x, int y) => PixelToTileNoValidation(new IntVector2(x, y));
        /// <summary>
        /// Gets the tile that the pixel lands in, or null if there isn't one. Doesn't check if the pixel is within the layer.
        /// </summary>
        private Tile PixelToTileNoValidation(IntVector2 pixel)
        {
            return pixelToTile.ContainsKey(pixel) ? pixelToTile[pixel] : null;
        }

        /// <summary>
        /// Get the pixels that are linked to the given pixel due to multiple tiles having the same file - i.e. they point to the same pixel within the tiles' file.
        /// </summary>
        public IEnumerable<IntVector2> GetLinkedPixels(IntVector2 pixel) => GetLinkedPixels(new IntVector2[] { pixel });
        /// <summary>
        /// Get the pixels that are linked to the given pixels due to multiple tiles having the same file - i.e. they point to the same pixels within the tiles' file.
        /// </summary>
        public IEnumerable<IntVector2> GetLinkedPixels(IEnumerable<IntVector2> pixels)
        {
            HashSet<IntVector2> pixelsFilled = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in pixels)
            {
                if (!rect.Contains(pixel))
                {
                    throw new System.Exception("Pixel (" + pixel.x + ", " + pixel.y + ") outside of layer dimensions " + width + "x" + height);
                }

                Tile tile = PixelToTileNoValidation(pixel);
                if (tile != null)
                {
                    foreach (Tile otherTile in tiles)
                    {
                        if (otherTile.TileLayerToLayerInTile(this) == tile.TileLayerToLayerInTile(this))
                        {
                            IntVector2 pixelMappedToTile = pixel - tile.bottomLeft + otherTile.bottomLeft;
                            if (!pixelsFilled.Contains(pixelMappedToTile))
                            {
                                pixelsFilled.Add(pixelMappedToTile);
                            }
                        }
                    }
                }
                else if (!pixelsFilled.Contains(pixel))
                {
                    pixelsFilled.Add(pixel);
                }
            }
            return pixelsFilled.ToArray();
        }

        protected override void FlipNoEvent(CardinalAxis axis)
        {
            ignoreOnTilePixelsChanged = true;
            HashSet<Layer> flippedLayers = new HashSet<Layer>();
            foreach (Tile tile in tiles)
            {
                if (!flippedLayers.Contains(tile.TileLayerToLayerInTile(this)))
                {
                    tile.TileLayerToLayerInTile(this).Flip(axis);
                    flippedLayers.Add(tile.TileLayerToLayerInTile(this));
                }

                if (axis == Axes.Vertical)
                {
                    tile.bottomRight = new IntVector2(-tile.bottomLeft.x + width - 1, tile.bottomLeft.y);
                }
                else if (axis == Axes.Horizontal)
                {
                    tile.topLeft = new IntVector2(tile.bottomLeft.x, -tile.bottomLeft.y + height - 1);
                }
                else
                {
                    throw new UnreachableException();
                }
            }

            ignoreOnTilePixelsChanged = false;
            RerenderKeyFrames();
        }

        protected override void RotateNoEvent(QuadrantalAngle angle)
        {
            if (angle == QuadrantalAngle._0)
            {
                return;
            }

            ignoreOnTilePixelsChanged = true;
            HashSet<Layer> flippedLayers = new HashSet<Layer>();
            foreach (Tile tile in tiles)
            {
                if (!flippedLayers.Contains(tile.TileLayerToLayerInTile(this)))
                {
                    tile.TileLayerToLayerInTile(this).Rotate(angle);
                    flippedLayers.Add(tile.TileLayerToLayerInTile(this));
                }

                if (angle == QuadrantalAngle.Clockwise90)
                {
                    tile.topLeft = new IntVector2(tile.bottomLeft.y, -tile.bottomLeft.x + height - 1);
                }
                else if (angle == QuadrantalAngle.Anticlockwise90)
                {
                    tile.bottomRight = new IntVector2(-tile.bottomLeft.y + width - 1, tile.bottomLeft.x);
                }
                else if (angle == QuadrantalAngle._180)
                {
                    tile.topRight = new IntVector2(-tile.bottomLeft.x + width - 1, -tile.bottomLeft.y + height - 1);
                }
            }

            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = keyFrame.texture.Rotate(angle);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;

            ignoreOnTilePixelsChanged = false;
            RerenderKeyFrames();
        }

        protected override void ExtendNoEvent(int left, int right, int up, int down)
        {
            ignoreOnTilePixelsChanged = true;
            foreach (Tile tile in tiles)
            {
                tile.bottomLeft += new IntVector2(left, down);
            }

            ImageEdit.ExtendCropOptions extendCropOptions = new ImageEdit.ExtendCropOptions { left = left, right = right, top = up, bottom = down };
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = keyFrame.texture.ExtendCrop(extendCropOptions);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;

            ignoreOnTilePixelsChanged = false;
            RerenderKeyFrames();
        }

        protected override void ScaleNoEvent(float xScaleFactor, float yScaleFactor)
        {
            ignoreOnTilePixelsChanged = true;
            HashSet<Layer> flippedLayers = new HashSet<Layer>();
            foreach (Tile tile in tiles)
            {
                if (!flippedLayers.Contains(tile.TileLayerToLayerInTile(this)))
                {
                    tile.TileLayerToLayerInTile(this).Scale(xScaleFactor, yScaleFactor);
                    flippedLayers.Add(tile.TileLayerToLayerInTile(this));
                }

                tile.bottomLeft = IntVector2.CeilToIntVector2(tile.bottomLeft * new Vector2(Mathf.RoundToInt(width * xScaleFactor) / width, Mathf.RoundToInt(height * yScaleFactor) / height));
            }

            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = keyFrame.texture.Scale(xScaleFactor, yScaleFactor);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;

            ignoreOnTilePixelsChanged = false;
            RerenderKeyFrames();
        }
        protected override void ScaleNoEvent(int newWidth, int newHeight)
        {
            ignoreOnTilePixelsChanged = true;
            HashSet<Layer> flippedLayers = new HashSet<Layer>();
            foreach (Tile tile in tiles)
            {
                if (!flippedLayers.Contains(tile.TileLayerToLayerInTile(this)))
                {
                    tile.TileLayerToLayerInTile(this).Scale(newWidth, newHeight);
                    flippedLayers.Add(tile.TileLayerToLayerInTile(this));
                }

                tile.bottomLeft = IntVector2.CeilToIntVector2(tile.bottomLeft * new Vector2(newWidth / width, newHeight / height));
            }

            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = keyFrame.texture.Scale(newWidth, newHeight);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;

            ignoreOnTilePixelsChanged = false;
            RerenderKeyFrames();
        }

        protected override AnimationKeyFrame DeleteKeyFrameNoEvent(int keyframe)
        {
            if (HasKeyFrameAt(keyframe))
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.TileLayerToLayerInTile(this).HasKeyFrameAt(keyframe))
                    {
                        tile.TileLayerToLayerInTile(this).DeleteKeyFrame(keyframe);
                    }
                }

                AnimationKeyFrame keyFrame = GetKeyFrame(keyframe);
                keyFrames.Remove(keyFrame);

                if (keyframe == 0)
                {
                    AddKeyFrame(0, Texture2DCreator.Transparent(width, height));
                }

                return keyFrame;
            }
            return null;
        }

        public override void ClearFrames()
        {
            foreach (Tile tile in tiles)
            {
                tile.TileLayerToLayerInTile(this).ClearFrames();
            }

            keyFrames = new List<AnimationKeyFrame>();
            AddKeyFrame(0, Texture2DCreator.Transparent(width, height));
        }

        /// <summary>
        /// Rerenders all keyframes.
        /// </summary>
        public void RerenderKeyFrames() => RerenderKeyFrames(keyFrameIndices);
        /// <summary>
        /// Rerenders the section of every keyframe within the given rect.
        /// </summary>
        public void RerenderKeyFrames(IntRect rect) => RerenderKeyFrames(keyFrameIndices, rect);
        /// <summary>
        /// Rerenders the given pixels of every keyframe.
        /// </summary>
        public void RerenderKeyFrames(IEnumerable<IntVector2> pixels) => RerenderKeyFrames(keyFrameIndices, pixels);
        /// <summary>
        /// Rerenders the given keyframes.
        /// </summary>
        public void RerenderKeyFrames(int[] keyFrames) => RerenderKeyFrames(keyFrames, rect);
        /// <summary>
        /// Rerenders the section of the given keyframes within the given rect.
        /// </summary>
        public void RerenderKeyFrames(int[] keyFrames, IntRect rect) => RerenderKeyFrames(keyFrames, rect);
        /// <summary>
        /// Rerenders the given pixels of the given keyframes.
        /// </summary>
        public void RerenderKeyFrames(int[] keyFrames, IEnumerable<IntVector2> pixels)
        {
            foreach (int keyframeIndex in keyFrames)
            {
                RerenderKeyFrame(keyframeIndex, pixels);
            }
        }
        /// <summary>
        /// Rerenders the keyframe.
        /// </summary>
        public void RerenderKeyFrame(int frame) => RerenderKeyFrame(frame, rect);
        /// <summary>
        /// Rerenders the section of the keyframe within the given rect.
        /// </summary>
        public void RerenderKeyFrame(int frame, IntRect rect) => RerenderKeyFrame(frame, rect);
        /// <summary>
        /// Rerenders the given pixels of the keyframe.
        /// </summary>
        public void RerenderKeyFrame(int frame, IEnumerable<IntVector2> pixels)
        {
            foreach (IntVector2 pixel in pixels)
            {
                Tile tile = PixelToTileNoValidation(pixel);
                if (tile == null)
                {
                    GetKeyFrame(frame).texture.SetPixel(pixel, Config.Colours.transparent);
                }
                else
                {
                    GetKeyFrame(frame).texture.SetPixel(pixel, tile.TileLayerToLayerInTile(this).GetPixel(new IntVector2(pixel.x, pixel.y) - tile.bottomLeft, frame));
                }
            }
        }

        // Event callbacks

        /// <summary>
        /// Called when some pixels are changed within a tile.
        /// </summary>
        private void OnTilePixelsChanged(IEnumerable<IntVector2> pixels, Layer layer, int[] frames)
        {
            if (ignoreOnTilePixelsChanged)
            {
                return;
            }

            foreach (Tile tile in tiles)
            {
                if (tile.TileLayerToLayerInTile(this) == layer)
                {
                    IEnumerable<IntVector2> linkedPixels = GetLinkedPixels(pixels.Select(p => p + tile.bottomLeft));
                    RerenderKeyFrames(frames, linkedPixels);
                    onPixelsChanged.Invoke(linkedPixels, frames);

                    return;
                }
            }
        }

        /// <summary>
        /// Called when a tile's rect changes - e.g. by moving / rotating / scaling.
        /// </summary>
        private void OnTileRectChanged(Tile tile, IntRect previousRect)
        {
            foreach (IntVector2 pixel in previousRect)
            {
                if (PixelToTileNoValidation(pixel) == tile)
                {
                    pixelToTile.Remove(pixel);
                }
            }
            foreach (IntVector2 pixel in tile.rect)
            {
                pixelToTile[pixel] = tile;
            }

            if (!ignoreOnTilePixelsChanged)
            {
                RerenderKeyFrames(previousRect);
                RerenderKeyFrames(tile.rect);
            }
        }

        public new class JsonConverter : JsonConversion.JsonConverter<TileLayer, JsonData.Object>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            public override JsonData.Object ToJson(TileLayer layer)
            {
                throw new System.NotImplementedException();
            }

            public override TileLayer FromJson(JsonData.Object jsonData)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
