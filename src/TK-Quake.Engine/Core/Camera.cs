using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            MoveSpeed = 10;
            RotationSpeed = 1;
            Position = Vector3.Zero;

            Components.Add(new CameraComponent(this));
        }

        public override void Rotate(float dx, float dy, float dz)
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
            var renderer = Renderer.Singleton();
            var program = renderer.Program;

            var model = new Matrix4();
            var uniModel = GL.GetUniformLocation(program, "model");
            GL.UniformMatrix4(uniModel, false, ref model);

            var mat = GLX.MatrixLookAt(_entity.Position, _entity.Position + _entity.ViewDirection, Vector3.UnitY);

            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref mat);

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float) 4/3, 1f, 10f);
            var uniProg = GL.GetUniformLocation(program, "proj");
            GL.UniformMatrix4(uniProg, false, ref proj);
        }
    }
}
