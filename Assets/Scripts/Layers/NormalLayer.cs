using System.Collections.Generic;
using System.Runtime.Serialization;

using PAC.Animation;
using PAC.Colour;
using PAC.DataStructures;
using PAC.Extensions;
using PAC.Json;

using UnityEngine;

namespace PAC.Layers
{
    /// <summary>
    /// A class to represent a normal layer - one that can be drawn on as a regular image.
    /// </summary>
    public class NormalLayer : Layer
    {
        public override LayerType layerType => LayerType.Normal;

        public NormalLayer(int width, int height) : this("", Texture2DExtensions.Transparent(width, height)) { }
        public NormalLayer(Texture2D texture) : this("", texture) { }
        public NormalLayer(string name, int width, int height) : this(name, Texture2DExtensions.Transparent(width, height)) { }
        public NormalLayer(string name, Texture2D texture) : base(name, texture) { }

        /// <summary>
        /// Creates a deep copy of the NormalLayer.
        /// </summary>
        public NormalLayer(NormalLayer layer) : base(layer) { }

        public override Layer DeepCopy()
        {
            return new NormalLayer(this);
        }

        protected override IEnumerable<IntVector2> SetPixelsNoEvent(IEnumerable<IntVector2> pixels, int frame, Color colour, AnimFrameRefMode frameRefMode)
        {
            foreach (IntVector2 pixel in pixels)
            {
                if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
                {
                    AddKeyFrame(frame);
                }

                GetKeyFrame(frame).texture.SetPixel(pixel.x, pixel.y, colour);
            }
            return pixels;
        }

        public override Color GetPixel(IntVector2 pixel, int frame, bool useLayerOpacity = true)
        {
            if (pixel.x < 0 || pixel.y < 0 || pixel.x >= width || pixel.y >= height)
            {
                throw new System.Exception("Coords (" + pixel.x + ", " + pixel.y + ") outside of dimensions " + width + "x" + height);
            }

            return useLayerOpacity ? BlendMode.MultiplyColours(GetKeyFrame(frame).texture.GetPixel(pixel.x, pixel.y), new Color(1f, 1f, 1f, opacity)) : GetKeyFrame(frame).texture.GetPixel(pixel.x, pixel.y);
        }

        /// <summary>
        /// Sets the texture at the given frame.
        /// </summary>
        public void SetTexture(int frame, Texture2D texture, AnimFrameRefMode frameRefMode)
        {
            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame, texture);
            }
            else if (frameRefMode == AnimFrameRefMode.MostRecentKeyFrame)
            {
                GetKeyFrame(frame).texture = texture;
            }

            onPixelsChanged.Invoke(rect, new int[] { GetKeyFrame(frame).frame });
        }

        /// <summary>
        /// Overlays the texture onto the given frame. Uses Normal blend mode.
        /// </summary>
        public void OverlayTexture(int frame, Texture2D overlayTex, AnimFrameRefMode frameRefMode) => OverlayTexture(frame, overlayTex, IntVector2.zero, frameRefMode);
        /// <summary>
        /// Overlays the texture onto the given frame, placing the bottom-left corner at the coordinates 'offset' (which don't have to be within the image). Uses Normal blend mode.
        /// </summary>
        public void OverlayTexture(int frame, Texture2D overlayTex, IntVector2 offset, AnimFrameRefMode frameRefMode)
        {
            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame);
            }

            GetKeyFrame(frame).texture = Texture2DExtensions.Blend(overlayTex, GetKeyFrame(frame).texture, offset, BlendMode.Normal);
            onPixelsChanged.Invoke(rect, new int[] { GetKeyFrame(frame).frame });
        }
        /// <summary>
        /// Overlays the texture onto every frame. Uses Normal blend mode.
        /// </summary>
        public void OverlayTexture(Texture2D overlayTex) => OverlayTexture(overlayTex, IntVector2.zero);
        /// <summary>
        /// Overlays the texture onto every frame, placing the bottom-left corner at the coordinates 'offset' (which don't have to be within the image). Uses Normal blend mode.
        /// </summary>
        public void OverlayTexture(Texture2D overlayTex, IntVector2 offset)
        {
            foreach (int keyFrameIndex in keyFrameIndices)
            {
                OverlayTexture(keyFrameIndex, overlayTex, offset, AnimFrameRefMode.MostRecentKeyFrame);
            }
        }

        /// <summary>
        /// Flips the given frame of the layer.
        /// </summary>
        public void Flip(int frame, FlipAxis axis, AnimFrameRefMode frameRefMode)
        {
            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame);
            }

            GetKeyFrame(frame).texture = Texture2DExtensions.Flip(GetKeyFrame(frame).texture, axis);

            onPixelsChanged.Invoke(rect, new int[] { GetKeyFrame(frame).frame });
        }
        protected override void FlipNoEvent(FlipAxis axis)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Texture2DExtensions.Flip(keyFrame.texture, axis);
            }
        }

        /// <summary>
        /// Rotates the given frame of the layer. Rotation is clockwise.
        /// </summary>
        public void Rotate(int frame, RotationAngle angle, AnimFrameRefMode frameRefMode)
        {
            if (width != height && keyFrames.Count > 1)
            {
                throw new System.Exception("When the animation has > 1 key frame, you only rotate a single frame when the texture is square: (width, height) = (" + width + ", " + height + ")");
            }

            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame);
            }

            GetKeyFrame(frame).texture = Texture2DExtensions.Rotate(GetKeyFrame(frame).texture, angle);

            onPixelsChanged.Invoke(rect, new int[] { GetKeyFrame(frame).frame });
        }
        protected override void RotateNoEvent(RotationAngle angle)
        {
            if (angle == RotationAngle._0)
            {
                return;
            }

            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Texture2DExtensions.Rotate(keyFrame.texture, angle);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }

        protected override void ExtendNoEvent(int left, int right, int up, int down)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Texture2DExtensions.ExtendCrop(keyFrame.texture, left, right, down, up);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }

        protected override void ScaleNoEvent(float xScaleFactor, float yScaleFactor)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Texture2DExtensions.Scale(keyFrame.texture, xScaleFactor, yScaleFactor);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }
        protected override void ScaleNoEvent(int newWidth, int newHeight)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Texture2DExtensions.Scale(keyFrame.texture, newWidth, newHeight);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }

        protected override AnimationKeyFrame DeleteKeyFrameNoEvent(int keyframe)
        {
            if (HasKeyFrameAt(keyframe))
            {
                AnimationKeyFrame keyFrame = GetKeyFrame(keyframe);
                keyFrames.Remove(keyFrame);

                if (keyframe == 0)
                {
                    AddKeyFrame(0, Texture2DExtensions.Transparent(width, height));
                }

                return keyFrame;
            }
            return null;
        }

        public override void ClearFrames()
        {
            keyFrames = new List<AnimationKeyFrame>();
            AddKeyFrame(0, Texture2DExtensions.Transparent(width, height));
        }

        public new class JsonConverter : JsonConversion.JsonConverter<NormalLayer, JsonData.Object>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            public override JsonData.Object ToJson(NormalLayer layer)
            {
                return new JsonData.Object
                    {
                        { "layer type", "Normal" },
                        { "name", layer.name },
                        { "width", layer.width },
                        { "height", layer.height },
                        { "visible", layer.visible },
                        { "locked", layer.locked },
                        { "opacity", layer.opacity },
                        { "blend mode", JsonConversion.ToJson(layer.blendMode, new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter()), false) },
                        { "keyframes", JsonConversion.ToJson(layer.keyFrames, new JsonConversion.JsonConverterSet(new AnimationKeyFrame.JsonConverter(Config.Files.fileFormatVersion)), false) }
                    };
            }

            public override NormalLayer FromJson(JsonData.Object jsonData)
            {
                if (fromJsonFileFormatVersion > Config.Files.fileFormatVersion)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is ahead of the current version " + Config.Files.fileFormatVersion);
                }
                if (fromJsonFileFormatVersion.major < Config.Files.fileFormatVersion.major)
                {
                    throw new SerializationException("The JSON uses file format version " + fromJsonFileFormatVersion + ", which is out of date with the current version " + Config.Files.fileFormatVersion);
                }

                string layerType = JsonConversion.FromJson<string>(jsonData["layer type"]);
                if (layerType.ToLower() != "normal")
                {
                    throw new SerializationException("Expected layer type normal, but found " + layerType);
                }

                string name = JsonConversion.FromJson<string>(jsonData["name"]);
                int width = JsonConversion.FromJson<int>(jsonData["width"]);
                int height = JsonConversion.FromJson<int>(jsonData["height"]);

                NormalLayer layer = new NormalLayer(name, width, height);

                layer.visible = JsonConversion.FromJson<bool>(jsonData["visible"]);
                layer.locked = JsonConversion.FromJson<bool>(jsonData["locked"]);

                layer.opacity = JsonConversion.FromJson<float>(jsonData["opacity"]);
                layer.blendMode = JsonConversion.FromJson<BlendMode>(jsonData["blend mode"], new JsonConversion.JsonConverterSet(new BlendMode.JsonConverter()), false);

                layer.keyFrames = JsonConversion.FromJson<List<AnimationKeyFrame>>(jsonData["keyframes"],
                    new JsonConversion.JsonConverterSet(new AnimationKeyFrame.JsonConverter(fromJsonFileFormatVersion)), false);

                return layer;
            }
        }
    }
}
