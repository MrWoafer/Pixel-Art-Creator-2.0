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
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }
        public static float Mod(float a, float b)
        {
            return (a % b + b) % b;
        }

        /// <summary>
        /// Computes the non-negative greatest common divisor of a and b.
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
        /// Computes the non-negative lowest common multiple of a and b.
        /// </summary>
        public static int Lcm(int a, int b)
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
