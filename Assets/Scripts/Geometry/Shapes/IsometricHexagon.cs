using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Maths;

using UnityEngine;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// An isometric pixel art hexagon shape.
    /// <example>
    /// For example,
    /// <code>
    ///           # #
    ///       # #     # #
    ///   # #             # #
    /// #                     #
    /// #                     #
    /// #                     #
    /// #                     #
    /// #                     #
    ///   # #             # #
    ///       # #     # #
    ///           # #
    /// </code>
    /// </example>
    /// </summary>
    ///  <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// The shape is symmetric across the central vertical / horizontal axis of the <see cref="boundingRect"/>.
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class IsometricHexagon : IIsometricShape<IsometricHexagon>, IFlippableShape<IsometricHexagon, CardinalAxis>, IDeepCopyableShape<IsometricHexagon>, IEquatable<IsometricHexagon>
    {
        /// <summary>
        /// The outline of the <see cref="IsometricHexagon"/>.
        /// </summary>
        public Path border { get; private set; }

        public bool filled { get; set; }

        /// <summary>
        /// The smallest rect containing the whole shape.
        /// </summary>
        /// <remarks>
        /// Note that when setting this, it may not be possible to draw an <see cref="IsometricHexagon"/> with that bounding rect. The rect will be made narrower (height unchanged) - moving
        /// the left and right sides by equal amounts - until an <see cref="IsometricHexagon"/> can be drawn with that bounding rect.
        /// </remarks>
        public IntRect boundingRect
        {
            get => border.boundingRect;
            set => border = CalculateBorder(value);
        }

        public int Count => filled ? boundingRect.xRange.Sum(x => border.MaxY(x) - border.MinY(x) + 1) : border.Count;

        /// <summary>
        /// See <see cref="IsometricRectangle"/> for details.
        /// </summary>
        /// <param name="rect"><see cref="boundingRect"/> will be assigned this value. See <see cref="boundingRect"/> for some caveats.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public IsometricHexagon(IntRect rect, bool filled)
        {
            this.filled = filled;
            this.boundingRect = rect;
        }

        /// <summary>
        /// Determines the border of the largest <see cref="IsometricHexagon"/> that fits within the given rect and spans its whole height (not necessarily its whole width).
        /// </summary>
        private static Path CalculateBorder(IntRect rect)
        {
            int midpointXLeft = Mathf.FloorToInt((rect.minX + rect.maxX) / 2f);
            int midpointXRight = Mathf.FloorToInt((rect.minX + rect.maxX + 1) / 2f);

            if (rect.height == 1)
            {
                return new Path(
                    new IntVector2(midpointXLeft, rect.minY),
                    new IntVector2(midpointXRight, rect.minY)
                    );
            }
            else if (rect.width == 1)
            {
                return new Path(rect.bottomLeft, rect.topRight);
            }
            else if (rect.width == 2)
            {
                return new Path(rect.bottomLeft, rect.bottomRight, rect.topRight, rect.topLeft, rect.bottomLeft);
            }

            int numBlocksInDiagonalEdge = Math.Min(
                (midpointXLeft - rect.minX) / 2,
                (rect.height - 2).ClampNonNegative() / 2
                );
            // Whether the vertical edges are in their own column or are in the same column as the end of the diagonal edges
            bool verticalEdgesInOwnColumn =
                (midpointXLeft - numBlocksInDiagonalEdge * 2) != rect.minX // The diagonal edges don't go to the edge of the rect
                && rect.minY + numBlocksInDiagonalEdge + 1 != rect.maxY - numBlocksInDiagonalEdge; // If the top & bottom diagonal edges meet, don't make a new column, or we'd have a horizontal block of 3

            List<Line> lines = new List<Line>(numBlocksInDiagonalEdge == 0 ? 4 : 8)
            {
                // Bottom corner
                new Line(
                    new IntVector2(midpointXLeft, rect.minY),
                    new IntVector2(midpointXRight, rect.minY)
                    )
            };

            // Bottom-right edge
            if (numBlocksInDiagonalEdge != 0)
            {
                lines.Add(new Line(
                    new IntVector2(midpointXRight + 1, rect.minY + 1),
                    new IntVector2(midpointXRight + numBlocksInDiagonalEdge * 2, rect.minY + numBlocksInDiagonalEdge)
                    ));
            }

            // Right vertical edge
            lines.Add(new Line(
                new IntVector2(midpointXRight + numBlocksInDiagonalEdge * 2 + (verticalEdgesInOwnColumn ? 1 : 0), rect.minY + numBlocksInDiagonalEdge + (verticalEdgesInOwnColumn ? 1 : 0)),
                new IntVector2(midpointXRight + numBlocksInDiagonalEdge * 2 + (verticalEdgesInOwnColumn ? 1 : 0), rect.maxY - numBlocksInDiagonalEdge - (verticalEdgesInOwnColumn ? 1 : 0))
                ));

            // Top-right edge
            if (numBlocksInDiagonalEdge != 0)
            {
                lines.Add(new Line(
                    new IntVector2(midpointXRight + numBlocksInDiagonalEdge * 2, rect.maxY - numBlocksInDiagonalEdge),
                    new IntVector2(midpointXRight + 1, rect.maxY - 1)
                    ));
            }

            // Top corner
            lines.Add(new Line(
                new IntVector2(midpointXRight, rect.maxY),
                new IntVector2(midpointXLeft, rect.maxY)
                ));

            // Top-left edge
            if (numBlocksInDiagonalEdge != 0)
            {
                lines.Add(new Line(
                    new IntVector2(midpointXLeft - 1, rect.maxY - 1),
                    new IntVector2(midpointXLeft - numBlocksInDiagonalEdge * 2, rect.maxY - numBlocksInDiagonalEdge)
                    ));
            }

            // Left vertical edge
            lines.Add(new Line(
                new IntVector2(midpointXLeft - numBlocksInDiagonalEdge * 2 - (verticalEdgesInOwnColumn ? 1 : 0), rect.maxY - numBlocksInDiagonalEdge - (verticalEdgesInOwnColumn ? 1 : 0)),
                new IntVector2(midpointXLeft - numBlocksInDiagonalEdge * 2 - (verticalEdgesInOwnColumn ? 1 : 0), rect.minY + numBlocksInDiagonalEdge + (verticalEdgesInOwnColumn ? 1 : 0))
                ));

            // Bottom-left edge
            if (numBlocksInDiagonalEdge != 0)
            {
                lines.Add(new Line(
                    new IntVector2(midpointXLeft - numBlocksInDiagonalEdge * 2, rect.minY + numBlocksInDiagonalEdge),
                    new IntVector2(midpointXLeft - 1, rect.minY + 1)
                    ));
            }

            return new Path(lines);
        }

        public bool Contains(IntVector2 point)
        {
            if (!filled)
            {
                return border.Contains(point);
            }

            return boundingRect.ContainsX(point.x) && border.MinY(point.x) <= point.y && point.y <= border.MaxY(point.x);
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricHexagon"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricHexagon operator +(IsometricHexagon isometricHexagon, IntVector2 translation) => isometricHexagon.Translated(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricHexagon"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricHexagon operator +(IntVector2 translation, IsometricHexagon isometricHexagon) => isometricHexagon + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricHexagon"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static IsometricHexagon operator -(IsometricHexagon isometricHexagon, IntVector2 translation) => isometricHexagon + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricHexagon"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Flipped(CardinalAxis)"/>
        public static IsometricHexagon operator -(IsometricHexagon isometricHexagon) => new IsometricHexagon(-isometricHexagon.boundingRect, isometricHexagon.filled);

        public void Translate(IntVector2 translation)
        {
            boundingRect += translation;
        }
        public IsometricHexagon Translated(IntVector2 translation) => new IsometricHexagon(boundingRect + translation, filled);

        public void Flip(VerticalAxis axis) => Flip((CardinalAxis)axis);
        public void Flip(CardinalAxis axis)
        {
            boundingRect = boundingRect.Flip(axis);
        }
        public IsometricHexagon Flipped(VerticalAxis axis) => Flipped((CardinalAxis)axis);
        public IsometricHexagon Flipped(CardinalAxis axis) => new IsometricHexagon(boundingRect.Flip(axis), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (!filled)
            {
                foreach (IntVector2 point in border)
                {
                    yield return point;
                }
                yield break;
            }

            foreach (int x in boundingRect.xRange)
            {
                for (int y = border.MinY(x); y <= border.MaxY(x); y++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }

        /// <summary>
        /// Whether the two <see cref="IsometricHexagon"/>s have the same shape, and the same value for <see cref="filled"/>.
        /// </summary>
        public static bool operator ==(IsometricHexagon a, IsometricHexagon b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(IsometricHexagon, IsometricHexagon)"/>.
        /// </summary>
        public static bool operator !=(IsometricHexagon a, IsometricHexagon b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(IsometricHexagon, IsometricHexagon)"/>.
        /// </summary>
        public bool Equals(IsometricHexagon other) => this == other;
        /// <summary>
        /// See <see cref="Equals(IsometricHexagon)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is IsometricHexagon other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => $"{nameof(IsometricHexagon)}({boundingRect}, {(filled ? "filled" : "unfilled")})";

        public IsometricHexagon DeepCopy() => new IsometricHexagon(boundingRect, filled);
    }
}