using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Engine.Extensions
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
    }
}
