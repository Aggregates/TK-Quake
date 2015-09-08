using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class FloorEntity : Entity
    {
        public float XLength { get; set; }
        public float ZLength { get; set; }

        public FloorEntity(Vector3 anchor, float xLength, float zLength)
        {
            this.Position = anchor;
            this.XLength = xLength;
            this.ZLength = zLength;

            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            // Define the Components
            FloorGridComponent grid = new FloorGridComponent(this, 1f, true);
            BoundingBoxEntity box = new BoundingBoxEntity(this,
                new Vector3(0, 0 + 0.5f, 0), // This seems to work pretty well, but it's a bit hacky
                new Vector3(XLength, 0 - 0.5f, ZLength));

            // Set up event handling
            box.Collided += Box_Collided;

            // Add to the Components list
            Components.Add(grid);
            Children.Add(box);
        }

        private void Box_Collided(object sender, CollisionEventArgs e)
        {
            // Stop the other object from falling through
            var entity = e.Sender;
            if (this == e.Sender) entity = e.Collider;

            // If two floors are overlapping, don't move the floor
            if (!(entity is FloorEntity) && entity.Position.Y <= this.Position.Y)
            {
                entity.Position = new Vector3(entity.Position.X, this.Position.Y, entity.Position.Z);
                var gravity = entity.Components.OfType<GravityComponent>().FirstOrDefault();
                if (gravity != null)
                    gravity.Velocity = 0;
            }
        }

    }


    class FloorGridComponent : IComponent
    {
        private FloorEntity _entity;
        private float _lineSpacing;

        public bool Render { get; set; }

        public FloorGridComponent(FloorEntity entity, float lineSpacing, bool render = false)
        {
            this._entity = entity;
            this._lineSpacing = lineSpacing;
            this.Render = render;
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {

            if (Render)
            {
                // Calculate the number of lines to render
                var numZLines = System.Math.Abs(_entity.ZLength / _lineSpacing);
                var numXLines = System.Math.Abs(_entity.XLength / _lineSpacing);

                GL.Begin(PrimitiveType.Lines);
                GL.Color3(0f, 0, 255);

                // Parallel to x-axis
                for (int i = 0; i < numZLines + 1; i++)
                {
                    GL.Vertex3(_entity.Position.X, _entity.Position.Y, _entity.Position.Z + i * _lineSpacing);
                    GL.Vertex3(_entity.Position.X + _entity.XLength, _entity.Position.Y, _entity.Position.Z + i * _lineSpacing);
                }

                // Parallel to z-axis
                for (int i = 0; i < numXLines + 1; i++)
                {
                    GL.Vertex3(_entity.Position.X + i * _lineSpacing, _entity.Position.Y, _entity.Position.Z);
                    GL.Vertex3(_entity.Position.X + i * _lineSpacing, _entity.Position.Y, _entity.Position.Z + _entity.ZLength);
                }

                GL.Color3(255f, 255, 255);
                GL.End();
            }
        }
    }
}
