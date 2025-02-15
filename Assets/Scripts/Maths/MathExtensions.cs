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
        /// Returns the sign of the given value.
        /// </summary>
        /// <remarks>
        /// This is different from Unity's <see cref="Mathf.Sign(float)"/> since this method defines the sign of 0 to be 0, whereas <see cref="Mathf.Sign(float)"/> defines it to be 1.
        /// </remarks>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// 1 if <paramref name="x"/> &gt; 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> = 0
        /// </item>
        /// <item>
        /// -1 if <paramref name="x"/> &lt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static float Sign(this float x) => x switch
        {
            0f => 0f,
            > 0f => 1f,
            < 0f => -1f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Clamps the value so it's non-negative.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <paramref name="x"/> if <paramref name="x"/> &gt;= 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> &lt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static int ClampNonNegative(this int x) => x switch
        {
            > 0 => x,
            <= 0 => 0
        };
        /// <summary>
        /// Clamps the value so it's non-negative.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <paramref name="x"/> if <paramref name="x"/> &gt;= 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> &lt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static long ClampNonNegative(this long x) => x switch
        {
            > 0L => x,
            <= 0L => 0L
        };
        /// <summary>
        /// Clamps the value so it's non-negative.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <paramref name="x"/> if <paramref name="x"/> &gt;= 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> &lt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static float ClampNonNegative(this float x) => x switch
        {
            > 0f => x,
            <= 0f => 0f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Clamps the value so it's non-positive.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <paramref name="x"/> if <paramref name="x"/> &lt;= 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> &gt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static int ClampNonPositive(this int x) => x switch
        {
            < 0 => x,
            >= 0 => 0
        };
        /// <summary>
        /// Clamps the value so it's non-positive.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <paramref name="x"/> if <paramref name="x"/> &lt;= 0
        /// </item>
        /// <item>
        /// 0 if <paramref name="x"/> &gt; 0
        /// </item>
        /// </list>
        /// </returns>
        public static float ClampNonPositive(this float x) => x switch
        {
            < 0f => x,
            >= 0f => 0f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// Returns <c><paramref name="a"/> mod <paramref name="b"/></c>, giving a result in the range <c>[0, abs(b))</c>.
        /// </summary>
        /// <exception cref="DivideByZeroException"><paramref name="b"/> is 0.</exception>
        public static int Mod(this int a, int b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });
        /// <summary>
        /// Returns <c><paramref name="a"/> mod <paramref name="b"/></c>, giving a result in the range <c>[0, abs(b))</c>.
        /// </summary>
        /// <exception cref="DivideByZeroException"><paramref name="b"/> is 0.</exception>
        public static long Mod(this long a, long b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });
        /// <summary>
        /// Returns <c><paramref name="a"/> mod <paramref name="b"/></c>, giving a result in the range <c>[0, abs(b))</c>.
        /// </summary>
        /// <exception cref="DivideByZeroException"><paramref name="b"/> is 0.</exception>
        public static float Mod(this float a, float b) => b == 0 ? throw new DivideByZeroException($"{nameof(b)} cannot be 0.") : ((a >= 0, b >= 0) switch
        {
            (true, true) => a % b,
            (false, true) => ((a % b) + b) % b,
            (true, false) => a % b,
            (false, false) => ((a % b) - b) % b,
        });

        /// <summary>
        /// Computes the greatest non-negative common divisor of <paramref name="a"/> and <paramref name="b"/>.
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
        /// Computes the greatest non-negative common divisor of <paramref name="a"/> and <paramref name="b"/>.
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
        /// Returns the multiple of <paramref name="multipleOf"/> that is closest to <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static int RoundToMultipleOf(this float toRound, int multipleOf) => (int)RoundToMultipleOf(toRound, (float)multipleOf);
        /// <summary>
        /// Returns the multiple of <paramref name="multipleOf"/> that is closest to <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static float RoundToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new ArgumentException("Cannot round to a multiple of 0.", nameof(multipleOf)) : Mathf.Round(toRound / multipleOf) * multipleOf;

        /// <summary>
        /// Returns the greatest multiple of <paramref name="multipleOf"/> that is &lt;= <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static int FloorToMultipleOf(this float toRound, int multipleOf) => (int)FloorToMultipleOf(toRound, (float)multipleOf);
        /// <summary>
        /// Returns the greatest multiple of <paramref name="multipleOf"/> that is &lt;= <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static float FloorToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new ArgumentException("Cannot round to a multiple of 0.", nameof(multipleOf)) : Mathf.Floor(toRound / multipleOf) * multipleOf;

        /// <summary>
        /// Returns the least multiple of <paramref name="multipleOf"/> that is &gt;= <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static int CeilToMultipleOf(this float toRound, int multipleOf) => (int)CeilToMultipleOf(toRound, (float)multipleOf);
        /// <summary>
        /// Returns the least multiple of <paramref name="multipleOf"/> that is &gt;= <paramref name="toRound"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="multipleOf"/> is 0.</exception>
        public static float CeilToMultipleOf(this float toRound, float multipleOf)
            => multipleOf == 0 ? throw new ArgumentException("Cannot round to a multiple of 0.", nameof(multipleOf)) : Mathf.Ceil(toRound / multipleOf) * multipleOf;

        /// <summary>
        /// Rounds <paramref name="toRound"/> to an integer, choosing to round up or down so that it becomes closer (or unchanged) to <paramref name="towards"/>.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <c>floor(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &gt;= <paramref name="towards"/>
        /// </item>
        /// <item>
        /// <c>ceil(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &lt; <paramref name="towards"/>
        /// </item>
        /// </list>
        /// </returns>
        public static float RoundTowards(this float toRound, int towards) => toRound >= towards ? Mathf.Floor(toRound) : Mathf.Ceil(toRound);
        /// <summary>
        /// Rounds <paramref name="toRound"/> to an integer, choosing to round up or down so that it becomes closer (or unchanged) to <paramref name="towards"/>.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <c>floor(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &gt;= <paramref name="towards"/>
        /// </item>
        /// <item>
        /// <c>ceil(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &lt; <paramref name="towards"/>
        /// </item>
        /// </list>
        /// </returns>
        public static int RoundToIntTowards(this float toRound, int towards) => toRound >= towards ? Mathf.FloorToInt(toRound) : Mathf.CeilToInt(toRound);

        /// <summary>
        /// Rounds <paramref name="toRound"/> to an integer, choosing to round up or down so that it becomes further (or unchanged) away from <paramref name="awayFrom"/>.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <c>ceil(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &gt;= <paramref name="awayFrom"/>
        /// </item>
        /// <item>
        /// <c>floor(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &lt; <paramref name="awayFrom"/>
        /// </item>
        /// </list>
        /// </returns>
        public static float RoundAwayFrom(this float toRound, int awayFrom) => toRound >= awayFrom ? Mathf.Ceil(toRound) : Mathf.Floor(toRound);
        /// <summary>
        /// Rounds <paramref name="toRound"/> to an integer, choosing to round up or down so that it becomes further (or unchanged) away from <paramref name="awayFrom"/>.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <c>ceil(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &gt;= <paramref name="awayFrom"/>
        /// </item>
        /// <item>
        /// <c>floor(<paramref name="toRound"/>)</c> if <paramref name="toRound"/> &lt; <paramref name="awayFrom"/>
        /// </item>
        /// </list>
        /// </returns>
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

        /// <summary>
        /// Truncates <paramref name="f"/> to <paramref name="decimalPlaces"/> decimal places.
        /// </summary>
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

        /// <summary>
        /// Rounds <paramref name="f"/> to <paramref name="decimalPlaces"/> decimal places.
        /// </summary>
        public static float RoundDecimalPlaces(this float f, int decimalPlaces) => float.Parse(f.ToString("0." + new string('#', decimalPlaces)));

        /// <summary>
        /// Computes <paramref name="n"/> ^ <paramref name="exponent"/> (<paramref name="n"/> raised to the <paramref name="exponent"/>).
        /// </summary>
        /// <remarks>
        /// 0^0 is defined to be 1.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="exponent"/> is negative.</exception>
        public static int Pow(this int n, int exponent)
        {
            if (exponent < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(exponent)} cannot be negative: {exponent}.", nameof(exponent));
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
