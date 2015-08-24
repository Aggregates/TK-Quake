using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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
        public Matrix4 Matrix { get; set; }

        public Camera()
        {
            MoveSpeed = 0.1;
            RotationSpeed = 0.01;
            Position = Vector.Zero;

            Components.Add(new CameraComponent(this));
        }

        public override void Rotate(double dx, double dy, double dz)
        {
            // Stop the angle from exceeding 45degees

            dx = (Rotation.X + dx > Math.PI / 2) ? 0 : dx;
            dx = (Rotation.X + dx < -Math.PI / 2) ? 0 : dx;

            base.Rotate(dx, dy, dz);
        }
    }

    class CameraComponent : IComponent
    {
        private readonly Camera _camera;
        public CameraComponent(Camera camera)
        {
            _camera = camera;
        }

        public void Startup()
        {
            
        }

        public void Shutdown()
        {
            
        }

        public void Update(double elapsedTime)
        {
            _camera.Matrix = WorldToLocalMatrix();

            GL.MatrixMode(MatrixMode.Modelview);

            // Unable to directly pass Matrix property as ref object
            // http://stackoverflow.com/a/4519028
            Matrix4 mat = _camera.Matrix;
            GL.LoadMatrix(ref mat);
        }

        /// <summary>
        /// Calculates the matrix orientation with which to render the world through the camera's eyes at the current position
        /// </summary>
        /// <returns>The local view matrix represenation of the camera</returns>
        public Matrix4 WorldToLocalMatrix()
        {
            // When the YawAngle is 0, the camera will look down the negative Z axis
            return GLX.MarixLookAt(_camera.Position, _camera.Position + _camera.ViewDirection, Vector.UnitY);
        }
    }
}
