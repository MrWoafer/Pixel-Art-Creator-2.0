using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAC
{
    public static class Functions
    {
        public static Vector2 Vector3ToVector2(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }
        public static Vector3 Vector2ToVector3(Vector3 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0f);
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

        public static T[] ToArray<T>(this HashSet<T> hashSet)
        {
            T[] array = new T[hashSet.Count];
            hashSet.CopyTo(array);
            return array;
        }
    }
}
