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
using TKQuake.Engine.Infrastructure.Texture;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using Vertex = TKQuake.Engine.Infrastructure.Math.Vertex;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class SkyboxComponent : IComponent
    {
        // Takes a size, takes a string related to the sky type
        private readonly IEntity _scene;

        private readonly TextureManager texManager = TextureManager.Singleton();

        public string Back { get; set; }
        public string Front { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public float Size { get; set; } = 500;
        public string Name { get; set; }

        public SkyboxComponent(IEntity scene, string name)
        {
            _scene = scene;
            Name = name;
        }

        public void Startup()
        {
            var renderer = Renderer.Singleton();

            //back face
            var backMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(Size, Size, Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-Size, Size, Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-Size, -Size, Size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(Size, -Size, Size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var back = RenderableEntity.Create();
            back.Id = $"{Name}_back";
            renderer.RegisterMesh(back.Id, backMesh);
            texManager.Add(back.Id, Back);
            _scene.Children.Add(back);

            //left face
            var leftMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(-Size, Size, Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-Size, Size, -Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-Size, -Size, -Size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(-Size, -Size, Size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var left = RenderableEntity.Create();
            left.Id = $"{Name}_left";
            renderer.RegisterMesh(left.Id, leftMesh);
            texManager.Add(left.Id, Left);
            _scene.Children.Add(left);

            //front face
            var frontMesh = new Mesh()
            {
                Vertices = new []
                {
                    new Vertex(new Vector3(Size, Size, -Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-Size, Size, -Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-Size, -Size, -Size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(Size, -Size, -Size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var front = RenderableEntity.Create();
            front.Id = $"{Name}_front";
            renderer.RegisterMesh(front.Id, frontMesh);
            texManager.Add(front.Id, Front);
            _scene.Children.Add(front);

            //right face
            var rightMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(Size, Size, -Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(Size, Size, Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(Size, -Size, Size), Vector3.Zero, new Vector2(1, 1)),
                    new Vertex(new Vector3(Size, -Size, -Size), Vector3.Zero, new Vector2(0, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var right = RenderableEntity.Create();
            right.Id = $"{Name}_right";
            renderer.RegisterMesh(right.Id, rightMesh);
            texManager.Add(right.Id, Right);
            _scene.Children.Add(right);

            //top face
            var topMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(Size, Size, Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-Size, Size, Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-Size, Size, -Size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(Size, Size, -Size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var top = RenderableEntity.Create();
            top.Id = $"{Name}_top";
            renderer.RegisterMesh(top.Id, topMesh);
            texManager.Add(top.Id, Top);
            _scene.Children.Add(top);

            //bottom face
            var bottomMesh = new Mesh()
            {
                Vertices = new[]
                {
                    new Vertex(new Vector3(Size, -Size, Size), Vector3.Zero, new Vector2(1, 0)),
                    new Vertex(new Vector3(-Size, -Size, Size), Vector3.Zero, new Vector2(0, 0)),
                    new Vertex(new Vector3(-Size, -Size, -Size), Vector3.Zero, new Vector2(0, 1)),
                    new Vertex(new Vector3(Size, -Size, -Size), Vector3.Zero, new Vector2(1, 1)),
                },
                Indices = new [] {0, 1, 2, 2, 3, 0}
            };

            var bottom = RenderableEntity.Create();
            bottom.Id = $"{Name}_bottom";
            renderer.RegisterMesh(bottom.Id, bottomMesh);
            texManager.Add(bottom.Id, Bottom);
            _scene.Children.Add(bottom);
        }

        public void Shutdown() { }
        public void Update(double elapsedTime) { }
    }
}
