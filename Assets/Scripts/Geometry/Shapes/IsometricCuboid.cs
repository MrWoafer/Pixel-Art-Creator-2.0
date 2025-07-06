using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Geometry.Axes;
using PAC.Exceptions;
using PAC.Extensions.System.Collections;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// An isometric pixel art cuboid shape.
    /// <example>
    /// For example,
    /// <code>
    ///         # #
    ///     # #     # #
    /// # #             # #
    /// #   # #             # #
    /// #       # #     # #   #
    /// #           # #       #
    /// # #           #       #
    ///     # #       #     # #
    ///         # #   # # #
    ///             # #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// The top/bottom faces are identical <see cref="IsometricRectangle"/>s.
    /// </para>
    /// <para>
    /// If you imagine a real solid 3D cuboid, then 3 of the 12 edges are not visible in the isometric view as they are at the back of the cuboid. Whether the <see cref="IsometricCuboid"/>
    /// includes these edges or not is determined by <see cref="includeBackEdges"/>. 
    /// <example>
    /// For example, the example <see cref="IsometricCuboid"/> diagram above has <see cref="includeBackEdges"/> set to <see langword="false"/>. Setting it to <see langword="true"/> would look like:
    /// <code>
    ///         # #
    ///     # # #   # #
    /// # #     #       # #
    /// #   # # #           # #
    /// #       # #     # #   #
    /// #   # #     # #       #
    /// # #           # # #   #
    ///     # #       #     # #
    ///         # #   # # #
    ///             # #
    /// </code>
    /// </example>
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator may repeat some points.
    /// </para>
    /// </remarks>
    public class IsometricCuboid : IIsometricShape<IsometricCuboid>, IDeepCopyableShape<IsometricCuboid>, IEquatable<IsometricCuboid>
    {
        /// <summary>
        /// The bottom face of the <see cref="IsometricCuboid"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IsometricRectangle.filled"/> property of this is ignored, and the <see cref="filled"/> property of the <see cref="IsometricCuboid"/> is used instead.
        /// </remarks>
        /// <seealso cref="topFace"/>
        public IsometricRectangle bottomFace { get; private set; }
        /// <summary>
        /// The top face of the <see cref="IsometricCuboid"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="IsometricRectangle.filled"/> property of this is ignored, and the <see cref="filled"/> property of the <see cref="IsometricCuboid"/> is used instead.
        /// </remarks>
        /// <seealso cref="bottomFace"/>
        public IsometricRectangle topFace => bottomFace + Math.Abs(height) * IntVector2.up;

        /// <summary>
        /// The height of the cuboid if you imagined it in 3D space. In other words, how much <see cref="bottomFace"/> is translated upward to form <see cref="topFace"/>.
        /// </summary>
        public int height { get; set; }

        public bool filled { get; set; }
        /// <summary>
        /// Whether or not to show the 3 edges at the back of the cuboid. See <see cref="IsometricCuboid"/> for more details.
        /// </summary>
        /// <remarks>
        /// This has no effect if <see cref="filled"/> is <see langword="true"/>.
        /// </remarks>
        public bool includeBackEdges { get; set; }

        /// <summary>
        /// The edges of the <see cref="IsometricCuboid"/>.
        /// </summary>
        private I1DShape<IShape>[] wireframe
        {
            get
            {
                I1DShape<IShape>[] border = new I1DShape<IShape>[includeBackEdges ? 6 : 5];

                border[0] = includeBackEdges ? bottomFace.border : bottomFace.lowerBorder; // Bottom face
                border[1] = topFace.border;                                             // Top face
                border[2] = new Line(bottomFace.leftCorner, topFace.leftCorner);        // Left vertical edge
                border[3] = new Line(bottomFace.rightCorner, topFace.rightCorner);      // Right vertical edge
                border[4] = new Line(bottomFace.bottomCorner, topFace.bottomCorner);    // Front vertical edge
                if (includeBackEdges)
                {
                    border[5] = new Line(bottomFace.topCorner, topFace.topCorner);      // Back vertical edge
                }

                return border;
            }
        }

        public IntRect boundingRect => IntRect.BoundingRect(bottomFace.boundingRect, topFace.boundingRect);

        public int Count
        {
            get
            {
                if (!filled)
                {
                    return wireframe.Sum(path => path.Count);
                }

                Path lowerBorder = bottomFace.lowerBorder;
                Path upperBorder = topFace.upperBorder;
                return boundingRect.xRange.Sum(x => upperBorder.MaxY(x) - lowerBorder.MinY(x) + 1);
            }
        }

        /// <summary>
        /// Creates an <see cref="IsometricCuboid"/> with the given <see cref="IsometricRectangle"/> as either:
        /// <list type="bullet">
        /// <item>
        /// The bottom face, if <paramref name="height"/> &gt;= 0
        /// </item>
        /// <item>
        /// The top face, if <paramref name="height"/> &lt; 0
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="IsometricRectangle.filled"/> property of the <see cref="IsometricRectangle"/> is ignored, and the parameter <paramref name="filled"/> is used instead.
        /// </para>
        /// <para>
        /// The <see cref="IsometricRectangle"/> is not deep-copied, so changes to it will affect the <see cref="IsometricCuboid"/> as well.
        /// </para>
        /// </remarks>
        /// <param name="height">See <see cref="height"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        /// <param name="includeBackEdges">See <see cref="includeBackEdges"/>.</param>
        public IsometricCuboid(IsometricRectangle topOrBottomFace, int height, bool filled, bool includeBackEdges)
        {
            bottomFace = height >= 0 ? topOrBottomFace : (topOrBottomFace + new IntVector2(0, height));
            this.height = height;
            this.filled = filled;
            this.includeBackEdges = includeBackEdges;
        }

        public bool Contains(IntVector2 point)
            => filled
            ? boundingRect.ContainsX(point.x) && bottomFace.lowerBorder.MinY(point.x) <= point.y && point.y <= topFace.upperBorder.MaxY(point.x)
            : wireframe.Any(path => path.Contains(point));

        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricCuboid"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricCuboid operator +(IsometricCuboid isometricCuboid, IntVector2 translation) => isometricCuboid.Translated(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricCuboid"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricCuboid operator +(IntVector2 translation, IsometricCuboid isometricCuboid) => isometricCuboid + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricCuboid"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricCuboid operator -(IsometricCuboid isometricCuboid, IntVector2 translation) => isometricCuboid + (-translation);

        public void Translate(IntVector2 translation) => bottomFace.Translate(translation);
        public IsometricCuboid Translated(IntVector2 translation) => new IsometricCuboid((height >= 0 ? bottomFace.DeepCopy() : topFace.DeepCopy()) + translation, height, filled, includeBackEdges);

        public void Flip(VerticalAxis axis)
        {
            bottomFace.Flip(axis);
        }
        /// <inheritdoc/>
        /// <remarks>
        /// Can only be flipped across the vertical axis (or none axis).
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="axis"/> is an invalid axis.</exception>
        public IsometricCuboid Flipped(VerticalAxis axis) => axis switch
        {
            VerticalAxis => new IsometricCuboid((height >= 0 ? bottomFace.DeepCopy() : topFace.DeepCopy()).Flipped(axis), height, filled, includeBackEdges),
            _ => throw new UnreachableException()
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (filled)
            {
                Path lowerBorder = bottomFace.lowerBorder;
                Path upperBorder = topFace.upperBorder;
                foreach (int x in boundingRect.xRange)
                {
                    foreach (int y in IntRange.InclIncl(lowerBorder.MinY(x), upperBorder.MaxY(x)))
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                yield break;
            }

            foreach (IntVector2 point in wireframe.Flatten())
            {
                yield return point;
            }
        }

        /// <summary>
        /// Whether the two <see cref="IsometricCuboid"/>s have the same <see cref="bottomFace"/> and the same values for <see cref="height"/>, <see cref="filled"/> and
        /// <see cref="includeBackEdges"/>.
        /// </summary>
        /// <remarks>
        /// Note that if two <see cref="IsometricCuboid"/>s have identical values except their <see cref="height"/>s differ in sign then they will look identical but will not be considered ==.
        /// </remarks>
        public static bool operator ==(IsometricCuboid a, IsometricCuboid b)
            => a.bottomFace == b.bottomFace && a.height == b.height && a.filled == b.filled && a.includeBackEdges == b.includeBackEdges;
        /// <summary>
        /// See <see cref="operator ==(IsometricCuboid, IsometricCuboid)"/>.
        /// </summary>
        public static bool operator !=(IsometricCuboid a, IsometricCuboid b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(IsometricCuboid, IsometricCuboid)"/>.
        /// </summary>
        public bool Equals(IsometricCuboid other) => this == other;
        /// <summary>
        /// See <see cref="Equals(IsometricCuboid)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is IsometricCuboid other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(bottomFace, height, filled, includeBackEdges);

        public override string ToString()
        {
            string filledStr = (filled ? "filled" : "unfilled");
            string includeBackEdgesStr = (includeBackEdges ? "show back edges" : "don't show back edges");
            return $"{nameof(IsometricCuboid)}({bottomFace.startCorner}, {bottomFace.endCorner}, {height}, {filledStr}, {includeBackEdgesStr})";
        }

        public IsometricCuboid DeepCopy() => new IsometricCuboid(height >= 0 ? bottomFace.DeepCopy() : topFace.DeepCopy(), height, filled, includeBackEdges);
    }
}