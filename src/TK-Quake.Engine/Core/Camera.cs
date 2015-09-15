using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing.Drawing2D;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake.Engine.Core
{
    /// <summary>
    /// Representation of a virtual camera system in the world through which the player sees
    /// </summary>
    public class Camera : PlayerEntity
    {
        public Camera()
        {
            MoveSpeed = 1;
            RotationSpeed = 1;
            Position = new Vector3(0, 0.5f, 0);
            Rotation = -Vector3.UnitZ;

            CameraComponent cam = new CameraComponent(this);
            cam.PositionOffset = new Vector3(0, 3, 0);

            BoundingBoxEntity box = new BoundingBoxEntity(this,
                new Vector3(0.5f, 0.5f, 0.5f) + cam.PositionOffset,
                new Vector3(-0.5f, 0.0f, -0.5f), true);

            Components.Add(cam);
            Children.Add(box);
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

        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;

        public Vector3 PositionOffset { get; set; }

        public CameraComponent(IEntity entity)
        {
            _entity = entity;
            PositionOffset = new Vector3(0, 0, 0);
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            var program = _renderer.Program;
            var pos = _entity.Position + PositionOffset;

            var view = Matrix4.LookAt(pos, pos + _entity.ViewDirection, Vector3.UnitY);
            var uniView = GL.GetUniformLocation(program, "view");
            GL.UniformMatrix4(uniView, false, ref view);
        }
    }
}
