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

            CollisionDetector collisionDetector = CollisionDetector.Singleton();
            Children.Add(collisionDetector);

            _camera.Position = new Vector3(0, 10, 0);
            _camera.Rotation = new Vector3(0, MathHelper.PiOver2, 0);
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

            var floor1 = new FloorEntity(new Vector3(0, 0, 0), 100, 100, "floor1", true);
            var floor2 = new FloorEntity(new Vector3(0, -10, 100), 100, 100, "floor2", true);
            var floor3 = new FloorEntity(new Vector3(100, -10, 0), 100, 100, "floor3", true);

            floor1.TextureId = "floor";
            floor2.TextureId = "floor";
            floor3.TextureId = "floor";

            _renderer.RegisterMesh("floor1", floor1.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());
            _renderer.RegisterMesh("floor2", floor2.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());
            _renderer.RegisterMesh("floor3", floor3.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());

            Children.Add(floor1);
            Children.Add(floor2);
            Children.Add(floor3);

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(0, 0, 0);
            gunEntity.Scale = 1f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float) Math.PI/2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));

            _textureManager.Add("gun", "nerfrevolverMapped.bmp");
            _textureManager.Add("floor", "floor.jpg");

            //gunEntity.Components.Add(new GravityComponent(gunEntity));
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, 1, 0)));

            BoundingBoxEntity box = new BoundingBoxEntity(gunEntity, mesh.Max, mesh.Min, true);
            gunEntity.Children.Add(box);

            box.Collided += Box_Collided;

            Children.Add(gunEntity);

            foreach (var entity in Children)
            {
                foreach (var component in entity.Components)
                {
                    component.Startup();
                }
            }
        }

        private void Box_Collided(object sender, CollisionEventArgs e)
        {
            // Do World-Scope collision detection
        }
    }
}
