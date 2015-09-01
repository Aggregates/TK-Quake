using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;
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

        /// <summary>
        /// Extension of Matrix4.LookAt to work with engine defined vectors. When calling this
        /// method, make sure to add the Position to the LookAt normalised directional vector
        /// for the viewDirection argument.
        /// </summary>
        /// <param name="position">The current position of the entity</param>
        /// <param name="viewDirection">The position added to the LookAt direction</param>
        /// <param name="up">The upward axis direction</param>
        /// <returns></returns>
        public static Matrix4 MatrixLookAt(Vector3 position, Vector3 viewDirection, Vector3 up)
        {
            var pos = new Vector3(position.X, position.Y, position.Z);
            var view = new Vector3(viewDirection.X, viewDirection.Y, viewDirection.Z);
            var upVec = new Vector3(up.X, up.Y, up.Z);

            return Matrix4.LookAt(pos, view, upVec);
        }
    }
}
