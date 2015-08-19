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

        public Camera()
        {
            MoveSpeed = 0.2;
            RotationSpeed = 0.2;
            Position = Vector.Zero;
            ViewDirection = new Vector(0, 0, -1);
            this.Matrix = Matrix4.Identity;
        }

        public override void Move(double x, double y, double z)
        {
            // This code comes from
            // http://neokabuto.blogspot.com.au/2014/01/opentk-tutorial-5-basic-camera.html
            // The line "offset.Y += z;" looks suspicious

            Vector offset = Vector.Zero;
            offset += OrthogonalView * x;
            offset += ViewDirection* y;
            offset.Y += z;

            offset.Normalise();
            offset *= MoveSpeed;
            Position += offset;

        }

        public Matrix4 WorldToLocalMatrix()
        {
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
