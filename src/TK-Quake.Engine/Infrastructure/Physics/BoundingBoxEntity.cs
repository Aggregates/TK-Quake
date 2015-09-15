using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Physics
{
    /// <summary>
    /// Defines a 3D volume which completely surrounds an entity.
    /// Enables collision of entities by registering itself with the CollisionDetector
    /// </summary>
    public class BoundingBoxEntity : Entity
    {

        /*
             * Looks like this


                  (6)  ------------- (7)
                      /|          /|
                     / |         / |
                    /  |        /  |
                (5) ------------   | (4)
                    |  |       |   |
                (1) |  /-------|---/ (2)
                    | /        |  /
                    |/         | /
                (0) ------------/ (3)


             */


        /// <summary>
        /// The top-right-far coordinate of the volume in 3D Space. Relative to the Origin.
        /// </summary>
        public Vector3 Top { get; private set; }

        /// <summary>
        /// The bottom-left-near coordinate of the volume in 3D Space. Relative to the Origin.
        /// </summary>
        public Vector3 Bottom { get; private set; }

        /// <summary>
        /// The width of the bounding box along the X-Axis
        /// </summary>
        public float Width { get { return System.Math.Abs(Top.X - Bottom.X); } }

        /// <summary>
        /// The height of the bounding box along the Y-Axis
        /// </summary>
        public float Height { get { return System.Math.Abs(Top.Y - Bottom.Y); } }

        /// <summary>
        /// The depth of the bounding box along the Z-Axis
        /// </summary>
        public float Depth { get { return System.Math.Abs(Top.Z - Bottom.Z); } }

        public Vector3 Center { get { return _entity.Position + (Top - Bottom) / 2.0f; } }

        /// <summary>
        /// Gets or Sets whether to render the bounding box volume to the screen
        /// </summary>
        public bool Render {
            get { return _render; }
            set
            {
                _render = value;
                var box = Components.OfType<BoundingBoxDrawComponent>().FirstOrDefault();
                if (box != null) box.Render = value;
            }
        }

        private Entity _entity;
        private Vector4 _initialTop;
        private Vector4 _initialBottom;
        private bool _render;

        // Event Handling
        public delegate void OnCollidedHandler(object sender, CollisionEventArgs e);
        public event OnCollidedHandler Collided;

        /// <summary>
        /// Constructor for a bounding box. Defines the volume of the box and registers
        /// with the CollisionDetector to enable collision detection.
        /// </summary>
        /// <param name="entity">The parent entity the box is attached to</param>
        /// <param name="top">The Top-Right-Far 3D Coordinate of the box volume. Relative to the Origin.</param>
        /// <param name="bottom">The Bottom-Left-Near 3D Coordinate of the box volume. Relative to the Origin.</param>
        /// <param name="render">Whether to render the bounding box volume or not</param>
        public BoundingBoxEntity(Entity entity, Vector3 top, Vector3 bottom, bool render = false)
        {
            this._entity = entity;
            this.Top = top;
            this.Bottom = bottom;
            this.Render = render;

            this._initialTop = new Vector4(top, 0);
            this._initialBottom = new Vector4(bottom, 0);

            // Resgister with Collision Detector
            CollisionDetector.Singleton().RegisterCollider(this);
            InitialiseComponents();
        }

        /// <summary>
        /// Attaches all Components required to use the bounding box by default
        /// </summary>
        private void InitialiseComponents()
        {
            BoundingBoxDrawComponent box = new BoundingBoxDrawComponent(this, Render);
            this.Components.Add(box);
        }

        /// <summary>
        /// The Collision detection routine for two Bounding Boxes.
        /// Checks the positions of the two boxes with respect to their
        /// dimensions to detect overlap
        /// </summary>
        /// <param name="box">The BoundingBox to overlap with</param>
        /// <returns></returns>
        public bool CheckCollision(BoundingBoxEntity box)
        {
            var xDiff = this.Top.X - box.Top.X;
            var yDiff = this.Top.Y - box.Top.Y;
            var zDiff = this.Top.Z - box.Top.Z;

            // Check the X axis
            if (xDiff > 0 && System.Math.Abs(xDiff) < this.Width)
            {
                // Check the Y axis
                if (System.Math.Abs(this.Top.Y - box.Top.Y) < System.Math.Abs(this.Height + box.Height))
                {
                    // Check the Z axis
                    if (zDiff < 0 && System.Math.Abs(zDiff) < this.Depth)
                    {
                        // The boxes collide. Send a message to both
                        Collided?.Invoke(this, new CollisionEventArgs(this._entity, box._entity));
                        box.Collided?.Invoke(box, new CollisionEventArgs(box._entity, this._entity));

                        return true;
                    }
                }
            }

            // Boxes do not collide
            return false;
        }

        /// <summary>
        /// Updates the bounding box's position and rotation based on the attached entitiy's information
        /// </summary>
        /// <param name="elapsedTime">The time since the last update</param>
        public override void Update(double elapsedTime)
        {

            /*
             * The bounding box is based on the position of the entity centered around the origin
             * As the entity moves away, we need to cater for this movement and move our bounding box
             * as well
             */

            Top = _initialTop.Xyz + _entity.Position;
            Bottom = _initialBottom.Xyz + _entity.Position;

            /*
             * Now we handle the rotation.
             * NOTE: Not currently working. Currently rotating around the origin, not the entity
             * The following code is left as a reference point when re-attempting this

            var translate = Matrix4.CreateTranslation(_entity.Position); // Bottom row contains tranlsation information
            var rotate = Matrix4.CreateRotationX(_entity.Rotation.X) *
                         Matrix4.CreateRotationY(_entity.Rotation.Y) *
                         Matrix4.CreateRotationZ(_entity.Rotation.Z);

            var mult = rotate * translate;

            Top = Vector4.Transform(new Vector4(Top, 0), mult).Xyz;
            Bottom = Vector4.Transform(new Vector4(Bottom, 0), mult).Xyz;
             
             */

            base.Update(elapsedTime);

        }

        /// <summary>
        /// Draws the Bounding-Box component using OpenGL
        /// </summary>
        class BoundingBoxDrawComponent : IComponent
        {
            private BoundingBoxEntity _box;
            public bool Render {get; set;}

            /// <summary>
            /// Constructor for the Draw Component. Sets the instance's variables
            /// </summary>
            /// <param name="box">The Bounding Box to attach and render to</param>
            /// <param name="render">Whether to render the volume or not</param>
            public BoundingBoxDrawComponent(BoundingBoxEntity box, bool render = false)
            {
                this._box = box;
                this.Render = render;
            }

            public void Shutdown() { }

            public void Startup() { }

            public void Update(double elapsedTime)
            {

                if (Render)
                {
                    // Calculate the 8 corners
                    var corners = new List<Vector3>();
                    corners.Add(_box.Bottom);
                    corners.Add(new Vector3(_box.Bottom.X, _box.Bottom.Y, _box.Top.Z));
                    corners.Add(new Vector3(_box.Top.X, _box.Bottom.Y, _box.Top.Z));
                    corners.Add(new Vector3(_box.Top.X, _box.Bottom.Y, _box.Bottom.Z));

                    corners.Add(new Vector3(_box.Top.X, _box.Top.Y, _box.Bottom.Z));
                    corners.Add(new Vector3(_box.Bottom.X, _box.Top.Y, _box.Bottom.Z));
                    corners.Add(new Vector3(_box.Bottom.X, _box.Top.Y, _box.Top.Z));
                    corners.Add(_box.Top);

                    // Create the 12 lines between each corner
                    var lines = new List<Line3>();
                    lines.Add(new Line3(corners[0], corners[1]));
                    lines.Add(new Line3(corners[1], corners[2]));
                    lines.Add(new Line3(corners[2], corners[3]));
                    lines.Add(new Line3(corners[3], corners[0]));

                    lines.Add(new Line3(corners[4], corners[5]));
                    lines.Add(new Line3(corners[5], corners[6]));
                    lines.Add(new Line3(corners[6], corners[7]));
                    lines.Add(new Line3(corners[7], corners[4]));

                    lines.Add(new Line3(corners[0], corners[5]));
                    lines.Add(new Line3(corners[1], corners[6]));
                    lines.Add(new Line3(corners[2], corners[7]));
                    lines.Add(new Line3(corners[3], corners[4]));

                    // Draw the volume
                    GL.Begin(PrimitiveType.LineStrip); // Or Lines
                    {
                        GL.Color3(0f, 255f, 0f);
                        GL.Enable(EnableCap.DepthTest);
                        foreach (var l in lines)
                        {
                            GL.Vertex3(l.A);
                            GL.Vertex3(l.B);
                        }
                    }
                    GL.End();
                }
            }
        }
    }
}
