using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Infrastructure.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector2 LightMapCoord;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, Vector2 lightMapCoord)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            LightMapCoord = lightMapCoord;
        }
    }
}
