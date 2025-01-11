using PAC.DataStructures;

using UnityEngine;

namespace PAC.Patterns
{
    public static partial class Gradient
    {
        /// <summary>
        /// <para>
        /// A gradient that linearly interpolates from the colour of <see cref="start"/> to the colour of <see cref="end"/> based on how far a point is in the direction of the vector between these
        /// two endpoints.
        /// </para>
        /// <para>
        /// <see href="https://en.wikipedia.org/wiki/Color_gradient#Axial_gradients"/>
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>In More Detail:</b>
        /// </para>
        /// <para>
        /// We imagine a (real) line through <see cref="start"/> and <see cref="end"/>. Given a point, we project it perpendicularly onto the line. The colour is then the linear
        /// interpolation from the <see cref="start"/> colour to the <see cref="end"/> colour using the proportion <c>distance of projected point from <see cref="start"/> / distance of
        /// <see cref="end"/> from <see cref="start"/></c>.
        /// </para>
        /// <para>
        /// Points that project to outside the segment of the line between <see cref="start"/> and <see cref="end"/> will be given the colour of the closer endpoint. In other words, the colour
        /// is clamped between the <see cref="start"/> colour and <see cref="end"/> colour.
        /// </para>
        /// <para>
        /// If the coord of <see cref="start"/> is the same as the coord of <see cref="end"/>, then the pattern will be the <see cref="start"/> colour everywhere.
        /// </para>
        /// </remarks>
        public record Linear : IPattern2D<Color>
        {
            /// <summary>
            /// The position and colour of the start of the gradient.
            /// </summary>
            /// <seealso cref="end"/>
            public (IntVector2 coord, Color colour) start{ get; init; }
            /// <summary>
            /// The position and colour of the end of the gradient.
            /// </summary>
            /// <seealso cref="start"/>
            public (IntVector2 coord, Color colour) end { get; init; }

            /// <summary>
            /// See <see cref="Linear"/> for details.
            /// </summary>
            /// <param name="start">The position and colour of the start of the gradient.</param>
            /// <param name="end">The position and colour of the end of the gradient.</param>
            public Linear((IntVector2 coord, Color colour) start, (IntVector2 coord, Color colour) end)
            {
                this.start = start;
                this.end = end;
            }

            public Color this[IntVector2 point]
            {
                get
                {
                    if (start.coord == end.coord)
                    {
                        return start.colour;
                    }

                    if (point == start.coord)
                    {
                        return start.colour;
                    }
                    if (point == end.coord)
                    {
                        return end.colour;
                    }

                    float lengthOfLine = IntVector2.Distance(start.coord, end.coord);
                    Vector2 directionOfLine = ((Vector2)end.coord - start.coord) / lengthOfLine;

                    // See https://en.wikipedia.org/wiki/Dot_product#Scalar_projection_and_first_properties
                    float distanceOfProjectedPointAlongLine = Vector2.Dot(point - start.coord, directionOfLine);

                    return Color.Lerp(start.colour, end.colour, distanceOfProjectedPointAlongLine / lengthOfLine);
                }
            }
        }
    }
}