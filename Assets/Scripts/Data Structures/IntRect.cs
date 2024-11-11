using PAC.DataStructures;
using System.Collections;
using System.Linq;
using UnityEngine;
using PAC;
using System.Collections.Generic;

namespace PAC.DataStructures
{
    /// <summary>
    /// A struct to represent a rectangular region of integer coordinates.
    /// </summary>
    public struct IntRect : IEnumerable<IntVector2>
    {
        private IntVector2 _bottomLeft;
        public IntVector2 bottomLeft
        {
            get => _bottomLeft;
            set => _bottomLeft = value;
        }
        private IntVector2 _topRight;
        public IntVector2 topRight
        {
            get => _topRight;
            set => _topRight = value;
        }
        public IntVector2 bottomRight
        {
            get => new IntVector2(topRight.x, bottomLeft.y);
            set
            {
                _bottomLeft = new IntVector2(bottomLeft.x, value.y);
                _topRight = new IntVector2(value.x, topRight.y);
            }
        }
        public IntVector2 topLeft
        {
            get => new IntVector2(bottomLeft.x, topRight.y);
            set
            {
                _bottomLeft = new IntVector2(value.x, bottomLeft.y);
                _topRight = new IntVector2(topRight.x, value.y);
            }
        }
        public Vector2 centre => (Vector2)(bottomLeft + topRight + IntVector2.one) / 2f;

        /// <summary>
        /// <para>Gets the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </para>
        /// <para>
        /// WARNING: this is very expensive for large rects.
        /// </para>
        /// </summary>
        public IntVector2[] points => GetPoints();

        public int width => topRight.x - bottomLeft.x + 1;
        public int height => topRight.y - bottomLeft.y + 1;

        public int area => Area(this);

        /// <summary>True if the rect is a square.</summary>
        public bool isSquare => IsSquare(this);


        public IntRect(IntVector2 corner, IntVector2 oppositeCorner)
        {
            _bottomLeft = new IntVector2(Mathf.Min(corner.x, oppositeCorner.x), Mathf.Min(corner.y, oppositeCorner.y));
            _topRight = new IntVector2(Mathf.Max(corner.x, oppositeCorner.x), Mathf.Max(corner.y, oppositeCorner.y));
        }

        public static bool operator !=(IntRect rect1, IntRect rect2) => !(rect1 == rect2);
        public static bool operator ==(IntRect rect1, IntRect rect2)
        {
            return rect1.bottomLeft == rect2.bottomLeft && rect1.topRight == rect2.topRight;
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
        /// Cast to Unity Rect.
        /// </summary>
        public static explicit operator Rect(IntRect intRect) => intRect.ToRect();
        /// <summary>
        /// Cast to Unity Rect.
        /// </summary>
        public Rect ToRect()
        {
            return new Rect(bottomLeft, new Vector2(width + 1, height + 1));
        }

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
        public static bool Overlap(IntRect rect1, IntRect rect2)
        {
            bool xOverlaps = (rect1.bottomLeft.x >= rect2.bottomLeft.x && rect1.bottomLeft.x <= rect2.topRight.x) ||
                             (rect1.topRight.x >= rect2.bottomLeft.x && rect1.topRight.x <= rect2.topRight.x) ||
                             (rect1.bottomLeft.x <= rect2.bottomLeft.x && rect1.topRight.x >= rect2.topRight.x);

            bool yOverlaps = (rect1.bottomLeft.y >= rect2.bottomLeft.y && rect1.bottomLeft.y <= rect2.topRight.y) ||
                             (rect1.topRight.y >= rect2.bottomLeft.y && rect1.topRight.y <= rect2.topRight.y) ||
                             (rect1.bottomLeft.y <= rect2.bottomLeft.y && rect1.topRight.y >= rect2.topRight.y);

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
            return new IntVector2(Mathf.Clamp(intVector.x, bottomRight.x, topRight.x), Mathf.Clamp(intVector.y, bottomRight.y, topRight.y));
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
        /// <para>Gets the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </para>
        /// <para>
        /// WARNING: this is very expensive for large rects.
        /// </para>
        /// </summary>
        public IntVector2[] GetPoints()
        {
            IntVector2[] points = new IntVector2[area];

            int index = 0;
            foreach (IntVector2 point in this)
            {
                points[index] = point;
                index++;
            }

            return points;
        }

        /// <summary>
        /// Enumerates the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int i = 0; i < area; i++)
            {
                yield return bottomLeft + new IntVector2(i % width, i / height);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
