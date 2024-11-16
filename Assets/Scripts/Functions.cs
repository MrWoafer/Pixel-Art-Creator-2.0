using System.Collections.Generic;
using UnityEngine;

namespace PAC
{
    public static class Functions
    {
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static int Mod(int a, int b)
        {
            return (a % b + b) % b;
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

        public static float RoundTowardsZero(float f)
        {
            if (f >= 0)
            {
                return Mathf.Floor(f);
            }

            return Mathf.Ceil(f);
        }

        public static float RoundAwayFromZero(float f)
        {
            if (f >= 0)
            {
                return Mathf.Ceil(f);
            }

            return Mathf.Floor(f);
        }

        public static int RoundToIntTowardsZero(float f)
        {
            if (f >= 0)
            {
                return Mathf.FloorToInt(f);
            }

            return Mathf.CeilToInt(f);
        }

        public static int RoundToIntAwayFromZero(float f)
        {
            if (f >= 0)
            {
                return Mathf.CeilToInt(f);
            }

            return Mathf.FloorToInt(f);
        }

        public static float TruncateDecimalPlaces(float f, int decimalPlaces)
        {
            int integerPart = Functions.RoundToIntTowardsZero(f);
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

        /// <summary>
        /// Creates a copy of the array, but DOES NOT create a deep copy of the elements: reference types will just have the reference copied.
        /// </summary>
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
        /// Creates an IEnumerable starting at start and ending at end (inclusive).
        /// </summary>
        public static IEnumerable<int> Range(int start, int end)
        {
            if (start <= end)
            {
                for (int i = start; i <= end; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = start; i >= end; i--)
                {
                    yield return i;
                }
            }
        }
    }
}
