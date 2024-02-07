using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Tools
{
    public static void UsePencil(File file, int layer, int frame, IntVector2 pixel, Color colour) => UsePencil(file, layer, frame, pixel.x, pixel.y, colour);
    public static void UsePencil(File file, int layer, int frame, int x, int y, Color colour)
    {
        file.layers[layer].SetPixel(x, y, frame, colour, AnimFrameRefMode.NewKeyFrame);
    }

    public static bool PencilLineSmoothing(File file, int layer, int frame, IntVector2[] line, IntVector2[] lineSmoothingPreviousLine, Color colourLineMeetingPoint)
    {
        if (lineSmoothingPreviousLine.Length != 0)
        {
            IntVector2[] offsets = new IntVector2[] { IntVector2.left, IntVector2.right, IntVector2.down, IntVector2.up };
            foreach (IntVector2 offset1 in offsets)
            {
                foreach (IntVector2 offset2 in offsets)
                {
                    bool pixelShouldBeSmoothed = IntVector2.Dot(offset1, offset2) == 0 && lineSmoothingPreviousLine.Contains(line[0] + offset1) && line.Contains(line[0] + offset2) &&
                        !line.Contains(line[0] - offset1);
                    if (pixelShouldBeSmoothed)
                    {
                        file.layers[layer].SetPixel(line[0], frame, colourLineMeetingPoint, AnimFrameRefMode.NewKeyFrame);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static void UseBrush(File file, int layer, int frame, int x, int y, IntVector2[] brushBorderMaskPixels, Color colour)
    {
        UseBrush(file, layer, frame, new IntVector2(x, y), brushBorderMaskPixels, colour);
    }
    public static void UseBrush(File file, int layer, int frame, IntVector2 pixel, IntVector2[] brushBorderMaskPixels, Color colour)
    {
        file.layers[layer].SetPixels(file.rect.FilterPointsInside(pixel + brushBorderMaskPixels), frame, colour, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseRubber(File file, int layer, int frame, int x, int y) => UseRubber(file, layer, frame, new IntVector2(x, y));
    public static void UseRubber(File file, int layer, int frame, IntVector2 pixel) => UseRubber(file, layer, frame, pixel, new IntVector2[] { IntVector2.zero });
    public static void UseRubber(File file, int layer, int frame, int x, int y, IntVector2[] brushBorderMaskPixels) => UseRubber(file, layer, frame, new IntVector2(x, y), brushBorderMaskPixels);
    public static void UseRubber(File file, int layer, int frame, IntVector2 pixel, IntVector2[] brushBorderMaskPixels)
    {
        UseBrush(file, layer, frame, pixel, brushBorderMaskPixels, Color.clear);
    }

    public static Color UseEyeDropper(File file, int layer, int frame, IntVector2 pixel) => UseEyeDropper(file, layer, frame, pixel.x, pixel.y);
    public static Color UseEyeDropper(File file, int layer, int frame, int x, int y)
    {
        return file.layers[layer].GetPixel(x, y, frame);
    }

    public static void UseFill(File file, int layer, int frame, int x, int y, Color colour, int maxNumOfIterations = 100000)
    {
        UseFill(file, layer, frame, new IntVector2(x, y), colour, maxNumOfIterations);
    }
    public static void UseFill(File file, int layer, int frame, IntVector2 pixel, Color colour, int maxNumOfIterations = 100000)
    {
        file.layers[layer].SetPixels(Tex2DSprite.GetPixelsToFill(file.layers[layer][frame].texture, pixel, maxNumOfIterations), frame, colour, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseLine(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D line = Shapes.Line(file.width, file.height, start, end, colour);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, line, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseSquare(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D square = Shapes.Square(file.width, file.height, start, end, colour, filled, stayWithinImageBounds);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, square, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseRectangle(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D rectangle = Shapes.Rectangle(file.width, file.height, start, end, colour, filled);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, rectangle, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseCircle(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour, bool filled, bool stayWithinImageBounds)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D circle = Shapes.Circle(file.width, file.height, start, end, colour, filled, stayWithinImageBounds);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, circle, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseEllipse(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour, bool filled)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D ellipse = Shapes.Ellipse(file.width, file.height, start, end, colour, filled);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, ellipse, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseRightTriangle(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color colour, bool rightAngleOnBottom, bool filled)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }
        Texture2D triangle = Shapes.RightTriangle(file.width, file.height, start, end, colour, rightAngleOnBottom, filled);
        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, triangle, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseGradient(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, gradientMode, new IntVector2[0]);
    }
    public static void UseGradient(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode, Texture2D mask)
    {
        if (mask.width != file.width || mask.height != file.height)
        {
            throw new System.Exception("Mask dimensions " + mask.width + "x" + mask.height + " do not match file dimensions " + file.width + "x" + file.height);
        }
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }

        Texture2D gradient = Shapes.Gradient(file.width, file.height, start, end, startColour, endColour, gradientMode);
        if (mask != null)
        {
            gradient = Tex2DSprite.ApplyMask(gradient, mask);
        }

        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, gradient, AnimFrameRefMode.NewKeyFrame);
    }
    public static void UseGradient(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode, IntVector2[] mask)
    {
        if (file.layers[layer].layerType != LayerType.Normal)
        {
            throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
        }

        Texture2D gradient = Shapes.Gradient(file.width, file.height, start, end, startColour, endColour, gradientMode);
        if (mask.Length != 0)
        {
            gradient = Tex2DSprite.ApplyMask(gradient, mask);
        }

        ((NormalLayer)file.layers[layer]).OverlayTexture(frame, gradient, AnimFrameRefMode.NewKeyFrame);
    }

    public static void UseGradientLinear(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Linear);
    }
    public static void UseGradientLinear(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, Texture2D mask)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Linear, mask);
    }
    public static void UseGradientLinear(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, IntVector2[] mask)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Linear, mask);
    }

    public static void UseGradientRadial(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Radial);
    }
    public static void UseGradientRadial(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, Texture2D mask)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Radial, mask);
    }
    public static void UseGradientRadial(File file, int layer, int frame, IntVector2 start, IntVector2 end, Color startColour, Color endColour, IntVector2[] mask)
    {
        UseGradient(file, layer, frame, start, end, startColour, endColour, GradientMode.Radial, mask);
    }
}
