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

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private readonly Camera _camera = new Camera();        

        public CameraTestScreen()
        {            
            Children.Add(_camera);
            Components.Add(new UserInputComponent(_camera));
            var a = new ObjectComponent();
            var b = new ObjectComponent();
            var c = new ObjectComponent();
            a.Startup();
            //b.Startup("nerfrevolver1.obj");
            //c.Startup("soldier.obj");
            //Components.Add(new PlanesComponent());            
            Components.Add(a);
            //Components.Add(b);
        }
    }

    /*
        From Documentation:
        var objLoaderFactory = new ObjLoaderFactory();
        var objLoader = objLoaderFactory.Create();
        Or provide your own:

        //With the signature Func<string, Stream>
        var objLoaderFactory = new ObjLoaderFactory();
        var objLoader = objLoaderFactory.Create(materialFileName => File.Open(materialFileName);
        Then it is just a matter of invoking the loader with a stream containing the model.

        var fileStream = new FileStream("model.obj");
        var result = objLoader.Load(fileStream);
        The result object contains the loaded model in this form:

        public class LoadResult  
        {
            public IList<Vertex> Vertices { get; set; }
            public IList<Texture> Textures { get; set; }
            public IList<Normal> Normals { get; set; }
            public IList<Group> Groups { get; set; }
            public IList<Material> Materials { get; set; }
        }


    */

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

            // Fixed function pipeline, old now. Programmable funtion pipeline is preferred           
            // Immediate mode is deprecated

            //GL.Begin(PrimitiveType.Points);
            //foreach (var vertice in vertices)
            //{
            //    GL.Vertex3(vertice.X, vertice.Y, vertice.Z);                
            //}
            //GL.End();

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
                        

            //foreach (var group in groups)
            //{
            //    Console.WriteLine(group.Faces);
            //    Console.ReadKey();
            //}

            //foreach (var texture in textures)
            //{
            //    Console.WriteLine("Textures: " + texture.X + " " + texture.Y);
            //    Console.ReadKey();
            //}

            GL.Begin(PrimitiveType.Points); // you can also use Points, Lines, Quads                                
            foreach (var vertice in vertices)
            {
                GL.Vertex3(vertice.X, vertice.Y, vertice.Z);
            }
            GL.End();

            //foreach (var normal in normals)
            //{
            //    gl.vertex3(normal.x, normal.y, normal.z);
            //}

            

            //GL.DrawArrays(BeginMode.Triangles, 0, 3);     

            /*for (var x = 0; x < vertices.Count; x++)
            {
                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(vertices[x].X, vertices[x].Y, vertices[x].Z);                
                GL.Normal3(normals[x].X, normals[x].Y, normals[x].Z);
                GL.End();

            }*/

            /*
            GL.Begin(PrimitiveType.Points);                               
            foreach (var vertice in vertices)
            {                                                                                                                           
                GL.Vertex3(vertice.X, vertice.Y, vertice.Z);                                                    
            }            
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            foreach (var normal in normals)
            {               
                GL.Normal3(normal.X, normal.Y, normal.Z);
            }
            GL.End();
            
    */
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
