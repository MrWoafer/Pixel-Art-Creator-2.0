using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PAC.Extensions;
using System.Runtime.CompilerServices;
using PAC.Maths;
using PAC.Geometry;

namespace PAC.DataStructures
{
    /// <summary>
    /// A 2-dimensional vector with integer coordinates.
    /// </summary>
    /// <remarks>
    /// Can be implicitly cast to/from <c>(<see cref="int"/> x, <see cref="int"/> y)</c>. This allows writing <see cref="IntVector2"/>s in a more readable way.
    /// <example>
    /// For example,
    /// <code language="csharp">
    /// IntVector2 point = (5, 3);
    /// </code>
    /// instead of
    /// <code language="csharp">
    /// IntVector2 point = new IntVector2(5, 3);
    /// </code>
    /// </example>
    /// </remarks>
    public readonly struct IntVector2 : IEquatable<IntVector2>
    {
        #region Fields
        /// <summary>
        /// The x coordinate.
        /// </summary>
        /// <seealso cref="y"/>
        public readonly int x;
        /// <summary>
        /// The y coordinate.
        /// </summary>
        /// <seealso cref="x"/>
        public readonly int y;
        #endregion

        #region Properties
        /// <summary>
        /// The standard Euclidean magnitude of the vector, which is <c>sqrt(<see cref="x"/>^2 + <see cref="y"/>^2)</c>.
        /// </summary>
        /// <seealso cref="Magnitude(IntVector2)"/>
        /// <seealso cref="Distance(IntVector2, IntVector2)"/>
        /// <seealso cref="sqrMagnitude"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Euclidean_norm"/>
        public float magnitude => Magnitude(this);
        /// <summary>
        /// The square of the Euclidean magnitude of the vector, so <c><see cref="x"/>^2 + <see cref="y"/>^2</c>.
        /// </summary>
        /// <remarks>
        /// This is faster than squaring <see cref="magnitude"/>.
        /// </remarks>
        /// <seealso cref="SqrMagnitude(IntVector2)"/>
        /// <seealso cref="SqrDistance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Euclidean_norm"/>
        public int sqrMagnitude => SqrMagnitude(this);

        /// <summary>
        /// The l1 norm of the vector, which is <c>abs(<see cref="x"/>) + abs(<see cref="y"/>)</c>.
        /// Also known as the taxicab/Manhattan norm.
        /// </summary>
        /// <seealso cref="L1Norm(IntVector2)"/>
        /// <seealso cref="L1Distance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Taxicab_norm_or_Manhattan_norm"/>
        public int l1Norm => L1Norm(this);
        /// <summary>
        /// The supremum norm of the vector, which is <c>max(abs(<see cref="x"/>), abs(<see cref="y"/>))</c>.
        /// Also known as the Chebyshev norm or l-infinity norm.
        /// </summary>
        /// <seealso cref="SupNorm(IntVector2)"/>
        /// <seealso cref="SupDistance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Uniform_norm"/>
        public int supNorm => SupNorm(this);

        /// <summary>
        /// The sign of each component of the vector.
        /// </summary>
        /// <remarks>
        /// The sign of a component that is 0 is defined to be 0.
        /// </remarks>
        /// <seealso cref="Sign(IntVector2)"/>
        public IntVector2 sign => Sign(this);
        #endregion

        #region Predefined Instances
        /// <summary>
        /// The vector (0, 0).
        /// </summary>
        public static readonly IntVector2 zero = (0, 0);
        /// <summary>
        /// The vector (1, 1).
        /// </summary>
        /// <remarks>
        /// An alias for <see cref="upRight"/>.
        /// </remarks>
        public static readonly IntVector2 one = (1, 1);

        /// <summary>
        /// The vector (1, 0).
        /// </summary>
        /// <seealso cref="left"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 right = (1, 0);
        /// <summary>
        /// The vector (-1, 0).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 left = (-1, 0);
        /// <summary>
        /// The vector (0, 1).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 up = (0, 1);
        /// <summary>
        /// The vector (0, -1).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="up"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 down = (0, -1);

        /// <summary>
        /// The vector (1, 1).
        /// </summary>
        /// <remarks>
        /// An alias for <see cref="one"/>.
        /// </remarks>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upLeft"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 upRight = (1, 1);
        /// <summary>
        /// The vector (-1, 1).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="downRight"/><seealso cref="downLeft"/>
        public static readonly IntVector2 upLeft = (-1, 1);
        /// <summary>
        /// The vector (1, -1).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downLeft"/>
        public static readonly IntVector2 downRight = (1, -1);
        /// <summary>
        /// The vector (-1, -1).
        /// </summary>
        /// <seealso cref="right"/><seealso cref="left"/><seealso cref="up"/><seealso cref="down"/>
        /// <seealso cref="upRight"/><seealso cref="upLeft"/><seealso cref="downRight"/>
        public static readonly IntVector2 downLeft = (-1, -1);

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> over the vectors <c>(0, 1), (0, -1), (-1, 0), (1, 0)</c>.
        /// </summary>
        /// <seealso cref="up"/><seealso cref="down"/><seealso cref="left"/><seealso cref="right"/>
        public static readonly IEnumerable<IntVector2> upDownLeftRight = new IntVector2[] { up, down, left, right };
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new vector with the given x and y coordinates.
        /// </summary>
        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Deconstructs the vector into its x and y coordinates.
        /// </summary>
        public void Deconstruct(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }
        #endregion

        #region Conversion
        /// <remarks>
        /// This conversion should be inlined by the JIT compiler.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator (int x, int y)(IntVector2 vector) => (vector.x, vector.y);
        /// <summary>
        /// <para>
        /// Allows writing <see cref="IntVector2"/>s in a more readable way
        /// </para>
        /// <example>
        /// For example,
        /// <code language="csharp">
        /// IntVector2 point = (5, 3);
        /// </code>
        /// instead of
        /// <code language="csharp">
        /// IntVector2 point = new IntVector2(5, 3);
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// This conversion should be inlined by the JIT compiler.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntVector2((int x, int y) tuple) => new IntVector2(tuple.x, tuple.y);

        /// <summary>
        /// Casts to Unity's <see cref="Vector2Int"/>.
        /// </summary>
        public static implicit operator Vector2Int(IntVector2 vector) => new Vector2Int(vector.x, vector.y);
        /// <summary>
        /// Casts from Unity's <see cref="Vector2Int"/>.
        /// </summary>
        public static implicit operator IntVector2(Vector2Int vector) => new IntVector2(vector.x, vector.y);

        /// <summary>
        /// Casts to Unity's <see cref="Vector2"/>, by casting each coordinate to <see cref="float"/>.
        /// </summary>
        public static implicit operator Vector2(IntVector2 vector) => new Vector2(vector.x, vector.y);
        /// <summary>
        /// Casts from Unity's <see cref="Vector2"/>, by casting each coordinate to <see cref="int"/>, which truncates them towards zero.
        /// </summary>
        public static explicit operator IntVector2(Vector2 vector) => ((int)vector.x, (int)vector.y);

        /// <summary>
        /// Casts to Unity's <see cref="Vector3"/>, by casting each coordinate to <see cref="float"/>, with a 0 in the z-coord.
        /// </summary>
        public static implicit operator Vector3(IntVector2 vector) => new Vector3(vector.x, vector.y, 0f);
        /// <summary>
        /// Casts from Unity's <see cref="Vector3"/>, by casting each coordinate to <see cref="int"/>, which truncates them towards zero. The z coordinate is ignored.
        /// </summary>
        public static explicit operator IntVector2(Vector3 vector) => ((int)vector.x, (int)vector.y);

        /// <summary>
        /// Floors the vector component-wise.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Mathf.FloorToInt(float)"/>.
        /// </remarks>
        /// <seealso cref="CeilToIntVector2(Vector2)"/>
        /// <seealso cref="RoundToIntVector2(Vector2)"/>
        public static IntVector2 FloorToIntVector2(Vector2 vector2) => (Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
        /// <summary>
        /// Ceils the vector component-wise.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Mathf.CeilToInt(float)"/>.
        /// </remarks>
        /// <seealso cref="FloorToIntVector2(Vector2)"/>
        /// <seealso cref="RoundToIntVector2(Vector2)"/>
        public static IntVector2 CeilToIntVector2(Vector2 vector2) => (Mathf.CeilToInt(vector2.x), Mathf.CeilToInt(vector2.y));
        /// <summary>
        /// Rounds the vector component-wise.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Mathf.RoundToInt(float)"/>.
        /// </remarks>
        /// <seealso cref="FloorToIntVector2(Vector2)"/>
        /// <seealso cref="CeilToIntVector2(Vector2)"/>
        public static IntVector2 RoundToIntVector2(Vector2 vector2) => (Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
        #endregion

        #region Comparison
        /// <summary>
        /// Whether the two vectors have identical x and y coords.
        /// </summary>
        /// <seealso cref="Equals(IntVector2)"/>
        public static bool operator ==(IntVector2 a, IntVector2 b) => a.x == b.x && a.y == b.y;
        /// <summary>
        /// Whether the two vectors differ in their x and/or y coords.
        /// </summary>
        /// <seealso cref="Equals(IntVector2)"/>
        public static bool operator !=(IntVector2 a, IntVector2 b) => !(a == b);
        /// <summary>
        /// The same as <see cref="operator ==(IntVector2, IntVector2)"/>
        /// </summary>
        public bool Equals(IntVector2 other) => this == other;
        /// <summary>
        /// The same as <see cref="Equals(IntVector2)"/>
        /// </summary>
        public override bool Equals(object obj) => obj is IntVector2 other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y);

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
        /// <para>
        /// Returns whether this vector is an integer multiple of <paramref name="divisor"/>.
        /// </para>
        /// <para>
        /// <example>
        /// For example, <c>(3, 6)</c> is a multiple of <c>(1, 2)</c> as <c>(3, 6) = 3 * (1, 2)</c>.
        /// </example>
        /// </para>
        /// </summary>
        /// <remarks>
        /// More precisely, this determines whether there exists an integer <c>n</c> such that <c>this = n * <paramref name="divisor"/></c>.
        /// This means, for example, that <c>(0, 6)</c> is a multiple of <c>(0, 3)</c>, and that <c>(0, 0)</c> is a multiple of everything. 
        /// </remarks>
        /// <seealso cref="Divides(IntVector2)"/>
        public bool IsMultipleOf(IntVector2 divisor) => divisor.Divides(this);
        /// <summary>
        /// <para>
        /// Returns whether <paramref name="dividend"/> is an integer multiple of this vector.
        /// </para>
        /// <para>
        /// <example>
        /// For example, <c>(1, 2)</c> divides <c>(3, 6)</c> as <c>(3, 6) = 3 * (1, 2)</c>.
        /// </example>
        /// </para>
        /// </summary>
        /// <remarks>
        /// More precisely, this determines whether there exists an integer <c>n</c> such that <c><paramref name="dividend"/> = n * this</c>.
        /// This means, for example, that <c>(0, 3)</c> divides <c>(0, 6)</c>, and that everything divides <c>(0, 0)</c>. 
        /// </remarks>
        /// <seealso cref="IsMultipleOf(IntVector2)"/>
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
        /// Returns whether the two vectors lie on the same real line through <c>(0, 0)</c>. This is equivalent to whether there is an <see cref="IntVector2"/> dividing both of them.
        /// </summary>
        /// <remarks>
        /// For a proof that the two statements given in the summary are logically equivalent, see the source code for this method.
        /// </remarks>
        /// <seealso cref="Divides(IntVector2)"/>
        public static bool AreColinear(IntVector2 a, IntVector2 b)
        {
            // Proof that being colinear in the sense of real vectors is equivalent to there being an IntVector2 dividing both of them:
            //      The <= direction is trivial, so we just prove the => direction.
            //      The case where a = 0 or b = 0 is trivial, so assume a and b are non-zero. Without loss of generality, let a, b be in the top-right quadrant. Let c be the smallest positive
            //      integer point on the line. It is enough to the prove that c divides a (as then, by symmetry, c divides b; and by definition of Simplify() and minimality of c, c = Simplify(a),
            //      Simplify(b)). So suppose c did not divide a. Let d be the greatest multiple of c less than a (note c < a). Note d is on the line too, and hence so is a - d, which is an integer
            //      vector. But, by definition of d, 0 < a - d < c, contradicting minimality of c!

            // This is a rearrangement of a.y / a.x == b.y / b.x (comparing gradients) which avoids floats and division by 0.
            return a.y * b.x == b.y * a.x;
        }

        /// <summary>
        /// Determines whether the two vectors are perpendicular to each other.
        /// </summary>
        /// <remarks>
        /// Being <i>perpendicular</i> is also known as being <i>orthogonal</i>.
        /// </remarks>
        /// <seealso cref="Dot(IntVector2, IntVector2)"/>
        public static bool ArePerpendicular(IntVector2 a, IntVector2 b) => Dot(a, b) == 0;
        #endregion

        #region Operations
        /// <summary>
        /// Adds the two vectors component-wise.
        /// </summary>
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => (a.x + b.x, a.y + b.y);

        /// <summary>
        /// Negates each component of the vector.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a) => (-a.x, -a.y);
        /// <summary>
        /// Subtracts the two vectors component-wise.
        /// </summary>
        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => (a.x - b.x, a.y - b.y);

        /// <summary>
        /// Multiplies the two vectors component-wise.
        /// </summary>
        public static IntVector2 operator *(IntVector2 a, IntVector2 b) => (a.x * b.x, a.y * b.y);
        /// <summary>
        /// Multiplies each component of the vector by <paramref name="scalar"/>.
        /// </summary>
        public static IntVector2 operator *(int scalar, IntVector2 vector) => (scalar * vector.x, scalar * vector.y);
        /// <summary>
        /// Multiplies each component of the vector by <paramref name="scalar"/>.
        /// </summary>
        public static IntVector2 operator *(IntVector2 vector, int scalar) => scalar * vector;

        /// <summary>
        /// Divides (integer division) the two vectors component-wise.
        /// </summary>
        public static IntVector2 operator /(IntVector2 a, IntVector2 b) => (a.x / b.x, a.y / b.y);
        /// <summary>
        /// Divides (integer division) each component of the vector by <paramref name="scalar"/>.
        /// </summary>
        public static IntVector2 operator /(IntVector2 vector, int scalar) => (vector.x / scalar, vector.y / scalar);

        /// <summary>
        /// Scales the vector down so its components are coprime (have no common divisors). In other words, it computes the smallest <see cref="IntVector2"/> dividing <paramref name="a"/>.
        /// </summary>
        /// <remarks>
        /// Preserves signs.
        /// </remarks>
        public static IntVector2 Simplify(IntVector2 a) => (a == zero) ? zero : (a / MathExtensions.Gcd(a.x, a.y));

        /// <summary>
        /// Computes the dot product of the two vectors.
        /// </summary>
        public static int Dot(IntVector2 a, IntVector2 b) => a.x * b.x + a.y * b.y;

        /// <summary>
        /// The standard Euclidean magnitude of the vector, which is <c>sqrt(<see cref="x"/>^2 + <see cref="y"/>^2)</c>.
        /// </summary>
        /// <seealso cref="magnitude"/>
        /// <seealso cref="Distance(IntVector2, IntVector2)"/>
        /// <seealso cref="SqrMagnitude(IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Euclidean_norm"/>
        public static float Magnitude(IntVector2 a) => Mathf.Sqrt(a.x * a.x + a.y * a.y);
        /// <summary>
        /// The square of the Euclidean magnitude of the vector, so <c><see cref="x"/>^2 + <see cref="y"/>^2</c>.
        /// </summary>
        /// <remarks>
        /// This is faster than squaring <see cref="Magnitude(IntVector2)"/>.
        /// </remarks>
        /// <seealso cref="sqrMagnitude"/>
        /// <seealso cref="SqrDistance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Euclidean_norm"/>
        public static int SqrMagnitude(IntVector2 a) => a.x * a.x + a.y * a.y;
        /// <summary>
        /// Computes the standard Euclidean distance between the two vectors - i.e. the <see cref="Magnitude(IntVector2)"/> of <c><paramref name="a"/> - <paramref name="b"/></c>.
        /// </summary>
        /// <seealso cref="SqrDistance(IntVector2, IntVector2)"/>
        public static float Distance(IntVector2 a, IntVector2 b) => Magnitude(a - b);
        /// <summary>
        /// Computes the square of the Euclidean distance between the two vectors - i.e. the <see cref="SqrMagnitude(IntVector2)"/> of <c><paramref name="a"/> - <paramref name="b"/></c>.
        /// </summary>
        /// <remarks>
        /// This is faster than squaring <see cref="Distance(IntVector2, IntVector2)"/>.
        /// </remarks>
        public static int SqrDistance(IntVector2 a, IntVector2 b) => SqrMagnitude(a - b);

        /// <summary>
        /// The l1 norm of the vector, which is <c>abs(<see cref="x"/>) + abs(<see cref="y"/>)</c>.
        /// Also known as the taxicab/Manhattan norm.
        /// </summary>
        /// <seealso cref="l1Norm"/>
        /// <seealso cref="L1Distance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Norm_(mathematics)#Taxicab_norm_or_Manhattan_norm"/>
        public static int L1Norm(IntVector2 a) => Math.Abs(a.x) + Math.Abs(a.y);
        /// <summary>
        /// Computes the l1 distance between the two vectors - i.e. the <see cref="L1Norm(IntVector2)"/> of <c><paramref name="a"/> - <paramref name="b"/></c>.
        /// Also known as the taxicab/Manhattan distance or rectilinear distance.
        /// </summary>
        public static int L1Distance(IntVector2 a, IntVector2 b) => L1Norm(a - b);

        /// <summary>
        /// The supremum norm of the vector, which is <c>max(abs(<see cref="x"/>), abs(<see cref="y"/>))</c>.
        /// Also known as the Chebyshev norm or l-infinity norm.
        /// </summary>
        /// <seealso cref="supNorm"/>
        /// <seealso cref="SupDistance(IntVector2, IntVector2)"/>
        /// <seealso href="https://en.wikipedia.org/wiki/Uniform_norm"/>
        public static int SupNorm(IntVector2 a) => Math.Max(Math.Abs(a.x), Math.Abs(a.y));
        /// <summary>
        /// Computes the supremum distance between the two vectors - i.e. the <see cref="SupNorm(IntVector2)"/> of <c><paramref name="a"/> - <paramref name="b"/></c>.
        /// Also known as the Chebyshev distance or l-infinity distance.
        /// </summary>
        public static int SupDistance(IntVector2 a, IntVector2 b) => SupNorm(a - b);

        /// <summary>
        /// Returns the sign of each component of the vector.
        /// </summary>
        /// <remarks>
        /// The sign of a component that is 0 is defined to be 0.
        /// </remarks>
        /// <seealso cref="sign"/>
        public static IntVector2 Sign(IntVector2 a) => (Math.Sign(a.x), Math.Sign(a.y));
        /// <summary>
        /// Takes the absolute value component-wise.
        /// </summary>
        public static IntVector2 Abs(IntVector2 a) => (Math.Abs(a.x), Math.Abs(a.y));

        /// <summary>
        /// Takes the maximum of the two vectors component-wise.
        /// </summary>
        /// <seealso cref="Min(IntVector2, IntVector2)"/>
        public static IntVector2 Max(IntVector2 a, IntVector2 b) => (Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        /// <summary>
        /// Takes the maximum of the vectors component-wise.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        /// <seealso cref="Min(IntVector2[])"/>
        public static IntVector2 Max(params IntVector2[] vectors) => Max((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Takes the maximum of the vectors component-wise.
        /// </summary>
        /// <remarks>
        /// This is not provided as an <see cref="IEnumerable{T}"/> extension method since it seems C# will favour using the LINQ <see cref="Enumerable.Max{TSource}(IEnumerable{TSource})"/>
        /// instead, which causes errors.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        /// <seealso cref="Min(IEnumerable{IntVector2})"/>
        public static IntVector2 Max(IEnumerable<IntVector2> vectors)
        {
            if (vectors is null)
            {
                throw new ArgumentNullException($"Cannot perform {nameof(Max)}() on null.", nameof(vectors));
            }
            if (vectors.None())
            {
                throw new ArgumentException($"Cannot perform {nameof(Max)}() on an empty collection of {nameof(IntVector2)}s.", nameof(vectors));
            }

            return (vectors.Max(vector => vector.x), vectors.Max(vector => vector.y));
        }

        /// <summary>
        /// Takes the minimum of the two vectors component-wise.
        /// </summary>
        /// <seealso cref="Max(IntVector2, IntVector2)"/>
        public static IntVector2 Min(IntVector2 a, IntVector2 b) => (Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        /// <summary>
        /// Takes the minmum of the vectors component-wise.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        /// <seealso cref="Max(IntVector2[])"/>
        public static IntVector2 Min(params IntVector2[] vectors) => Min((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Takes the minmum of the vectors component-wise.
        /// </summary>
        /// <remarks>
        /// This is not provided as an <see cref="IEnumerable{T}"/> extension method since it seems C# will favour using the LINQ <see cref="Enumerable.Min{TSource}(IEnumerable{TSource})"/>
        /// instead, which causes errors.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        /// <seealso cref="Max(IEnumerable{IntVector2})"/>
        public static IntVector2 Min(IEnumerable<IntVector2> vectors)
        {
            if (vectors is null)
            {
                throw new ArgumentNullException($"Cannot perform {nameof(Min)}() on null.", nameof(vectors));
            }
            if (vectors.None())
            {
                throw new ArgumentException($"Cannot perform {nameof(Min)}() on an empty collection of {nameof(IntVector2)}s.", nameof(vectors));
            }

            return (vectors.Min(vector => vector.x), vectors.Min(vector => vector.y));
        }

        /// <summary>
        /// Returns the vector rotated clockwise by the given angle.
        /// </summary>
        public IntVector2 Rotate(RotationAngle angle) => angle switch
        {
            RotationAngle._0 => this,
            RotationAngle._90 => (y, -x),
            RotationAngle._180 => (-x, -y),
            RotationAngle.Minus90 => (-y, x),
            _ => throw new NotImplementedException("Unknown / unimplemented angle: " + angle)
        };

        /// <summary>
        /// Returns the vector flipped across the given axis.
        /// </summary>
        public IntVector2 Flip(FlipAxis axis) => axis switch
        {
            FlipAxis.None => this,
            FlipAxis.Vertical => (-x, y),
            FlipAxis.Horizontal => (x, -y),
            FlipAxis._45Degrees => (y, x),
            FlipAxis.Minus45Degrees => (-y, -x),
            _ => throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis)
        };
        #endregion

        /// <summary>
        /// Represents the vector as a string in the form <c>"(x, y)"</c>.
        /// </summary>
        public override string ToString() => $"({x}, {y})";
    }
}
