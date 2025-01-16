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
        public static float Sign(float x) => x switch
        {
            0 => 0f,
            > 0 => 1f,
            < 0 => -1f,
            float.NaN => float.NaN
        };

        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ClampPositive().
        /// </para>
        /// </summary>
        public static int ReLU(int x) => x switch
        {
            > 0 => x,
            <= 0 => 0
        };
        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ClampPositive().
        /// </para>
        /// </summary>
        public static long ReLU(long x) => x switch
        {
            > 0 => x,
            <= 0 => 0
        };
        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ClampPositive().
        /// </para>
        /// </summary>
        public static float ReLU(float x) => x switch
        {
            > 0 => x,
            <= 0 => 0f,
            float.NaN => float.NaN
        };
        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ReLU().
        /// </para>
        /// </summary>
        public static int ClampNonNegative(int x) => ReLU(x);
        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ReLU().
        /// </para>
        /// </summary>
        public static long ClampNonNegative(long x) => ReLU(x);
        /// <summary>
        /// <para>
        /// Returns x if x &gt; 0, and returns 0 if x &lt;= 0.
        /// </para>
        /// <para>
        /// The same as ReLU().
        /// </para>
        /// </summary>
        public static float ClampNonNegative(float x) => ReLU(x);
        /// <summary>
        /// Returns x if x &lt; 0, and returns 0 if x &gt;= 0.
        /// </summary>
        public static int ClampNonPositive(int x) => -ReLU(-x);
        /// <summary>
        /// Returns x if x &lt; 0, and returns 0 if x &gt;= 0.
        /// </summary>
        public static float ClampNonPositive(float x) => -ReLU(-x);

        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static long Mod(long a, long b)
        {
            return (a % b + b) % b;
        }
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static float Mod(float a, float b)
        {
            return (a % b + b) % b;
        }

        /// <summary>
        /// Computes the greatest non-negative common divisor of a and b.
        /// </summary>
        public static int Gcd(int a, int b)
        {
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
        public static long Gcd(long a, long b)
        {
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
        public static int Lcm(int a, int b)
        {
            if (a == 0 && b == 0)
            {
                return 0;
            }
            return Math.Abs(a * b) / Gcd(a, b);
        }
        /// <summary>
        /// Computes the lowest non-negative common multiple of a and b.
        /// </summary>
        public static long Lcm(long a, long b)
        {
            if (a == 0 && b == 0)
            {
                return 0;
            }
            return Math.Abs(a * b) / Gcd(a, b);
        }

        public static int RoundToMultiple(float toRound, int multipleOf)
        {
            return (int)RoundToMultiple(toRound, (float)multipleOf);
        }
        public static float RoundToMultiple(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }

            return Mathf.Round(toRound / multipleOf) * multipleOf;
        }

        public static int FloorToMultiple(float toRound, int multipleOf)
        {
            return (int)FloorToMultiple(toRound, (float)multipleOf);
        }
        public static float FloorToMultiple(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }

            return Mathf.Floor(toRound / multipleOf) * multipleOf;
        }

        public static int CeilToMultiple(float toRound, int multipleOf)
        {
            return (int)CeilToMultiple(toRound, (float)multipleOf);
        }
        public static float CeilToMultiple(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }

            return Mathf.Ceil(toRound / multipleOf) * multipleOf;
        }

        public static float SymmetricFloor(float f)
        {
            if (f >= 0)
            {
                return Mathf.Floor(f);
            }

            return Mathf.Ceil(f);
        }

        public static float SymmetricCeil(float f)
        {
            if (f >= 0)
            {
                return Mathf.Ceil(f);
            }

            return Mathf.Floor(f);
        }

        public static int SymmetricFloorToInt(float f)
        {
            if (f >= 0)
            {
                return Mathf.FloorToInt(f);
            }

            return Mathf.CeilToInt(f);
        }

        public static int SymmetricCeilToInt(float f)
        {
            if (f >= 0)
            {
                return Mathf.CeilToInt(f);
            }

            return Mathf.FloorToInt(f);
        }

        private static string FirstNChars(string str, int numOfChars)
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

        public static float TruncateDecimalPlaces(float f, int decimalPlaces)
        {
            int integerPart = SymmetricFloorToInt(f);
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

        public static float RoundDecimalPlaces(float f, int decimalPlaces)
        {
            return float.Parse(f.ToString("0." + new string('#', decimalPlaces)));
        }

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
