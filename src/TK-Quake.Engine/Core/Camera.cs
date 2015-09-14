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

            var radians = MathHelper.DegreesToRadians(50f*(float) _time);
            var model = Matrix4.CreateTranslation(0.5f, 1f, 0f)*Matrix4.CreateRotationX(radians);
            var uniModel = GL.GetUniformLocation(program, "model");
            GL.UniformMatrix4(uniModel, false, ref model);

            //left to right handed
            var eye = new Vector3(_entity.Position.X, _entity.Position.Y, _entity.Position.Z);
            //var view = Matrix4.LookAt(eye, Vector3.Zero, new Vector3(0, 0, 1));
            var view = Matrix4.CreateTranslation(0, 0, -20);
            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref view);

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 800f / 600f, 0.1f, 100f);
            var uniProj = GL.GetUniformLocation(program, "proj");
            GL.UniformMatrix4(uniProj, false, ref proj);
        }
    }
}
