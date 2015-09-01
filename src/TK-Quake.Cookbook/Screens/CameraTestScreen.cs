using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;
using ObjLoader.Loader.Loaders;
using TKQuake.Engine.Infrastructure;

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private readonly Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();

        public CameraTestScreen()
        {
            _renderer = Renderer.Singleton();

            InitEntities();
            Components.Add(new UserInputComponent(_camera));
            Components.Add(new FloorGridComponent());
        }

        private void InitEntities()
        {
            Children.Add(_camera);

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();
            _renderer.RegisterMesh("gun", mesh);

            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(0, 1, -10);
            gunEntity.Rotation = new Vector3(0f, (float)-(Math.PI / 2), 0);
            gunEntity.Scale = 0.5f;

            Children.Add(gunEntity);
        }
    }

    class FloorGridComponent : IComponent
    {
        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            var lineLength = 100f;
            var lineSpacing = 2.5f;
            var y = -2.5f;

            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < 100; i++)
            {
                GL.Color3(0f, 0, 255);

                //parallel to x-axis
                GL.Vertex3(-lineLength, y, i * lineSpacing - 100f);
                GL.Vertex3(lineLength, y, i * lineSpacing - 100f);

                //perpendicular to x-axis
                GL.Vertex3(i + lineSpacing - 50f, y, -lineLength);
                GL.Vertex3(i + lineSpacing - 50f, y, lineLength);
            }
            GL.End();

            GL.Color3(255f, 255, 255);
        }
    }

    // Follow this idea, create as a component and then render it using the update
    class ObjectComponent : IComponent
    {
        private IObjLoader objLoader;
        private LoadResult results;
        private bool firstRun = true;

        public void Startup()
        {
            var objLoaderFactory = new ObjLoaderFactory();
            objLoader = objLoaderFactory.Create();

            var fileStream = File.OpenRead("face.obj");
            results = objLoader.Load(fileStream);
        }

        public void Startup(String file)
        {
            var objLoaderFactory = new ObjLoaderFactory();
            objLoader = objLoaderFactory.Create();

            var fileStream = File.OpenRead(file);
            results = objLoader.Load(fileStream);
        }

        public void Shutdown()
        {
            // Used for anything?
        }

        public void Update(double elapsedTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);


            //gunEntity.Rotation = new Vector3(45f, 45f, 45f);


            float[] mat_ambient = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
            float[] mat_diffuse = new float[4] { 0.2f, 0.3f, 0.5f, 0.0f };
            float[] mat_specular = new float[4] { 1.0f, 0.89f, 0.55f, 0.0f };

            Random random = new Random();
            float number = (float)random.NextDouble();
            Color customColor = new Color(mat_diffuse[0], mat_diffuse[1], mat_diffuse[2], 0);

            var vertices = results.Vertices;
            var textures = results.Textures;
            var normals = results.Normals;
            var materials = results.Materials;
            var groups = results.Groups;

            GLX.Color3(customColor);

            if (firstRun)
            {
                Console.WriteLine("Vertices: " + vertices.Count);
                Console.WriteLine("Textures: " + textures.Count);
                Console.WriteLine("Normals: " + normals.Count);
                Console.WriteLine("Materials: " + materials.Count);
                Console.WriteLine("Groups: " + groups.Count);
                Console.WriteLine("Faces: " + groups[0].Faces.Count);

                firstRun = false;
            }

            GL.Begin(PrimitiveType.Triangles); // you can also use Points, Lines, Quads
            foreach (var group in groups)
            {
                foreach (var face in group.Faces)
                {
                    for (var i = 0; i < face.Count; i++)
                    {
                        var faceVertex = face[i];
                        var vertex = vertices[faceVertex.VertexIndex -1];
                        var normal = normals[faceVertex.NormalIndex -1];
                        var texture = textures[faceVertex.TextureIndex - 1];
                        GL.Vertex3(vertex.X, vertex.Y, vertex.Z);
                        GL.Normal3(normal.X, normal.Y, normal.Z);
                        GL.TexCoord2(texture.X, texture.Y);
                    }
                }
            }
            GL.End();
        }
    }


    class PlanesComponent : IComponent
    {
        public void Startup() { }
        public void Shutdown() { }
        public void Update(double elapsedTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            // Display some planes
            for (int x = -10; x <= 10; x++)
            {
                for (int z = -10; z <= 10; z++)
                {
                    GL.PushMatrix();
                    GL.Translate((float)x * 5f, 0f, (float)z * 5f);
                    GL.Begin(PrimitiveType.Quads);
                    {
                        GLX.Color3(Color.Red);
                        GL.Vertex3(1f, 4f, 0f);
                        GLX.Color3(Color.Green);
                        GL.Vertex3(-1f, 4f, 0f);
                        GLX.Color3(Color.Blue);
                        GL.Vertex3(-1f, -4f, 0f);
                        GLX.Color3(Color.White);
                        GL.Vertex3(1f, -4f, 0f);
                    }
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }
    }


}
