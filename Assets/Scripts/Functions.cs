using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAC
{
    public static class Functions
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

        public static Vector2 Vector3ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
        public static Vector3 Vector2ToVector3(Vector3 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0f);
        }

        public static string FirstNChars(string str, int numOfChars)
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

        public static float TruncateDecimalPlaces(float f, int decimalPlaces)
        {
            int integerPart = Functions.SymmetricFloorToInt(f);
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

            // Let's number bits so the 0-th bit is the least significant bit, etc.
            //
            // We exploit the fact that e.g.
            //      n ^ (2^4 + 2^1 + 2^0) = n ^ (2^4) * n ^ (2^1) * n ^ (2^0)
            //                            = f(4) * f(1) * f(0)
            // Where we define f(i) = n ^ (2^i)
            //
            // So, n ^ exponent = product of f(i) over each i where the i-th bit of exponent is 1
            //
            // Therefore we could use the following algorithm:
            //      output = 1
            //      for i-th bit from 0-th bit to last bit of exponent
            //          if i-th bit = 1
            //              output = output * f(i)
            //
            // This doesn't work yet, as this requires computing exponents to compute f(i).
            // However, f(i) satisfies the recurrence relation f(i + 1) = f(i) * f(i), so the algorithm can be changed to:
            //      output = 1
            //      f = n                                                   # f stores the value of f(i) and starts at f(0) = n 
            //      for i-th bit from 0-th bit to last bit of exponent
            //          if i-th bit = 1
            //              output = output * f
            //          f = f * f                                           # use the recurrence relation to set f = f(i + 1)
            //
            // The algorithm below is a slight rewriting of this. Instead of indexing the bits, we always look at the 0-th bit and just bit-shift so the next bit becomes the 0-th bit

            int output = 1;
            int f = n;
            // Loop until we've looked at all bits that are 1
            while (exponent > 0)
            {
                // If 0-th bit of exponent is 1
                if ((exponent & 1) == 1)
                {
                    output = checked(output * f);
                }
                // Double the power in the part
                f = checked(f * f);
                // Move on to look at next bit
                exponent >>= 1;
            }
            return output;
        }

        public static T[] ConcatArrays<T>(T[] array1, T[] array2)
        {
            T[] array = new T[array1.Length + array2.Length];

            for (int i = 0; i < array1.Length; i++)
            {
                array[i] = array1[i];
            }
            for (int i = 0; i < array2.Length; i++)
            {
                array[array1.Length + i] = array2[i];
            }

            return array;
        }

        public static T[] CopyArray<T>(T[] array)
        {
            T[] copy = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                copy[i] = array[i];
            }

            return copy;
        }

        public static bool CompareArrays<T>(T[] array1, T[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ArrayToString<T>(T[] array)
        {
            string str = "{";

            for (int i = 0; i < array.Length; i++)
            {
                str += array[i].ToString();
                if (i < array.Length - 1)
                {
                    str += ", ";
                }
            }

            str += "}";

            return str;
        }

        /// <summary>
        /// Creates an array starting at start and ending at end (inclusive).
        /// </summary>
        public static int[] Range(int start, int end)
        {
            if (start <= end)
            {
                int[] range = new int[end - start + 1];
                for (int i = start; i <= end; i++)
                {
                    range[i - start] = i;
                }
                return range;
            }
            else
            {
                int[] range = new int[start - end + 1];
                for (int i = start; i >= end; i--)
                {
                    range[start - i] = i;
                }
                return range;
            }
        }

        public static T[] ToArray<T>(this HashSet<T> hashSet)
        {
            T[] array = new T[hashSet.Count];
            hashSet.CopyTo(array);
            return array;
        }
    }
}
