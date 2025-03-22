using System;
using System.Linq;
using System.Reflection;

using PAC.Json;

using UnityEngine;

namespace PAC.Colour.Compositing
{
    /// <summary>
    /// Defines how a foreground colour (the <i>top</i> colour) is blended onto a background colour (the <i>bottom</i> colour) to form a new colour, ignoring their alpha.
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
        /// <paramref name="top"/> and <paramref name="bottom"/> should be in straight alpha form.
        /// </para>
        /// <para>
        /// No clamping is performed on <paramref name="top"/> or <paramref name="bottom"/>, but the output may be clamped depending on the <see cref="BlendMode"/>.
        /// </para>
        /// <para>
        /// Does not do any colour space conversion.
        /// </para>
        /// </remarks>
        public abstract RGB Blend(RGB top, RGB bottom);
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
        /// <paramref name="top"/> and <paramref name="bottom"/> should be in straight alpha form.
        /// </para>
        /// <para>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#generalformula"/>.
        /// </para>
        /// <para>
        /// No clamping is performed on <paramref name="top"/> or <paramref name="bottom"/>, but the output may be clamped depending on the <see cref="BlendMode"/>.
        /// </para>
        /// <para>
        /// Does not do any colour space conversion.
        /// </para>
        /// </remarks>
        public Color Blend(Color top, Color bottom)
            => AlphaCompositing.Straight.SourceOver(
                RGB.LerpUnclamped(
                    (RGB)top,
                    Blend((RGB)top, (RGB)bottom),
                    bottom.a
                ).WithAlpha(top.a),
                bottom
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
        /// The <i>Overlay</i> blend mode.
        /// </summary>
        /// <remarks>
        /// This follows the specification from <see href="https://www.w3.org/TR/compositing-1/#blendingoverlay"/>.
        /// </remarks>
        public static readonly BlendMode Overlay;
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
        /// <remarks>
        /// There is no guarantee as to what order this is in.
        /// </remarks>
        public static readonly BlendMode[] BlendModes;

        static BlendMode()
        {
            Normal = new NormalBlendMode();
            Overlay = new OverlayBlendMode();
            Multiply = new MultiplyBlendMode();
            Screen = new ScreenBlendMode();
            Add = new AddBlendMode();
            Subtract = new SubtractBlendMode();

            BlendModes =
                typeof(BlendMode)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => typeof(BlendMode).IsAssignableFrom(f.FieldType))
                .Select(f => (BlendMode)f.GetValue(null))
                .ToArray();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// The type of <see cref="Normal"/>.
        /// </summary>
        private sealed record NormalBlendMode : BlendMode
        {
            public override string name => "Normal";

            internal NormalBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom) => top;
        }

        /// <summary>
        /// The type of <see cref="Multiply"/>.
        /// </summary>
        private sealed record MultiplyBlendMode : BlendMode
        {
            public override string name => "Multiply";

            internal MultiplyBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom) => top * bottom;
        }

        /// <summary>
        /// The type of <see cref="Screen"/>.
        /// </summary>
        private sealed record ScreenBlendMode : BlendMode
        {
            public override string name => "Screen";

            internal ScreenBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom) => top + bottom - top * bottom;
        }

        /// <summary>
        /// The type of <see cref="Overlay"/>.
        /// </summary>
        private sealed record OverlayBlendMode : BlendMode
        {
            public override string name => "Overlay";

            internal OverlayBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom)
            {
                static float BlendComponent(float top, float bottom)
                    => bottom <= 0.5f
                    ? 2f * bottom * top
                    : 1f - 2f * (1f - bottom) * (1f - top);

                return new RGB(
                    BlendComponent(top.r, bottom.r),
                    BlendComponent(top.g, bottom.g),
                    BlendComponent(top.b, bottom.b)
                    );
            }
        }

        /// <summary>
        /// The type of <see cref="Add"/>.
        /// </summary>
        private sealed record AddBlendMode : BlendMode
        {
            public override string name => "Add";

            internal AddBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom) => (top + bottom).Clamp01();
        }

        /// <summary>
        /// The type of <see cref="Subtract"/>.
        /// </summary>
        private sealed record SubtractBlendMode : BlendMode
        {
            public override string name => "Subtract";

            internal SubtractBlendMode() { }

            public override RGB Blend(RGB top, RGB bottom) => (bottom - top).Clamp01();
        }
        #endregion

        #region JSON Conversion
        /// <summary>
        /// Returns the blend mode with that name (case-insensitive).
        /// </summary>
        public static BlendMode Parse(string blendModeName)
        {
            foreach (BlendMode blendMode in BlendModes)
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
