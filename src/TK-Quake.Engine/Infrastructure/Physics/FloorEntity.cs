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
    /// <summary>
    /// A horizontal plane that an Entity can walk along
    /// </summary>
    public class FloorEntity : RenderableEntity
    {
        /// <summary>
        /// Gets or sets the Length of the Floor along the X-Axis, starting from the Floor's X-Position
        /// </summary>
        public float XLength { get; set; }

        /// <summary>
        /// Gets or sets the Length of the Floor along the Z-Axis, starting from the Floor's Z-Position
        /// </summary>
        public float ZLength { get; set; }

        /// <summary>
        /// Gets or Sets whether to render the floor's grid and bounding box volume to the screen
        /// </summary>
        public bool Render
        {
            get { return _render; }
            set
            {
                _render = value;
                var box = Children.OfType<BoundingBoxEntity>().FirstOrDefault();
                var grid = Components.OfType<FloorGridComponent>().FirstOrDefault();
                if (box != null) box.Render = value;
                if (grid != null) grid.Render = value;
            }
        }
        private bool _render;

        /// <summary>
        /// Constructor for a FloorEntity. Sets the instance data
        /// </summary>
        /// <param name="anchor">The point in 3D space to anchor the floor at</param>
        /// <param name="xLength">The length of the floor along the x-axis from the anchor point</param>
        /// <param name="zLength">The length of the floow along thw z-axis from the anchro point</param>
        /// <param name="id"></param>
        /// <param name="render">Whether to render the grid and bounding box or not</param>
        public FloorEntity(Vector3 anchor, float xLength, float zLength, string id, bool render = false)
        {
            this.Position = anchor;
            this.XLength = xLength;
            this.ZLength = zLength;
            this.Render = render;

            this.Id = id;

            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            // Define the Components
            FloorGridComponent grid = new FloorGridComponent(this, 1f, _render);
            BoundingBoxEntity box = new BoundingBoxEntity(this,
                new Vector3(XLength, 0.1f, 0),
                new Vector3(0, -0.1f, ZLength), _render) // This seems to work pretty well, but it's a bit hacky
                ;

            // Set up event handling
            box.Collided += Box_Collided;

            // Add to the Components list
            Components.Add(grid);
            Components.Add(new RenderComponent(this));

            Children.Add(box);
        }

        /// <summary>
        /// Handler for when an entity collides with the floor.
        /// Sets the entity's vertical position to the floor's y-position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_Collided(object sender, CollisionEventArgs e)
        {
            // Get the other object
            var entity = e.Sender;
            if (this == e.Sender) entity = e.Collider;

            // If two floors are overlapping, don't move the floor
            if (!(entity is FloorEntity) && entity.Position.Y <= this.Position.Y)
            {
                // Stop the other object from falling through
                entity.Position = new Vector3(entity.Position.X, this.Position.Y, entity.Position.Z);
                var gravity = entity.Components.OfType<GravityComponent>().FirstOrDefault();

                // Reset gravity's velocity so it doesn't accumulate whilst on floor
                if (gravity != null)
                    gravity.Velocity = 0;
            }
        }

    }

    /// <summary>
    /// Renders a grid for the floor using OpenGL Immediate mode calls
    /// </summary>
    class FloorGridComponent : IComponent
    {
        private FloorEntity _entity;
        private float _lineSpacing;

        /// <summary>
        /// Gets or sets whether to render the Floor Grid Component or not
        /// </summary>
        public bool Render { get; set; }

        /// <summary>
        /// Constructor for the Floor Grid Component. Sets the instance variables
        /// </summary>
        /// <param name="entity">The entity the component is attached to</param>
        /// <param name="lineSpacing">The spacing between grid lines</param>
        /// <param name="render">Whether to render the grid component or not</param>
        public FloorGridComponent(FloorEntity entity, float lineSpacing, bool render = false)
        {
            this._entity = entity;
            this._lineSpacing = lineSpacing;
            this.Render = render;
        }

        public void Startup() { }
        public void Shutdown() { }

        /// <summary>
        /// Render's the Floor Grid Component using OpenGL Immediate Mode
        /// </summary>
        /// <param name="elapsedTime"></param>
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
