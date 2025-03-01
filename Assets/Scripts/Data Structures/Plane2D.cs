using System;

using PAC.Exceptions;

namespace PAC.DataStructures
{
    /// <summary>
    /// Provides ways to deal with the 2D plane.
    /// </summary>
    public static class Plane2D
    {
        /// <summary>
        /// One of 8 regions of the 2D plane.
        /// </summary>
        [Flags]
        public enum Octant : byte
        {
            /// <summary>
            /// The north-northeast octant.
            /// </summary>
            NorthNortheast = 1,
            /// <summary>
            /// The east-northeast octant.
            /// </summary>
            EastNortheast = 1 << 1,
            /// <summary>
            /// The east-southeast octant.
            /// </summary>
            EastSoutheast = 1 << 2,
            /// <summary>
            /// The south-southeast octant.
            /// </summary>
            SouthSoutheast = 1 << 3,
            /// <summary>
            /// The south-southwest octant.
            /// </summary>
            SouthSouthwest = 1 << 4,
            /// <summary>
            /// The west-southwest octant.
            /// </summary>
            WestSouthwest = 1 << 5,
            /// <summary>
            /// The west-northwest octant.
            /// </summary>
            WestNorthwest = 1 << 6,
            /// <summary>
            /// The north-northwest octant.
            /// </summary>
            NorthNorthwest = 1 << 7,
        }

        /// <summary>
        /// Returns which <see cref="Octant"/>(s) the given point is in.
        /// </summary>
        /// <remarks>
        /// For this method, <see cref="Octant"/>s include their boundaries and hence overlap. For example, <see cref="Octant.NorthNortheast"/> and <see cref="Octant.NorthNorthwest"/> both
        /// contain the half-line <c>{ (0, y) : y >= 0 }</c>. In this case, the flag nature of <see cref="Octant"/> is used to return multiple values.
        /// </remarks>
        public static Octant GetOctant(IntVector2 point)
        {
            Octant octant = 0;

            if (point.y >= point.x)
            {
                if (point.x >= 0)
                {
                    octant |= Octant.NorthNortheast;
                }
                if (point.y <= 0)
                {
                    octant |= Octant.WestSouthwest;
                }
            }
            if (point.y <= point.x)
            {
                if (point.x <= 0)
                {
                    octant |= Octant.SouthSouthwest;
                }
                if (point.y >= 0)
                {
                    octant |= Octant.EastNortheast;
                }
            }
            if (point.y >= -point.x)
            {
                if (point.x <= 0)
                {
                    octant |= Octant.NorthNorthwest;
                }
                if (point.y <= 0)
                {
                    octant |= Octant.EastSoutheast;
                }
            }
            if (point.y <= -point.x)
            {
                if (point.x >= 0)
                {
                    octant |= Octant.SouthSoutheast;
                }
                if (point.y >= 0)
                {
                    octant |= Octant.WestNorthwest;
                }
            }

            if (octant == 0)
            {
                throw new UnreachableException();
            }
            return octant;
        }
    }
}