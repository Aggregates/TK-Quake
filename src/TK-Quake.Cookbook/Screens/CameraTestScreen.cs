using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Physics;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Loader;
using TKQuake.Engine.Loader.BSP;
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
        private Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();
        private string _BSP = null;

        public CameraTestScreen(string BSPFile)
        {
            _renderer = Renderer.Singleton();
            _textureManager = TextureManager.Singleton();
            _BSP = BSPFile;
            CollisionDetector collidionDetector = CollisionDetector.Singleton();
            collidionDetector.Active = true;
            Children.Add(collidionDetector);

            _camera.Position = new Vector3(0, 10, 0);
            _camera.Components.Add(new GravityComponent(_camera));

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
//            var skyboxPath = Path.Combine("skybox", "space");
//            var skybox = new SkyboxComponent(this, "skybox")
//            {
//                Back = Path.Combine(skyboxPath, "back.bmp"),
//                Front = Path.Combine(skyboxPath, "front.bmp"),
//                Top = Path.Combine(skyboxPath, "top.bmp"),
//                Bottom = Path.Combine(skyboxPath, "top.bmp"),
//                Left = Path.Combine(skyboxPath, "left.bmp"),
//                Right = Path.Combine(skyboxPath, "right.bmp")
//            };
//            Components.Add(skybox);

            foreach (var component in Components)
            {
                component.Startup();
            }

            Components.Add(new BSPComponent(_BSP, ref _camera));
        }

        private void InitEntities()
        {
            Children.Add(_camera);
            Children.Add(new FloorEntity(new Vector3(-10,0,-10), 20, 20));

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            for (int i = 0; i < 2; i++)
            {

                var gunEntity = RenderableEntity.Create();
                gunEntity.Id = "gun";
                gunEntity.Position = new Vector3(0, 0, 5);
                gunEntity.Rotation = new Vector3(0, 0, 0);
                gunEntity.Scale = 1f;
                gunEntity.Components.Add(new GravityComponent(gunEntity));

                BoundingBoxComponent box = new BoundingBoxComponent(gunEntity, mesh.Max, mesh.Min, true);
                gunEntity.Components.Add(box);

                box.Collided += Box_Collided;

                if (i == 1)
                {
                    MoveComponent mv = new MoveComponent(gunEntity);
                    gunEntity.Position = new Vector3(1, 1, 1);
                    gunEntity.Components.Add(mv);
                }

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

        private void Box_Collided(object sender, EventArgs e)
        {
            // Do World-Scope collision detection
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
        }

        public void Shutdown()
        {
        }

        public void Update(double elapsedTime)
        {
            
        }

        private static class FloorGridEntity
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
                    var v1 = new Vector3(-lineLength, y, i*lineSpacing - 100f);
                    var v2 = new Vector3(lineLength, y, i*lineSpacing - 100f);

                    //perpendicular to x-axis
                    var v3 = new Vector3(i + lineSpacing - 50f, y, -lineLength);
                    var v4 = new Vector3(i + lineSpacing - 50f, y, lineLength);

                    vertices.AddRange(new[]
                    {
                        new Vertex(v1, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                        new Vertex(v2, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                        new Vertex(v3, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                        new Vertex(v4, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                    });

                    indices.AddRange(new[]
                    {
                        index, index + 1, index,
                        index + 2, index + 3, index + 2
                    });
                }

                return new Mesh
                {
                    Vertices = vertices.ToArray(),
                    Indices = indices.ToArray()
                };
            }
        }

    }

    public class MoveComponent : IComponent
    {
        private Engine.Infrastructure.Entities.Entity _entity;

        public MoveComponent(Engine.Infrastructure.Entities.Entity entity)
        {
            this._entity = entity;
        }

        public void Shutdown()
        {
        }

        public void Startup()
        {
        }

        public void Update(double elapsedTime)
        {
            _entity.Position -= new Vector3((float)(elapsedTime), (float)(elapsedTime), (float)(elapsedTime));
        }
    }

}
