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
    public class BoundingBoxComponent : IComponent
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
        public bool Render { get; set; }

        public float Width { get { return Top.X - Bottom.X; } }
        public float Height { get { return Top.Y - Bottom.Y; } }
        public float Depth { get { return Top.Z - Bottom.Z; } }

        private Entity _entity { get; set; }

        public BoundingBoxComponent(Entity entity, Vector3 top, Vector3 bottom, bool render = false)
        {
            this._entity = entity;
            this.Top = top;
            this.Bottom = bottom;

            this.Render = render;

        }

        public void Startup() { }

        public void Shutdown() { }

        public void Update(double elapsedTime)
        {

            _entity.Rotation += new Vector3(1, 1, 1) * (float)elapsedTime;
            _entity.Position += new Vector3(1, 1, 1) * (float)elapsedTime;

            var rotate = Matrix4.CreateRotationX(_entity.Rotation.X) *
                         Matrix4.CreateRotationY(_entity.Rotation.Y) *
                         Matrix4.CreateRotationZ(_entity.Rotation.Z);

            var translate = Matrix4.CreateTranslation(_entity.Position / 8);
            var scale = Vector3.One * _entity.Scale;

            GL.PushMatrix();
            GL.MultMatrix(ref translate);
            GL.MultMatrix(ref rotate);
            GL.Scale(scale);

            if (Render)
            {

                var corners = new List<Vector3>();
                corners.Add(Bottom);
                corners.Add(new Vector3(Bottom.X, Bottom.Y, Top.Z));
                corners.Add(new Vector3(Top.X,    Bottom.Y, Top.Z));
                corners.Add(new Vector3(Top.X,    Bottom.Y, Bottom.Z));

                corners.Add(new Vector3(Top.X,    Top.Y, Bottom.Z));
                corners.Add(new Vector3(Bottom.X, Top.Y, Bottom.Z));
                corners.Add(new Vector3(Bottom.X, Top.Y, Top.Z));
                corners.Add(Top);

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

            GL.PopMatrix();
        }
    }
}
