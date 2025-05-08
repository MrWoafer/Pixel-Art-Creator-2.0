using UnityEngine;

namespace PAC.Extensions.UnityEngine
{
    public static class Vector3Extensions
    {
        public static void Deconstruct(this Vector3 vector, out float x, out float y, out float z)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }
}