using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Math
{
    /// <summary>
    /// Representation of a line segment in 3D space. Does not extend infinitely.
    /// Useful for drawing lines in OpenGL
    /// </summary>
    public class Line3
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Line3(Vector3 a, Vector3 b)
        {
            this.A = a;
            this.B = b;
        }
    }
}
