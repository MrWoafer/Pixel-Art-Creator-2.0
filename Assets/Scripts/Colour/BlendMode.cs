using PAC.Json;
using System;
using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A class for blend modes.
    /// </summary>
    public abstract record BlendMode
    {
        #region Contract
        /// <summary>The display name of this blend mode.</summary>
        public abstract string name { get; }
        /// <summary>The function defining how the blend mode works.</summary>
        public abstract Color Blend(Color topColour, Color bottomColour);
        #endregion

        #region Default Method Implementations
        public override string ToString() => name;
        #endregion

        #region Predefined Instances
        /// <summary>Replace blend mode.</summary>
        public static readonly BlendMode Replace = new ReplaceBlendMode();
        /// <summary>Normal blend mode.</summary>
        public static readonly BlendMode Normal = new NormalBlendMode();
        /// <summary>Overlay blend mode.</summary>
        public static readonly BlendMode Overlay = new OverlayBlendMode();
        /// <summary>Multiply blend mode.</summary>
        public static readonly BlendMode Multiply = new MultiplyBlendMode();
        /// <summary>Screen blend mode.</summary>
        public static readonly BlendMode Screen = new ScreenBlendMode();
        /// <summary>Add blend mode.</summary>
        public static readonly BlendMode Add = new AddBlendMode();
        /// <summary>Subtract blend mode.</summary>
        public static readonly BlendMode Subtract = new SubtractBlendMode();

        /// <summary>All implemented blend modes.</summary>
        public static readonly BlendMode[] blendModes = new BlendMode[] { Replace, Normal, Overlay, Multiply, Screen, Add, Subtract };
        #endregion

        #region Implementations
        /// <summary>
        /// The type of <see cref="BlendMode.Replace"/>.
        /// </summary>
        private sealed record ReplaceBlendMode : BlendMode
        {
            public override string name => "Replace";

            internal ReplaceBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour) => topColour;
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Normal"/>.
        /// </summary>
        private sealed record NormalBlendMode : BlendMode
        {
            public override string name => "Normal";

            internal NormalBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour)
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
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Overlay"/>.
        /// </summary>
        private sealed record OverlayBlendMode : BlendMode
        {
            public override string name => "Overlay";

            internal OverlayBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour)
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
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Multiply"/>.
        /// </summary>
        private sealed record MultiplyBlendMode : BlendMode
        {
            public override string name => "Multiply";

            internal MultiplyBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour)
            {
                if (topColour.a == 0f)
                {
                    return bottomColour;
                }
                if (bottomColour.a == 0f)
                {
                    return topColour;
                }
                return topColour * bottomColour;
            }
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Screen"/>.
        /// </summary>
        private sealed record ScreenBlendMode : BlendMode
        {
            public override string name => "Screen";

            internal ScreenBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour)
            {
                if (topColour.a == 0f)
                {
                    return bottomColour;
                }
                if (bottomColour.a == 0f)
                {
                    return topColour;
                }
                return new Color(1f, 1f, 1f, 1f) - (new Color(1f, 1f, 1f, 1f) - topColour) * (new Color(1f, 1f, 1f, 1f) - bottomColour);
            }
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Add"/>.
        /// </summary>
        private sealed record AddBlendMode : BlendMode
        {
            public override string name => "Add";

            internal AddBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour) => topColour + bottomColour;
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Subtract"/>.
        /// </summary>
        private sealed record SubtractBlendMode : BlendMode
        {
            public override string name => "Subtract";

            internal SubtractBlendMode() { }

            public override Color Blend(Color topColour, Color bottomColour) => topColour - bottomColour;
        }
        #endregion

        #region JSON Conversion
        /// <summary>
        /// Returns the blend mode with that name (case-insensitive).
        /// </summary>
        public static BlendMode Parse(string blendModeName)
        {
            foreach (BlendMode blendMode in blendModes)
            {
                if (blendMode.name.ToLower() == blendModeName.ToLower())
                {
                    return blendMode;
                }
            }
            throw new FormatException($"Unknown / unimplemented blend mode: {blendModeName}");
        }

        public class JsonConverter : JsonConversion.JsonConverter<BlendMode, JsonData.String>
        {
            public override JsonData.String ToJson(BlendMode blendMode)
            {
                return blendMode.name;
            }

            public override BlendMode FromJson(JsonData.String jsonData)
            {
                return Parse(jsonData);
            }
        }
        #endregion
    }
}
