using GameLoop.Engine.Extensions;
using GameLoop.Engine.Infrastructure.Abstract;
using GameLoop.Engine.Infrastructure.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.ViewPort
{
    public class Camera : IGameObject
    {
        public Vector Position { get; set; }
        public double MoveSpeed { get; set; }
        public double RotationSpeed { get; set; }

        /// <summary>
        /// Unit Vector from the Position to any point in space, one unit away
        /// </summary>
        public Vector ViewDirection { get; set; }

        /// <summary>
        /// The Vector orthogonal to the view direction
        /// </summary>
        public Vector OrthogonalView { get { return ViewDirection * Up; } }

        public readonly Vector Up = Vector.UnitY;

        private Matrix4 _cameraMatrix;

        public Camera()
        {
            this.MoveSpeed = 0.2;
            this.RotationSpeed = 0.01;
            this.Position = Vector.Zero;
            this.ViewDirection = new Vector(0, 0, -1);
            this._cameraMatrix = GetWorldToViewMatrix();
        }

        public void Move(double x, double y, double z)
        {
            Vector offset = Vector.Zero;

            offset += OrthogonalView * x;
            offset += ViewDirection * y;
            offset.Y += z; // ?????

        }

        public void Move(Vector newPosition)
        {
            Vector delta = newPosition - Position;
            //this.ViewDirection = Matrix4.MGL.Rotate(delta.X, new Vector3d(Up.X, Up.Y, Up.Z)) * ViewDirection;
        }

        public Matrix4 GetWorldToViewMatrix()
        {
            return GLX.MarixLookAt(Position, Position + ViewDirection, Up);
        }

        public void Update(double elapsedTime)
        {
            this._cameraMatrix = GetWorldToViewMatrix();
        }

        public void Render()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref _cameraMatrix);
        }
    }
}
