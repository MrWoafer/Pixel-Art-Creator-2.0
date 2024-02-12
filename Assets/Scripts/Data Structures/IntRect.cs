using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct IntRect
{
    private IntVector2 _bottomLeft;
    public IntVector2 bottomLeft
    {
        get => _bottomLeft;
        set
        {
            _bottomLeft = value;
            RecalculatePoints();
        }
    }
    private IntVector2 _topRight;
    public IntVector2 topRight
    {
        get => _topRight;
        set
        {
            _topRight = value;
            RecalculatePoints();
        }
    }
    public IntVector2 bottomRight
    {
        get => new IntVector2(topRight.x, bottomLeft.y);
        set
        {
            _bottomLeft = new IntVector2(bottomLeft.x, value.y);
            _topRight = new IntVector2(value.x, topRight.y);
            RecalculatePoints();
        }
    }
    public IntVector2 topLeft
    {
        get => new IntVector2(bottomLeft.x, topRight.y);
        set
        {
            _bottomLeft = new IntVector2(value.x, bottomLeft.y);
            _topRight = new IntVector2(topRight.x, value.y);
            RecalculatePoints();
        }
    }
    public Vector2 centre { get => (Vector2)(bottomLeft + topRight + IntVector2.one) / 2f; }

    /// <summary>The points in the rect, starting with the bottom row, read left to right, then the next row, etc.</summary>
    public IntVector2[] points { get; private set; }

    public int width { get => topRight.x - bottomLeft.x + 1; }
    public int height {  get => topRight.y - bottomLeft.y + 1; }

    public int area { get => Area(this); }

    public bool isSquare { get => IsSquare(this); }


    public IntRect(IntVector2 corner, IntVector2 oppositeCorner)
    {
        _bottomLeft = new IntVector2(Mathf.Min(corner.x, oppositeCorner.x), Mathf.Min(corner.y, oppositeCorner.y));
        _topRight = new IntVector2(Mathf.Max(corner.x, oppositeCorner.x), Mathf.Max(corner.y, oppositeCorner.y));

        points = new IntVector2[0];
        RecalculatePoints();
    }

    public static bool operator ==(IntRect rect1, IntRect rect2)
    {
        return rect1.bottomLeft == rect2.bottomLeft && rect1.topRight == rect2.topRight;
    }
    public static bool operator !=(IntRect a, IntRect b)
    {
        return !(a == b);
    }
    public override bool Equals(System.Object obj)
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

    public static IntRect operator +(IntVector2 intVector, IntRect intRect) => intRect + intVector;
    public static IntRect operator +(IntRect intRect, IntVector2 intVector)
    {
        return new IntRect(intRect.bottomLeft + intVector, intRect.topRight + intVector);
    }

    public static IntRect operator -(IntRect intRect, IntVector2 intVector) => intRect + (-intVector);

    public static explicit operator Rect(IntRect intRect) => intRect.ToRect();
    public Rect ToRect()
    {
        return new Rect(bottomLeft, new Vector2(width + 1, height + 1));
    }

    public static bool IsSquare(IntRect intRect)
    {
        return intRect.width == intRect.height;
    }

    public static int Area(IntRect rect)
    {
        return rect.width * rect.height;
    }

    public bool Contains(IntVector2 point) => Contains(point.x, point.y);
    public bool Contains(int x, int y)
    {
        return x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x && y <= topRight.y;
    }
    public bool Contains(Vector2 point) => Contains(point.x, point.y);
    public bool Contains(float x, float y)
    {
        return x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x + 1 && y <= topRight.y + 1;
    }

    public bool Contains(IntRect intRect)
    {
        return bottomLeft <= intRect.bottomLeft && topRight >= intRect.topRight;
    }
    public bool IsContainedIn(IntRect intRect)
    {
        return intRect.Contains(this);
    }

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
    public bool Overlaps(IntRect intRect)
    {
        return Overlap(this, intRect);
    }

    public IntVector2 Clamp(IntVector2 intVector)
    {
        return new IntVector2(Mathf.Clamp(intVector.x, bottomRight.x, topRight.x), Mathf.Clamp(intVector.y, bottomRight.y, topRight.y));
    }
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

    private void RecalculatePoints()
    {
        points = new IntVector2[area];

        for (int i = 0; i < area; i++)
        {
            points[i] = bottomLeft + new IntVector2(i % width, i / width);
        }
    }
}
