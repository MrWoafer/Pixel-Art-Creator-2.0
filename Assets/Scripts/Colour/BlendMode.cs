using System;

using PAC.Json;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// Defines how two colours (the <i>top colour</i> and the <i>bottom colour</i>) are blended together to form a new colour, ignoring their alpha.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Blend_modes"/>
    public abstract record BlendMode
    {
        #region Contract
        /// <summary>
        /// The display name of the blend mode.
        /// </summary>
        public abstract string name { get; }

        /// <summary>
        /// Applies the blend mode to the two colours.
        /// </summary>
        /// <returns>
        /// The blended colour in straight alpha form.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="topColour"/> and <paramref name="bottomColour"/> should be in straight alpha form.
        /// </para>
        /// <para>
        /// No clamping is performed on <paramref name="topColour"/> or <paramref name="bottomColour"/>, but the output may be clamped depending on the <see cref="BlendMode"/>.
        /// </para>
        /// </remarks>
        public abstract RGB Blend(RGB topColour, RGB bottomColour);
        #endregion

        #region Default Method Implementations
        /// <summary>
        /// Applies the blend mode with source-over alpha compositing.
        /// </summary>
        /// <returns>
        /// The blended colour in straight alpha form.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="topColour"/> and <paramref name="bottomColour"/> should be in straight alpha form.
        /// </para>
        /// <para>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#generalformula"/>.
        /// </para>
        /// <para>
        /// No clamping is performed on <paramref name="topColour"/> or <paramref name="bottomColour"/>, but the output may be clamped depending on the <see cref="BlendMode"/>.
        /// </para>
        /// </remarks>
        public Color Blend(Color topColour, Color bottomColour)
            => AlphaCompositing.Straight.SourceOver(
                RGB.LerpUnclamped(
                    (RGB)topColour,
                    Blend((RGB)topColour, (RGB)bottomColour),
                    bottomColour.a
                ).WithAlpha(topColour.a),
                bottomColour
                );

        public override string ToString() => name;
        #endregion

        #region Predefined Instances
        /// <summary>
        /// The <i>Normal</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#blendingnormal"/>.
        /// </remarks>
        public static readonly BlendMode Normal;
        /// <summary>
        /// The <i>Overlay</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#blendingoverlay"/>.
        /// </remarks>
        public static readonly BlendMode Overlay;
        /// <summary>
        /// The <i>Multiply</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#blendingmultiply"/>.
        /// </remarks>
        public static readonly BlendMode Multiply;
        /// <summary>
        /// The <i>Screen</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#blendingscreen"/>.
        /// </remarks>
        public static readonly BlendMode Screen;
        /// <summary>
        /// The <i>Add</i> blend mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Also known as <i>Linear Dodge</i>.
        /// </para>
        /// <para>
        /// This follows the specification from <see href="https://docs.krita.org/en/reference_manual/blending_modes/arithmetic.html#addition"/>.
        /// </para>
        /// </remarks>
        public static readonly BlendMode Add;
        /// <summary>
        /// The <i>Subtract</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://docs.krita.org/en/reference_manual/blending_modes/arithmetic.html#subtract"/>.
        /// </remarks>
        public static readonly BlendMode Subtract;

        /// <summary>
        /// All <see cref="BlendMode"/>s implemented in <see cref="BlendMode"/>.
        /// </summary>
        public static readonly BlendMode[] blendModes;

        static BlendMode()
        {
            Normal = new NormalBlendMode();
            Overlay = new OverlayBlendMode();
            Multiply = new MultiplyBlendMode();
            Screen = new ScreenBlendMode();
            Add = new AddBlendMode();
            Subtract = new SubtractBlendMode();
            
            blendModes = new BlendMode[] { Normal, Overlay, Multiply, Screen, Add, Subtract };
        }
        #endregion

        #region Implementations
        /// <summary>
        /// The type of <see cref="BlendMode.Normal"/>.
        /// </summary>
        private sealed record NormalBlendMode : BlendMode
        {
            public override string name => "Normal";

            internal NormalBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour) => topColour;
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Overlay"/>.
        /// </summary>
        private sealed record OverlayBlendMode : BlendMode
        {
            public override string name => "Overlay";

            internal OverlayBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour)
            {
                static float BlendComponent(float top, float bottom)
                    => bottom <= 0.5f
                    ? 2f * bottom * top
                    : 1f - 2f * (1f - bottom) * (1f - top);

                return new RGB(
                    BlendComponent(topColour.r, bottomColour.r),
                    BlendComponent(topColour.g, bottomColour.g),
                    BlendComponent(topColour.b, bottomColour.b)
                    );
            }
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Multiply"/>.
        /// </summary>
        private sealed record MultiplyBlendMode : BlendMode
        {
            public override string name => "Multiply";

            internal MultiplyBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour) => topColour * bottomColour;
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Screen"/>.
        /// </summary>
        private sealed record ScreenBlendMode : BlendMode
        {
            public override string name => "Screen";

            internal ScreenBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour) => topColour + bottomColour - topColour * bottomColour;
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Add"/>.
        /// </summary>
        private sealed record AddBlendMode : BlendMode
        {
            public override string name => "Add";

            internal AddBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour) => (topColour + bottomColour).Clamp01();
        }

        /// <summary>
        /// The type of <see cref="BlendMode.Subtract"/>.
        /// </summary>
        private sealed record SubtractBlendMode : BlendMode
        {
            public override string name => "Subtract";

            internal SubtractBlendMode() { }

            public override RGB Blend(RGB topColour, RGB bottomColour) => (bottomColour - topColour).Clamp01();
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
