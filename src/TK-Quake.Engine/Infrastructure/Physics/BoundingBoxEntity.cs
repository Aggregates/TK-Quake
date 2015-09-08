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


        public Vector3 Top { get; private set; }
        public Vector3 Bottom { get; private set; }

        public float Width { get { return Top.X - Bottom.X; } }
        public float Height { get { return Top.Y - Bottom.Y; } }
        public float Depth { get { return Top.Z - Bottom.Z; } }

        private Entity _entity { get; set; }
        private Vector4 _initialTop;
        private Vector4 _initialBottom;

        // Event Handling
        public delegate void OnCollidedHandler(object sender, CollisionEventArgs e);
        public event OnCollidedHandler Collided;

        public BoundingBoxEntity(Entity entity, Vector3 top, Vector3 bottom)
        {
            this._entity = entity;
            this.Top = top;
            this.Bottom = bottom;

            this._initialTop = new Vector4(top, 0);
            this._initialBottom = new Vector4(bottom, 0);

            // Resgister with Collision Detector
            CollisionDetector.Singleton().RegisterCollider(this);
            InitialiseComponents();
        }

        private void InitialiseComponents()
        {
            BoundingBoxDrawComponent box = new BoundingBoxDrawComponent(this, true);
            this.Components.Add(box);
        }

        public bool CheckCollision(BoundingBoxEntity box)
        {
            // Check the X axis
            if (System.Math.Abs(this.Top.X - box.Top.X) < System.Math.Abs(this.Width + box.Width))
            {
                // Check the Y axis
                if (System.Math.Abs(this.Top.Y - box.Bottom.Y) < System.Math.Abs(this.Height + box.Height))
                {
                    // Check the Z axis
                    if (System.Math.Abs(this.Top.Z - box.Bottom.Z) < System.Math.Abs(this.Depth + box.Depth))
                    {
                        if (Collided != null)
                        {
                            Collided(this, new CollisionEventArgs(this._entity, box._entity));
                            if (box.Collided != null)
                                box.Collided(box, new CollisionEventArgs(box._entity, this._entity));
                        }
                        return true;
                    }
                }
            }

            return false;
        }

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
             */

            var translate = Matrix4.CreateTranslation(_entity.Position); // Bottom row contains tranlsation information
            var rotate = Matrix4.CreateRotationX(_entity.Rotation.X) *
                         Matrix4.CreateRotationY(_entity.Rotation.Y) *
                         Matrix4.CreateRotationZ(_entity.Rotation.Z);

            var mult = rotate * translate;

            //Top = Vector4.Transform(new Vector4(Top, 0), mult).Xyz;
            //Bottom = Vector4.Transform(new Vector4(Bottom, 0), mult).Xyz;

            base.Update(elapsedTime);

        }

        class BoundingBoxDrawComponent : IComponent
        {
            private BoundingBoxEntity _box;
            public bool Render {get; set;}

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
                    var corners = new List<Vector3>();
                    corners.Add(_box.Bottom);
                    corners.Add(new Vector3(_box.Bottom.X, _box.Bottom.Y, _box.Top.Z));
                    corners.Add(new Vector3(_box.Top.X, _box.Bottom.Y, _box.Top.Z));
                    corners.Add(new Vector3(_box.Top.X, _box.Bottom.Y, _box.Bottom.Z));

                    corners.Add(new Vector3(_box.Top.X, _box.Top.Y, _box.Bottom.Z));
                    corners.Add(new Vector3(_box.Bottom.X, _box.Top.Y, _box.Bottom.Z));
                    corners.Add(new Vector3(_box.Bottom.X, _box.Top.Y, _box.Top.Z));
                    corners.Add(_box.Top);

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
                    GL.Color3(255f, 255, 255);
                }
            }
        }
    }
}
