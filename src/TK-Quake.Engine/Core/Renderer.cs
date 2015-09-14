﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class Something
    {
        public int VerticesId { get; set; }
        public int IndicesId { get; set; }
        public Mesh Mesh { get; set; }
    }

    public class Renderer
    {
        private readonly ResourceManager<Mesh> _meshes = new MeshManager();
        public TextureManager TextureManager;
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
            int vao, vbo, ebo;
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);
            GL.GenBuffers(1, out ebo);

            var vStride = BlittableValueType.StrideOf(mesh.Vertices);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(mesh.Vertices.Length * vStride), mesh.Vertices, BufferUsageHint.StaticDraw);

            var eStride = BlittableValueType.StrideOf(mesh.Indices);
            GL.BindVertexArray(ebo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(mesh.Indices.Length * eStride), mesh.Indices, BufferUsageHint.StaticDraw);

            var posAttrib = GL.GetAttribLocation(Program, "position");
            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, vStride, 0);
            GL.EnableVertexAttribArray(posAttrib);

            var normalAttrib = GL.GetAttribLocation(Program, "normal");
            GL.VertexAttribPointer(normalAttrib, 3, VertexAttribPointerType.Float, false, vStride, 3*sizeof (float));
            GL.EnableVertexAttribArray(normalAttrib);

            var textureAttrib = GL.GetAttribLocation(Program, "texcoord");
            GL.VertexAttribPointer(textureAttrib, 2, VertexAttribPointerType.Float, false, vStride, 6*sizeof (float));
            GL.EnableVertexAttribArray(textureAttrib);

            GL.BindVertexArray(0);

            mesh.VaoId = vao;
            mesh.VboId = vbo;
            mesh.EboId = ebo;
            _meshes.Add(entityId, mesh);
        }

        public void DrawEntity(IEntity entity)
        {
            var mesh = _meshes.Get(entity.Id);
            System.Diagnostics.Debug.Assert(mesh != null, "Null mesh");

            //bind texture
            TextureManager.Bind(entity.Id);

            var rotate = Matrix4.CreateRotationX(entity.Rotation.X) *
                         Matrix4.CreateRotationY(entity.Rotation.Y) *
                         Matrix4.CreateRotationZ(entity.Rotation.Z);

            var entityTranslation = entity.Translation;
            var position = Matrix4.CreateTranslation(entity.Position / 8);
            var scale = Vector3.One * entity.Scale;

            var uniColor = GL.GetUniformLocation(Program, "uniColor");
            GL.Uniform3(uniColor, 1.0f, 1.0f, 1.0f);

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
