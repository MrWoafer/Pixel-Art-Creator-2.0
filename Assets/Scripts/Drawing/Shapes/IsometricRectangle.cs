using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Extensions;

using UnityEngine;

namespace PAC.Drawing
{
    public class IsometricRectangle : IIsometricShape, IEquatable<IsometricRectangle>
    {
        private IntVector2 startCorner;
        private IntVector2 endCorner;

        private IntVector2[] inferredCorners;

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

        public Path border => Path.Concat(lowerBorder, upperBorder);
        /// <summary>
        /// The lower edges of the border, i.e. (leftCorner, bottomCorner) and (rightCorner, bottomCorner).
        /// </summary>
        public Path lowerBorder { get; private set; }
        /// <summary>
        /// The upper edges of the border, i.e. (leftCorner, topCorner) and (rightCorner, topCorner).
        /// </summary>
        public Path upperBorder { get; private set; }

        public bool filled { get; set; }

        /// <summary>True if the isometric rectangle is an isometric square.</summary>
        public bool isIsometricSquare => lowerBorder.start.y == lowerBorder.end.y;

        public IntRect boundingRect => new IntRect(new IntVector2(leftCorner.x, bottomCorner.y), new IntVector2(rightCorner.x, topCorner.y));

        public int Count
        {
            get
            {
                if (filled)
                {
                    Path border = this.border;
                    int count = 0;
                    for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
                    {
                        count += border.MaxX(y) - border.MinX(y) + 1;
                    }
                    return count;
                }
                else
                {
                    // This computation uses the fact that each column of x coords contains exactly 1 or 2 points on the isometric rectangle's border
                    Path border = this.border;
                    int count = 0;
                    for (int x = boundingRect.bottomLeft.x; x <= boundingRect.topRight.x; x++)
                    {
                        count += (border.MinY(x) == border.MaxY(x)) ? 1 : 2;
                    }
                    return count;
                }
            }
        }

        public IsometricRectangle(IntVector2 startCorner, IntVector2 endCorner, bool filled)
        {
            this.startCorner = startCorner;
            this.endCorner = endCorner;
            this.filled = filled;

            InferCornersAndBorder();
        }

        /// <summary>
        /// Using just the start and end corners, deduces the other two corners and the shape of the border.
        /// </summary>
        private void InferCornersAndBorder()
        {
            // Specific cases

            if (startCorner == endCorner)
            {
                inferredCorners = new IntVector2[] { startCorner };
                lowerBorder = upperBorder = new Path(startCorner);
                return;
            }
            if (Math.Abs(startCorner.y - endCorner.y) == 1)
            {
                if (startCorner.x == endCorner.x)
                {
                    IntRect rect = new IntRect(startCorner, endCorner);
                    IntVector2 leftCorner = rect.bottomLeft + IntVector2.left;
                    IntVector2 rightCorner = rect.topRight + IntVector2.right;
                    inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                    lowerBorder = new Path(leftCorner, rightCorner + IntVector2.down);
                    upperBorder = new Path(rightCorner, leftCorner + IntVector2.up);
                    return;
                }
                else if (Math.Abs(startCorner.x - endCorner.x) == 1)
                {
                    IntRect rect = new IntRect(startCorner, endCorner);
                    IntVector2 leftCorner = rect.bottomLeft;
                    IntVector2 rightCorner = rect.topRight;
                    inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                    lowerBorder = new Path(leftCorner, rightCorner + IntVector2.down);
                    upperBorder = new Path(rightCorner, leftCorner + IntVector2.up);
                    return;
                }
            }

            // General cases

            IntVector2 sign = IntVector2.Sign(endCorner - startCorner);
            IntVector2 diff = endCorner - startCorner;
            IntVector2 absDiff = IntVector2.Abs(diff);
            IntVector2 adjustedEndCorner = endCorner + ((absDiff.x + 1) % 2) * new IntVector2(sign.x, 0);
            int region = Math.Abs(adjustedEndCorner.x - startCorner.x) + 1 - 2 * (Math.Abs(adjustedEndCorner.y - startCorner.y) + 1);
            // If startCorner and endCorner are on a line with gradient +/- 1/2
            if (region == 0)
            {
                inferredCorners = new IntVector2[] { startCorner, endCorner };
                if ((absDiff.x + 1) % 2 == 0)
                {
                    lowerBorder = new Path(startCorner, endCorner);
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
            if (region > 0)
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
            else
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
                            lowerBorder = new Path(startCorner, corner2);
                            upperBorder = new Path(endCorner, corner1);
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
                            lowerBorder = new Path(new Line(endCorner, corner1));
                            upperBorder = new Path(new Line(startCorner, corner2));
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
        }

        public bool Contains(IntVector2 pixel)
        {
            if (!filled)
            {
                return border.Contains(pixel);
            }
            return border.Contains(pixel) || border.WindingNumber(pixel) != 0;
        }

        /// <summary>
        /// Translates the isometric rectangle by the given vector.
        /// </summary>
        public static IsometricRectangle operator +(IsometricRectangle isoRectangle, IntVector2 translation) => isoRectangle.Translate(translation);
        /// <summary>
        /// Translates the isometric rectangle by the given vector.
        /// </summary>
        public static IsometricRectangle operator +(IntVector2 translation, IsometricRectangle isoRectangle) => isoRectangle + translation;
        /// <summary>
        /// Translates the isometric rectangle by the given vector.
        /// </summary>
        public static IsometricRectangle operator -(IsometricRectangle isoRectangle, IntVector2 translation) => isoRectangle + (-translation);
        /// <summary>
        /// Reflects the isometric rectangle through the origin.
        /// </summary>
        public static IsometricRectangle operator -(IsometricRectangle isoRectangle) => new IsometricRectangle(-isoRectangle.startCorner, -isoRectangle.endCorner, isoRectangle.filled);

        IIsometricShape IIsometricShape.Translate(IntVector2 translation) => Translate(translation);
        /// <summary>
        /// Translates the isometric rectangle by the given vector.
        /// </summary>
        public IsometricRectangle Translate(IntVector2 translation) => new IsometricRectangle(startCorner + translation, endCorner + translation, filled);

        IIsometricShape IIsometricShape.Flip(FlipAxis axis) => Flip(axis);
        /// <summary>
        /// Reflects the isometric rectangle across the given axis.
        /// </summary>
        public IsometricRectangle Flip(FlipAxis axis) => new IsometricRectangle(startCorner.Flip(axis), endCorner.Flip(axis), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (!filled)
            {
                foreach (IntVector2 pixel in lowerBorder.Concat(upperBorder.Where(p => !lowerBorder.Contains(p))))
                {
                    yield return pixel;
                }
                yield break;
            }

            Path border = this.border;
            for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
            {
                for (int x = border.MinX(y); x <= border.MaxX(y); x++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }

        public static bool operator ==(IsometricRectangle a, IsometricRectangle b) => a.startCorner == b.startCorner && a.endCorner == b.endCorner && a.filled == b.filled;
        public static bool operator !=(IsometricRectangle a, IsometricRectangle b) => !(a == b);
        public bool Equals(IsometricRectangle other) => this == other;
        public override bool Equals(object obj) => obj is IsometricRectangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(startCorner, endCorner, filled);

        public override string ToString() => "IsometricRectangle(" + startCorner + ", " + endCorner + ", " + (filled ? "filled" : "unfilled") + ")";

        IIsometricShape IIsometricShape.DeepCopy() => DeepCopy();
        public IsometricRectangle DeepCopy() => new IsometricRectangle(startCorner, endCorner, filled);
    }
}