using System.Collections;
using System.Linq;
using UnityEngine;
using PAC;
using System.Collections.Generic;
using System;
using PAC.Extensions;

namespace PAC.DataStructures
{
    /// <summary>
    /// A struct to represent a rectangular region of integer coordinates.
    /// </summary>
    public struct IntRect : IReadOnlyCollection<IntVector2>
    {
        private IntVector2 _bottomLeft;
        public IntVector2 bottomLeft
        {
            get => _bottomLeft;
            set => this = new IntRect(value, topRight);
        }
        private IntVector2 _topRight;
        public IntVector2 topRight
        {
            get => _topRight;
            set => this = new IntRect(value, bottomLeft);
        }
        public IntVector2 bottomRight
        {
            get => new IntVector2(topRight.x, bottomLeft.y);
            set => this = new IntRect(value, topLeft);
        }
        public IntVector2 topLeft
        {
            get => new IntVector2(bottomLeft.x, topRight.y);
            set => this = new IntRect(value, bottomRight);
        }
        public Vector2 centre => (Vector2)(bottomLeft + topRight + IntVector2.one) / 2f;

        public int width => topRight.x - bottomLeft.x + 1;
        public int height => topRight.y - bottomLeft.y + 1;

        public int Count => width * height;
        public int area => Area(this);

        /// <summary>True if the rect is a square.</summary>
        public bool isSquare => IsSquare(this);


        public IntRect(IntVector2 corner, IntVector2 oppositeCorner)
        {
            _bottomLeft = new IntVector2(Math.Min(corner.x, oppositeCorner.x), Math.Min(corner.y, oppositeCorner.y));
            _topRight = new IntVector2(Math.Max(corner.x, oppositeCorner.x), Math.Max(corner.y, oppositeCorner.y));
        }

        public static bool operator ==(IntRect a, IntRect b) => a.bottomLeft == b.bottomLeft && a.topRight == b.topRight;
        public static bool operator !=(IntRect a, IntRect b) => !(a == b);
        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return this == (IntRect)obj;
            }
        }

        public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight);

        public override string ToString() => "(" + bottomLeft + ", " + topRight + ")";

        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntVector2 vector, IntRect rect) => rect + vector;
        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntRect rect, IntVector2 vector) => new IntRect(rect.bottomLeft + vector, rect.topRight + vector);

        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator -(IntRect rect, IntVector2 vector) => rect + (-vector);

        /// <summary>
        /// Cast to Unity Rect. Doesn't just convert each coordinate to a float. It expands the rect so it contains all the pixels described by the IntRect.
        /// </summary>
        public static implicit operator Rect(IntRect rect) => new Rect(rect.bottomLeft, new Vector2(rect.width, rect.height));
        // I haven't yet added a cast from Rect since it's more complicated than just 'new IntRect(new IntVector2(rect.xMin, rect.yMin), new IntVector2(rect.xMax, rect.yMax))'.
        // Doing that would mean casting to Rect and from Rect aren't inverses. To make that the case I also need to decide how Rects that don't lie on integer coords get rounded.
        // It would also mean that the way the top-right corners gets rounded is different from the bottom-left.

        /// <summary>
        /// Returns true if the rect is a square.
        /// </summary>
        public static bool IsSquare(IntRect rect) => rect.width == rect.height;

        public static int Area(IntRect rect) => rect.width * rect.height;

        /// <summary>
        /// Returns true if the point is in the rect.
        /// </summary>
        public bool Contains(IntVector2 point) => Contains(point.x, point.y);
        /// <summary>
        /// Returns true if the point is in the rect.
        /// </summary>
        public bool Contains(int x, int y) => x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x && y <= topRight.y;
        /// <summary>
        /// Returns true if the point is in the rect.
        /// Behaves differently than the overload when point is an IntVector2. This overload considers an IntVector2 comprising the rect as taking up the whole 1x1 square.
        /// e.g. a rect comprised of just the point (1, 1) is treated in this overload as the square [0,1]x[0,1], where [0,1] = {x : 0 <= x <= 1}.
        /// </summary>
        public bool Contains(Vector2 point) => Contains(point.x, point.y);
        /// <summary>
        /// Returns true if the point is in the rect.
        /// Behaves differently than the overload when point is an IntVector2. This overload considers an IntVector2 comprising the rect as taking up the whole 1x1 square.
        /// e.g. a rect comprised of just the point (1, 1) is treated in this overload as the square [0,1]x[0,1], where [0,1] = {x : 0 <= x <= 1}.
        /// </summary>
        public bool Contains(float x, float y) => x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x + 1 && y <= topRight.y + 1;

        /// <summary>
        /// Returns true if the given rect is (weakly) contained in this rect.
        /// </summary>
        public bool Contains(IntRect rect) => bottomLeft <= rect.bottomLeft && topRight >= rect.topRight;
        /// <summary>
        /// Returns true if this rect is (weakly) contained in the given rect.
        /// </summary>
        public bool IsContainedIn(IntRect rect) => rect.Contains(this);

        /// <summary>
        /// Returns true if the two rects overlap at all.
        /// </summary>
        public static bool Overlap(IntRect a, IntRect b)
        {
            bool xOverlaps = (a.bottomLeft.x >= b.bottomLeft.x && a.bottomLeft.x <= b.topRight.x) ||
                             (a.topRight.x >= b.bottomLeft.x && a.topRight.x <= b.topRight.x) ||
                             (a.bottomLeft.x <= b.bottomLeft.x && a.topRight.x >= b.topRight.x);

            bool yOverlaps = (a.bottomLeft.y >= b.bottomLeft.y && a.bottomLeft.y <= b.topRight.y) ||
                             (a.topRight.y >= b.bottomLeft.y && a.topRight.y <= b.topRight.y) ||
                             (a.bottomLeft.y <= b.bottomLeft.y && a.topRight.y >= b.topRight.y);

            return xOverlaps && yOverlaps;
        }
        /// <summary>
        /// Returns true if this rect overlaps the given rect at all.
        /// </summary>
        public bool Overlaps(IntRect rect) => Overlap(this, rect);

        /// <summary>
        /// Clamps the vector component-wise so its coordinates are within the rect.
        /// </summary>
        public IntVector2 Clamp(IntVector2 vector) => new IntVector2(Math.Clamp(vector.x, bottomRight.x, topRight.x), Math.Clamp(vector.y, bottomRight.y, topRight.y));
        /// <summary>
        /// Shifts the given rect so it is (weakly) contained within the rect.
        /// </summary>
        public IntRect Clamp(IntRect rect)
        {
            if (rect.width > width)
            {
                throw new ArgumentException("An IntRect cannot clamp a wider IntRect. Width: " + width + " > " + rect.width, "rect");
            }
            if (rect.height > height)
            {
                throw new ArgumentException("An IntRect cannot clamp a taller IntRect. Height: " + height + " > " + rect.height, "rect");
            }

            if (rect.bottomLeft.x < bottomLeft.x)
            {
                rect += new IntVector2(bottomLeft.x - rect.bottomLeft.x, 0);
            }
            else if (rect.topRight.x > topRight.x)
            {
                rect -= new IntVector2(rect.topRight.x - topRight.x, 0);
            }
            if (rect.bottomLeft.y < bottomLeft.y)
            {
                rect += new IntVector2(0, bottomLeft.y - rect.bottomLeft.y);
            }
            else if (rect.topRight.y > topRight.y)
            {
                rect -= new IntVector2(0, rect.topRight.y - topRight.y);
            }
            return rect;
        }

        /// <summary>
        /// Removes all IntVector2s outside the rect.
        /// </summary>
        public IntVector2[] FilterPointsInside(IntVector2[] vectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return vectors.Where(x => rect.Contains(x)).ToArray();
        }
        /// <summary>
        /// Removes all IntVector2s inside the rect.
        /// </summary>
        public IntVector2[] FilterPointsOutside(IntVector2[] vectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return vectors.Where(x => !rect.Contains(x)).ToArray();
        }

        /// <summary>
        /// Gets the smallest IntRect containing all the given IntVector2s.
        /// </summary>
        public static IntRect BoundingRect(params IntVector2[] vectors) => BoundingRect((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Gets the smallest IntRect containing all the given IntVector2s.
        /// </summary>
        public static IntRect BoundingRect(IEnumerable<IntVector2> vectors) => new IntRect(IntVector2.Min(vectors), IntVector2.Max(vectors));
        /// <summary>
        /// Gets the smallest IntRect containing both the given IntRects.
        /// </summary>
        public static IntRect BoundingRect(IntRect a, IntRect b) => new IntRect(IntVector2.Min(a.bottomLeft, b.bottomLeft), IntVector2.Max(a.topRight, b.topRight));
        /// <summary>
        /// Gets the smallest IntRect containing all the given IntRects.
        /// </summary>
        public static IntRect BoundingRect(params IntRect[] rects) => BoundingRect((IEnumerable<IntRect>)rects);
        /// <summary>
        /// Gets the smallest IntRect containing all the given IntRects.
        /// </summary>
        public static IntRect BoundingRect(IEnumerable<IntRect> rects)
        {
            if (rects.IsEmpty())
            {
                throw new ArgumentException("Cannot perform GetBoundingRect() on an empty collection of IntRects.", "rects");
            }
            return new IntRect(IntVector2.Min(rects.Select(rect => rect.bottomLeft)), IntVector2.Max(rects.Select(rect => rect.topRight)));
        }

        /// <summary>
        /// Returns the IntRect rotated clockwise by the given angle.
        /// </summary>
        public IntRect Rotate(RotationAngle angle) => new IntRect(bottomLeft.Rotate(angle), topRight.Rotate(angle));
        /// <summary>
        /// Returns the IntRect flipped across the given axis.
        /// </summary>
        public IntRect Flip(FlipAxis axis) => new IntRect(bottomLeft.Flip(axis), topRight.Flip(axis));

        /// <summary>
        /// Generates a uniformly random point within the rect.
        /// </summary>
        public IntVector2 RandomPoint() => new IntVector2(UnityEngine.Random.Range(bottomLeft.x, topRight.x + 1), UnityEngine.Random.Range(bottomLeft.y, topRight.y + 1));

        /// <summary>
        /// Indexes the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        public IntVector2 this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException("Index cannot be negative. Index: " + index);
                }
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException("Index cannot be more than or equal to the number of pixels in the IntRect. Index: " + index + ". Count: " + Count);
                }

                return bottomLeft + new IntVector2(index % width, index / width);
            }
        }

        /// <summary>
        /// Indexes the points in the rect by coordinates relative to the bottom-left (so indexing with x = 0, y = 0 would give the bottom-left of the IntRect).
        /// </summary>
        public IntVector2 this[int x, int y]
        {
            get
            {
                if (x < 0)
                {
                    throw new IndexOutOfRangeException("Relative coords cannot be negative. x: " + x);
                }
                if (y < 0)
                {
                    throw new IndexOutOfRangeException("Relative coords cannot be negative. y: " + y);
                }
                if (x >= width)
                {
                    throw new IndexOutOfRangeException("Relative x coord cannot be >= width. x: " + x + "). Width: " + width);
                }
                if (y >= height)
                {
                    throw new IndexOutOfRangeException("Relative y coord cannot be >= height. y: " + y + "). Height: " + height);
                }

                return bottomLeft + new IntVector2(x, y);
            }
        }

        /// <summary>
        /// Enumerates the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Enumerates the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int y = bottomLeft.y; y <= topRight.y; y++)
            {
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }
    }
}
