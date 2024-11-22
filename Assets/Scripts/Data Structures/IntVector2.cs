using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PAC.Extensions;

namespace PAC.DataStructures
{
    /// <summary>
    /// A struct to represent a 2-dimensional vector with integer coordinates.
    /// </summary>
    public struct IntVector2
    {
        public int x;
        public int y;

        /// <summary>
        /// The magnitude of the vector, which is sqrt(x^2 + y^2).
        /// </summary>
        public float magnitude => Magnitude(this);
        /// <summary>
        /// The square of the magnitude of the vector, which is x^2 + y^2. Faster than squaring magnitude.
        /// </summary>
        public float sqrMagnitude => SqrMagnitude(this);

        /// <summary>
        /// The l1 norm of the vector, which is abs(x) + abs(y).
        /// Also known as the taxicab/Manhattan norm.
        /// </summary>
        public int l1Norm => L1Norm(this);
        /// <summary>
        /// The supremum norm of the vector, which is max(abs(x), abs(y)).
        /// Also known as the Chebyshev norm or l-infinity norm.
        /// </summary>
        public int supNorm => SupNorm(this);

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
        public static readonly IntVector2 zero = new IntVector2(0, 0);
        /// <summary>The vector (1, 1).</summary>
        public static readonly IntVector2 one = new IntVector2(1, 1);
        /// <summary>The vector (1, 0).</summary>
        public static readonly IntVector2 right = new IntVector2(1, 0);
        /// <summary>The vector (-1, 0).</summary>
        public static readonly IntVector2 left = new IntVector2(-1, 0);
        /// <summary>The vector (0, 1).</summary>
        public static readonly IntVector2 up = new IntVector2(0, 1);
        /// <summary>The vector (0, -1).</summary>
        public static readonly IntVector2 down = new IntVector2(0, -1);
        /// <summary>The vector (1, 1).</summary>
        public static readonly IntVector2 upRight = new IntVector2(1, 1);
        /// <summary>The vector (-1, 1).</summary>
        public static readonly IntVector2 upLeft = new IntVector2(-1, 1);
        /// <summary>The vector (1, -1).</summary>
        public static readonly IntVector2 downRight = new IntVector2(1, -1);
        /// <summary>The vector (-1, -1).</summary>
        public static readonly IntVector2 downLeft = new IntVector2(-1, -1);

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
                return this == (IntVector2)obj;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        /// <summary>
        /// Returns true iff &lt; holds for both components.
        /// </summary>
        public static bool operator <(IntVector2 a, IntVector2 b)
        {
            return a.x < b.x && a.y < b.y;
        }
        /// <summary>
        /// Returns true iff &gt; holds for both components.
        /// </summary>
        public static bool operator >(IntVector2 a, IntVector2 b)
        {
            return a.x > b.x && a.y > b.y;
        }
        /// <summary>
        /// Returns true iff &lt;= holds for both components.
        /// </summary>
        public static bool operator <=(IntVector2 a, IntVector2 b)
        {
            return a.x <= b.x && a.y <= b.y;
        }
        /// <summary>
        /// Returns true iff &gt;= holds for both components.
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
        /// Returns whether the this vector is an integer multiple of the divisor. For example, (3, 6) is a multiple of (1, 2) as (3, 6) = 3 * (1, 2).
        /// </summary>
        public bool IsMultipleOf(IntVector2 divisor) => divisor.Divides(this);
        /// <summary>
        /// Returns whether the dividend is an integer multiple of this vector. For example, (1, 2) divides (3, 6) as (3, 6) = 3 * (1, 2).
        /// </summary>
        public bool Divides(IntVector2 dividend)
        {
            // Cases where x or y are 0
            if (x == 0)
            {
                if (y == 0)
                {
                    return dividend == IntVector2.zero;
                }

                if (dividend.x != 0)
                {
                    return false;
                }
                return dividend.y % y == 0;
            }
            if (y == 0)
            {
                if (dividend.y != 0)
                {
                    return false;
                }
                return dividend.x % x == 0;
            }

            // Case where x, y both non-zero
            if (dividend.x % x != 0 || dividend.y % y != 0)
            {
                return false;
            }
            return dividend.x / x == dividend.y / y;
        }

        /// <summary>
        /// <para>
        /// Scales the vector down so its components are coprime (have no common divisors). In other words, it computes the smallest IntVector2 dividing a.
        /// </para>
        /// <para>
        /// Preserves signs.
        /// </para>
        /// </summary>
        public static IntVector2 Simplify(IntVector2 a)
        {
            if (a == IntVector2.zero)
            {
                return IntVector2.zero;
            }
            return a / Functions.Gcd(a.x, a.y);
        }

        /// <summary>
        /// Returns whether the two vectors lie on the same real line through (0, 0). This is equivalent to whether there is an IntVector2 dividing both of them.
        /// </summary>
        public static bool AreColinear(IntVector2 a, IntVector2 b)
        {
            // Proof that being colinear in the sense of real vectors is equivalent to there being an IntVector2 dividing both of them:
            //      The case where a = 0 or b = 0 is trivial, so assume a and b are non-zero. Without loss of generality, let a, b be in the top-right quadrant. Let c be the smallest positive
            //      integer point on the line. It is enough to the prove that c divides a (as then, by symmetry, c divides b; and by definition of Simplify() and minimality of c, c = Simplify(a),
            //      Simplify(b)). So suppose c did not divide a. Let d be the greatest multiple of c less than a (note c < a). Note d is on the line too, and hence so is a - d, which is an integer
            //      vector. But, by definition of d, 0 < a - d < c, contradicting minimality of c!

            // This is a rearrangement of a.y / a.x == b.y / b.x (comparing gradients) which avoids floats and division by 0.
            return a.y * b.x == b.y * a.x;
        }

        /// <summary>
        /// Cast to Unity Vector2.
        /// </summary>
        public static implicit operator Vector2(IntVector2 intVector) => new Vector2(intVector.x, intVector.y);
        /// <summary>
        /// Cast from Unity Vector2, by casting each coordinate to int.
        /// </summary>
        public static explicit operator IntVector2(Vector2 vector) => new IntVector2((int)vector.x, (int)vector.y);

        /// <summary>
        /// Cast to Unity Vector3, with a 0 in the z-coord.
        /// </summary>
        public static implicit operator Vector3(IntVector2 intVector) => new Vector3(intVector.x, intVector.y, 0f);
        /// <summary>
        /// Cast from Unity Vector3, by casting the x and y coordinates to int and ignoring the z coordinate.
        /// </summary>
        public static explicit operator IntVector2(Vector3 vector) => new IntVector2((int)vector.x, (int)vector.y);

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
        /// Determines whether the two vectors are perpendicular to each other.
        /// </summary>
        public static bool ArePerpendicular(IntVector2 a, IntVector2 b)
        {
            return Dot(a, b) == 0;
        }

        /// <summary>
        /// Computes the magnitude of the vector, which is sqrt(a.x^2 + a.y^2).
        /// </summary>
        public static float Magnitude(IntVector2 a)
        {
            return Mathf.Sqrt(a.x * a.x + a.y * a.y);
        }
        /// <summary>
        /// Computes the square of the magnitude of the vector, which is a.x^2 + a.y^2. Faster than using Magnitude() and squaring.
        /// </summary>
        public static float SqrMagnitude(IntVector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }
        /// <summary>
        /// Computes the Euclidean distance between the vectors, which is sqrt((a.x - b.x)^2 + (a.y - b.y)^2).
        /// </summary>
        public static float Distance(IntVector2 a, IntVector2 b)
        {
            return Magnitude(a - b);
        }
        /// <summary>
        /// Computes the square of the Euclidean distance between the vectors, which is (a.x - b.x)^2 + (a.y - b.y)^2. Faster than using Distance() and squaring.
        /// </summary>
        public static float SqrDistance(IntVector2 a, IntVector2 b)
        {
            return SqrMagnitude(a - b);
        }

        /// <summary>
        /// Computes the l1 norm of the vector, which is abs(a.x) + abs(a.y).
        /// Also known as the taxicab/Manhattan norm.
        /// </summary>
        public static int L1Norm(IntVector2 a)
        {
            return Math.Abs(a.x) + Math.Abs(a.y);
        }
        /// <summary>
        /// Computes the l1 distance of the vectors, which is abs(a.x - b.x) + abs(a.y - b.y).
        /// Also known as the taxicab/Manhattan distance or rectilinear distance.
        /// </summary>
        public static int L1Distance(IntVector2 a, IntVector2 b)
        {
            return L1Norm(a - b);
        }

        /// <summary>
        /// Computes the supremum norm of the vector, which is max(abs(a.x), abs(a.y)).
        /// Also known as the Chebyshev norm or l-infinity norm.
        /// </summary>
        public static int SupNorm(IntVector2 a)
        {
            return Math.Max(Math.Abs(a.x), Math.Abs(a.y));
        }
        /// <summary>
        /// Computes the supremum distance of the vectors, which is max(abs(a.x - b.y), abs(a.y - b.y)).
        /// Also known as the Chebyshev distance or l-infinity distance.
        /// </summary>
        public static int SupDistance(IntVector2 a, IntVector2 b)
        {
            return SupNorm(a - b);
        }

        /// <summary>
        /// Takes the absolute value component-wise.
        /// </summary>
        public static IntVector2 Abs(IntVector2 a)
        {
            return new IntVector2(Math.Abs(a.x), Math.Abs(a.y));
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
        public static IntVector2 Max(params IntVector2[] intVectors) => Max((IEnumerable<IntVector2>)intVectors);
        /// <summary>
        /// Takes the maximum of the vectors component-wise.
        /// </summary>
        public static IntVector2 Max(IEnumerable<IntVector2> intVectors)
        {
            if (intVectors.IsEmpty())
            {
                throw new ArgumentException("Cannot perform Max() on an empty collection of IntVector2s.", "intVectors");
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
        public static IntVector2 Min(params IntVector2[] intVectors) => Min((IEnumerable<IntVector2>)intVectors);
        /// <summary>
        /// Takes the minimum of the vectors component-wise.
        /// </summary>
        public static IntVector2 Min(IEnumerable<IntVector2> intVectors)
        {
            if (intVectors.IsEmpty())
            {
                throw new ArgumentException("Cannot perform Min() on an empty collection of IntVector2s.", "intVectors");
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
                default: throw new NotImplementedException("Unknown / unimplemented angle: " + angle);
            }
        }

        /// <summary>
        /// Returns the IntVector2 flipped across the given axis.
        /// </summary>
        public IntVector2 Flip(FlipAxis axis)
        {
            switch (axis)
            {
                case FlipAxis.None: return this;
                case FlipAxis.Vertical: return new IntVector2(-x, y);
                case FlipAxis.Horizontal: return new IntVector2(x, -y);
                case FlipAxis._45Degrees: return new IntVector2(y, x);
                case FlipAxis.Minus45Degrees: return new IntVector2(-y, -x);
                default: throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis);
            }
        }
    }
}
