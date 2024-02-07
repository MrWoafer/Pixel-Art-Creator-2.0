using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct IntVector2
{
    public int x;
    public int y;

    public float magnitude => Magnitude(this);
    public float squaredMagnitude => SquaredMagnitude(this);

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public IntVector2(Vector2 vector2)
    {
        x = (int)vector2.x;
        y = (int)vector2.y;
    }
    public IntVector2(IntVector2 intVector2) : this(intVector2.x, intVector2.y) { }

    /// <summary>The vector (0, 0).</summary>
    public static IntVector2 zero { get => new IntVector2(0, 0); }
    /// <summary>The vector (1, 1).</summary>
    public static IntVector2 one { get => new IntVector2(1, 1); }
    /// <summary>The vector (1, 0).</summary>
    public static IntVector2 right { get => new IntVector2(1, 0); }
    /// <summary>The vector (-1, 0).</summary>
    public static IntVector2 left { get => new IntVector2(-1, 0); }
    /// <summary>The vector (0, 1).</summary>
    public static IntVector2 up { get => new IntVector2(0, 1); }
    /// <summary>The vector (0, -1).</summary>
    public static IntVector2 down { get => new IntVector2(0, -1); }

    public static bool operator ==(IntVector2 a, IntVector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(IntVector2 a, IntVector2 b)
    {
        return !(a == b);
    }
    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            IntVector2 vector = (IntVector2)obj;
            return this == vector;
        }
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(x, y);
    }

    public override string ToString()
    {
        return "(" + x.ToString() + ", " + y.ToString() + ")";
    }

    public static bool operator <(IntVector2 a, IntVector2 b)
    {
        return a.x < b.x && a.y < b.y;
    }
    public static bool operator >(IntVector2 a, IntVector2 b)
    {
        return a.x > b.x && a.y > b.y;
    }
    public static bool operator <=(IntVector2 a, IntVector2 b)
    {
        return a.x <= b.x && a.y <= b.y;
    }
    public static bool operator >=(IntVector2 a, IntVector2 b)
    {
        return a.x >= b.x && a.y >= b.y;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x + b.x, a.y + b.y);
    }
    public static IntVector2[] operator +(IntVector2 intVector, IntVector2[] intVectorArray)
    {
        IntVector2[] result = new IntVector2[intVectorArray.Length];
        for (int i = 0; i < intVectorArray.Length; i++)
        {
            result[i] = intVector + intVectorArray[i];
        }
        return result;
    }
    public static IntVector2[] operator +(IntVector2[] intVectorArray, IntVector2 intVector)
    {
        return intVector + intVectorArray;
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x - b.x, a.y - b.y);
    }
    public static IntVector2 operator -(IntVector2 a)
    {
        return new IntVector2(-a.x, -a.y);
    }
    public static IntVector2[] operator -(IntVector2 intVector, IntVector2[] intVectorArray)
    {
        IntVector2[] result = new IntVector2[intVectorArray.Length];
        for (int i = 0; i < intVectorArray.Length; i++)
        {
            result[i] = intVector - intVectorArray[i];
        }
        return result;
    }
    public static IntVector2[] operator -(IntVector2[] intVectorArray, IntVector2 intVector)
    {
        return intVectorArray + (-intVector);
    }

    public static IntVector2 operator *(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x * b.x, a.y * b.y);
    }
    public static IntVector2 operator *(int scalar, IntVector2 vector)
    {
        return new IntVector2(vector.x * scalar, vector.y * scalar);
    }
    public static IntVector2 operator *(IntVector2 vector, int scalar)
    {
        return scalar * vector;
    }
    public static IntVector2[] operator *(IntVector2 intVector, IntVector2[] intVectorArray)
    {
        IntVector2[] result = new IntVector2[intVectorArray.Length];
        for (int i = 0; i < intVectorArray.Length; i++)
        {
            result[i] = intVector * intVectorArray[i];
        }
        return result;
    }
    public static IntVector2[] operator *(IntVector2[] intVectorArray, IntVector2 intVector)
    {
        return intVector * intVectorArray;
    }

    public static IntVector2 operator /(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.x / b.x, a.y / b.y);
    }
    public static IntVector2 operator /(IntVector2 vector, int scalar)
    {
        return new IntVector2(vector.x / scalar, vector.y / scalar);
    }
    public static IntVector2[] operator /(IntVector2 intVector, IntVector2[] intVectorArray)
    {
        IntVector2[] result = new IntVector2[intVectorArray.Length];
        for (int i = 0; i < intVectorArray.Length; i++)
        {
            result[i] = intVector / intVectorArray[i];
        }
        return result;
    }
    public static IntVector2[] operator /(IntVector2[] intVectorArray, IntVector2 intVector)
    {
        IntVector2[] result = new IntVector2[intVectorArray.Length];
        for (int i = 0; i < intVectorArray.Length; i++)
        {
            result[i] = intVectorArray[i] / intVector;
        }
        return result;
    }

    public static implicit operator Vector2(IntVector2 intVector) => intVector.ToVector2();
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public static implicit operator Vector3(IntVector2 intVector) => intVector.ToVector3();
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, 0f);
    }

    public static IntVector2 FloorToIntVector2(Vector2 vector2)
    {
        return new IntVector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
    }
    public static IntVector2 CeilToIntVector2(Vector2 vector2)
    {
        return new IntVector2(Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
    }
    public static IntVector2 RoundToIntVector2(Vector2 vector2)
    {
        return new IntVector2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
    }

    public static int Dot(IntVector2 a, IntVector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static float Distance(IntVector2 a, IntVector2 b)
    {
        return Magnitude(a - b);
    }
    public static float Magnitude(IntVector2 a)
    {
        return Mathf.Sqrt(a.x * a.x + a.y * a.y);
    }
    public static float SquaredMagnitude(IntVector2 a)
    {
        return a.x * a.x + a.y * a.y;
    }

    /// <summary>
    /// Takes the maximum of a and b component-wise.
    /// </summary>
    public static IntVector2 Max(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    
    }
    /// <summary>
    /// Takes the maximum of the vectors component-wise.
    /// </summary>
    public static IntVector2 Max(params IntVector2[] intVectors)
    {
        if (intVectors.Length == 0)
        {
            throw new System.Exception("Cannot perform Max() on an empty array of IntVectors.");
        }
        return new IntVector2(Enumerable.Max(from intVector in intVectors select intVector.x), Enumerable.Max(from intVector in intVectors select intVector.y));
    }
    /// <summary>
    /// Takes the minimum of a and b component-wise.
    /// </summary>
    public static IntVector2 Min(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    }
    /// <summary>
    /// Takes the minimum of the vectors component-wise.
    /// </summary>
    public static IntVector2 Min(params IntVector2[] intVectors)
    {
        if (intVectors.Length == 0)
        {
            throw new System.Exception("Cannot perform Min() on an empty array of IntVectors.");
        }
        return new IntVector2(Enumerable.Min(from intVector in intVectors select intVector.x), Enumerable.Min(from intVector in intVectors select intVector.y));
    }
}
