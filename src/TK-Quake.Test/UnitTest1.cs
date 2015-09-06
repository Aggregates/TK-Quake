using NUnit.Framework;
using OpenTK;
using System;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake.Test
{
    public class OctreeObject : Entity
    {
        public int Id { get; set; }
        public override string ToString() { return "ID:" + Id; }
    }

    [TestFixture]
    public class OctreeTests
    {
        [Test]
        public void TestAdd_000()
        {
            Vector3 c1 = new Vector3(1,1,1);
            Vector3 c2 = new Vector3(-1,-1,-1);

            OctreeObject obj1 = new OctreeObject() { Id = 1, Position = new Vector3(0.25f, 0.25f, 0.25f) };
            OctreeObject obj2 = new OctreeObject() { Id = 2, Position = new Vector3(0.75f, 0.75f, 0.75f) };
            OctreeObject obj3 = new OctreeObject() { Id = 3, Position = new Vector3(-0.75f, -0.75f, -0.75f) };

            Octree tree = new Octree(c1, c2);
            //tree.Add(ref obj1);
            //tree.Add(ref obj2);
            //tree.Add(ref obj3);

        }
    }
}
