using UnityEngine;

namespace PAC.Extensions.UnityEngine
{
    public static class Vector2Extensions
    {
        public static void Deconstruct(this Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }
    }
}