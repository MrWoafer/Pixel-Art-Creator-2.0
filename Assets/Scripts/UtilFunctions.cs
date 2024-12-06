using System.Collections.Generic;
using UnityEngine;

namespace PAC.Utils
{
    public static class UtilFunctions
    {
        /// <summary>
        /// Returns a mod b, giving a non-negative result.
        /// </summary>
        public static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }

        public static int RoundToMultipleOf(float toRound, int multipleOf) => (int)RoundToMultipleOf(toRound, (float) multipleOf);
        public static float RoundToMultipleOf(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }
            return Mathf.Round(toRound / multipleOf) * multipleOf;
        }

        public static int FloorToMultipleOf(float toRound, int multipleOf) => (int)FloorToMultipleOf(toRound, (float)multipleOf);
        public static float FloorToMultipleOf(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }
            return Mathf.Floor(toRound / multipleOf) * multipleOf;
        }

        public static int CeilToMultipleOf(float toRound, int multipleOf) => (int)CeilToMultipleOf(toRound, (float)multipleOf);
        public static float CeilToMultipleOf(float toRound, float multipleOf)
        {
            if (multipleOf == 0)
            {
                throw new System.Exception("Cannot round to a multiple of 0.");
            }
            return Mathf.Ceil(toRound / multipleOf) * multipleOf;
        }

        public static float RoundTowards(float toRound, int towards)
        {
            if (toRound >= towards)
            {
                return Mathf.Floor(toRound);
            }
            return Mathf.Ceil(toRound);
        }
        public static int RoundToIntTowards(float toRound, int towards)
        {
            if (toRound >= towards)
            {
                return Mathf.FloorToInt(toRound);
            }
            return Mathf.CeilToInt(toRound);
        }

        public static float RoundAwayFrom(float toRound, int awayFrom)
        {
            if (toRound >= awayFrom)
            {
                return Mathf.Ceil(toRound);
            }
            return Mathf.Floor(toRound);
        }
        public static int RoundToIntAwayFrom(float toRound, int awayFrom)
        {
            if (toRound >= awayFrom)
            {
                return Mathf.CeilToInt(toRound);
            }
            return Mathf.FloorToInt(toRound);
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

        public static float TruncateDecimalPlaces(float f, int decimalPlaces)
        {
            int integerPart = UtilFunctions.RoundToIntTowards(f, 0);
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

        /// <summary>
        /// Compares the arrays element-wise using .Equals()
        /// </summary>
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
            return "{" + string.Join(", ", array) + "}";
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
