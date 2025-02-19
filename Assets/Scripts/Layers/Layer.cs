using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using PAC.Animation;
using PAC.Colour;
using PAC.DataStructures;
using PAC.Extensions;
using PAC.Json;

using UnityEngine;
using UnityEngine.Events;

namespace PAC.Layers
{
    public enum AnimFrameRefMode
    {
        NewKeyFrame = 0,
        MostRecentKeyFrame = 1
    }

    public enum LayerType
    {
        Normal = 0,
        Tile = 1
    }

    /// <summary>
    /// An abstract class to define what each type of layer must have.
    /// </summary>
    public abstract class Layer
    {
        public abstract LayerType layerType { get; }

        public string name { get; set; } = "";

        public int width { get; protected set; }
        public int height { get; protected set; }
        public IntRect rect { get => new IntRect(IntVector2.zero, new IntVector2(width - 1, height - 1)); }

        protected bool _visible = true;
        public bool visible
        {
            get => _visible;
            set
            {
                _visible = value;
                onVisibilityChanged.Invoke();
            }
        }
        public bool locked { get; set; } = false;
        public float opacity { get; set; } = 1f;
        protected BlendMode _blendMode = BlendMode.Normal;
        public BlendMode blendMode
        {
            get => _blendMode;
            set
            {
                _blendMode = value;
                onBlendModeChanged.Invoke();
            }
        }

        /// <summary>
        /// The key frames of the animation, in order.
        /// </summary>
        public List<AnimationKeyFrame> keyFrames { get; protected set; } = new List<AnimationKeyFrame>();
        /// <summary>
        /// The frame indices of the key frames in the animation, in order.
        /// </summary>
        public int[] keyFrameIndices
        {
            get
            {
                int[] indices = new int[keyFrames.Count];
                for (int i = 0; i < keyFrames.Count; i++)
                {
                    indices[i] = keyFrames[i].frame;
                }
                return indices;
            }
        }
        /// <summary>
        /// The textures of the key frames in the animation, in order.
        /// </summary>
        public Texture2D[] keyFrameTextures
        {
            get
            {
                Texture2D[] textures = new Texture2D[keyFrames.Count];
                for (int i = 0; i < keyFrames.Count; i++)
                {
                    textures[i] = keyFrames[i].texture;
                }
                return textures;
            }
        }

        // Events

        /// <summary>
        /// Called when the SetPixel() or SetPixels() has been called.
        /// Passes the array of pixels changed and the frames it was called on.
        /// </summary>
        protected UnityEvent<IEnumerable<IntVector2>, int[]> onPixelsChanged = new UnityEvent<IEnumerable<IntVector2>, int[]>();
        /// <summary>Called when the layer's visibility is changed.</summary>
        private UnityEvent onVisibilityChanged = new UnityEvent();
        /// <summary>Called when the layer's blend mode is changed.</summary>
        private UnityEvent onBlendModeChanged = new UnityEvent();
        /// <summary>
        /// Called when a key frame is added to a layer.
        /// Passes the frame index that the key frame was added to.
        /// </summary>
        private UnityEvent<int> onKeyFrameAdded = new UnityEvent<int>();
        /// <summary>
        /// Called when a key frame is removed from a layer.
        /// Passes the frame index that the key frame was removed at.
        /// </summary>
        private UnityEvent<int> onKeyFrameRemoved = new UnityEvent<int>();

        public Layer(string name, Texture2D texture)
        {
            this.name = name;
            width = texture.width;
            height = texture.height;

            AddKeyFrame(0, texture);
        }

        public Layer(Layer layer) : this(layer.name, new Texture2D(layer.width, layer.height))
        {
            visible = layer.visible;
            locked = layer.locked;
            opacity = layer.opacity;
            blendMode = layer.blendMode;

            foreach (AnimationKeyFrame keyFrame in layer.keyFrames)
            {
                AddKeyFrame(new AnimationKeyFrame(keyFrame));
            }
        }

        /// <summary>
        /// Creates a deep copy of the Layer.
        /// </summary>
        public abstract Layer DeepCopy();

        /// <summary>
        /// Sets the colour of the pixel (x, y). Throws an error if the pixel is outside the layer.
        /// </summary>
        public IEnumerable<IntVector2> SetPixel(int x, int y, int frame, Color colour, AnimFrameRefMode frameRefMode) => SetPixel(new IntVector2(x, y), frame, colour, frameRefMode);
        /// <summary>
        /// Sets the colour of the pixel. Throws an error if the pixel is outside the layer.
        /// </summary>
        public IEnumerable<IntVector2> SetPixel(IntVector2 pixel, int frame, Color colour, AnimFrameRefMode frameRefMode) => SetPixels(new IntVector2[] { pixel }, frame, colour, frameRefMode);
        /// <summary>
        /// Sets the colour of the pixels.
        /// You do not need to check the pixels are in the layer as this check is done in Layer.SetPixels(), which is the only way this method is called.
        /// </summary>
        protected abstract IEnumerable<IntVector2> SetPixelsNoEvent(IEnumerable<IntVector2> pixels, int frame, Color colour, AnimFrameRefMode frameRefMode);
        /// <summary>
        /// Sets the colour of the pixels. Throws an error if a pixel is outside the layer.
        /// </summary>
        public IEnumerable<IntVector2> SetPixels(IEnumerable<IntVector2> pixels, int frame, Color colour, AnimFrameRefMode frameRefMode)
        {
            foreach (IntVector2 pixel in pixels)
            {
                if (!rect.Contains(pixel))
                {
                    throw new System.Exception("Pixel (" + pixel.x + ", " + pixel.y + ") outside of dimensions " + width + "x" + height);
                }
            }
            IEnumerable<IntVector2> pixelsFilled = SetPixelsNoEvent(pixels, frame, colour, frameRefMode);
            onPixelsChanged.Invoke(pixelsFilled, new int[] { frame });
            return pixelsFilled;
        }

        /// <summary>
        /// Gets the colour of the pixel (x, y).
        /// </summary>
        public Color GetPixel(int x, int y, int frame, bool useLayerOpacity = true) => GetPixel(new IntVector2(x, y), frame, useLayerOpacity);
        /// <summary>
        /// Gets the colour of the pixel.
        /// </summary>
        public abstract Color GetPixel(IntVector2 pixel, int frame, bool useLayerOpacity = true);

        /// <summary>
        /// Returns true if and only if all pixels are completely transparent.
        /// </summary>
        public bool IsBlank()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    foreach (int frame in keyFrameIndices)
                    {
                        if (GetPixel(x, y, frame, false).a != 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Flips the layer, but does not invoke the onPixelsChanged event.
        /// </summary>
        protected abstract void FlipNoEvent(FlipAxis axis);
        /// <summary>
        /// Flips the layer.
        /// </summary>
        public void Flip(FlipAxis axis)
        {
            FlipNoEvent(axis);
            onPixelsChanged.Invoke(rect, keyFrameIndices);
        }

        /// <summary>
        /// Rotates the layer, but does not invoke the onPixelsChanged event.
        /// Rotation is clockwise.
        /// </summary>
        protected abstract void RotateNoEvent(RotationAngle angle);
        /// <summary>
        /// Rotates the layer. Rotation is clockwise.
        /// </summary>
        public void Rotate(RotationAngle angle)
        {
            RotateNoEvent(angle);
            onPixelsChanged.Invoke(rect, keyFrameIndices);
        }

        /// <summary>
        /// Extends the dimensions of the layer in each direction by the given amounts, but does not invoke the onPixelsChanged event.
        /// </summary>
        protected abstract void ExtendNoEvent(int left, int right, int up, int down);
        /// <summary>
        /// Extends the dimensions of the layer in each direction by the given amounts.
        /// </summary>
        public void Extend(int left, int right, int up, int down)
        {
            ExtendNoEvent(left, right, up, down);
            onPixelsChanged.Invoke(rect, keyFrameIndices);
        }

        /// <summary>
        /// Resizes the dimensions of the file by the scale factors, but does not invoke the onPixelsChanged event.
        /// </summary>
        protected abstract void ScaleNoEvent(float xScaleFactor, float yScaleFactor);
        /// <summary>
        /// Resizes the dimensions to the new width and height, but does not invoke the onPixelsChanged event.
        /// </summary>
        protected abstract void ScaleNoEvent(int newWidth, int newHeight);
        /// <summary>
        /// Resizes the dimensions of the file by the scale factor.
        /// </summary>
        public void Scale(float scaleFactor) => ScaleNoEvent(scaleFactor, scaleFactor);
        /// <summary>
        /// Resizes the dimensions of the file by the scale factors.
        /// </summary>
        public void Scale(float xScaleFactor, float yScaleFactor)
        {
            ScaleNoEvent(xScaleFactor, yScaleFactor);
            onPixelsChanged.Invoke(rect, keyFrameIndices);
        }
        /// <summary>
        /// Resizes the dimensions of the file by the scale factor.
        /// </summary>
        public void Scale(int newWidth, int newHeight)
        {
            ScaleNoEvent(newWidth, newHeight);
            onPixelsChanged.Invoke(rect, keyFrameIndices);
        }


        /// Animation

        /// <summary>
        /// Returns the texture of the most recent key frame before or at the frame index. Same as GetTexture(frame).
        /// </summary>
        public AnimationKeyFrame this[int frame]
        {
            get => GetKeyFrame(frame);
        }

        /// <summary>
        /// Returns the most recent key frame before or at the frame index - i.e. the key frame the animation will play at the frame index.
        /// </summary>
        public AnimationKeyFrame GetKeyFrame(int frame)
        {
            if (keyFrames.Count == 0)
            {
                throw new System.Exception("Animation has no key frames.");
            }

            if (frame < 0)
            {
                throw new System.IndexOutOfRangeException("Frame index must be non-negative: " + frame);
            }

            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].frame > frame)
                {
                    return keyFrames[i - 1];
                }
            }

            return keyFrames[keyFrames.Count - 1];
        }

        /// <summary>
        /// Returns whether or not there is a key frame at the given frame index.
        /// </summary>
        public bool HasKeyFrameAt(int frame)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                if (keyFrame.frame == frame)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a key frame at frame frame. The texture will be that of the most recent key frame. Returns true if it replaces an existing key frame, and false otherwise.
        /// </summary>
        protected bool AddKeyFrame(int frame)
        {
            if (HasKeyFrameAt(frame))
            {
                return false;
            }
            return AddKeyFrame(frame, GetKeyFrame(frame).texture);
        }
        /// <summary>
        /// Adds a key frame with the given texture at frame frame. Returns true if it replaces an existing key frame, and false otherwise.
        /// </summary>
        protected bool AddKeyFrame(int frame, Texture2D texture) => AddKeyFrame(new AnimationKeyFrame(frame, Texture2DExtensions.DeepCopy(texture)));
        /// <summary>
        /// Adds the given key frame. Returns true if it replaces an existing key frame, and false otherwise.
        /// </summary>
        protected bool AddKeyFrame(AnimationKeyFrame keyFrame)
        {
            if (keyFrame.frame < 0)
            {
                throw new System.IndexOutOfRangeException("Frame index cannot be negative: " + keyFrame.frame);
            }

            if (keyFrame.texture.width != width || keyFrame.texture.height != height)
            {
                throw new System.Exception("Key frame dimensions (" + keyFrame.texture.width + ", " + keyFrame.texture.height + ") do not match animation dimensions (" + width + ", " + height + ").");
            }

            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].frame == keyFrame.frame)
                {
                    keyFrames[i] = keyFrame;
                    return true;
                }
                else if (keyFrames[i].frame > keyFrame.frame)
                {
                    keyFrames.Insert(i, keyFrame);
                    return false;
                }
            }

            keyFrames.Add(keyFrame);

            onKeyFrameAdded.Invoke(keyFrame.frame);
            return false;
        }

        /// <summary>
        /// Deletes the most recent key frame before or at the given frame index and returns that key frame.
        /// </summary>
        public AnimationKeyFrame DeleteMostRecentKeyFrame(int frame)
        {
            return DeleteKeyFrame(GetKeyFrame(frame).frame);
        }

        /// <summary>
        /// Deletes the key frame at the given frame index, if there is one, in which case it returns that key frame. Otherwise it returns null.
        /// </summary>
        protected abstract AnimationKeyFrame DeleteKeyFrameNoEvent(int keyframe);
        public AnimationKeyFrame DeleteKeyFrame(int keyFrame)
        {
            AnimationKeyFrame removed = DeleteKeyFrameNoEvent(keyFrame);
            onKeyFrameRemoved.Invoke(keyFrame);
            return removed;
        }
        /// <summary>
        /// Deletes the given key frame, if it's in the animation, in which case it returns true. Otherwise it returns false.
        /// </summary>
        public bool DeleteKeyFrame(AnimationKeyFrame keyFrame)
        {
            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i] == keyFrame)
                {
                    DeleteKeyFrame(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes all key frames.
        /// </summary>
        public abstract void ClearFrames();


        /// Events

        public void SubscribeToOnPixelsChanged(UnityAction<IEnumerable<IntVector2>, int[]> call)
        {
            onPixelsChanged.AddListener(call);
        }
        public void SubscribeToOnVisibilityChanged(UnityAction call)
        {
            onVisibilityChanged.AddListener(call);
        }
        public void SubscribeToOnBlendModeChanged(UnityAction call)
        {
            onBlendModeChanged.AddListener(call);
        }
        public void SubscribeToOnKeyFrameAdded(UnityAction<int> call)
        {
            onKeyFrameAdded.AddListener(call);
        }
        public void SubscribeToOnKeyFrameRemoved(UnityAction<int> call)
        {
            onKeyFrameRemoved.AddListener(call);
        }

        public static JsonConversion.JsonConverterSet GetJsonConverterSet(SemanticVersion fromJsonFileFormatVersion)
        {
            return new JsonConversion.JsonConverterSet(
                new NormalLayer.JsonConverter(fromJsonFileFormatVersion),
                new TileLayer.JsonConverter(fromJsonFileFormatVersion)
                );
        }

        public class JsonConverter : JsonConversion.JsonConverter<Layer, JsonData.Object>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            /// <summary>
            /// This currently can't be used since JsonConversion.ToJson() only works on concrete types, but Layer is abstract so you cannot have an object without concrete type Layer.
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public override JsonData.Object ToJson(Layer obj)
            {
                throw new NotImplementedException();
            }

            public override Layer FromJson(JsonData.Object jsonData)
            {
                string layerType = JsonConversion.FromJson<string>(jsonData["layer type"]).ToLower();
                if (layerType == "normal")
                {
                    return JsonConversion.FromJson<NormalLayer>(jsonData, new JsonConversion.JsonConverterSet(new NormalLayer.JsonConverter(fromJsonFileFormatVersion)), false);
                }
                if (layerType == "tile")
                {
                    return JsonConversion.FromJson<NormalLayer>(jsonData, new JsonConversion.JsonConverterSet(new NormalLayer.JsonConverter(fromJsonFileFormatVersion)), false);
                }
                throw new SerializationException("Unknown / unimplemented layer type: " + layerType);
            }
        }
    }
}