using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Core
{
    /// <summary>
    /// Representation of a virtual camera system in the world through which the player sees
    /// </summary>
    public class Camera : PlayerEntity
    {

        public Camera()
        {
            MoveSpeed = 0.1;
            RotationSpeed = 0.01;
            Position = Vector.Zero;
            this.Matrix = WorldToLocalMatrix();
        }

        /// <summary>
        /// Calculates the matrix orientation with which to render the world through the camera's eyes at the current position
        /// </summary>
        /// <returns>The local view matrix represenation of the camera</returns>
        public Matrix4 WorldToLocalMatrix()
        {
            // When the YawAngle is 0, the camera will look down the negative Z axis
            return GLX.MarixLookAt(Position, Position + ViewDirection, Vector.UnitY);
        }

        public override void Rotate(double dx, double dy, double dz)
        {
            // Stop the angle from exceeding 45degees

            dx = (Rotation.X + dx > Math.PI / 2) ? 0 : dx;
            dx = (Rotation.X + dx < -Math.PI / 2) ? 0 : dx;

            base.Rotate(dx, dy, dz);
        }

        /// <summary>
        /// Updates the camera's view frustum
        /// </summary>
        /// <param name="elapsedTime"></param>
        public override void Update(double elapsedTime)
        {
            this.Matrix = WorldToLocalMatrix();
        }

        /// <summary>
        /// Renders the world through the camera's eyes. Must be done before rendering other objects to the screen
        /// </summary>
        public override void Render()
        {
            GL.MatrixMode(MatrixMode.Modelview);

            // Unable to directly pass Matrix property as ref object
            // http://stackoverflow.com/a/4519028
            Matrix4 mat = Matrix;
            GL.LoadMatrix(ref mat);
        }

    }
}
