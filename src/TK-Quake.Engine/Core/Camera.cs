using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using TKQuake.Engine.Extensions;
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
        private readonly Renderer _renderer = Renderer.Singleton();

        public CameraComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void Startup() { }
        public void Shutdown() { }

        private double _time;
        public void Update(double elapsedTime)
        {
            var program = _renderer.Program;

            /*
            var uniModel = GL.GetUniformLocation(program, "model");
            var model = Matrix4.Identity;
            GL.UniformMatrix4(uniModel, false, ref model);

            var view = GLX.MatrixLookAt(_entity.Position, _entity.Position + _entity.ViewDirection, Vector3.UnitY);
            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref view);

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 800f/600f, 1f, 10f);
            var uniProj = GL.GetUniformLocation(program, "proj");
            GL.UniformMatrix4(uniProj, false, ref proj);
            */

            _time += elapsedTime;
            var model = Matrix4.CreateRotationZ((float)_time * MathHelper.PiOver2);
            var uniModel = GL.GetUniformLocation(program, "model");
            GL.UniformMatrix4(uniModel, false, ref model);

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 800f / 600f, 1f, 10f);
            var uniProj = GL.GetUniformLocation(program, "proj");
            GL.UniformMatrix4(uniProj, false, ref proj);

            var view = Matrix4.LookAt(new Vector3(1.2f, 1.2f, 1.2f) * 3, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref view);

            /*
            var mat = GLX.MatrixLookAt(_entity.Position, _entity.Position + _entity.ViewDirection, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);

            // http://stackoverflow.com/a/4519028
            GL.LoadMatrix(ref mat);
            */


        }
    }
}
