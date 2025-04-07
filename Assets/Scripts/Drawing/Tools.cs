using System.Linq;
using PAC.ImageEditing;
using PAC.Patterns;
using PAC.Geometry.Shapes;
using PAC.Geometry.Shapes.Interfaces;

using UnityEngine;
using PAC.Patterns.Extensions;
using PAC.Geometry;
using PAC.Image;
using PAC.Image.Layers;

namespace PAC.Drawing
{
    /// <summary>
    /// Defines how different tools act.
    /// I may rework this to be more like the BlendMode class where each Tool instance defines how it acts. Then to use a tool you would just do tool.Use(pixel) or similar.
    /// </summary>
    public static class Tools
    {
        public static void UsePencil(File file, int layer, int frame, IntVector2 pixel, Color colour) => UsePencil(file, layer, frame, pixel.x, pixel.y, colour);
        public static void UsePencil(File file, int layer, int frame, int x, int y, Color colour)
        {
            file.layers[layer].SetPixel(x, y, frame, colour, AnimFrameRefMode.NewKeyFrame);
        }

        /// <summary>
        /// Smooth the meeting point of the two lines (given as coords) so it is pixel-perfect - i.e. no hard 90-degree corner.
        /// </summary>
        public static bool PencilLineSmoothing(Line line, Line previousLine, bool previousLineWasSmoothed)
        {
            if (previousLine is null)
            {
                return false;
            }

            IntVector2[] offsets = new IntVector2[] { IntVector2.left, IntVector2.right, IntVector2.down, IntVector2.up };
            foreach (IntVector2 offset1 in offsets)
            {
                foreach (IntVector2 offset2 in offsets)
                {
                    if (!IntVector2.ArePerpendicular(offset1, offset2))
                    {
                        continue;
                    }

                    if (previousLineWasSmoothed && line.start + offset1 == previousLine.start)
                    {
                        continue;
                    }

                    bool pixelShouldBeSmoothed = previousLine.Contains(line.start + offset1) && line.Contains(line.start + offset2) && !line.Contains(line.start - offset1);
                    if (pixelShouldBeSmoothed)
                    {
                        return true;
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
            file.layers[layer].SetPixels(file.rect.FilterPointsInside(brushBorderMaskPixels.Select(p => p + pixel)), frame, colour, AnimFrameRefMode.NewKeyFrame);
        }

        public static void UseRubber(File file, int layer, int frame, int x, int y) => UseRubber(file, layer, frame, new IntVector2(x, y));
        public static void UseRubber(File file, int layer, int frame, IntVector2 pixel) => UseRubber(file, layer, frame, pixel, new IntVector2[] { IntVector2.zero });
        public static void UseRubber(File file, int layer, int frame, int x, int y, IntVector2[] brushBorderMaskPixels) => UseRubber(file, layer, frame, new IntVector2(x, y), brushBorderMaskPixels);
        public static void UseRubber(File file, int layer, int frame, IntVector2 pixel, IntVector2[] brushBorderMaskPixels)
        {
            UseBrush(file, layer, frame, pixel, brushBorderMaskPixels, Config.Colours.transparent);
        }

        public static Color UseEyeDropper(File file, int layer, int frame, IntVector2 pixel) => UseEyeDropper(file, layer, frame, pixel.x, pixel.y);
        public static Color UseEyeDropper(File file, int layer, int frame, int x, int y)
        {
            return file.layers[layer].GetPixel(x, y, frame);
        }

        public static void UseFill(File file, int layer, int frame, int x, int y, Color colour, bool includeDiagonallyAdjacent, int maxNumOfIterations = 1_000_000)
        {
            UseFill(file, layer, frame, new IntVector2(x, y), colour, includeDiagonallyAdjacent, maxNumOfIterations);
        }
        public static void UseFill(File file, int layer, int frame, IntVector2 pixel, Color colour, bool includeDiagonallyAdjacent, int maxNumOfIterations = 1_000_000)
        {
            file.layers[layer].SetPixels(
                FloodFill.GetPixelsToFill(file.layers[layer][frame].texture, pixel, includeDiagonallyAdjacent, maxNumOfIterations),
                frame,
                colour,
                AnimFrameRefMode.NewKeyFrame
                );
        }

        public static void UseShape(File file, int layer, int frame, IShape shape, Color colour)
        {
            if (file.layers[layer].layerType != LayerType.Normal)
            {
                throw new System.Exception("Layer is not a normal layer. Layer type: " + file.layers[layer].layerType);
            }

            file.layers[layer].SetPixels(shape.Where(p => file.rect.Contains(p)), frame, colour, AnimFrameRefMode.NewKeyFrame);
        }

        public static void UsePattern(File file, int layer, int frame, IPattern2D<Color> pattern)
        {
            if (file.layers[layer].layerType != LayerType.Normal)
            {
                throw new System.Exception($"Layer is not a normal layer. Layer type: {file.layers[layer].layerType}");
            }

            (file.layers[layer] as NormalLayer).SetTexture(frame, pattern.ToTexture(file.rect), AnimFrameRefMode.NewKeyFrame);
        }
        public static void UsePattern(File file, int layer, int frame, IPattern2D<Color32> pattern)
        {
            if (file.layers[layer].layerType != LayerType.Normal)
            {
                throw new System.Exception($"Layer is not a normal layer. Layer type: {file.layers[layer].layerType}");
            }

            (file.layers[layer] as NormalLayer).SetTexture(frame, pattern.ToTexture(file.rect), AnimFrameRefMode.NewKeyFrame);
        }
    }
}
