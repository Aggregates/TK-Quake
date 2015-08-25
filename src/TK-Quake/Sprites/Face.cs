using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Sprites
{
    public class Face : Sprite2
    {
        public Face(Texture texture) : base(texture)
        {

        }

        /*
        public override void Update(double elapsedTime)
        {
            //X++;
            //ScaleX *= 1.001;
            //ScaleY *= 0.999;
            this.ScaleUniform(1.005);
        }
        */

        public override void Render()
        {
            double halfHeight = Height / 2;
            double halfWidth = Width / 2;

            // Enable rendering of texture
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, this.Texture.Id);

            // Enable blending of texture's alpha channel
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Triangles);
            {

                // Top Triangle

                GL.TexCoord2(TopLeft.X, TopLeft.Y);
                GL.Vertex2(X - halfWidth, Y + halfHeight);

                GL.TexCoord2(BottomRight.X, TopLeft.Y);
                GL.Vertex2(X + halfWidth, Y + halfHeight);

                GL.TexCoord2(TopLeft.X, BottomRight.Y);
                GL.Vertex2(X - halfWidth, Y - halfHeight);

                // Bottom Triangle

                GL.TexCoord2(BottomRight.X, TopLeft.Y);
                GL.Vertex2(X + halfWidth, Y + halfHeight);

                GL.TexCoord2(BottomRight.X, BottomRight.Y);
                GL.Vertex2(X + halfWidth, Y - halfHeight);

                GL.TexCoord2(TopLeft.X, BottomRight.Y);
                GL.Vertex2(X - halfWidth, Y - halfHeight);
            }
            GL.End();

        }
    }
}
