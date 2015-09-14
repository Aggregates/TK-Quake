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

            _time += elapsedTime;
            var model = Matrix4.CreateRotationZ((float)_time * MathHelper.PiOver4);
            var uniModel = GL.GetUniformLocation(program, "model");
            GL.UniformMatrix4(uniModel, false, ref model);

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 800f / 600f, 1f, 100f);
            var uniProj = GL.GetUniformLocation(program, "proj");
            GL.UniformMatrix4(uniProj, false, ref proj);

            //left to right handed
            var eye = new Vector3(_entity.Position.X, _entity.Position.Z, _entity.Position.Y);
            var view = Matrix4.LookAt(eye, _entity.ViewDirection, new Vector3(0, 0, 1));
            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref view);
        }
    }
}
