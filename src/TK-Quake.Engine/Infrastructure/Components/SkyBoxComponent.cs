using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class SkyBoxComponent : IComponent
    {
        // Takes a size, takes a string related to the sky type
        private int[] skybox = new int[6];
        private int skySelection = 1;
        private readonly IEntity _scene;

        public SkyBoxComponent(IEntity scene)
        {
            _scene = scene;
        }

        public void Startup()
        {
            var renderer = Renderer.Singleton();
            const float size = 500;

            //back face
            var backMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(size, size, size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-size, size, size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-size, -size, size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(size, -size, size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var back = RenderableEntity.Create();
            back.Id = "skybox_back";
            renderer.RegisterMesh(back.Id, backMesh);
            renderer.TextureManager.Add(back.Id, Path.Combine("skybox", "space", "back.bmp"));
            _scene.Children.Add(back);

            //left face
            var leftMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(-size, size, size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-size, size, -size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-size, -size, -size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(-size, -size, size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var left = RenderableEntity.Create();
            left.Id = "skybox_left";
            renderer.RegisterMesh(left.Id, leftMesh);
            renderer.TextureManager.Add(left.Id, Path.Combine("skybox", "space", "left.bmp"));
            _scene.Children.Add(left);

            //front face
            var frontMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(size, size, -size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-size, size, -size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-size, -size, -size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(size, -size, -size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var front = RenderableEntity.Create();
            front.Id = "skybox_front";
            renderer.RegisterMesh(front.Id, frontMesh);
            renderer.TextureManager.Add(front.Id, Path.Combine("skybox", "space", "front.bmp"));
            _scene.Children.Add(front);

            //right face
            var rightMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(size, size, -size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(size, size, size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(size, -size, size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(size, -size, -size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var right = RenderableEntity.Create();
            right.Id = "skybox_right";
            renderer.RegisterMesh(right.Id, rightMesh);
            renderer.TextureManager.Add(right.Id, Path.Combine("skybox", "space", "right.bmp"));
            _scene.Children.Add(right);

            //top face
            var topMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(size, size, size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-size, size, size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-size, size, -size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(size, size, -size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var top = RenderableEntity.Create();
            top.Id = "skybox_top";
            renderer.RegisterMesh(top.Id, topMesh);
            renderer.TextureManager.Add(top.Id, Path.Combine("skybox", "space", "top.bmp"));
            _scene.Children.Add(top);

            //bottom face
            var bottomMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(size, -size, size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-size, -size, size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-size, -size, -size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(size, -size, -size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var bottom = RenderableEntity.Create();
            bottom.Id = "skybox_bottom";
            renderer.RegisterMesh(bottom.Id, bottomMesh);
            renderer.TextureManager.Add(bottom.Id, Path.Combine("skybox", "space", "top.bmp"));
            _scene.Children.Add(bottom);
        }

        public void Shutdown() { }
        public void Update(double elapsedTime) { }

        int loadTexture(string filename)  //load the filename named texture
        {

            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // Increase or decrease the size of the texture
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Repeat the pixels in the edge of the texture, it will hide that 1px wide line at the edge of the cube
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            // Create bmp
            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Make texture... at least that's what I think this does...
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            return id;     //and we return the id
        }

        //load all of the textures, to the skybox array
        private void chooseSky(int choice)
        {
            string selection;
            switch (choice)
            {
                case 0: selection = "anotherworld"; break;
                case 1: selection = "space"; break;
                default: selection = ""; break;
            }

            // Location of textures *root of build* / skybox / selection / position.bmp
            skybox[0] = loadTexture(Path.Combine("skybox", selection, "left.bmp"));
            skybox[1] = loadTexture(Path.Combine("skybox", selection, "back.bmp"));
            skybox[2] = loadTexture(Path.Combine("skybox", selection, "right.bmp"));
            skybox[3] = loadTexture(Path.Combine("skybox", selection, "front.bmp"));
            skybox[4] = loadTexture(Path.Combine("skybox", selection, "top.bmp"));
            skybox[5] = loadTexture(Path.Combine("skybox", selection, "top.bmp")); // bottom?
        }
    }
}
