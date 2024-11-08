using System.Collections.Generic;
using System.Runtime.Serialization;
using PAC.Animation;
using PAC.Colour;
using PAC.DataStructures;
using PAC.Json;
using PAC.JSON;
using UnityEngine;

namespace PAC.Layers
{
    /// <summary>
    /// A class to represent a normal layer - one that can be drawn on as a regular image.
    /// </summary>
    public class NormalLayer : Layer
    {
        public override LayerType layerType => LayerType.Normal;

        public NormalLayer(int width, int height) : this("", Tex2DSprite.BlankTexture(width, height)) { }
        public NormalLayer(Texture2D texture) : this("", texture) { }
        public NormalLayer(string name, int width, int height) : this(name, Tex2DSprite.BlankTexture(width, height)) { }
        public NormalLayer(string name, Texture2D texture) : base(name, texture) { }

        /// <summary>
        /// Creates a deep copy of the NormalLayer.
        /// </summary>
        public NormalLayer(NormalLayer layer) : base(layer) { }

        public override Layer DeepCopy()
        {
            return new NormalLayer(this);
        }

        protected override IntVector2[] SetPixelsNoEvent(IntVector2[] pixels, int frame, Color colour, AnimFrameRefMode frameRefMode)
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

            onPixelsChanged.Invoke(rect.points, new int[] { GetKeyFrame(frame).frame });
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

            GetKeyFrame(frame).texture = Tex2DSprite.Overlay(overlayTex, GetKeyFrame(frame).texture, offset);
            onPixelsChanged.Invoke(rect.points, new int[] { GetKeyFrame(frame).frame });
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
        /// Offsets the texture at the given frame. (Moves the texture so the bottom-left corner is at the coordinates 'offset'.
        /// </summary>
        public void Offset(int frame, IntVector2 offset, AnimFrameRefMode frameRefMode)
        {
            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame);
            }

            GetKeyFrame(frame).texture = Tex2DSprite.Offset(GetKeyFrame(frame).texture, offset);
        }
        /// <summary>
        /// Offsets the texture of every frame. (Moves the texture so the bottom-left corner is at the coordinates 'offset'.
        /// </summary>
        public void Offset(IntVector2 offset)
        {
            foreach (int keyFrameIndex in keyFrameIndices)
            {
                Offset(keyFrameIndex, offset, AnimFrameRefMode.MostRecentKeyFrame);
            }
        }

        /// <summary>
        /// Flips the given frame of the layer.
        /// </summary>
        public void Flip(int frame, FlipDirection direction, AnimFrameRefMode frameRefMode)
        {
            if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
            {
                AddKeyFrame(frame);
            }

            GetKeyFrame(frame).texture = Tex2DSprite.Flip(GetKeyFrame(frame).texture, direction);

            onPixelsChanged.Invoke(rect.points, new int[] { GetKeyFrame(frame).frame });
        }
        protected override void FlipNoEvent(FlipDirection direction)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Tex2DSprite.Flip(keyFrame.texture, direction);
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

            GetKeyFrame(frame).texture = Tex2DSprite.Rotate(GetKeyFrame(frame).texture, angle);

            onPixelsChanged.Invoke(rect.points, new int[] { GetKeyFrame(frame).frame });
        }
        protected override void RotateNoEvent(RotationAngle angle)
        {
            if (angle == RotationAngle._0)
            {
                return;
            }

            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Tex2DSprite.Rotate(keyFrame.texture, angle);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }

        protected override void ExtendNoEvent(int left, int right, int up, int down)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Tex2DSprite.Extend(keyFrame.texture, left, right, up, down);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }

        protected override void ScaleNoEvent(float xScaleFactor, float yScaleFactor)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Tex2DSprite.Scale(keyFrame.texture, xScaleFactor, yScaleFactor);
            }

            width = keyFrames[0].texture.width;
            height = keyFrames[0].texture.height;
        }
        protected override void ScaleNoEvent(int newWidth, int newHeight)
        {
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFrame.texture = Tex2DSprite.Scale(keyFrame.texture, newWidth, newHeight);
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
                    AddKeyFrame(0, Tex2DSprite.BlankTexture(width, height));
                }

                return keyFrame;
            }
            return null;
        }

        public override void ClearFrames()
        {
            keyFrames = new List<AnimationKeyFrame>();
            AddKeyFrame(0, Tex2DSprite.BlankTexture(width, height));
        }

        public new class JsonConverter : JsonConversion.JsonConverter<NormalLayer, JsonObj>
        {
            private SemanticVersion fromJsonFileFormatVersion;

            public JsonConverter(SemanticVersion fromJsonFileFormatVersion)
            {
                this.fromJsonFileFormatVersion = fromJsonFileFormatVersion;
            }

            public override JsonObj ToJson(NormalLayer layer)
            {
                return new JsonObj
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

            public override NormalLayer FromJson(JsonObj jsonData)
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
