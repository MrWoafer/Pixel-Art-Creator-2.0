using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PAC.Extensions;

namespace PAC.DataStructures
{
    /// <summary>
    /// Represents a 2-dimensional vector with integer coordinates.
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

        /// <summary>
        /// The sign of each component of the vector.
        /// </summary>
        public IntVector2 sign => Sign(this);

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
        /// <summary>
        /// An IEnumerable over the vectors (0, 1), (0, -1), (-1, 0), (1, 0).
        /// </summary>
        public static readonly IEnumerable<IntVector2> upDownLeftRight = new IntVector2[] { up, down, left, right };

        public static bool operator ==(IntVector2 a, IntVector2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(IntVector2 a, IntVector2 b) => !(a == b);
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

        public override int GetHashCode() => HashCode.Combine(x, y);

        public override string ToString() => "(" + x + ", " + y + ")";

        /// <summary>
        /// Returns true iff &lt; holds for both components.
        /// </summary>
        public static bool operator <(IntVector2 a, IntVector2 b) => a.x < b.x && a.y < b.y;
        /// <summary>
        /// Returns true iff &gt; holds for both components.
        /// </summary>
        public static bool operator >(IntVector2 a, IntVector2 b) => a.x > b.x && a.y > b.y;
        /// <summary>
        /// Returns true iff &lt;= holds for both components.
        /// </summary>
        public static bool operator <=(IntVector2 a, IntVector2 b) => a.x <= b.x && a.y <= b.y;
        /// <summary>
        /// Returns true iff &gt;= holds for both components.
        /// </summary>
        public static bool operator >=(IntVector2 a, IntVector2 b) => a.x >= b.x && a.y >= b.y;

        /// <summary>
        /// Adds component-wise.
        /// </summary>
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.x + b.x, a.y + b.y);

        /// <summary>
        /// Negates component-wise.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a) => new IntVector2(-a.x, -a.y);
        /// <summary>
        /// Subtracts component-wise.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.x - b.x, a.y - b.y);

        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(IntVector2 a, IntVector2 b) => new IntVector2(a.x * b.x, a.y * b.y);
        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(int scalar, IntVector2 vector) => new IntVector2(scalar * vector.x, scalar * vector.y);
        /// <summary>
        /// Multiplies component-wise.
        /// </summary>
        public static IntVector2 operator *(IntVector2 vector, int scalar) => scalar * vector;

        /// <summary>
        /// Divides (integer division) component-wise.
        /// </summary>
        public static IntVector2 operator /(IntVector2 a, IntVector2 b) => new IntVector2(a.x / b.x, a.y / b.y);
        /// <summary>
        /// Divides (integer division) component-wise.
        /// </summary>
        public static IntVector2 operator /(IntVector2 vector, int scalar) => new IntVector2(vector.x / scalar, vector.y / scalar);

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
                    return dividend == zero;
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
            if (a == zero)
            {
                return zero;
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
        public static implicit operator Vector2(IntVector2 vector) => new Vector2(vector.x, vector.y);
        /// <summary>
        /// Cast from Unity Vector2, by casting each coordinate to int.
        /// </summary>
        public static explicit operator IntVector2(Vector2 vector) => new IntVector2((int)vector.x, (int)vector.y);

        /// <summary>
        /// Cast to Unity Vector3, with a 0 in the z-coord.
        /// </summary>
        public static implicit operator Vector3(IntVector2 vector) => new Vector3(vector.x, vector.y, 0f);
        /// <summary>
        /// Cast from Unity Vector3, by casting the x and y coordinates to int and ignoring the z coordinate.
        /// </summary>
        public static explicit operator IntVector2(Vector3 vector) => new IntVector2((int)vector.x, (int)vector.y);

        /// <summary>
        /// Floors component-wise.
        /// </summary>
        public static IntVector2 FloorToIntVector2(Vector2 vector2) => new IntVector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
        /// <summary>
        /// Ceils component-wise.
        /// </summary>
        public static IntVector2 CeilToIntVector2(Vector2 vector2) => new IntVector2(Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
        /// <summary>
        /// Rounds component-wise.
        /// </summary>
        public static IntVector2 RoundToIntVector2(Vector2 vector2) => new IntVector2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));

        /// <summary>
        /// Computes the dot product.
        /// </summary>
        public static int Dot(IntVector2 a, IntVector2 b) => a.x * b.x + a.y * b.y;
        /// <summary>
        /// Determines whether the two vectors are perpendicular to each other.
        /// </summary>
        public static bool ArePerpendicular(IntVector2 a, IntVector2 b) => Dot(a, b) == 0;

        /// <summary>
        /// Computes the magnitude of the vector, which is sqrt(a.x^2 + a.y^2).
        /// </summary>
        public static float Magnitude(IntVector2 a) => Mathf.Sqrt(a.x * a.x + a.y * a.y);
        /// <summary>
        /// Computes the square of the magnitude of the vector, which is a.x^2 + a.y^2. Faster than using Magnitude() and squaring.
        /// </summary>
        public static float SqrMagnitude(IntVector2 a) => a.x * a.x + a.y * a.y;
        /// <summary>
        /// Computes the Euclidean distance between the vectors, which is sqrt((a.x - b.x)^2 + (a.y - b.y)^2).
        /// </summary>
        public static float Distance(IntVector2 a, IntVector2 b) => Magnitude(a - b);
        /// <summary>
        /// Computes the square of the Euclidean distance between the vectors, which is (a.x - b.x)^2 + (a.y - b.y)^2. Faster than using Distance() and squaring.
        /// </summary>
        public static float SqrDistance(IntVector2 a, IntVector2 b) => SqrMagnitude(a - b);

        /// <summary>
        /// Computes the l1 norm of the vector, which is abs(a.x) + abs(a.y).
        /// Also known as the taxicab/Manhattan norm.
        /// </summary>
        public static int L1Norm(IntVector2 a) => Math.Abs(a.x) + Math.Abs(a.y);
        /// <summary>
        /// Computes the l1 distance of the vectors, which is abs(a.x - b.x) + abs(a.y - b.y).
        /// Also known as the taxicab/Manhattan distance or rectilinear distance.
        /// </summary>
        public static int L1Distance(IntVector2 a, IntVector2 b) => L1Norm(a - b);

        /// <summary>
        /// Computes the supremum norm of the vector, which is max(abs(a.x), abs(a.y)).
        /// Also known as the Chebyshev norm or l-infinity norm.
        /// </summary>
        public static int SupNorm(IntVector2 a) => Math.Max(Math.Abs(a.x), Math.Abs(a.y));
        /// <summary>
        /// Computes the supremum distance of the vectors, which is max(abs(a.x - b.y), abs(a.y - b.y)).
        /// Also known as the Chebyshev distance or l-infinity distance.
        /// </summary>
        public static int SupDistance(IntVector2 a, IntVector2 b) => SupNorm(a - b);

        /// <summary>
        /// Returns the sign of each component of the vector.
        /// </summary>
        public static IntVector2 Sign(IntVector2 a) => new IntVector2(Math.Sign(a.x), Math.Sign(a.y));
        /// <summary>
        /// Takes the absolute value component-wise.
        /// </summary>
        public static IntVector2 Abs(IntVector2 a) => new IntVector2(Math.Abs(a.x), Math.Abs(a.y));

        /// <summary>
        /// Takes the maximum of a and b component-wise.
        /// </summary>
        public static IntVector2 Max(IntVector2 a, IntVector2 b) => new IntVector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        /// <summary>
        /// Takes the maximum of the vectors component-wise.
        /// </summary>
        public static IntVector2 Max(params IntVector2[] vectors) => Max((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Takes the maximum of the vectors component-wise.
        /// </summary>
        // NOTE: This is not provided as an extension method to IEnumerable<IntVector2> since it seems C# will favour using the LINQ Enumerable.Max() instead, which causes errors.
        public static IntVector2 Max(IEnumerable<IntVector2> vectors)
        {
            if (vectors.IsEmpty())
            {
                throw new ArgumentException("Cannot perform Max() on an empty collection of IntVector2s.", "vectors");
            }
            return new IntVector2(vectors.Select(vector => vector.x).Max(), vectors.Select(vector => vector.y).Max());
        }

        /// <summary>
        /// Takes the minimum of a and b component-wise.
        /// </summary>
        public static IntVector2 Min(IntVector2 a, IntVector2 b) => new IntVector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        /// <summary>
        /// Takes the minimum of the vectors component-wise.
        /// </summary>
        public static IntVector2 Min(params IntVector2[] vectors) => Min((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Takes the minimum of the vectors component-wise.
        /// </summary>
        // NOTE: This is not provided as an extension method to IEnumerable<IntVector2> since it seems C# will favour using the LINQ Enumerable.Min() instead, which causes errors.
        public static IntVector2 Min(IEnumerable<IntVector2> vectors)
        {
            if (vectors.IsEmpty())
            {
                throw new ArgumentException("Cannot perform Min() on an empty collection of IntVector2s.", "vectors");
            }
            return new IntVector2(vectors.Select(vector => vector.x).Min(), vectors.Select(vector => vector.y).Min());
        }

        /// <summary>
        /// Returns the IntVector2 rotated clockwise by the given angle.
        /// </summary>
        public IntVector2 Rotate(RotationAngle angle) => angle switch
        {
            RotationAngle._0 => this,
            RotationAngle._90 => new IntVector2(y, -x),
            RotationAngle._180 => new IntVector2(-x, -y),
            RotationAngle.Minus90 => new IntVector2(-y, x),
            _ => throw new NotImplementedException("Unknown / unimplemented angle: " + angle)
        };
        /// <summary>
        /// Returns the IntVector2 flipped across the given axis.
        /// </summary>
        public IntVector2 Flip(FlipAxis axis) => axis switch
        {
            FlipAxis.None => this,
            FlipAxis.Vertical => new IntVector2(-x, y),
            FlipAxis.Horizontal => new IntVector2(x, -y),
            FlipAxis._45Degrees => new IntVector2(y, x),
            FlipAxis.Minus45Degrees => new IntVector2(-y, -x),
            _ => throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis)
        };
    }
}
