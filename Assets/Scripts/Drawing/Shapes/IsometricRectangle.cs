using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Extensions;
using PAC.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Shapes
{
    /// <summary>
    /// An isometric pixel art rectangle shape.
    /// <example>
    /// For example,
    /// <code>
    ///         # #
    ///     # #     # #
    /// # #             # #
    ///     # #             # #
    ///         # #     # #
    ///             # #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// The shape is determined by <see cref="startCorner"/> and <see cref="endCorner"/>, which will be opposite corners of the rectangle. The lines will always be perfect in blocks of 2, except
    /// possibly at the left/right/top/bottom blocks of the shape:
    /// <list type="bullet">
    /// <item>
    /// The top/bottom will be a horizontal block of 2 or 3. They will match.
    /// </item>
    /// <item>
    /// The left/bottom will either be a single point, a horizontal block of 2 or a vertical block of 2.
    /// <list type="bullet">
    /// <item>
    /// If one is in the vertical case, so will be the other.
    /// </item>
    /// <item>
    /// In the other two cases, they may not match, but if <see cref="endCorner"/> is in a horizontal block of 2, then so is <see cref="startCorner"/>.
    /// </item>
    /// </list>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class IsometricRectangle : IIsometricShape<IsometricRectangle>, IDeepCopyableShape<IsometricRectangle>, IEquatable<IsometricRectangle>
    {
        /// <summary>
        /// The point the mouse started dragging from.
        /// </summary>
        /// <remarks>
        /// This will always be one of <see cref="leftCorner"/>, <see cref="rightCorner"/>, <see cref="bottomCorner"/> or <see cref="topCorner"/>.
        /// </remarks>
        /// <seealso cref="endCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        public IntVector2 startCorner;
        /// <summary>
        /// The point the mouse has dragged to.
        /// </summary>
        /// <remarks>
        /// This will always be one of <see cref="leftCorner"/>, <see cref="rightCorner"/>, <see cref="bottomCorner"/> or <see cref="topCorner"/>.
        /// </remarks>
        /// <seealso cref="startCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        public IntVector2 endCorner;

        /// <summary>
        /// The list of the 4 (or fewer in special cases) corners that make up the <see cref="IsometricRectangle"/>, that have been inferred from <see cref="startCorner"/> and
        /// <see cref="endCorner"/>.
        /// </summary>
        /// <remarks>
        /// This contains precisely <see cref="leftCorner"/>, <see cref="rightCorner"/>, <see cref="bottomCorner"/> and <see cref="topCorner"/>.
        /// </remarks>
        private IntVector2[] inferredCorners;

        /// <summary>
        /// (One of) the leftmost point(s) of the <see cref="IsometricRectangle"/>.
        /// </summary>
        /// <seealso cref="startCorner"/>
        /// <seealso cref="endCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        public IntVector2 leftCorner
        {
            get => inferredCorners.ArgMin(p => p.x);
            set
            {
                startCorner = rightCorner;
                endCorner = leftCorner;
                InferCornersAndBorder();
            }
        }
        /// <summary>
        /// (One of) the rightmost point(s) of the <see cref="IsometricRectangle"/>.
        /// </summary>
        /// <seealso cref="startCorner"/>
        /// <seealso cref="endCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        public IntVector2 rightCorner
        {
            get => inferredCorners.ArgMax(p => p.x);
            set
            {
                startCorner = leftCorner;
                endCorner = rightCorner;
                InferCornersAndBorder();
            }
        }
        /// <summary>
        /// One of the bottommost points of the <see cref="IsometricRectangle"/>.
        /// </summary>
        /// <seealso cref="startCorner"/>
        /// <seealso cref="endCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="topCorner"/>
        public IntVector2 bottomCorner
        {
            get => inferredCorners.ArgMin(p => p.y);
            set
            {
                startCorner = topCorner;
                endCorner = bottomCorner;
                InferCornersAndBorder();
            }
        }
        /// <summary>
        /// One of the topmost points of the <see cref="IsometricRectangle"/>.
        /// </summary>
        /// <seealso cref="startCorner"/>
        /// <seealso cref="endCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="bottomCorner"/>
        public IntVector2 topCorner
        {
            get => inferredCorners.ArgMax(p => p.y);
            set
            {
                startCorner = bottomCorner;
                endCorner = topCorner;
                InferCornersAndBorder();
            }
        }

        /// <summary>
        /// The outline of the <see cref="IsometricRectangle"/>.
        /// </summary>
        /// <remarks>
        /// This is the concatenation of <see cref="lowerBorder"/> and <see cref="upperBorder"/>.
        /// </remarks>
        public Path border => Path.Concat(lowerBorder, upperBorder);
        /// <summary>
        /// The lower edges of the <see cref="border"/>, i.e. (<see cref="leftCorner"/>, <see cref="bottomCorner"/>) and (<see cref="bottomCorner"/>, <see cref="rightCorner"/>) - not necessarily in
        /// this direction.
        /// </summary>
        /// <remarks>
        /// This is precisely the set of points in <see cref="border"/> that have the lowest y coord among those with the same x coord. (Note there's at most one other point with the same x coord.)
        /// </remarks>
        public Path lowerBorder { get; private set; }
        /// <summary>
        /// The upper edges of the <see cref="border"/>, i.e. (<see cref="leftCorner"/>, <see cref="topCorner"/>) and (<see cref="topCorner"/>, <see cref="rightCorner"/>) - not necessarily in
        /// this direction.
        /// </summary>
        /// <remarks>
        /// This is precisely the set of points in <see cref="border"/> that have the highest y coord among those with the same x coord. (Note there's at most one other point with the same x coord.)
        /// </remarks>
        public Path upperBorder { get; private set; }

        public bool filled { get; set; }

        /// <summary>
        /// Whether the sides lengths of the <see cref="IsometricRectangle"/> are all equal.
        /// </summary>
        public bool isIsometricSquare => (lowerBorder.MinX(boundingRect.minY) - boundingRect.minX) == (boundingRect.maxX - lowerBorder.MaxX(boundingRect.minY));

        public IntRect boundingRect => new IntRect(new IntVector2(leftCorner.x, bottomCorner.y), new IntVector2(rightCorner.x, topCorner.y));

        public int Count => filled ?
            boundingRect.xRange.Sum(x => upperBorder.MaxY(x) - lowerBorder.MinY(x) + 1) :
            // This computation uses the fact that each column of x coords contains exactly 1 or 2 points on the isometric rectangle's border
            boundingRect.xRange.Sum(x => upperBorder.MaxY(x) == lowerBorder.MinY(x) ? 1 : 2);

        /// <summary>
        /// See <see cref="IsometricRectangle"/> for details.
        /// </summary>
        /// <param name="startCorner">See <see cref="startCorner"/>.</param>
        /// <param name="endCorner">See <see cref="endCorner"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public IsometricRectangle(IntVector2 startCorner, IntVector2 endCorner, bool filled)
        {
            this.startCorner = startCorner;
            this.endCorner = endCorner;
            this.filled = filled;

            InferCornersAndBorder();
        }

        private enum IsometricGradient
        {
            /// <summary>
            /// The gradient is &lt; 1/2 and &gt; -1/2.
            /// </summary>
            ShallowerThanHalf,
            /// <summary>
            /// The gradient is +/- 1/2.
            /// </summary>
            PlusMinusHalf,
            /// <summary>
            /// The gradient is &gt; 1/2 or &lt; -1/2.
            /// </summary>
            SteeperThanHalf
        }
        /// <summary>
        /// Using just <see cref="startCorner"/> and <see cref="endCorner"/>, deduces the other two corners and the shape of the <see cref="border"/>.
        /// </summary>
        private void InferCornersAndBorder()
        {
            // Specific cases

            if (startCorner == endCorner)
            {
                // Single point

                inferredCorners = new IntVector2[] { startCorner };
                lowerBorder = upperBorder = new Line(startCorner);
                return;
            }
            if (Math.Abs(startCorner.y - endCorner.y) == 1)
            {
                if (startCorner.x == endCorner.x)
                {
                    // 3x2 rectangle

                    IntRect rect = new IntRect(startCorner, endCorner);
                    IntVector2 leftCorner = rect.bottomLeft + IntVector2.left;
                    IntVector2 rightCorner = rect.topRight + IntVector2.right;
                    inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                    lowerBorder = new Line(leftCorner, rightCorner + IntVector2.down);
                    upperBorder = new Line(rightCorner, leftCorner + IntVector2.up);
                    return;
                }
                else if (Math.Abs(startCorner.x - endCorner.x) == 1)
                {
                    // 2x2 rectangle

                    IntRect rect = new IntRect(startCorner, endCorner);
                    IntVector2 leftCorner = rect.bottomLeft;
                    IntVector2 rightCorner = rect.topRight;
                    inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                    lowerBorder = new Line(leftCorner, rightCorner + IntVector2.down);
                    upperBorder = new Line(rightCorner, leftCorner + IntVector2.up);
                    return;
                }
            }

            // General cases

            IntVector2 sign = IntVector2.Sign(endCorner - startCorner);
            IntVector2 diff = endCorner - startCorner;
            IntVector2 absDiff = IntVector2.Abs(diff);
            IntVector2 adjustedEndCorner = endCorner + ((absDiff.x + 1) % 2) * new IntVector2(sign.x, 0);

            IsometricGradient region = (Math.Abs(adjustedEndCorner.x - startCorner.x) + 1 - 2 * (Math.Abs(adjustedEndCorner.y - startCorner.y) + 1)) switch
            {
                > 0 => IsometricGradient.ShallowerThanHalf,
                0 => IsometricGradient.PlusMinusHalf,
                < 0 => IsometricGradient.SteeperThanHalf,
            };

            // If startCorner and endCorner are on a line with gradient +/- 1/2
            if (region == IsometricGradient.PlusMinusHalf)
            {
                inferredCorners = new IntVector2[] { startCorner, endCorner };
                int width = absDiff.x + 1;
                if (width % 2 == 0)
                {
                    lowerBorder = new Line(startCorner, endCorner);
                    upperBorder = lowerBorder.reverse;
                }
                else
                {
                    lowerBorder = new Path(new Line(startCorner, endCorner - sign), new Line(endCorner, endCorner));
                    upperBorder = lowerBorder.reverse;
                }
                return;
            }

            // In comments below, '= 2 horizontal' etc signifies the size of the block (in the border lines) that corner is in
            // E.g. 'start = 2 horizontal' indicates that startCorner will be in a horizontal block of 2 pixels in the border
            //
            // This value completely determines these block sizes / directions
            int shapeType = (absDiff.x + 2 * absDiff.y) % 4;

            // If {startCorner, endCorner} = {leftCorner, rightCorner}
            if (region == IsometricGradient.ShallowerThanHalf)
            {
                if (shapeType == 0)
                {
                    // start = 2 horizontal, end = 1, top / bottom = 2 horizontal

                    int numBlocks = Mathf.CeilToInt((absDiff.x + 2) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                    IntVector2 bottomCorner = startCorner + offset;
                    IntVector2 topCorner = adjustedEndCorner - offset;

                    inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                    lowerBorder = new Path(new Line[] {
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner - new IntVector2(sign.x, 1)),
                            new Line(endCorner, endCorner)
                            }
                        // Remove any lines that go backwards (possible when bottomCorner and endCorner are diagonally adjacent).
                        // This doesn't affect how it looks, but it's useful for making the enumerator not repeat any pixels.
                        .Where(l => l.vector.sign.x != -Math.Sign(endCorner.x - startCorner.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(endCorner, endCorner),
                            new Line(endCorner + new IntVector2(-sign.x, 1), topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(startCorner.x - endCorner.x))
                        );
                }
                else if (shapeType == 1)
                {
                    // start = 2 horizontal, end = 2 horizontal, top / bottom = 2 horizontal

                    int numBlocks = Mathf.CeilToInt((absDiff.x + 1) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                    IntVector2 bottomCorner = startCorner + offset;
                    IntVector2 topCorner = endCorner - offset;

                    inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                    lowerBorder = new Path(new Line[] {
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(endCorner.x - startCorner.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(endCorner, topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(startCorner.x - endCorner.x))
                        );
                }
                else if (shapeType == 2)
                {
                    // start = 2 horizontal, end = 2 horizontal, top / bottom = 3 horizontal

                    int numBlocks = Mathf.CeilToInt(absDiff.x / 4f) - Mathf.FloorToInt(diff.y / 2f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                    IntVector2 bottomCorner = startCorner + offset;
                    IntVector2 topCorner = endCorner - offset;

                    inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                    lowerBorder = new Path(new Line[] {
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner, endCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(endCorner.x - startCorner.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(endCorner, topCorner),
                            new Line(topCorner, startCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(startCorner.x - endCorner.x))
                        );
                }
                else if (shapeType == 3)
                {
                    // start = 1, end = 1, top / bottom = 2 horizontal

                    int numBlocks = Mathf.CeilToInt((absDiff.x - 1) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1);
                    IntVector2 bottomCorner = startCorner + offset;
                    IntVector2 topCorner = adjustedEndCorner - offset;

                    inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                    lowerBorder = new Path(new Line[] {
                            new Line(startCorner, startCorner),
                            new Line(startCorner + new IntVector2(sign.x, -1), bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner - new IntVector2(sign.x, 1)),
                            new Line(endCorner, endCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(endCorner.x - startCorner.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(endCorner, endCorner),
                            new Line(endCorner + new IntVector2(-sign.x, 1), topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner + new IntVector2(sign.x, 1)),
                            new Line(startCorner, startCorner)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(startCorner.x - endCorner.x))
                        );
                }
                else
                {
                    throw new UnreachableException();
                }
            }
            // If {startCorner, endCorner} = {topCorner, bottomCorner}
            else if (region == IsometricGradient.SteeperThanHalf)
            {
                if (Math.Abs(adjustedEndCorner.x - startCorner.x) + 1 == 2 * Math.Abs(adjustedEndCorner.y - startCorner.y))
                {
                    IntVector2 corner1 = startCorner + sign.y * IntVector2.up;
                    IntVector2 corner2 = endCorner + sign.y * IntVector2.down;
                    inferredCorners = new IntVector2[] { startCorner, endCorner, corner1, corner2 };
                    if (startCorner.y < endCorner.y)
                    {
                        if ((absDiff.x + 1) % 2 == 0)
                        {
                            lowerBorder = new Line(startCorner, corner2);
                            upperBorder = new Line(endCorner, corner1);
                        }
                        else
                        {
                            lowerBorder = new Path(new Line(startCorner, corner2 - sign), new Line(corner2, corner2));
                            upperBorder = new Path(new Line(endCorner, endCorner), new Line(endCorner - sign, corner1));
                        }
                    }
                    else
                    {
                        if ((absDiff.x + 1) % 2 == 0)
                        {
                            lowerBorder = new Line(endCorner, corner1);
                            upperBorder = new Line(startCorner, corner2);
                        }
                        else
                        {
                            lowerBorder = new Path(new Line(endCorner, endCorner), new Line(endCorner - sign, corner1));
                            upperBorder = new Path(new Line(startCorner, corner2 - sign), new Line(corner2, corner2));
                        }
                    }
                    return;
                }

                (IntVector2 topCorner, IntVector2 bottomCorner) = (endCorner.y - startCorner.y) switch
                {
                    > 0 => (endCorner, startCorner),
                    < 0 => (startCorner, endCorner),
                    _ => throw new UnreachableException()
                };
                sign = IntVector2.Sign(topCorner - bottomCorner);

                if (shapeType == 0)
                {
                    if (bottomCorner.x == topCorner.x)
                    {
                        // start / end = 3 horizontal, left / right = 1

                        int numBlocks = absDiff.y / 2;
                        IntVector2 offset = numBlocks * new IntVector2(2, 1);
                        IntVector2 rightCorner = bottomCorner + offset;
                        IntVector2 leftCorner = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, rightCorner, leftCorner };
                        lowerBorder = new Path(
                            new Line(leftCorner, leftCorner),
                            new Line(leftCorner + IntVector2.downRight, bottomCorner),
                            new Line(bottomCorner, rightCorner + IntVector2.downLeft),
                            new Line(rightCorner, rightCorner)
                            );
                        upperBorder = new Path(
                            new Line(rightCorner, rightCorner),
                            new Line(rightCorner + IntVector2.upLeft, topCorner),
                            new Line(topCorner, leftCorner + IntVector2.upRight),
                            new Line(leftCorner, leftCorner)
                            );
                    }
                    else
                    {
                        // start / end = 2 horizontal, left / right = 1

                        int numBlocks = absDiff.y / 2 + Mathf.CeilToInt(absDiff.x / 4f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                        IntVector2 corner1 = bottomCorner + offset;
                        IntVector2 corner2 = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                        lowerBorder = new Path(new Line[] {
                                new Line(corner2, corner2),
                                new Line(corner2 + new IntVector2(sign.x, -1), bottomCorner),
                                new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                                new Line(corner1, corner1)
                                }
                            .Where(l => l.vector.sign.x != -Math.Sign(corner1.x - corner2.x))
                            );
                        upperBorder = new Path(new Line[] {
                                new Line(corner1, corner1),
                                new Line(corner1 - new IntVector2(sign.x, -1), topCorner),
                                new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                                new Line(corner2, corner2)
                                }
                            .Where(l => l.vector.sign.x != -Math.Sign(corner2.x - corner1.x))
                            );
                    }
                }
                else if (shapeType == 1)
                {
                    // start / end = 2 horizontal, left / right = 1

                    int numBlocks = absDiff.y / 2 + Mathf.CeilToInt((absDiff.x - 1) / 4f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                    IntVector2 corner1 = bottomCorner + offset;
                    IntVector2 corner2 = topCorner - offset;

                    inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                    lowerBorder = new Path(new Line[] {
                            new Line(corner2, corner2),
                            new Line(corner2 + new IntVector2(sign.x, -1), bottomCorner + new IntVector2(-sign.x, 1)),
                            new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                            new Line(corner1, corner1)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(corner1.x - corner2.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(corner1, corner1),
                            new Line(corner1 - new IntVector2(sign.x, -1), topCorner - new IntVector2(-sign.x, 1)),
                            new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                            new Line(corner2, corner2)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(corner2.x - corner1.x))
                        );
                }
                else if (shapeType == 2)
                {
                    if (bottomCorner.x == topCorner.x)
                    {
                        // start / end = 3 horizontal, left / right = 2 vertical

                        int numBlocks = (absDiff.y - 1) / 2;
                        IntVector2 offset = numBlocks * new IntVector2(2, 1);
                        // On the y level closer to bottomCorner
                        IntVector2 rightCorner = bottomCorner + offset;
                        // On the y level closer to topCorner
                        IntVector2 leftCorner = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, rightCorner, leftCorner };
                        lowerBorder = new Path(
                            new Line(leftCorner + IntVector2.down, leftCorner + IntVector2.down),
                            new Line(leftCorner + new IntVector2(1, -2 * 1), bottomCorner),
                            new Line(bottomCorner, rightCorner + IntVector2.downLeft),
                            new Line(rightCorner, rightCorner)
                            );
                        upperBorder = new Path(
                            new Line(rightCorner + IntVector2.up, rightCorner + IntVector2.up),
                            new Line(rightCorner - new IntVector2(1, -2 * 1), topCorner),
                            new Line(topCorner, leftCorner + IntVector2.upRight),
                            new Line(leftCorner, leftCorner)
                            );
                    }
                    else
                    {
                        // start / end = 2 horizontal, left / right = 2 vertical

                        int numBlocks = (absDiff.y - 1) / 2 + Mathf.CeilToInt(absDiff.x / 4f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                        // On the y level closer to bottomCorner
                        IntVector2 corner1 = bottomCorner + offset;
                        // On the y level closer to topCorner
                        IntVector2 corner2 = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                        lowerBorder = new Path(new Line[] {
                                new Line(corner2 + IntVector2.down, corner2 + IntVector2.down),
                                new Line(corner2 + new IntVector2(sign.x, -2 * 1), bottomCorner),
                                new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                                new Line(corner1, corner1)
                                }
                            .Where(l => l.vector.sign.x != -Math.Sign(corner1.x - corner2.x))
                            );
                        upperBorder = new Path(new Line[] {
                                new Line(corner1 + IntVector2.up, corner1 + IntVector2.up),
                                new Line(corner1 - new IntVector2(sign.x, -2 * 1), topCorner),
                                new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                                new Line(corner2, corner2)
                                }
                            .Where(l => l.vector.sign.x != -Math.Sign(corner2.x - corner1.x))
                            );
                    }
                }
                else if (shapeType == 3)
                {
                    // start / end = 2 horizontal, left / right = 2 vertical

                    int numBlocks = (absDiff.y - 1) / 2 + Mathf.CeilToInt((absDiff.x - 1) / 4f);
                    IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                    // On the y level closer to bottomCorner
                    IntVector2 corner1 = bottomCorner + offset;
                    // On the y level closer to topCorner
                    IntVector2 corner2 = topCorner - offset;

                    inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                    lowerBorder = new Path(new Line[] {
                            new Line(corner2 + IntVector2.down, corner2 + IntVector2.down),
                            new Line(corner2 + new IntVector2(sign.x, -2 * 1), bottomCorner + new IntVector2(-sign.x, 1)),
                            new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                            new Line(corner1, corner1)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(corner1.x - corner2.x))
                        );
                    upperBorder = new Path(new Line[] {
                            new Line(corner1 + IntVector2.up, corner1 + IntVector2.up),
                            new Line(corner1 - new IntVector2(sign.x, -2 * 1), topCorner - new IntVector2(-sign.x, 1)),
                            new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                            new Line(corner2, corner2)
                            }
                        .Where(l => l.vector.sign.x != -Math.Sign(corner2.x - corner1.x))
                        );
                }
            }
            else
            {
                throw new UnreachableException($"Unexpected {nameof(IsometricGradient)}: {region}.");
            }
        }

        public bool Contains(IntVector2 point) => filled ? (border.Contains(point) || border.WindingNumber(point) != 0) : border.Contains(point);

        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricRectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IsometricRectangle operator +(IsometricRectangle isometricRectangle, IntVector2 translation) => isometricRectangle.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricRectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IsometricRectangle operator +(IntVector2 translation, IsometricRectangle isometricRectangle) => isometricRectangle + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricRectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IsometricRectangle operator -(IsometricRectangle isometricRectangle, IntVector2 translation) => isometricRectangle + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="IsometricRectangle"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static IsometricRectangle operator -(IsometricRectangle isometricRectangle)
            => new IsometricRectangle(-isometricRectangle.startCorner, -isometricRectangle.endCorner, isometricRectangle.filled);

        public IsometricRectangle Translate(IntVector2 translation) => new IsometricRectangle(startCorner + translation, endCorner + translation, filled);
        public IsometricRectangle Flip(FlipAxis axis) => new IsometricRectangle(startCorner.Flip(axis), endCorner.Flip(axis), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (!filled)
            {
                // We do this instead of iterating over border so that we don't repeat any points (lower border and upper border can sometimes intersect in places other than the corners)
                foreach (IntVector2 point in lowerBorder.Concat(upperBorder.Where(p => !lowerBorder.Contains(p))))
                {
                    yield return point;
                }
                yield break;
            }

            foreach (int x in boundingRect.xRange)
            {
                for (int y = lowerBorder.MinY(x); y <= upperBorder.MaxY(x); y++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }

        /// <summary>
        /// Whether the two <see cref="IsometricRectangle"/>s have the same values for <see cref="startCorner"/>, <see cref="endCorner"/> and <see cref="filled"/>.
        /// </summary>
        public static bool operator ==(IsometricRectangle a, IsometricRectangle b) => a.startCorner == b.startCorner && a.endCorner == b.endCorner && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(IsometricRectangle, IsometricRectangle)"/>.
        /// </summary>
        public static bool operator !=(IsometricRectangle a, IsometricRectangle b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(IsometricRectangle, IsometricRectangle)"/>.
        /// </summary>
        public bool Equals(IsometricRectangle other) => this == other;
        /// <summary>
        /// See <see cref="Equals(IsometricRectangle)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is IsometricRectangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(startCorner, endCorner, filled);

        public override string ToString() => $"{nameof(IsometricRectangle)}({startCorner}, {endCorner} , {(filled ? "filled" : "unfilled")})";

        public IsometricRectangle DeepCopy() => new IsometricRectangle(startCorner, endCorner, filled);
    }
}