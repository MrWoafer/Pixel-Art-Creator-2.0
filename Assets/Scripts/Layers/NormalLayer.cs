using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        return useLayerOpacity ? Colours.Multiply(GetKeyFrame(frame).texture.GetPixel(pixel.x, pixel.y), new Color(1f, 1f, 1f, opacity)) : GetKeyFrame(frame).texture.GetPixel(pixel.x, pixel.y);
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

    public override JSON ToJSON()
    {
        JSON json = new JSON();

        json.Add("layerType", "normal");
        json.Add("name", name);
        json.Add("width", width);
        json.Add("height", height);
        json.Add("visible", visible);
        json.Add("locked", locked);
        json.Add("opacity", opacity);
        json.Add("blendMode", blendMode.ToString());

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

    protected override void LoadJSON(JSON json)
    {
        if (int.Parse(json[".pacVersion"]) < 7)
        {
            LayerAnimation layerAnimation = LayerAnimation.FromJSON(JSON.ParseString(json["animation"]));

            name = json["layerName"];
            width = int.Parse(json["width"]);
            height = int.Parse(json["height"]);
            visible = bool.Parse(json["visible"]);
            locked = bool.Parse(json["locked"]);
            opacity = float.Parse(json["opacity"]);
            blendMode = Colours.StringToBlendMode(json["blendMode"]);
            /// (Because I previously incorrectly named Normal blend mode as Overlay
            if (int.Parse(json[".pacVersion"]) <= 3 && blendMode == BlendMode.Overlay)
            {
                blendMode = BlendMode.Normal;
            }

            keyFrames = layerAnimation.keyFrames;
        }
        else
        {
            name = json["name"];
            width = int.Parse(json["width"]);
            height = int.Parse(json["height"]);
            visible = bool.Parse(json["visible"]);
            locked = bool.Parse(json["locked"]);
            opacity = float.Parse(json["opacity"]);
            blendMode = Colours.StringToBlendMode(json["blendMode"]);

            foreach (string keyFrameJSONStr in JSON.SplitArray(json["keyFrames"]))
            {
                JSON keyFrameJSON = new JSON();
                keyFrameJSON.Add(".pacVersion", json[".pacVersion"]);
                keyFrameJSON.Append(JSON.ParseString(keyFrameJSONStr));
                AddKeyFrame(AnimationKeyFrame.FromJSON(keyFrameJSON));
            }
        }
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
}
