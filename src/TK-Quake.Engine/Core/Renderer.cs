using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Infrastructure;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Core
{
    public class Renderer
    {
        private readonly ResourceManager<Mesh> _meshes = new MeshManager();
        private SpriteBatch _batch = new SpriteBatch();

        private Renderer() { }

        /// <summary>
        /// Registers an entity mesh to the Renderer
        /// </summary>
        /// <param name="entityId">The id of the entity</param>
        /// <param name="mesh">The mesh of the entity</param>
        public void RegisterMesh(string entityId, Mesh mesh)
        {
            _meshes.Add(entityId, mesh);
        }

        public void DrawEntity(IEntity entity)
        {
            var mesh = _meshes.Get(entity.Id);
            System.Diagnostics.Debug.Assert(mesh != null, "Null mesh");

            var rotate = Matrix4.CreateRotationX(entity.Rotation.X) *
                         Matrix4.CreateRotationY(entity.Rotation.Y) *
                         Matrix4.CreateRotationZ(entity.Rotation.Z);

            var entityTranslation = entity.Translation;
            var position = Matrix4.CreateTranslation(entity.Position / 8);
            var scale = Vector3.One * entity.Scale;

            GL.PushMatrix();
            GL.MultMatrix(ref entityTranslation);
            GL.MultMatrix(ref position);
            GL.MultMatrix(ref rotate);
            GL.Scale(scale);

            //todo: move away from immediate mode
            GL.Begin(PrimitiveType.Triangles);
            foreach (var index in mesh.Indices)
            {
                GL.Vertex3(mesh.Vertices[index]);
                GL.Normal3(mesh.Normals[index]);
                GL.TexCoord2(mesh.Textures[index]);
            }

            GL.End();
            GL.PopMatrix();

            //reset translation matrix?
            entity.Translation = Matrix4.Identity;
        }

        public void DrawImmediateModeVertex(Vector3 position, Color color, Point uvs)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
            GL.TexCoord2(uvs.X, uvs.Y);
            GL.Vertex3(position.X, position.Y, position.Z);
        }

        public void DrawSprites(List<Sprite2> sprites)
        {
            foreach (Sprite2 s in sprites)
            {
                DrawSprite(s);
            }
        }

        public void DrawSprite(Sprite2 sprite)
        {
            _batch.AddSprite(sprite);
        }

        /// <summary>
        /// Needs to be called every Frame.
        ///
        /// <remarks>
        /// If there is anything left in the SpriteBatch that hasn't
        /// been drawn by the end of the frame, this will ensure it gets drawn
        /// </remarks>
        /// </summary>
        public void Render()
        {
            _batch.Draw();
        }

        public void DrawText(TextEntity text)
        {
            foreach (CharacterSprite s in text.CharacterSprites)
            {
                s.Sprite.RenderWidth = s.Data.Width;
                s.Sprite.RenderHeight = s.Data.Height;
                DrawSprite(s.Sprite);
            }
        }

        private static Renderer _instance;
        public static Renderer Singleton()
        {
            return _instance ?? (_instance = new Renderer());
        }
    }
}
