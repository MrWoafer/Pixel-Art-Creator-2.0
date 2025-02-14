using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace PAC.Maths
{
    /// <summary>
    /// Defines some mathematical functions.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// <para>
        /// Returns the sign of the given float.
        /// </para>
        /// <para>
        /// Note: this is different from Unity's Mathf.Sign() since this method defines the sign of 0 to be 0, whereas Mathf.Sign() defines it as 1.
        /// </para>
        /// </summary>
        public static float Sign(this float x) => x switch
        {
            0 => 0f,
            > 0 => 1f,
            < 0 => -1f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </summary>
        public static int ClampNonNegative(this int x) => x switch
        {
            > 0 => x,
            <= 0 => 0
        };
        /// <summary>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </summary>
        public static long ClampNonNegative(this long x) => x switch
        {
            > 0L => x,
            <= 0L => 0L
        };
        /// <summary>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </summary>
        public static float ClampNonNegative(this float x) => x switch
        {
            > 0f => x,
            <= 0f => 0f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Returns x if x &lt; 0, and returns 0 if x &gt;= 0.
        /// </summary>
        public static int ClampNonPositive(this int x) => x switch
        {
            < 0 => x,
            >= 0 => 0
        };
        /// <summary>
        /// Returns x if x &lt; 0, and returns 0 if x &gt;= 0.
        /// </summary>
        public static float ClampNonPositive(this float x) => x switch
        {
            < 0f => x,
            >= 0f => 0f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Mod(this int a, int b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static long Mod(this long a, long b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static float Mod(this float a, float b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });

        /// <summary>
        /// Computes the greatest non-negative common divisor of a and b.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="a"/> or <paramref name="b"/> are <see cref="int.MinValue"/>.</exception>
        public static int Gcd(this int a, int b)
        {
            if (a == int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(a), $"{nameof(a)} cannot be int.MinValue.");
            }
            if (b == int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(b), $"{nameof(b)} cannot be int.MinValue.");
            }

            a = Math.Abs(a);
            b = Math.Abs(b);

            // We use Euclid's algorithm
            int r = b;
            while (r != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return a;
        }
        /// <summary>
        /// Computes the greatest non-negative common divisor of a and b.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="a"/> or <paramref name="b"/> are <see cref="long.MinValue"/>.</exception>
        public static long Gcd(this long a, long b)
        {
            if (a == long.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(a), $"{nameof(a)} cannot be long.MinValue.");
            }
            if (b == long.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(b), $"{nameof(b)} cannot be long.MinValue.");
            }

            a = Math.Abs(a);
            b = Math.Abs(b);

            // We use Euclid's algorithm
            long r = b;
            while (r != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return a;
        }

        /// <summary>
        /// Computes the lowest non-negative common multiple of a and b.
        /// </summary>
        public static int Lcm(this int a, int b) => (a == 0 && b == 0) ? 0 : Math.Abs(a * b) / Gcd(a, b);
        /// <summary>
        /// Computes the lowest non-negative common multiple of a and b.
        /// </summary>
        public static long Lcm(this long a, long b) => (a == 0 && b == 0) ? 0 : Math.Abs(a * b) / Gcd(a, b);

        public static int RoundToMultipleOf(this float toRound, int multipleOf) => (int)RoundToMultipleOf(toRound, (float)multipleOf);
        public static float RoundToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new System.Exception("Cannot round to a multiple of 0.") : Mathf.Round(toRound / multipleOf) * multipleOf;

        public static int FloorToMultipleOf(this float toRound, int multipleOf) => (int)FloorToMultipleOf(toRound, (float)multipleOf);
        public static float FloorToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new System.Exception("Cannot round to a multiple of 0.") : Mathf.Floor(toRound / multipleOf) * multipleOf;

        public static int CeilToMultipleOf(this float toRound, int multipleOf) => (int)CeilToMultipleOf(toRound, (float)multipleOf);
        public static float CeilToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new System.Exception("Cannot round to a multiple of 0.") : Mathf.Ceil(toRound / multipleOf) * multipleOf;

        public static float RoundTowards(this float toRound, int towards) => toRound >= towards ? Mathf.Floor(toRound) : Mathf.Ceil(toRound);
        public static int RoundToIntTowards(this float toRound, int towards) => toRound >= towards ? Mathf.FloorToInt(toRound) : Mathf.CeilToInt(toRound);

        public static float RoundAwayFrom(this float toRound, int awayFrom) => toRound >= awayFrom ? Mathf.Ceil(toRound) : Mathf.Floor(toRound);
        public static int RoundToIntAwayFrom(this float toRound, int awayFrom) => toRound >= awayFrom ? Mathf.CeilToInt(toRound) : Mathf.FloorToInt(toRound);

        private static string FirstNChars(this string str, int numOfChars)
        {
            if (numOfChars < 0)
            {
                return "";
            }

            if (numOfChars < str.Length)
            {
                return str.Remove(numOfChars);
            }

            return str;
        }

        public static float TruncateDecimalPlaces(this float f, int decimalPlaces)
        {
            int integerPart = f.RoundToIntTowards(0);
            float decimalPart = Mathf.Abs(f - integerPart);

            if (decimalPart.ToString() == "0")
            {
                return integerPart;
            }
            else
            {
                return float.Parse(integerPart.ToString() + "." + FirstNChars(decimalPart.ToString().Remove(0, 2), decimalPlaces));
            }
        }

        public static float RoundDecimalPlaces(this float f, int decimalPlaces) => float.Parse(f.ToString("0." + new string('#', decimalPlaces)));

        /// <summary>
        /// <para>
        /// Computes n ^ exponent (n raised to the exponent). 0^0 is defined to be 1.
        /// </para>
        /// <para>
        /// Exponent must be non-negative.
        /// </para>
        /// </summary>
        public static int Pow(this int n, int exponent)
        {
            if (exponent < 0)
            {
                throw new ArgumentOutOfRangeException($"Exponent cannot be negative: {exponent}.", nameof(exponent));
            }

            // Three special cases that won't terminate quickly due to overflow exceptions.
            if (n == 0)
            {
                return exponent == 0 ? 1 : 0;
            }
            if (n == 1)
            {
                return n;
            }
            if (n == -1)
            {
                return (exponent % 2 == 0) ? 1 : -1;
            }

            // Now, as abs(n) >= 2, this algorithm should always terminate in at most 32 steps - either because we have iterated up to exponent, or because we get an overflow exception.
            // (We will reach an overflow exception in at most this many steps because the maximum absolute value of an int is 2^31.)

            int output = 1;
            for (int i = 0; i < exponent; i++)
            {
                output = checked(output * n);
            }
            return output;
        }

        /// <summary>
        /// Computes <c><paramref name="n"/> * <paramref name="n"/></c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(this int n) => n * n;
        /// <summary>
        /// Computes <c><paramref name="n"/> * <paramref name="n"/></c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Square(this float n) => n * n;
    }
}
