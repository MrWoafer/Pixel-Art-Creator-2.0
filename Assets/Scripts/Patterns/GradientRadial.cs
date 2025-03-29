using PAC.Geometry;

using UnityEngine;

namespace PAC.Patterns
{
    public static partial class Gradient
    {
        /// <summary>
        /// <para>
        /// A gradient that linearly interpolates from the colour of <see cref="centre"/> to the colour of <see cref="onCircumference"/> based on the distance of a point from the
        /// <see cref="centre"/>.
        /// </para>
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Color_gradient#Radial_gradients"/>
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>In More Detail:</b>
        /// </para>
        /// <para>
        /// Let <c>r = distance(<see cref="centre"/>, <see cref="onCircumference"/>)</c>, and for a given point let <c>d = distance(<see cref="centre"/>, point)</c>.
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// If <c>d = 0</c>, the point's colour will be the colour of <see cref="centre"/>.
        /// </item>
        /// <item>
        /// If <c>d &gt;= r</c>, the point's colour will be the colour of <see cref="onCircumference"/>.
        /// </item>
        /// <item>
        /// If <c>0 &lt; d &lt; r</c>, the point's colour will be linearly interpolated from the colour of <see cref="centre"/> to the colour of <see cref="onCircumference"/>, using the proportion
        /// <c>d / r</c>.
        /// </item>
        /// </list>
        /// <para>
        /// <b>Design Note:</b>
        /// </para>
        /// <para>
        /// If <see cref="centre"/> and <see cref="onCircumference"/> have the same coord, the <see cref="centre"/> will still have its assigned colour and all other points will have the colour of
        /// <see cref="onCircumference"/>. This definition was chosen because I think it feels natural for a user of the gradient tool to always see the <see cref="centre"/> have its assigned
        /// colour.
        /// </para>
        /// </remarks>
        public record Radial : IPattern2D<Color>
        {
            /// <summary>
            /// The position and colour of the centre of the circle.
            /// </summary>
            /// <seealso cref="onCircumference"/>
            public (IntVector2 coord, Color colour) centre { get; init; }
            /// <summary>
            /// The position and colour of some point on the circumference of the circle.
            /// </summary>
            /// <seealso cref="centre"/>
            public (IntVector2 coord, Color colour) onCircumference { get; init; }

            /// <summary>
            /// See <see cref="Radial"/> for details.
            /// </summary>
            /// <param name="centre">The position and colour of the centre of the circle.</param>
            /// <param name="onCircumference">The position and colour of some point on the circumference of the circle.</param>
            public Radial((IntVector2 coord, Color colour) centre, (IntVector2 coord, Color colour) onCircumference)
            {
                this.centre = centre;
                this.onCircumference = onCircumference;
            }

            public Color this[IntVector2 point]
            {
                get
                {
                    if (point == centre.coord)
                    {
                        return centre.colour;
                    }

                    if (centre.coord == onCircumference.coord)
                    {
                        return onCircumference.colour;
                    }

                    float radius = IntVector2.Distance(centre.coord, onCircumference.coord);
                    float distanceFromCentre = IntVector2.Distance(centre.coord, point);

                    return Color.Lerp(centre.colour, onCircumference.colour, distanceFromCentre / radius);
                }
            }
        }
    }
}