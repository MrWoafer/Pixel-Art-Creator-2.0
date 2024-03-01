using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class representing a single keyframe for a layer.
/// </summary>
public class AnimationKeyFrame : IJSONable
{
    /// <summary>The number of the frame this keyframe is on.</summary>
    public int frame;
    /// <summary>The texture displayed at this keyframe.</summary>
    public Texture2D texture;

    public AnimationKeyFrame(int frame, Texture2D texture)
    {
        if (frame < 0)
        {
            throw new System.ArgumentException("Frame index cannot be negative: " + frame);
        }

        this.frame = frame;
        this.texture = texture;
    }

    /// <summary>
    /// Creates a deep copy of the AnimationKeyFrame.
    /// </summary>
    public AnimationKeyFrame(AnimationKeyFrame animationKeyFrame) : this(animationKeyFrame.frame, Tex2DSprite.Copy(animationKeyFrame.texture)) { }

    /// <summary>
    /// Creates a deep copy of the AnimationKeyFrame.
    /// </summary>
    public AnimationKeyFrame DeepCopy()
    {
        return new AnimationKeyFrame(this);
    }

    public JSON ToJSON()
    {
        JSON json = new JSON();

        json.Add("frame", frame);

        string[] texRows = new string[texture.height];
        for (int row = 0; row < texture.height; row++)
        {
            string[] rowColours = new string[texture.width];
            for (int column = 0; column < texture.width; column++)
            {
                Color colour = texture.GetPixel(column, row);
                rowColours[column] = "(" + colour.r + ", " + colour.g + ", " + colour.b + ", " + colour.a + ")";
            }

            texRows[row] = "[" + string.Join(", ", rowColours) + "]";
        }
        json.Add("texture", texRows, false, false);

        return json;
    }

    public static AnimationKeyFrame FromJSON(JSON json)
    {
        int height = JSON.SplitArray(json["texture"]).Length;
        int width = JSON.SplitArray(JSON.SplitArray(json["texture"])[0]).Length;
        Texture2D tex = new Texture2D(width, height);

        string[] rows = JSON.SplitArray(json["texture"]);
        for (int y = 0; y < height; y++)
        {
            string[] row = JSON.SplitArray(rows[y]);
            for (int x = 0; x < width; x++)
            {
                string[] colour = row[x].Trim('(', ')').Replace(" ", "").Split(',');

                tex.SetPixel(x, y, new Color(float.Parse(colour[0]), float.Parse(colour[1]), float.Parse(colour[2]), float.Parse(colour[3])));
            }
        }
        tex.Apply();

        if (!json.ContainsKey(".pacVersion") || int.Parse(json[".pacVersion"]) < 8)
        {
            return new AnimationKeyFrame(int.Parse(json["frameIndex"]), tex);
        }
        else
        {
            return new AnimationKeyFrame(int.Parse(json["frame"]), tex);
        }
    }
}
