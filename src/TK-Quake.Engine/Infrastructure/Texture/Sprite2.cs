using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Texture
{
    /// <summary>
    /// A 2D Sprite
    /// </summary>
    public class Sprite2 : Entity
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point TopLeft
        {
            get { return Vertices[0].UVs; }
            set
            {
                SetUVs(value, Vertices[4].UVs);
            }
        }

        public Point BottomRight
        {
            get { return Vertices[4].UVs; }
            set
            {
                SetUVs(Vertices[0].UVs, value);
            }
        }

        private Texture _texture;
        public Texture Texture {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;
                SetPosition(
                    new Vector3(X + value.Center.X, Y + value.Center.Y, Z),
                    value.Width,
                    value.Height);
            }
        }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public Vector3 Center
        {
            get
            {
                var halfWidth = RenderWidth / 2;
                var halfHeight = RenderHeight / 2;
                return new Vector3(Vertices[0].Position.X + halfWidth,
                    Vertices[0].Position.Y - halfHeight,
                    Vertices[0].Position.Z);
            }
        }

        /// <summary>
        /// Texture Height * Scale
        /// </summary>
        public float Height
        {
            get { return (int)(Texture.Height * ScaleY); }
        }

        public float RenderHeight
        {
            get
            {
                return Vertices[0].Position.Y - Vertices[2].Position.Y;
            }
            set
            {
                SetPosition(Center, RenderWidth, value);
            }
        }


        /// <summary>
        /// Texture Width * Scale
        /// </summary>
        public float Width
        {
            get { return (int)(Texture.Width * ScaleX); }
        }

        public float RenderWidth
        {
            get
            {
                return Vertices[1].Position.X - Vertices[0].Position.X;
            }
            set
            {
                SetPosition(Center, value, RenderHeight);
            }
        }

        public List<Vertex> Vertices { get; set; }

        public Sprite2(Texture texture)
        {
            Vertices = new List<Vertex>();
            for (int i = 0; i < 6; i++ )
            {
                Vertices.Add(new Vertex());
            }

            SetPosition(new Vector3(0, 0, 0), 1, 1);
            SetUVs(new Point(0, 0), new Point(1, 1));
            SetColor(new Color(1, 1, 1, 1));
            SetScale(1);

            this.Texture = texture;
        }

        public void SetPosition(float x, float y)
        {
            SetPosition(new Vector3(x, y, 0));
        }

        public void SetPosition(Vector3 position)
        {
            SetPosition(position, this.Width, this.Height);
        }

        private void SetPosition(Vector3 position, float width, float height)
        {
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            // Use clockwise rotation for Vertex positioning

            // TopLeft, TopRight, BottomLeft
            Vertices[0].Position = new Vector3(position.X - halfWidth, position.Y + halfHeight, position.Z);
            Vertices[1].Position = new Vector3(position.X + halfWidth, position.Y + halfHeight, position.Z);
            Vertices[2].Position = new Vector3(position.X - halfWidth, position.Y - halfHeight, position.Z);

            // TopRight, BottomRight, BottomLeft
            Vertices[3].Position = new Vector3(position.X + halfWidth, position.Y + halfHeight, position.Z);
            Vertices[4].Position = new Vector3(position.X + halfWidth, position.Y - halfHeight, position.Z);
            Vertices[5].Position = new Vector3(position.X - halfWidth, position.Y - halfHeight, position.Z);
        }

        public void SetColor(Color color)
        {
            foreach (Vertex v in Vertices)
            {
                v.Color = color;
            }
        }

        public void SetUVs(Point topLeft, Point bottomRight)
        {
            //TopLeft, TopRight, BottomLeft
            Vertices[0].UVs = topLeft;
            Vertices[1].UVs = new Point(bottomRight.X, topLeft.Y);
            Vertices[2].UVs = new Point(topLeft.X, bottomRight.Y);

            //TopRight, BottomRight, BottomLeft
            Vertices[3].UVs = new Point(bottomRight.X, topLeft.Y);
            Vertices[4].UVs = bottomRight;
            Vertices[5].UVs = new Point(topLeft.X, bottomRight.Y);
        }

        /// <summary>
        /// Uniformly sets the scale value
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            this.ScaleX = scale;
            this.ScaleY = scale;
        }

        /// <summary>
        /// Scales the image uniformly in the x,y direction
        /// </summary>
        /// <param name="scale"></param>
        public void ScaleUniform(float scale)
        {
            this.ScaleX *= scale;
            this.ScaleY *= scale;
        }

        public virtual void Render()
        {
            var renderer = Renderer.Singleton();
            renderer.DrawSprite(this);
        }
    }
}
