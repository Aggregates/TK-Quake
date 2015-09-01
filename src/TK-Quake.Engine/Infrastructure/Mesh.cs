using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKQuake.Engine.Infrastructure
{
    public class Mesh
    {
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] Textures { get; set; }
        public uint[] Indices { get; set; }
    }
}
