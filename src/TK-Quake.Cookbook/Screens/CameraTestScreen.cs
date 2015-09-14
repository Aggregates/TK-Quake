using OpenTK;
using OpenTK.Graphics.OpenGL4;
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
using OpenTK.Graphics;
using TKQuake.Engine.Infrastructure;
using Vertex = TKQuake.Engine.Infrastructure.Math.Vertex;
using System.Drawing;
using System.Drawing.Imaging;

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private readonly Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();

        public CameraTestScreen(Renderer renderer, string BSPFile)
        {
            _renderer = renderer;
            _textureManager = new TextureManager();
            _renderer.TextureManager = _textureManager;

            InitEntities();
            InitComponents();

            // List loaded 'components'
            Console.WriteLine("\n++++++++++++++++++++++\nLOADED COMPONENTS\n++++++++++++++++++++++\n");
            foreach (var component in Components)
            {
                Console.WriteLine(component.ToString());
            }
            Console.WriteLine("\n++++++++++++++++++++++\nFIN.LOADED COMPONENTS\n++++++++++++++++++++++\n");
        }

        private void InitComponents()
        {
            Components.Add(new UserInputComponent(_camera));

            //skybox
            var skyboxPath = Path.Combine("skybox", "space");
            var skybox = new SkyboxComponent(this, "skybox")
            {
                Back = Path.Combine(skyboxPath, "back.bmp"),
                Front = Path.Combine(skyboxPath, "front.bmp"),
                Top = Path.Combine(skyboxPath, "top.bmp"),
                Bottom = Path.Combine(skyboxPath, "top.bmp"),
                Left = Path.Combine(skyboxPath, "left.bmp"),
                Right = Path.Combine(skyboxPath, "right.bmp")
            };
            Components.Add(skybox);

            foreach (var component in Components)
            {
                component.Startup();
            }

            Components.Add(new FloorGridComponent());
            Components.Add(new BSPComponent(BSPFile, ref _camera));
        }

        private void InitEntities()
        {
            Children.Add(_camera);

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            // Add gun entitiy
            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(0, 0, 0);
            gunEntity.Scale = 0.05f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI/2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));
            _textureManager.Add("gun", "nerfrevolverMapped.bmp");
            Children.Add(gunEntity);

            foreach (var entity in Children)
            {
                foreach (var component in entity.Components)
                {
                    component.Startup();
                }
            }
        }
    }

    static class FloorGridEntity
    {
        public static Mesh Mesh()
        {
            var lineLength = 1000f;
            var lineSpacing = 2.5f;
            var y = -2.5f;

            var vertices = new List<Vertex>();
            var indices = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                var index = vertices.Count;

                //parallel to x-axis
                var v1 = new Vector3(-lineLength, y, i * lineSpacing - 100f);
                var v2 = new Vector3(lineLength, y, i * lineSpacing - 100f);

                //perpendicular to x-axis
                var v3 = new Vector3(i + lineSpacing - 50f, y, -lineLength);
                var v4 = new Vector3(i + lineSpacing - 50f, y, lineLength);

                vertices.AddRange(new []
                {
                    new Vertex(v1, Vector3.Zero, Vector2.Zero),
                    new Vertex(v2, Vector3.Zero, Vector2.Zero),
                    new Vertex(v3, Vector3.Zero, Vector2.Zero),
                    new Vertex(v4, Vector3.Zero, Vector2.Zero),
                });
            }
        }
    }

    class BSPComponent : IComponent
    {
        private const string ENTITY_ID = "BSP";

        private readonly Renderer _renderer = Renderer.Singleton();

        private BSPRenderer BSP;
        private Camera camera;

        private BSPComponent()
        {
            camera = null;
            BSP = null;
        }

        public BSPComponent(string BSPFile, ref Camera cam) : this()
        {
            camera = cam;
            ChangeBSP (BSPFile);
        }

        public void ChangeBSP(string BSPFile)
        {
            BSP = new BSPRenderer (BSPFile);
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {   
            // Make sure there are no meshes left over from the last rendering.
            int meshCount = 0;
            string id = string.Format ("{0}{1}", ENTITY_ID, meshCount);

            while (_renderer.IsMeshRegister (id) == true)
            {
                _renderer.UnregisterMesh (id);

                meshCount++;
                id = string.Format ("{0}{1}", ENTITY_ID, meshCount);
            }

            // Get the current ModelView and Projection matrices from OPenTK
            double[] matrix  = new double[16];
            GL.GetDouble (GetPName.ModelviewMatrix, matrix);

            Matrix4 modelView = new Matrix4(
                (float)matrix[ 0], (float)matrix[ 1], (float)matrix[ 2], (float)matrix[ 3],
                (float)matrix[ 4], (float)matrix[ 5], (float)matrix[ 6], (float)matrix[ 7],
                (float)matrix[ 8], (float)matrix[ 9], (float)matrix[10], (float)matrix[11],
                (float)matrix[12], (float)matrix[13], (float)matrix[14], (float)matrix[15]
            );

            GL.GetDouble (GetPName.ProjectionMatrix, matrix);

            Matrix4 projection = new Matrix4(
                (float)matrix[ 0], (float)matrix[ 1], (float)matrix[ 2], (float)matrix[ 3],
                (float)matrix[ 4], (float)matrix[ 5], (float)matrix[ 6], (float)matrix[ 7],
                (float)matrix[ 8], (float)matrix[ 9], (float)matrix[10], (float)matrix[11],
                (float)matrix[12], (float)matrix[13], (float)matrix[14], (float)matrix[15]
            );

            // Generate, register, and render the meshes that are potentially visible to the camera.
            List<Mesh> meshes = BSP.GetMesh (camera.Position, Matrix4.Mult (projection, modelView));

            meshCount = 0;

            foreach (Mesh mesh in meshes)
            {
                id = string.Format ("{0}{1}", ENTITY_ID, meshCount);
                _renderer.RegisterMesh (id, mesh);

                // Create a renderable entity.
                var BSPEntity = RenderableEntity.Create ();
                BSPEntity.Id = id;
                BSPEntity.Position = camera.Position;
                BSPEntity.Scale = 1.0f;

                // Render the BSP entity.
                _renderer.DrawEntity (BSPEntity);

                meshCount++;
            }
        }
    }
}
