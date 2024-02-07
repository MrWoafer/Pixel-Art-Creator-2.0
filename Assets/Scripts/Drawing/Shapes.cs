using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shapes
{
    public static IntVector2[] LineCoords(IntVector2 start, IntVector2 end)
    {
        bool startWasToLeft = start.x <= end.x;

        List<IntVector2> coords = new List<IntVector2>();

        coords.Add(new IntVector2(start.x, start.y));

        if (start.x == end.x)
        {
            if (start.y == end.y)
            {
                return new IntVector2[] { start };
            }

            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                coords.Add(new IntVector2(start.x, y));
            }
        }
        else if (start.y == end.y)
        {
            if (start.x > end.x)
            {
                IntVector2 temp = start;
                start = end;
                end = temp;
            }

            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                coords.Add(new IntVector2(x, start.y));
            }
        }
        else
        {
            if (start.x > end.x)
            {
                IntVector2 temp = start;
                start = end;
                end = temp;
            }

            if (end.y <= start.y + (end.x - start.x) && end.y >= start.y - (end.x - start.x))
            {
                float gradient = (end.y - start.y) / (float)(end.x - start.x);

                for (int x = start.x + 1; x < end.x; x++)
                {
                    float lineY = (x - start.x) * gradient + start.y + 0.5f;

                    int y;
                    if (start.y < end.y)
                    {
                        if (Mathf.Abs(lineY - Mathf.Round(lineY)) < 0.0001f)
                        {
                            if ((x - start.x) / (float)(end.x - start.x) < 0.5f || (startWasToLeft && (x - start.x) / (float)(end.x - start.x) == 0.5f))
                            {
                                y = Mathf.RoundToInt(lineY) - 1;
                            }
                            else
                            {
                                y = Mathf.RoundToInt(lineY);
                            }
                        }
                        else
                        {
                            y = Mathf.FloorToInt(lineY);
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(lineY - Mathf.Round(lineY)) < 0.0001f)
                        {
                            if ((x - start.x) / (float)(end.x - start.x) < 0.5f || (startWasToLeft && (x - start.x) / (float)(end.x - start.x) == 0.5f))
                            {
                                y = Mathf.RoundToInt(lineY);
                            }
                            else
                            {
                                y = Mathf.RoundToInt(lineY) - 1;
                            }
                        }
                        else
                        {
                            y = Mathf.FloorToInt(lineY);
                        }
                    }

                    coords.Add(new IntVector2(x, y));
                }
            }
            else
            {
                float gradient = (end.x - start.x) / (float)(end.y - start.y);
                int direction = end.y > start.y ? 1 : -1;

                for (int y = start.y + direction; direction * y < direction * end.y; y += direction)
                {
                    float lineX = (y - start.y) * gradient + start.x + 0.5f;

                    int x;
                    if (Mathf.Abs(lineX - Mathf.Round(lineX)) < 0.0001f)
                    {
                        if ((y - start.y) / (float)(end.y - start.y) < 0.5f || (startWasToLeft && (y - start.y) / (float)(end.y - start.y) == 0.5f))
                        {
                            x = Mathf.RoundToInt(lineX) - 1;
                        }
                        else
                        {
                            x = Mathf.RoundToInt(lineX);
                        }
                    }
                    else
                    {
                        x = Mathf.FloorToInt(lineX);
                    }

                    coords.Add(new IntVector2(x, y));
                }
            }
        }

        if (startWasToLeft)
        {
            coords.Add(new IntVector2(end.x, end.y));
        }
        else
        {
            coords.Add(new IntVector2(start.x, start.y));
        }

        return coords.ToArray();
    }

    public static Texture2D Line(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour)
    {
        Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

        foreach (IntVector2 pixel in LineCoords(start, end))
        {
            tex.SetPixel(pixel.x, pixel.y, colour);
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D Rectangle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        IntVector2 bottomLeft = new IntVector2(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
        IntVector2 topRight = new IntVector2(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));

        Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

        if (filled)
        {
            for (int x = bottomLeft.x; x <= topRight.x; x++)
            {
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    if (tex.ContainsPixel(x, y))
                    {
                        tex.SetPixel(x, y, colour);
                    }
                }
            }
        }
        else
        {
            for (int x = bottomLeft.x; x <= topRight.x; x++)
            {
                if (tex.ContainsPixel(x, bottomLeft.y))
                {
                    tex.SetPixel(x, bottomLeft.y, colour);
                }
                if (tex.ContainsPixel(x, topRight.y))
                {
                    tex.SetPixel(x, topRight.y, colour);
                }
            }

            for (int y = bottomLeft.y + 1; y < topRight.y; y++)
            {
                if (tex.ContainsPixel(bottomLeft.x, y))
                {
                    tex.SetPixel(bottomLeft.x, y, colour);
                }
                if (tex.ContainsPixel(topRight.x, y))
                {
                    tex.SetPixel(topRight.x, y, colour);
                }
            }
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D Square(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
    {
        return Rectangle(texWidth, texHeight, start, SnapEndCoordToSquare(texWidth, texHeight, start, end, stayWithinImageBounds), colour, filled);
    }

    public static IntVector2 SnapEndCoordToSquare(IntVector2 start, IntVector2 end)
    {
        int sideLength = Mathf.Max(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));

        return start + new IntVector2(sideLength * (int)Mathf.Sign(end.x - start.x), sideLength * (int)Mathf.Sign(end.y - start.y));
    }
    public static IntVector2 SnapEndCoordToSquare(int texWidth, int texHeight, IntVector2 start, IntVector2 end, bool stayWithinImageBounds)
    {
        int width = Mathf.Abs(end.x - start.x);
        int height = Mathf.Abs(end.y - start.y);

        int sideLength = Mathf.Max(width, height);

        do
        {
            end = start + new IntVector2(sideLength * (int)Mathf.Sign(end.x - start.x), sideLength * (int)Mathf.Sign(end.y - start.y));
            sideLength--;
        }
        while (stayWithinImageBounds && !new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1)).Contains(end) && sideLength >= 0);

        return end;
    }

    public static Texture2D Ellipse(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

        IntVector2 bottomLeft = new IntVector2(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
        IntVector2 topRight = new IntVector2(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));

        float xRadius = (topRight.x - bottomLeft.x) / 2f + 0.5f;
        float yRadius = (topRight.y - bottomLeft.y) / 2f + 0.5f;
        Vector2 centre = (bottomLeft.ToVector2() + topRight.ToVector2()) / 2f + new Vector2(0.5f, 0.5f);

        Vector2 focus1;
        Vector2 focus2;
        if (xRadius >= yRadius)
        {
            float focusDistance = Mathf.Sqrt(xRadius * xRadius - yRadius * yRadius);

            focus1 = centre - new Vector2(focusDistance, 0f);
            focus2 = centre + new Vector2(focusDistance, 0f);
        }
        else
        {
            float focusDistance = Mathf.Sqrt(yRadius * yRadius - xRadius * xRadius);

            focus1 = centre - new Vector2(0f, focusDistance);
            focus2 = centre + new Vector2(0f, focusDistance);
        }

        for (int x = bottomLeft.x; x <= topRight.x; x++)
        {
            for (int y = bottomLeft.y; y <= topRight.y; y++)
            {
                float sumOfDistances = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), focus1) + Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), focus2);

                if (sumOfDistances <= 2 * Mathf.Max(xRadius, yRadius) && tex.ContainsPixel(x, y))
                {
                    tex.SetPixel(x, y, colour);
                }
            }
        }

        if (topRight.x - bottomLeft.x == 2 && topRight.y - bottomLeft.y == 2)
        {
            if (tex.ContainsPixel(bottomLeft.x, bottomLeft.y))
            {
                tex.SetPixel(bottomLeft.x, bottomLeft.y, new Color(0f, 0f, 0f, 0f));
            }
            if (tex.ContainsPixel(bottomLeft.x, topRight.y))
            {
                tex.SetPixel(bottomLeft.x, topRight.y, new Color(0f, 0f, 0f, 0f));
            }
            if (tex.ContainsPixel(topRight.x, bottomLeft.y))
            {
                tex.SetPixel(topRight.x, bottomLeft.y, new Color(0f, 0f, 0f, 0f));
            }
            if (tex.ContainsPixel(topRight.x, topRight.y))
            {
                tex.SetPixel(topRight.x, topRight.y, new Color(0f, 0f, 0f, 0f));
            }
        }

        if (filled)
        {
            tex.Apply();
            return tex;
        }

        List<IntVector2> interiorPoints = new List<IntVector2>();

        for (int x = bottomLeft.x; x <= topRight.x; x++)
        {
            for (int y = bottomLeft.y; y <= topRight.y; y++)
            {
                bool interior = true;
                foreach (IntVector2 offset in new IntVector2[] { new IntVector2(1, 0), new IntVector2(0, 1), new IntVector2(-1, 0), new IntVector2(0, -1) })
                {
                    IntVector2 offsetCoord = new IntVector2(x, y) + offset;
                    if (!tex.ContainsPixel(offsetCoord) || tex.GetPixel(offsetCoord.x, offsetCoord.y) == new Color(0f, 0f, 0f, 0f))
                    {
                        interior = false;
                    }
                }

                if (interior)
                {
                    interiorPoints.Add(new IntVector2(x, y));
                }
            }
        }

        foreach (IntVector2 interiorPoint in interiorPoints)
        {
            tex.SetPixel(interiorPoint.x, interiorPoint.y, new Color(0f, 0f, 0f, 0f));
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D Circle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
    {
        return Ellipse(texWidth, texHeight, start, SnapEndCoordToSquare(texWidth, texHeight, start, end, stayWithinImageBounds), colour, filled);
    }

    public static Texture2D RightTriangle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool rightAngleOnBottom, bool filled)
    {
        IntVector2 corner;
        if (rightAngleOnBottom)
        {
            corner = new IntVector2(start.y <= end.y ? start.x : end.x, Mathf.Max(start.y, end.y));
        }
        else
        {
            corner = new IntVector2(start.y <= end.y ? end.x : start.x, Mathf.Min(start.y, end.y));
        }

        Texture2D tex = Line(texWidth, texHeight, start, corner, colour);
        tex = Tex2DSprite.Overlay(Line(texWidth, texHeight, end, corner, colour), tex);

        if (start.x > end.x)
        {
            IntVector2 temp = start;
            start = end;
            end = temp;
        }

        IntVector2 fillStartPoint;
        if (start.x != end.x && start.y != end.y)
        {
            if (rightAngleOnBottom)
            {
                if (start.y >= end.y)
                {
                    start = new IntVector2(start.x, start.y - 1);
                    end = new IntVector2(end.x - 1, end.y);
                    fillStartPoint = corner + new IntVector2(-1, -1);
                }
                else
                {
                    start = new IntVector2(start.x + 1, start.y);
                    end = new IntVector2(end.x, end.y - 1);
                    fillStartPoint = corner + new IntVector2(1, -1);
                }
            }
            else
            {
                if (start.y >= end.y)
                {
                    start = new IntVector2(start.x + 1, start.y);
                    end = new IntVector2(end.x, end.y + 1);
                    fillStartPoint = corner + new IntVector2(1, 1);
                }
                else
                {
                    start = new IntVector2(start.x, start.y + 1);
                    end = new IntVector2(end.x - 1, end.y);
                    fillStartPoint = corner + new IntVector2(-1, 1);
                }
            }

            tex = Tex2DSprite.Overlay(Line(texWidth, texHeight, start, end, colour), tex);

            if (filled)
            {
                tex = Tex2DSprite.Fill(tex, fillStartPoint, colour);
            }
        }

        return tex;
    }

    public static Texture2D Gradient(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode)
    {
        switch (gradientMode)
        {
            case GradientMode.Linear: return GradientLinear(texWidth, texHeight, start, end, startColour, endColour);
            case GradientMode.Radial: return GradientRadial(texWidth, texHeight, start, end, startColour, endColour);
            default: throw new System.Exception("Unknown / unimplemented gradient mode: " + gradientMode);
        }
    }

    public static Texture2D GradientLinear(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
    {
        Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

        if (start == end)
        {
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    tex.SetPixel(x, y, startColour);
                }
            }

            tex.Apply();
            return tex;
        }

        Vector2 dir = (end - start).ToVector2();
        float distance = dir.magnitude;

        for (int x = 0; x < texWidth; x++)
        {
            for (int y = 0; y < texHeight; y++)
            {
                if (x == start.x && y == start.y)
                {
                    tex.SetPixel(x, y, startColour);
                }
                else
                {
                    Vector2 point = new Vector2(x, y);
                    float perpendicularDistance = Vector2.Distance(start.ToVector2(), point) * Vector2.Dot(point - start.ToVector2(), dir) /
                        (point - start.ToVector2()).magnitude / dir.magnitude;

                    float scalar = Mathf.Clamp01(perpendicularDistance / distance);

                    tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
                }
            }
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D GradientRadial(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
    {
        Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

        if (start == end)
        {
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    tex.SetPixel(x, y, endColour);
                }
            }

            tex.Apply();
            return tex;
        }

        float distance = (end - start).magnitude;

        for (int x = 0; x < texWidth; x++)
        {
            for (int y = 0; y < texHeight; y++)
            {
                float distanceToPoint = Vector2.Distance(start.ToVector2(), new Vector2(x, y));

                float scalar = Mathf.Clamp01(distanceToPoint / distance);

                tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
            }
        }

        tex.Apply();
        return tex;
    }
}
