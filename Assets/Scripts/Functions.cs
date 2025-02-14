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
    }
}
