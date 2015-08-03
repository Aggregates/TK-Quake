using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Engine.Core
{
    public class Renderer
    {

        public Renderer()
        {
            //GL.Enable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public void DrawImmediateModeVertex(Vector position, Color color, Point uvs)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
            GL.TexCoord2(uvs.X, uvs.Y);
            GL.Vertex3(position.X, position.Y, position.Z);
        }

        public void DrawSprites(List<Sprite2> sprites)
        {
            foreach(Sprite2 s in sprites)
            {
                DrawSprite(s);
            }
        }

        public void DrawSprite(Sprite2 sprite)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Texture.Id);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Triangles);

            {
                for(int i = 0; i < sprite.Vertices.Count; i++)
                {
                    DrawImmediateModeVertex(
                        sprite.Vertices[i].Position,
                        sprite.Vertices[i].Color,
                        sprite.Vertices[i].UVs);
                }
            }

            GL.End();
        }

        public void DrawText(Text text)
        {
            foreach (CharacterSprite s in text.CharacterSprites)
            {
                s.Sprite.RenderWidth = s.Data.Width;
                s.Sprite.RenderHeight = s.Data.Height;
                DrawSprite(s.Sprite);
            }
        }

    }
}