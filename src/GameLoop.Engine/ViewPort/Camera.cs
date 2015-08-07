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

        /// <summary>
        /// Unit Vector from the Position to any point in space, one unit away
        /// </summary>
        public Vector ViewDirection { get; set; }

        /// <summary>
        /// The Vector orthogonal to the view direction
        /// </summary>
        public Vector OrthogonalView { get { return ViewDirection * Up; } }

        public readonly Vector Up = new Vector(0, 1, 0);

        private Matrix4 _cameraMatrix;

        public Camera()
        {
            this.Position = new Vector(0, 0, 0);
            this.ViewDirection = new Vector(0, 0, -1);
            this._cameraMatrix = GetWorldToViewMatrix();
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
