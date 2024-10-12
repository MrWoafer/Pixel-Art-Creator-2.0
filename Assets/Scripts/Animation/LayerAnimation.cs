using System.Collections.Generic;
using UnityEngine;

// This class is no longer used
namespace PAC.Animation
{
    [System.Serializable]
    [System.Obsolete("LayerAnimation has now been moved directly into the Layer class.")]
    public class LayerAnimation
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public int numOfFrames { get; private set; } = 10;

        /// <summary>
        /// The key frames of the animation, in order of frame number.
        /// </summary>
        public List<AnimationKeyFrame> keyFrames { get; private set; }

        /// <summary>
        /// The number of key frames in the animation.
        /// </summary>
        public int keyFrameCount
        {
            get
            {
                return keyFrames.Count;
            }
        }

        /// <summary>
        /// The frame indices of the key frames in the animation.
        /// </summary>
        public int[] keyFrameIndices
        {
            get
            {
                int[] indices = new int[keyFrameCount];
                for (int i = 0; i < keyFrameCount; i++)
                {
                    indices[i] = keyFrames[i].frame;
                }
                return indices;
            }
        }

        /// <summary>
        /// Create an animation with a blank starting key frame.
        /// </summary>
        /// <param name="width">The width of the frames.</param>
        /// <param name="height">The height of the frames.</param>
        /// <param name="texture">The texture for the starting key frame.</param>
        public LayerAnimation(int width, int height) : this(Tex2DSprite.BlankTexture(width, height)) { }

        /// <summary>
        /// Create an animation with the given texture as the starting key frame.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="texture">The texture for the starting key frame.</param>
        public LayerAnimation(Texture2D texture)
        {
            if (texture.width <= 0)
            {
                throw new System.ArgumentException("Width must be positive: " + texture.width);
            }
            if (texture.height <= 0)
            {
                throw new System.ArgumentException("Height must be positive: " + texture.height);
            }

            width = texture.width;
            height = texture.height;

            keyFrames = new List<AnimationKeyFrame>();
            AddKeyFrame(0, texture);
        }

        /// <summary>
        /// Creates a deep copy of the given layer animation, including deep copies of the key frames.
        /// </summary>
        /// <param name="layerAnimation"></param>
        public LayerAnimation(LayerAnimation layerAnimation)
        {
            width = layerAnimation.width;
            height = layerAnimation.height;

            keyFrames = new List<AnimationKeyFrame>();
            foreach (AnimationKeyFrame keyFrame in layerAnimation.keyFrames)
            {
                keyFrames.Add(new AnimationKeyFrame(keyFrame));
            }
        }

        /// <summary>
        /// Returns the most recent key frame before or at frame frame - i.e. the frame the animation will play at frame frame. Same as GetKeyFrame(i).
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public AnimationKeyFrame this[int i]
        {
            get
            {
                return GetKeyFrame(i);
            }
        }

        /// <summary>
        /// Returns the most recent key frame before or at frame frame - i.e. the frame the animation will play at frame frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public AnimationKeyFrame GetKeyFrame(int frame)
        {
            if (keyFrameCount == 0)
            {
                throw new System.Exception("Animation has no key frames.");
            }

            if (frame < 0)
            {
                throw new System.IndexOutOfRangeException("Frame index must be non-negative: " + frame);
            }

            for (int i = 0; i < keyFrameCount; i++)
            {
                if (keyFrames[i].frame > frame)
                {
                    return keyFrames[i - 1];
                }
            }

            return keyFrames[keyFrameCount - 1];
        }

        /// <summary>
        /// Returns whether or not there is a key frame at the given frame index.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool HasKeyFrameAt(int frame)
        {
            foreach(AnimationKeyFrame keyFrame in keyFrames)
            {
                if (keyFrame.frame == frame)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes the most recent key frame before or at the given frame index and returns that key frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public AnimationKeyFrame DeleteMostRecentKeyFrame(int frame)
        {
            return DeleteKeyFrame(GetKeyFrame(frame).frame);
        }
        /// <summary>
        /// Deletes the key frame at the given frame index, if there is one, in which case it returns that key frame. Otherwise it returns null.
        /// </summary>
        /// <param name="keyframe"></param>
        /// <returns></returns>
        public AnimationKeyFrame DeleteKeyFrame(int keyframe)
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
        /// <summary>
        /// Deletes the given key frame, if it's in the animation, in which case it returns true. Otherwise it returns false.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool DeleteKeyFrame(AnimationKeyFrame keyFrame)
        {
            for(int i = 0; i < keyFrameCount; i++)
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
        public void Clear()
        {
            keyFrames = new List<AnimationKeyFrame>();
            AddKeyFrame(0, Tex2DSprite.BlankTexture(width, height));
        }

        /// <summary>
        /// Adds a key frame at frame frame. The texture will be that of the most recent key frame. Returns true if it replaces an existing key frame, and false otherwise.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public bool AddKeyFrame(int frame)
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
        /// <param name="frame"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public bool AddKeyFrame(int frame, Texture2D texture)
        {
            return AddKeyFrame(new AnimationKeyFrame(frame, Tex2DSprite.Copy(texture)));
        }
        /// <summary>
        /// Adds the given key frame. Returns true if it replaces an existing key frame, and false otherwise.
        /// </summary>
        /// <param name="keyFrame"></param>
        /// <returns></returns>
        public bool AddKeyFrame(AnimationKeyFrame keyFrame)
        {
            if (keyFrame.frame < 0)
            {
                throw new System.IndexOutOfRangeException("Frame index cannot be negative: " + keyFrame.frame);
            }

            if (keyFrame.texture.width != width || keyFrame.texture.height != height)
            {
                throw new System.Exception("Key frame dimensions (" + keyFrame.texture.width + ", " + keyFrame.texture.height + ") do not match animation dimensions (" + width + ", " + height + ").");
            }

            for (int i = 0; i < keyFrameCount; i++)
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

            return false;
        }

        /*
    public void SetTexture(int frame, Texture2D texture, AnimFrameRefMode frameRefMode)
    {
        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame, texture);
        }
        else if (frameRefMode == AnimFrameRefMode.MostRecentKeyFrame)
        {
            this[frame].texture = texture;
        }
    }

    public void OverlayTexture(int frame, Texture2D overlayTex, AnimFrameRefMode frameRefMode)
    {
        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame);
        }

        this[frame].OverlayTexture(overlayTex);
    }
    public void OverlayTexture(int frame, Texture2D overlayTex, IntVector2 offset, AnimFrameRefMode frameRefMode)
    {
        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame);
        }

        this[frame].OverlayTexture(overlayTex, offset);
    }
    public void OverlayTexture(Texture2D overlayTex)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.OverlayTexture(overlayTex);
        }
    }
    public void OverlayTexture(Texture2D overlayTex, IntVector2 offset)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.OverlayTexture(overlayTex, offset);
        }
    }

    public void Offset(int frame, IntVector2 offset, AnimFrameRefMode frameRefMode)
    {
        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame);
        }

        this[frame].Offset(offset);
    }
    public void Offset(IntVector2 offset)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Offset(offset);
        }
    }

    public void Rotate(int frame, RotationAngle angle, AnimFrameRefMode frameRefMode)
    {
        if (width != height && keyFrameCount > 1)
        {
            throw new System.Exception("When the animation has > 1 key frame, you only rotate a single frame when the texture is square: (width, height) = (" + width + ", " + height + ")");
        }

        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame);
        }

        this[frame].Rotate(angle);
    }
    public void Rotate(RotationAngle angle)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Rotate(angle);
        }

        width = keyFrames[0].texture.width;
        height = keyFrames[0].texture.height;
    }

    public void Flip(int frame, FlipDirection direction, AnimFrameRefMode frameRefMode)
    {
        if (frameRefMode == AnimFrameRefMode.NewKeyFrame)
        {
            AddKeyFrame(frame);
        }

        this[frame].Flip(direction);
    }
    public void Flip(FlipDirection direction)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Flip(direction);
        }
    }

    public void Extend(int left, int right, int up, int down)
    {
        foreach(AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Extend(left, right, up, down);
        }

        width = keyFrames[0].texture.width;
        height = keyFrames[0].texture.height;
    }

    public void Scale(float scaleFactor)
    {
        Scale(scaleFactor, scaleFactor);
    }
    public void Scale(float xScaleFactor, float yScaleFactor)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Scale(xScaleFactor, yScaleFactor);
        }

        width = keyFrames[0].texture.width;
        height = keyFrames[0].texture.height;
    }
    public void Scale(int newWidth, int newHeight)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Scale(newWidth, newHeight);
        }

        width = keyFrames[0].texture.width;
        height = keyFrames[0].texture.height;
    }

    public void Multiply(Color colour)
    {
        foreach (AnimationKeyFrame keyFrame in keyFrames)
        {
            keyFrame.Multiply(colour);
        }
    }
    */

        public JSON.JSON ToJSON()
        {
            JSON.JSON json = new JSON.JSON();

            json.Add("width", width);
            json.Add("height", height);

            string keyFramesStr = "[";
            foreach (AnimationKeyFrame keyFrame in keyFrames)
            {
                keyFramesStr += "\n";

                string keyFrameJSON = keyFrame.ToJSON().ToString();

                string[] lines = keyFrameJSON.Split('\n');
                foreach (string line in lines)
                {
                    keyFramesStr += "\t" + line + "\n";
                }

                keyFramesStr = keyFramesStr.Remove(keyFramesStr.Length - 1);

                keyFramesStr += ",";
            }
            keyFramesStr = keyFramesStr.TrimEnd(',') + "\n]";

            json.Add("keyFrames", keyFramesStr, false);

            return json;
        }

        public static LayerAnimation FromJSON(JSON.JSON json)
        {
            LayerAnimation anim = new LayerAnimation(int.Parse(json["width"]), int.Parse(json["height"]));

            foreach (string keyFrameJSON in JSON.JSON.SplitArray(json["keyFrames"]))
            {
                anim.AddKeyFrame(AnimationKeyFrame.FromJSON(JSON.JSON.Parse(keyFrameJSON)));
            }

            return anim;
        }
    }
}
