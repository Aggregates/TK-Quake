using GameLoop.Engine.Infrastructure.Math;
using GameLoop.Engine.Infrastructure.Texture;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Extensions
{
    /// <summary>
    /// Extensionn class to OpenTK
    /// </summary>
    public class GLX
    {
        public static void Color3(Color c)
        {
            GL.Color3(c.R, c.G, c.B);
        }

        public static Matrix4 MarixLookAt(Vector position, Vector viewDirection, Vector up)
        {
            Vector3 pos = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
            Vector3 view = new Vector3((float)viewDirection.X, (float)viewDirection.Y, (float)viewDirection.Z);
            Vector3 upVec = new Vector3((float)up.X, (float)up.Y, (float)up.Z);

            return Matrix4.LookAt(pos, pos + view, upVec);
        }
    }
}
