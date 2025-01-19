using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Maths;
using PAC.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Shapes
{
    /// <summary>
    /// A pixel art ellipse shape.
    /// <example>
    /// For example,
    /// <code>
    ///     # # # #
    ///   #         #
    /// #             #
    /// #             #
    ///   #         #
    ///     # # # #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// If the width or height of the <see cref="boundingRect"/> is &lt;= 2, it will look like a rectangle.
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class Ellipse : I2DShape<Ellipse>, IDeepCopyableShape<Ellipse>, IEquatable<Ellipse>
    {
        /* How this shape works:
         * 
         * Given the bounding rect, we imagine the largest real ellipse (with its major/minor axes along the x/y axes) that fits inside it. For example, if the bounding rect is
         * 
         *      # = integer point in bounding rect
         * 
         *           y
         *           ^
         *         4 |
         *         3 |     # # # #
         *         2 |     # # # #
         *         1 |     # # # #
         *       — 0 + — — — — — — — — > x
         *           0 1 2 3 4 5 6 7 8
         *           |
         *
         * then the imaginary ellipse will have width 4, height 3. (Follow along on Desmos: https://www.desmos.com/calculator/jsuoq2ofkz)
         * 
         * We say a pixel is in the filled pixel art ellipse iff the centre of that grid square is in the filled imaginary ellipse. So, in our example we get
         * 
         *           y
         *           ^
         *         4 |
         *         3 |       # #
         *         2 |     # # # #
         *         1 |       # #
         *       — 0 + — — — — — — — — > x
         *           0 1 2 3 4 5 6 7 8
         *           |
         *     
         * (There are a few exceptions to this - namely the 2xn, nx2 and 3x3 cases. We manually override these to make them look more aesthetic.)
         *
         * The unfilled pixel art ellipse is obtained by just taking the pixels on the border of the filled pixel art ellipse:
         * 
         *           y
         *           ^
         *         4 |
         *         3 |       # #
         *         2 |     #     #
         *         1 |       # #
         *       — 0 + — — — — — — — — > x
         *           0 1 2 3 4 5 6 7 8
         *           |
         * 
         * Since we care about the centre of the grid square an integer point refers to, we work in a coordinate system where integer coordinates refer to the CENTRE of the grid square.
         */

        /// <summary>
        /// The imaginary ellipse described in the 'How this shape works' comment.
        /// </summary>
        /// <remarks>
        /// This is cached when you set the <see cref="boundingRect"/> of the <see cref="Ellipse"/>.
        /// </remarks>
        private ImaginaryEllipse imaginaryEllipse;
        /// <summary>
        /// A real ellipse that has its major/minor axes along the x/y axes.
        /// </summary>
        private readonly struct ImaginaryEllipse
        {
            public readonly float xRadius { get; init; }
            public readonly float yRadius { get; init; }
            public readonly Vector2 centre { get; init; }

            /// <summary>
            /// Whether the point is in this ellipse (as a filled ellipse).
            /// </summary>
            public bool Contains(Vector2 point) => (point.x - centre.x).Square() / xRadius.Square() + (point.y - centre.y).Square() / yRadius.Square() <= 1f;
        }

        public bool filled { get; set; }

        private IntRect _boundingRect;
        public IntRect boundingRect
        {
            get => _boundingRect;
            set
            {
                _boundingRect = value;

                imaginaryEllipse = new ImaginaryEllipse
                {
                    xRadius = boundingRect.width / 2f,
                    yRadius = boundingRect.height / 2f,
                    centre = ((Vector2)boundingRect.bottomLeft + boundingRect.topRight) / 2f
                };
            }
        }

        /// <summary>
        /// Whether the <see cref="Ellipse"/> is a circle.
        /// </summary>
        /// <remarks>
        /// This is equivalent to its <see cref="boundingRect"/> being a square.
        /// </remarks>
        public bool isCircle => boundingRect.isSquare;

        public int Count => Enumerable.Count(this);

        /// <summary>
        /// Creates the largest <see cref="Ellipse"/> that can fit in the given rect.
        /// </summary>
        /// <param name="boundingRect">See <see cref="boundingRect"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public Ellipse(IntRect boundingRect, bool filled)
        {
            this.boundingRect = boundingRect;
            this.filled = filled;
        }

        /// <summary>
        /// Determines whether the point <c>(<paramref name="x"/>, <paramref name="y"/>)</c> is inside the filled <see cref="Ellipse"/> (including the border).
        /// </summary>
        private bool FilledContains(int x, int y) => FilledContains(new IntVector2(x, y));
        /// <summary>
        /// Determines whether the given point is inside the filled <see cref="Ellipse"/> (including the border).
        /// </summary>
        private bool FilledContains(IntVector2 point)
        {
            // Manually override 1xn and 2xn case (as otherwise the algorithm doesn't include the top/bottom row in the 2xn case - the 1xn case is just because we might as well include it)
            if (boundingRect.width <= 2)
            {
                return boundingRect.Contains(point);
            }
            // Manually override nx1 and nx2 case (as otherwise the algorithm doesn't include the left/right column in the 2xn case - the 1xn case is just because we might as well include it)
            if (boundingRect.height <= 2)
            {
                return boundingRect.Contains(point);
            }
            // Manually override 3x3 case (as otherwise the algorithm gives a 3x3 square instead of the more aesthetic 'plus sign')
            if (boundingRect.width == 3 && boundingRect.height == 3)
            {
                IntVector2 centre = boundingRect.bottomLeft + IntVector2.one;
                // This just gives the 'plus sign' shape we want
                return IntVector2.L1Distance(point, centre) <= 1;
            }

            // General case
            return imaginaryEllipse.Contains(point);
        }

        public bool Contains(IntVector2 point)
        {
            if (filled)
            {
                return FilledContains(point);
            }
            else
            {
                if (!FilledContains(point))
                {
                    return false;
                }
                return !FilledContains(point + IntVector2.up) || !FilledContains(point + IntVector2.down) || !FilledContains(point + IntVector2.left) || !FilledContains(point + IntVector2.right);
            }
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="Ellipse"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Ellipse operator +(Ellipse ellipse, IntVector2 translation) => ellipse.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Ellipse"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Ellipse operator +(IntVector2 translation, Ellipse ellipse) => ellipse + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="Ellipse"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Ellipse operator -(Ellipse ellipse, IntVector2 translation) => ellipse + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Ellipse"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(RotationAngle)"/>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static Ellipse operator -(Ellipse ellipse) => new Ellipse(-ellipse.boundingRect, ellipse.filled);

        public Ellipse Translate(IntVector2 translation) => new Ellipse(boundingRect + translation, filled);
        public Ellipse Flip(FlipAxis axis) => new Ellipse(boundingRect.Flip(axis), filled);
        public Ellipse Rotate(RotationAngle angle) => new Ellipse(boundingRect.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            // Manually define the case where it's a horizontal/vertical line, to avoid repeating points
            if (boundingRect.width == 1)
            {
                foreach (int y in boundingRect.yRange)
                {
                    yield return new IntVector2(boundingRect.minX, y);
                }
                yield break;
            }
            if (boundingRect.height == 1)
            {
                foreach (int x in boundingRect.xRange)
                {
                    yield return new IntVector2(x, boundingRect.minY);
                }
                yield break;
            }

            // Filled
            if (filled)
            {
                int startX = Mathf.FloorToInt(imaginaryEllipse.centre.x);
                foreach (int y in boundingRect.yRange)
                {
                    for (int x = startX; FilledContains(x, y); x--)
                    {
                        yield return new IntVector2(x, y);
                    }
                    for (int x = startX + 1; FilledContains(x, y); x++)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                yield break;
            }

            // Unfilled

            /* Algorithm:
             * 
             * We start at the top of the ellipse:
             *              v
             *          # # o # #
             *      # #           # #
             *    #                   #
             *    #                   #
             *      # #           # #
             *          # # # # #
             *     
             * Then we do the following:
             * 
             * 1) Check the point to the right. If it's in the ellipse, we move to it and go to Step 1.
             * 
             *  (Step 1 gets us to here)
             *                  v
             *          # # # # o
             *      # #           # #
             *    #                   #
             *    #                   #
             *      # #           # #
             *          # # # # #
             * 
             * 2) If it's not, we check the point diagonally down to the right. If it's in the ellipse, we move to it and go to Step 1.
             *       
             * (Step 1 and 2 get us to here)
             * 
             *          # # # # #
             *      # #           # #
             *    #                   o <
             *    #                   #
             *      # #           # #
             *          # # # # #
             *     
             * 3) If it's not, we check the point below. If it's in the ellipse, we move to it and go to Step 1.
             * 
             * (Step 1, 2 and 3 get us to here)
             * 
             *          # # # # #
             *      # #           # #
             *    #                   #
             *    #                   o <
             *      # #           # #
             *          # # # # #
             *     
             * 4) If it's not, then we rotate the directions we check in by 90 degrees (clockwise) and go to Step 1.
             */

            IntVector2 primaryDirection = IntVector2.right;
            IntVector2 secondaryDirection = IntVector2.downRight;
            IntVector2 tertiaryDirection = IntVector2.down;
            IntVector2 start = new IntVector2(boundingRect.minX + boundingRect.width / 2, boundingRect.maxY);
            IntVector2 currentPoint = start;

            int iterations = 0; // Used to stop accidental infinite loops
            do
            {
                iterations++;

                if (FilledContains(currentPoint + primaryDirection))
                {
                    yield return currentPoint;
                    currentPoint += primaryDirection;
                }
                else if (FilledContains(currentPoint + secondaryDirection))
                {
                    yield return currentPoint;
                    currentPoint += secondaryDirection;
                }
                else if (FilledContains(currentPoint + tertiaryDirection))
                {
                    yield return currentPoint;
                    currentPoint += tertiaryDirection;
                }
                else
                {
                    primaryDirection = primaryDirection.Rotate(RotationAngle._90);
                    secondaryDirection = secondaryDirection.Rotate(RotationAngle._90);
                    tertiaryDirection = tertiaryDirection.Rotate(RotationAngle._90);
                }
            }
            while (currentPoint != start && iterations < 10_000);
        }

        /// <summary>
        /// Whether the two <see cref="Ellipse"/>s have the same shape, and the same value for <see cref="filled"/>.
        /// </summary>
        public static bool operator ==(Ellipse a, Ellipse b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(Ellipse, Ellipse)"/>.
        /// </summary>
        public static bool operator !=(Ellipse a, Ellipse b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(Ellipse, Ellipse)"/>.
        /// </summary>
        public bool Equals(Ellipse other) => this == other;
        /// <summary>
        /// See <see cref="Equals(Ellipse)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is Ellipse other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => $"{nameof(Ellipse)}({boundingRect}, {(filled ? "filled" : "unfilled")})";

        public Ellipse DeepCopy() => new Ellipse(boundingRect, filled);
    }
}