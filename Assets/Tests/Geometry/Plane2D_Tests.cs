using NUnit.Framework;

using PAC.Geometry;

namespace PAC.Tests.Geometry
{
    public static class Plane2D_Tests
    {
        /// <summary>
        /// Tests <see cref="Plane2D.GetOctant(IntVector2)"/>.
        /// </summary>
        [Test]
        public static void GetOctant()
        {
            Assert.AreEqual(Plane2D.Octant.NorthNortheast, Plane2D.GetOctant((1, 2)));
            Assert.AreEqual(Plane2D.Octant.EastNortheast, Plane2D.GetOctant((3, 2)));
            Assert.AreEqual(Plane2D.Octant.EastSoutheast, Plane2D.GetOctant((5, -3)));
            Assert.AreEqual(Plane2D.Octant.SouthSoutheast, Plane2D.GetOctant((2, -7)));
            Assert.AreEqual(Plane2D.Octant.SouthSouthwest, Plane2D.GetOctant((-3, -4)));
            Assert.AreEqual(Plane2D.Octant.WestSouthwest, Plane2D.GetOctant((-100, -43)));
            Assert.AreEqual(Plane2D.Octant.WestNorthwest, Plane2D.GetOctant((-2, 1)));
            Assert.AreEqual(Plane2D.Octant.NorthNorthwest, Plane2D.GetOctant((-5, 200)));

            Assert.AreEqual(Plane2D.Octant.NorthNortheast | Plane2D.Octant.EastNortheast, Plane2D.GetOctant((1, 1)));
            Assert.AreEqual(Plane2D.Octant.EastNortheast | Plane2D.Octant.EastSoutheast, Plane2D.GetOctant((2, 0)));
            Assert.AreEqual(Plane2D.Octant.EastSoutheast | Plane2D.Octant.SouthSoutheast, Plane2D.GetOctant((3, -3)));
            Assert.AreEqual(Plane2D.Octant.SouthSoutheast | Plane2D.Octant.SouthSouthwest, Plane2D.GetOctant((0, -1)));
            Assert.AreEqual(Plane2D.Octant.SouthSouthwest | Plane2D.Octant.WestSouthwest, Plane2D.GetOctant((-2, -2)));
            Assert.AreEqual(Plane2D.Octant.WestSouthwest | Plane2D.Octant.WestNorthwest, Plane2D.GetOctant((-10, 0)));
            Assert.AreEqual(Plane2D.Octant.WestNorthwest | Plane2D.Octant.NorthNorthwest, Plane2D.GetOctant((-45, 45)));
            Assert.AreEqual(Plane2D.Octant.NorthNorthwest | Plane2D.Octant.NorthNortheast, Plane2D.GetOctant((0, 4)));

            const Plane2D.Octant AllOctants = Plane2D.Octant.NorthNortheast | Plane2D.Octant.EastNortheast | Plane2D.Octant.EastSoutheast | Plane2D.Octant.SouthSoutheast
                | Plane2D.Octant.SouthSouthwest | Plane2D.Octant.WestSouthwest | Plane2D.Octant.WestNorthwest | Plane2D.Octant.NorthNorthwest;

            Assert.AreEqual(AllOctants, Plane2D.GetOctant((0, 0)));
        }
    }
}