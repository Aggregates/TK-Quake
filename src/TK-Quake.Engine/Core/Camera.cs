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
    public class Camera : PlayerEntity
    {
        /// <summary>
        /// Unit Vector from the Position to any point in space, one unit away,
        /// that defines where the Camera is looking
        /// </summary>
        public Vector ViewDirection { get; set; }

        /// <summary>
        /// The Vector orthogonal to the view direction
        /// </summary>
        public Vector OrthogonalView { get { return ViewDirection.CrossProduct(Up); } }

        public readonly Vector Up = Vector.UnitY;

        /// <summary>
        /// The angle (in radians) the camera is facing by rotating around the
        /// upward Y-Axis as if the camera moves its "head" left to right
        /// </summary>
        public double YawAngle { get; set; }

        /// <summary>
        /// The angle (in radians) the camera is facing by rotating around the
        /// X-Axis as if the camera tilts it's "head" up and down
        /// </summary>
        public double PitchAngle { get; set; }

        public Camera()
        {
            MoveSpeed = 0.2;
            RotationSpeed = 0.2;
            Position = Vector.Zero;
            ViewDirection = new Vector(0, 0, -1);
            YawAngle = 0;
            PitchAngle = 0;
            this.Matrix = WorldToLocalMatrix();
        }

        public Matrix4 WorldToLocalMatrix()
        {
            // When the YawAngle is 0, the camera will look down the negative Z axis

            ViewDirection = new Vector(Math.Sin(YawAngle), Math.Sin(PitchAngle), -Math.Cos(YawAngle));
            ViewDirection.Normalise();
            return GLX.MarixLookAt(Position, Position + ViewDirection, Up);
        }

        public override void Update(double elapsedTime)
        {
            this.Matrix = WorldToLocalMatrix();
        }

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
