using PAC.Json;
using System;
using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A class for blend modes.
    /// </summary>
    public class BlendMode
    {
        /// <summary>The display name of this blend mode.</summary>
        private string name;
        /// <summary>The function defining how the blend mode works. (topColour, bottomColour) -> outputColour</summary>
        private Func<Color, Color, Color> blendFunction;

        /// <param name="name">The display name of this blend mode.</param>
        /// <param name="blendFunction">The function defining how the blend mode works. (topColour, bottomColour) -> outputColour</param>
        private BlendMode(string name, Func<Color, Color, Color> blendFunction)
        {
            this.name = name;
            this.blendFunction = blendFunction;
        }

        /// <summary>Replace blend mode.</summary>
        public static readonly BlendMode Replace = new BlendMode("Replace", ReplaceBlend);
        /// <summary>Normal blend mode.</summary>
        public static readonly BlendMode Normal = new BlendMode("Normal", NormalBlend);
        /// <summary>Overlay blend mode.</summary>
        public static readonly BlendMode Overlay = new BlendMode("Overlay", OverlayBlend);
        /// <summary>Multiply blend mode.</summary>
        public static readonly BlendMode Multiply = new BlendMode("Multiply", MultiplyBlend);
        /// <summary>Screen blend mode.</summary>
        public static readonly BlendMode Screen = new BlendMode("Screen", ScreenBlend);
        /// <summary>Add blend mode.</summary>
        public static readonly BlendMode Add = new BlendMode("Add", AddBlend);
        /// <summary>Subtract blend mode.</summary>
        public static readonly BlendMode Subtract = new BlendMode("Subtract", SubtractBlend);

        /// <summary>All implemented blend modes.</summary>
        public static readonly BlendMode[] blendModes = new BlendMode[] { Replace, Normal, Overlay, Multiply, Screen, Add, Subtract };

        public static bool operator ==(BlendMode a, BlendMode b)
        {
            return a.name == b.name;
        }
        public static bool operator !=(BlendMode a, BlendMode b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                BlendMode blendNode = (BlendMode)obj;
                return this == blendNode;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Blend the two colours using the blend mode's blend function.
        /// </summary>
        public Color Blend(Color topColour, Color bottomColour)
        {
            return blendFunction.Invoke(topColour, bottomColour);
        }

        /// <summary>
        /// The blend function for the Replace blend mode.
        /// </summary>
        private static Color ReplaceBlend(Color topColour, Color bottomColour)
        {
            return topColour;
        }

        /// <summary>
        /// The blend function for the Normal blend mode.
        /// </summary>
        private static Color NormalBlend(Color topColour, Color bottomColour)
        {
            float a = (1f - topColour.a) * bottomColour.a + topColour.a;

            if (a == 0)
            {
                return bottomColour;
            }
            else
            {
                float r = ((1f - topColour.a) * bottomColour.a * bottomColour.r + topColour.a * topColour.r) / a;
                float g = ((1f - topColour.a) * bottomColour.a * bottomColour.g + topColour.a * topColour.g) / a;
                float b = ((1f - topColour.a) * bottomColour.a * bottomColour.b + topColour.a * topColour.b) / a;

                return new Color(r, g, b, a);
            }
        }

        /// <summary>
        /// The blend function for the Overlay blend mode.
        /// </summary>
        private static Color OverlayBlend(Color topColour, Color bottomColour)
        {
            if (topColour.a == 0f)
            {
                return bottomColour;
            }
            if (bottomColour.a == 0f)
            {
                return topColour;
            }

            float r, g, b, a;

            if (bottomColour.r < 0.5f)
            {
                r = 2f * bottomColour.r * topColour.r;
            }
            else
            {
                r = 1f - 2f * (1f - bottomColour.r) * (1f - topColour.r);
            }

            if (bottomColour.g < 0.5f)
            {
                g = 2f * bottomColour.g * topColour.g;
            }
            else
            {
                g = 1f - 2f * (1f - bottomColour.g) * (1f - topColour.g);
            }

            if (bottomColour.b < 0.5f)
            {
                b = 2f * bottomColour.b * topColour.b;
            }
            else
            {
                b = 1f - 2f * (1f - bottomColour.b) * (1f - topColour.b);
            }

            if (bottomColour.a < 0.5f)
            {
                a = 2f * bottomColour.a * topColour.a;
            }
            else
            {
                a = 1f - 2f * (1f - bottomColour.a) * (1f - topColour.a);
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Multiplies the colours component-wise.
        /// </summary>
        public static Color MultiplyColours(Color colour1, Color colour2)
        {
            return colour1 * colour2;
        }
        /// <summary>
        /// The blend function for the Multiply blend mode.
        /// </summary>
        private static Color MultiplyBlend(Color colour1, Color colour2)
        {
            if (colour1.a == 0f)
            {
                return colour2;
            }
            if (colour2.a == 0f)
            {
                return colour1;
            }
            return colour1 * colour2;
        }

        /// <summary>
        /// The blend function for the Screen blend mode.
        /// </summary>
        private static Color ScreenBlend(Color colour1, Color colour2)
        {
            if (colour1.a == 0f)
            {
                return colour2;
            }
            if (colour2.a == 0f)
            {
                return colour1;
            }
            return new Color(1f, 1f, 1f, 1f) - (new Color(1f, 1f, 1f, 1f) - colour1) * (new Color(1f, 1f, 1f, 1f) - colour2);
        }

        /// <summary>
        /// The blend function for the Add blend mode.
        /// </summary>
        private static Color AddBlend(Color colour1, Color colour2)
        {
            return colour1 + colour2;
        }

        /// <summary>
        /// The blend function for the Subtract blend mode.
        /// </summary>
        private static Color SubtractBlend(Color subtractFrom, Color toSubtract)
        {
            return subtractFrom - toSubtract;
        }

        /// <summary>
        /// Returns the blend mode with that name (case-insensitive).
        /// </summary>
        public static BlendMode StringToBlendMode(string blendModeName)
        {
            foreach (BlendMode blendMode in blendModes)
            {
                if (blendMode.name.ToLower() == blendModeName.ToLower())
                {
                    return blendMode;
                }
            }
            throw new ArgumentException("Unknown / unimplemented blend mode: " + blendModeName);
        }

        public class JsonConverter : JsonConversion.JsonConverter<BlendMode, JsonData.String>
        {
            public override JsonData.String ToJson(BlendMode blendMode)
            {
                return blendMode.name;
            }

            public override BlendMode FromJson(JsonData.String jsonData)
            {
                return StringToBlendMode(jsonData);
            }
        }
    }
}
