using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Texture
{
    /// <summary>
    /// A vertex in a sprite
    /// </summary>
    public class Vertex
    {
        public Vector Position { get; set; }
        public Color Color { get; set; }
        public Point UVs { get; set; }
    }
}
