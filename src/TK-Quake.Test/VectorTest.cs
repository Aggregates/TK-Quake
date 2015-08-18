using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Test
{
    [TestClass]
    public class VectorTest
    {
        [TestMethod]
        public void CrossProduct_100_010_001()
        {
            Vector a = new Vector(1, 0, 0);
            Vector b = new Vector(0, 1, 0);

            Vector expected = new Vector(0, 0, 1);
            Vector actual = a.CrossProduct(b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CrossProduct_3N31_492_N15N239()
        {
            Vector a = new Vector(3, -3, 1);
            Vector b = new Vector(4, 9, 2);

            Vector expected = new Vector(-15, -2, 39);
            Vector actual = a.CrossProduct(b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CrossProduct_3N31_N1212N4_000()
        {
            Vector a = new Vector(3, -3, 1);
            Vector b = new Vector(-12, 12, -4);

            Vector expected = new Vector(0, 0, 0);
            Vector actual = a.CrossProduct(b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Angle_001_010_90()
        {
            Vector a = new Vector(1, 0, 0);
            Vector b = new Vector(0, 1, 0);

            double expected = Math.PI / 2;
            double actual = a.Angle(b);

            Assert.AreEqual(expected, actual);
        }

    }
}
