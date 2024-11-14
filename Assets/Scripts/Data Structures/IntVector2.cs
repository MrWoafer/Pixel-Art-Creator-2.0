using System;
using System.Linq;
using UnityEngine;

namespace PAC.DataStructures
{
    /// <summary>
    /// A struct to represent a 2-dimensional vector with integer coordinates.
    /// </summary>
    public struct IntVector2
    {
        public int x;
        public int y;

        public float magnitude => Magnitude(this);
        public float squaredMagnitude => SqrMagnitude(this);

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Rounds the coords towards zero.
        /// </summary>
        public IntVector2(float x, float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }
        /// <summary>
        /// Rounds the coords towards zero.
        /// </summary>
        public IntVector2(Vector2 vector2)
        {
            x = (int)vector2.x;
            y = (int)vector2.y;
        }

        /// <summary>The vector (0, 0).</summary>
        public static IntVector2 zero = new IntVector2(0, 0);
        /// <summary>The vector (1, 1).</summary>
        public static IntVector2 one = new IntVector2(1, 1);
        /// <summary>The vector (1, 0).</summary>
        public static IntVector2 right = new IntVector2(1, 0);
        /// <summary>The vector (-1, 0).</summary>
        public static IntVector2 left = new IntVector2(-1, 0);
        /// <summary>The vector (0, 1).</summary>
        public static IntVector2 up = new IntVector2(0, 1);
        /// <summary>The vector (0, -1).</summary>
        public static IntVector2 down = new IntVector2(0, -1);

        public static bool operator !=(IntVector2 a, IntVector2 b) => !(a == b);
        public static bool operator ==(IntVector2 a, IntVector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
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

        /// <summary>
        /// Compares component-wise.
        /// </summary>
        public static bool operator <(IntVector2 a, IntVector2 b)
        {
            return a.x < b.x && a.y < b.y;
        }
        /// <summary>
        /// Compares component-wise.
        /// </summary>
        public static bool operator >(IntVector2 a, IntVector2 b)
        {
            return a.x > b.x && a.y > b.y;
        }
        /// <summary>
        /// Compares component-wise.
        /// </summary>
        public static bool operator <=(IntVector2 a, IntVector2 b)
        {
            return a.x <= b.x && a.y <= b.y;
        }
        /// <summary>
        /// Compares component-wise.
        /// </summary>
        public static bool operator >=(IntVector2 a, IntVector2 b)
        {
            return a.x >= b.x && a.y >= b.y;
        }

        /// <summary>
        /// Adds component-wise.
        /// </summary>
        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }
        /// <summary>
        /// Adds the vector to each element of the array.
        /// </summary>
        public static IntVector2[] operator +(IntVector2 intVector, IntVector2[] intVectorArray)
        {
            IntVector2[] result = new IntVector2[intVectorArray.Length];
            for (int i = 0; i < intVectorArray.Length; i++)
            {
                result[i] = intVector + intVectorArray[i];
            }
            return result;
        }
        /// <summary>
        /// Adds the vector to each element of the array.
        /// </summary>
        public static IntVector2[] operator +(IntVector2[] intVectorArray, IntVector2 intVector)
        {
            return intVector + intVectorArray;
        }

        /// <summary>
        /// Subtracts component-wise.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }
        /// <summary>
        /// Negates component-wise.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a)
        {
            return new IntVector2(-a.x, -a.y);
        }
        /// <summary>
        /// Subtracts each element of the array from the vector.
        /// </summary>
        public static IntVector2[] operator -(IntVector2 intVector, IntVector2[] intVectorArray)
        {
            IntVector2[] result = new IntVector2[intVectorArray.Length];
            for (int i = 0; i < intVectorArray.Length; i++)
            {
                result[i] = intVector - intVectorArray[i];
            }
            return result;
        }
        /// <summary>
        /// Subtracts the vector from each element of the array.
        /// </summary>
        public static IntVector2[] operator -(IntVector2[] intVectorArray, IntVector2 intVector)
        {
            return intVectorArray + (-intVector);
        }

        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x * b.x, a.y * b.y);
        }
        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(int scalar, IntVector2 vector)
        {
            return new IntVector2(vector.x * scalar, vector.y * scalar);
        }
        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(IntVector2 vector, int scalar)
        {
            return scalar * vector;
        }
        /// <summary>
        /// Multiplies each element of the array by the vector.
        /// </summary>
        public static IntVector2[] operator *(IntVector2 intVector, IntVector2[] intVectorArray)
        {
            IntVector2[] result = new IntVector2[intVectorArray.Length];
            for (int i = 0; i < intVectorArray.Length; i++)
            {
                result[i] = intVector * intVectorArray[i];
            }
            return result;
        }
        /// <summary>
        /// Multiplies each element of the array by the vector.
        /// </summary>
        public static IntVector2[] operator *(IntVector2[] intVectorArray, IntVector2 intVector)
        {
            return intVector * intVectorArray;
        }

        /// <summary>
        /// Divides (integer division) component-wise.
        /// </summary>
        public static IntVector2 operator /(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x / b.x, a.y / b.y);
        }
        /// <summary>
        /// Divides (integer division) component-wise.
        /// </summary>
        public static IntVector2 operator /(IntVector2 vector, int scalar)
        {
            return new IntVector2(vector.x / scalar, vector.y / scalar);
        }
        /// <summary>
        /// Divides the vector by each element of the array.
        /// </summary>
        public static IntVector2[] operator /(IntVector2 intVector, IntVector2[] intVectorArray)
        {
            IntVector2[] result = new IntVector2[intVectorArray.Length];
            for (int i = 0; i < intVectorArray.Length; i++)
            {
                result[i] = intVector / intVectorArray[i];
            }
            return result;
        }
        /// <summary>
        /// Divides each element of the array by the vector.
        /// </summary>
        public static IntVector2[] operator /(IntVector2[] intVectorArray, IntVector2 intVector)
        {
            IntVector2[] result = new IntVector2[intVectorArray.Length];
            for (int i = 0; i < intVectorArray.Length; i++)
            {
                result[i] = intVectorArray[i] / intVector;
            }
            return result;
        }

        /// <summary>
        /// Cast to Unity Vector2.
        /// </summary>
        public static implicit operator Vector2(IntVector2 intVector) => new Vector2(intVector.x, intVector.y);

        /// <summary>
        /// Cast to Unity Vector3, with a 0 in the z-coord.
        /// </summary>
        public static implicit operator Vector3(IntVector2 intVector) => new Vector3(intVector.x, intVector.y, 0f);

        /// <summary>
        /// Floors component-wise.
        /// </summary>
        public static IntVector2 FloorToIntVector2(Vector2 vector2)
        {
            return new IntVector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
        }
        /// <summary>
        /// Ceils component-wise.
        /// </summary>
        public static IntVector2 CeilToIntVector2(Vector2 vector2)
        {
            return new IntVector2(Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
        }
        /// <summary>
        /// Rounds component-wise.
        /// </summary>
        public static IntVector2 RoundToIntVector2(Vector2 vector2)
        {
            return new IntVector2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
        }

        /// <summary>
        /// Computes the dot product.
        /// </summary>
        public static int Dot(IntVector2 a, IntVector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        /// <summary>
        /// Computes the Euclidean distance between the vectors.
        /// </summary>
        public static float Distance(IntVector2 a, IntVector2 b)
        {
            return Magnitude(a - b);
        }
        public static float SqrDistance(IntVector2 a, IntVector2 b)
        {
            return SqrMagnitude(a - b);
        }
        public static float Magnitude(IntVector2 a)
        {
            return Mathf.Sqrt(a.x * a.x + a.y * a.y);
        }
        public static float SqrMagnitude(IntVector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        /// <summary>
        /// Takes the maximum of a and b component-wise.
        /// </summary>
        public static IntVector2 Max(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    
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
            return new IntVector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
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

        /// <summary>
        /// Returns the IntVector2 rotated clockwise by the given angle.
        /// </summary>
        public IntVector2 Rotate(RotationAngle angle)
        {
            switch (angle)
            {
                case RotationAngle._0: return new IntVector2(x, y);
                case RotationAngle._90: return new IntVector2(y, -x);
                case RotationAngle._180: return new IntVector2(-x, -y);
                case RotationAngle.Minus90: return new IntVector2(-y, x);
                default: throw new Exception("Unknown / unimplemented angle: " + angle);
            }
        }
    }
}
