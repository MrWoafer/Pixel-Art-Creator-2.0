using PAC.DataStructures;
using System.Collections;
using System.Linq;
using UnityEngine;
using PAC;
using System.Collections.Generic;
using System;

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
            set
            {
                _bottomLeft = new IntVector2(Math.Min(value.x, _topRight.x), Math.Min(value.y, _topRight.y));
                _topRight = new IntVector2(Math.Max(value.x, _topRight.x), Math.Max(value.y, _topRight.y));
            }
        }
        private IntVector2 _topRight;
        public IntVector2 topRight
        {
            get => _topRight;
            set
            {
                _topRight = new IntVector2(Math.Max(_bottomLeft.x, value.x), Math.Max(_bottomLeft.y, value.y));
                _bottomLeft = new IntVector2(Math.Min(_bottomLeft.x, value.x), Math.Min(_bottomLeft.y, value.y));
            }
        }
        public IntVector2 bottomRight
        {
            get => new IntVector2(topRight.x, bottomLeft.y);
            set
            {
                IntVector2 oldBottomLeft = _bottomLeft;
                IntVector2 oldTopRight = _topRight;
                _bottomLeft = new IntVector2(Math.Min(value.x, oldBottomLeft.x), Math.Min(value.y, oldTopRight.y));
                _topRight = new IntVector2(Math.Max(value.x, oldBottomLeft.x), Math.Max(value.y, oldTopRight.y));
            }
        }
        public IntVector2 topLeft
        {
            get => new IntVector2(bottomLeft.x, topRight.y);
            set
            {
                IntVector2 oldBottomLeft = _bottomLeft;
                IntVector2 oldTopRight = _topRight;
                _bottomLeft = new IntVector2(Math.Min(value.x, oldTopRight.x), Math.Min(value.y, oldBottomLeft.y));
                _topRight = new IntVector2(Math.Max(value.x, oldTopRight.x), Math.Max(value.y, oldBottomLeft.y));
            }
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

        public static bool operator !=(IntRect a, IntRect b) => !(a == b);
        public static bool operator ==(IntRect a, IntRect b)
        {
            return a.bottomLeft == b.bottomLeft && a.topRight == b.topRight;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                IntRect rect = (IntRect)obj;
                return this == rect;
            }
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(bottomLeft, topRight);
        }

        public override string ToString()
        {
            return "(" + bottomLeft.ToString() + ", " + topRight.ToString() + ")";
        }

        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntVector2 intVector, IntRect intRect) => intRect + intVector;
        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntRect intRect, IntVector2 intVector)
        {
            return new IntRect(intRect.bottomLeft + intVector, intRect.topRight + intVector);
        }

        /// <summary>
        /// Shifts the whole rect by the given vector.
        /// </summary>
        public static IntRect operator -(IntRect intRect, IntVector2 intVector) => intRect + (-intVector);

        /// <summary>
        /// Cast to Unity Rect. Doesn't just convert each coordinate to a float. It expands the rect so it contains all the pixels described by the IntRect.
        /// </summary>
        public static implicit operator Rect(IntRect intRect) => new Rect(intRect.bottomLeft, new Vector2(intRect.width, intRect.height));
        // I haven't yet added a cast from Rect since it's more complicated than just 'new IntRect(new IntVector2(rect.xMin, rect.yMin), new IntVector2(rect.xMax, rect.yMax))'.
        // Doing that would mean casting to Rect and from Rect aren't inverses. To make that the case I also need to decide how Rects that don't lie on integer coords get rounded.
        // It would also mean that the way the top-right corners gets rounded is different from the bottom-left.

        /// <summary>
        /// Returns true if the rect is a square.
        /// </summary>
        public static bool IsSquare(IntRect intRect)
        {
            return intRect.width == intRect.height;
        }

        public static int Area(IntRect rect)
        {
            return rect.width * rect.height;
        }

        /// <summary>
        /// Returns true if the point is in the rect.
        /// </summary>
        public bool Contains(IntVector2 point) => Contains(point.x, point.y);
        /// <summary>
        /// Returns true if the point is in the rect.
        /// </summary>
        public bool Contains(int x, int y)
        {
            return x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x && y <= topRight.y;
        }
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
        public bool Contains(float x, float y)
        {
            return x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x + 1 && y <= topRight.y + 1;
        }

        /// <summary>
        /// Returns true if the given rect is (weakly) contained in this rect.
        /// </summary>
        public bool Contains(IntRect intRect)
        {
            return bottomLeft <= intRect.bottomLeft && topRight >= intRect.topRight;
        }
        /// <summary>
        /// Returns true if this rect is (weakly) contained in the given rect.
        /// </summary>
        public bool IsContainedIn(IntRect intRect)
        {
            return intRect.Contains(this);
        }

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
        public bool Overlaps(IntRect intRect)
        {
            return Overlap(this, intRect);
        }

        /// <summary>
        /// Clamps the vector component-wise so its coordinates are within the rect.
        /// </summary>
        public IntVector2 Clamp(IntVector2 intVector)
        {
            return new IntVector2(Math.Clamp(intVector.x, bottomRight.x, topRight.x), Math.Clamp(intVector.y, bottomRight.y, topRight.y));
        }
        /// <summary>
        /// Shifts the given rect so it is (weakly) contained within the rect.
        /// </summary>
        public IntRect Clamp(IntRect intRect)
        {
            if (intRect.width > width)
            {
                throw new System.Exception("An IntRect cannot clamp a wider IntRect. Width: " + width + " > " + intRect.width);
            }
            if (intRect.height > height)
            {
                throw new System.Exception("An IntRect cannot clamp a taller IntRect. Height: " + height + " > " + intRect.height);
            }

            if (intRect.bottomLeft.x < bottomLeft.x)
            {
                intRect += new IntVector2(bottomLeft.x - intRect.bottomLeft.x, 0);
            }
            else if (intRect.topRight.x > topRight.x)
            {
                intRect -= new IntVector2(intRect.topRight.x - topRight.x, 0);
            }
            if (intRect.bottomLeft.y < bottomLeft.y)
            {
                intRect += new IntVector2(0, bottomLeft.y - intRect.bottomLeft.y);
            }
            else if (intRect.topRight.y > topRight.y)
            {
                intRect -= new IntVector2(0, intRect.topRight.y - topRight.y);
            }
            return intRect;
        }

        /// <summary>
        /// Removes all IntVector2s outside the rect.
        /// </summary>
        public IntVector2[] FilterPointsInside(IntVector2[] intVectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return intVectors.Where(x => rect.Contains(x)).ToArray();
        }
        /// <summary>
        /// Removes all IntVector2s inside the rect.
        /// </summary>
        public IntVector2[] FilterPointsOutside(IntVector2[] intVectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return intVectors.Where(x => !rect.Contains(x)).ToArray();
        }

        /// <summary>
        /// Returns the IntRect rotated clockwise by the given angle.
        /// </summary>
        public IntRect Rotate(RotationAngle angle)
        {
            return new IntRect(bottomLeft.Rotate(angle), topRight.Rotate(angle));
        }

        /// <summary>
        /// Returns the IntRect flipped across the given axis.
        /// </summary>
        public IntRect Flip(FlipAxis axis)
        {
            return new IntRect(bottomLeft.Flip(axis), topRight.Flip(axis));
        }

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
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
