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
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Components;

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
        private readonly IEntity _entity;
        public CameraComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            var mat = GLX.MarixLookAt(_entity.Position, _entity.Position + _entity.ViewDirection, Vector.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);

            // http://stackoverflow.com/a/4519028
            GL.LoadMatrix(ref mat);
        }
    }
}
