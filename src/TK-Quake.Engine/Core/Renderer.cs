using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private TextureManager TextureManager = TextureManager.Singleton ();
        private SpriteBatch _batch = new SpriteBatch();
        private int? _program;

        public int Program => _program ?? 0;
        private int? _vertexShader;
        private int? _fragmentShader;

        private Renderer() { }

        public void LoadShader(string shader, ShaderType type)
        {
            var id = GL.CreateShader(type);
            GL.ShaderSource(id, shader);
            GL.CompileShader(id);
            GetShaderCompileStatus(id);

            switch (type)
            {
                case ShaderType.VertexShader:
                    _vertexShader = id;
                    break;
                case ShaderType.FragmentShader:
                    _fragmentShader = id;
                    break;
                default:
                    throw new Exception("Invalid shader type");
            }
        }

        public void LinkShaders()
        {
            _program = GL.CreateProgram();

            if (_vertexShader.HasValue)
            {
                GL.AttachShader(_program.Value, _vertexShader.Value);
            }
            if (_fragmentShader.HasValue)
            {
                GL.AttachShader(_program.Value, _fragmentShader.Value);
                GL.BindFragDataLocation(_program.Value, 0, "outColor");
            }

            GL.LinkProgram(_program.Value);
            GL.UseProgram(_program.Value);
        }

        private bool GetShaderCompileStatus(int shader)
        {
            //Get status
            string info;
            GL.GetShaderInfoLog(shader, out info);

            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                Console.WriteLine(info);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Registers an entity mesh to the Renderer
        /// </summary>
        /// <param name="entityId">The id of the entity</param>
        /// <param name="mesh">The mesh of the entity</param>
        public void RegisterMesh(string entityId, Mesh mesh)
        {
            //push to video card
            int vao, vbo;
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);

            var vStride = BlittableValueType.StrideOf(mesh.Vertices);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(mesh.Vertices.Length*vStride), mesh.Vertices,
                BufferUsageHint.StaticDraw);

            var posAttrib = GL.GetAttribLocation(Program, "position");
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, vStride, 0);
            GL.EnableVertexAttribArray(posAttrib);

            var normalAttrib = GL.GetAttribLocation(Program, "normal");
            GL.VertexAttribPointer(normalAttrib, 3, VertexAttribPointerType.Float, false, vStride, 3*sizeof (float));
            GL.EnableVertexAttribArray(normalAttrib);

            var textureAttrib = GL.GetAttribLocation(Program, "texcoord");
            GL.VertexAttribPointer(textureAttrib, 2, VertexAttribPointerType.Float, false, vStride, 6*sizeof (float));
            GL.EnableVertexAttribArray(textureAttrib);

            int ebo;
            GL.GenBuffers(1, out ebo);

            var eStride = BlittableValueType.StrideOf(mesh.Indices);
            GL.BindVertexArray(ebo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(mesh.Indices.Length*eStride), mesh.Indices,
                BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            mesh.EboId = ebo;
            mesh.VaoId = vao;
            mesh.VboId = vbo;
            _meshes.Add(entityId, mesh);
        }

        /// <summary>
        /// Unregisters an entity mesh from the Renderer
        /// </summary>
        /// <param name="entityId">The id of the entity</param>
        public void UnregisterMesh(string entityId)
        {
            // Get the mesh first.
            Mesh mesh = GetMesh (entityId);

            // Unbind and remove textures that are no longer in use.
            if ((mesh.tex != null) && (TextureManager.Registered (mesh.tex.Path) == true))
            {
                TextureManager.Remove (mesh.tex.Path);
            }

            _meshes.Remove(entityId);
        }

        /// <summary>
        /// Retrieves an entity mesh from the Renderer
        /// </summary>
        /// <param name="entityId">The id of the entity</param>
        public Mesh GetMesh(string entityId)
        {
            return(_meshes.Get (entityId));
        }

        /// <summary>
        /// Checks if an entity's mesh has been registered.
        /// </summary>
        /// <param name="entityId">The id of the entity</param>
        public bool IsMeshRegistered(string entityId)
        {
            return(_meshes.Registered(entityId));
        }

        public void DrawEntity(IEntity entity)
        {
            var mesh = _meshes.Get(entity.Id);
            System.Diagnostics.Debug.Assert(mesh != null, "Null mesh");

            var rotation = Matrix4.CreateRotationX(entity.Rotation.X)*
                           Matrix4.CreateRotationY(entity.Rotation.Y)*
                           Matrix4.CreateRotationZ(entity.Rotation.Z);

            var model = rotation*entity.Translation*Matrix4.CreateTranslation(entity.Position)*Matrix4.CreateScale(entity.Scale);
            var uniModel = GL.GetUniformLocation(Program, "model");
            GL.UniformMatrix4(uniModel, false, ref model);

            //bind texture
            if (mesh.tex != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.tex.Id);
            }

            else if (TextureManager.Registered (entity.Id) == true)
            {
                TextureManager.Bind(entity.Id);
            }

            DrawVbo(mesh);

            //reset translation matrix?
            entity.Translation = Matrix4.Identity;
        }

        private void DrawVbo(Mesh mesh)
        {
            GL.BindVertexArray(mesh.VaoId);
            GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }

//        public void DrawSprites(List<Sprite2> sprites)
//        {
//            foreach (Sprite2 s in sprites)
//            {
//                DrawSprite(s);
//            }
//        }
//
//        public void DrawSprite(Sprite2 sprite)
//        {
//            _batch.AddSprite(sprite);
//        }

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

//        public void DrawText(TextEntity text)
//        {
//            foreach (CharacterSprite s in text.CharacterSprites)
//            {
//                s.Sprite.RenderWidth = s.Data.Width;
//                s.Sprite.RenderHeight = s.Data.Height;
//                DrawSprite(s.Sprite);
//            }
//        }
//
        private static Renderer _instance;
        public static Renderer Singleton()
        {
            return _instance ?? (_instance = new Renderer());
        }
    }
}
