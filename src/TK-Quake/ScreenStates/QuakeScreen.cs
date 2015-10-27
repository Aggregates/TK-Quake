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
using TKQuake.Engine.Debug;
using TKQuake.Engine.Infrastructure.Physics;
using TKQuake.Physics;

namespace TKQuake.ScreenStates
{
    public class QuakeScreen : GameScreen
    {
        private Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();
        private string _BSP = null;
        private CollisionDetector _collisionDetector;

        public static new string StateNameKey = "QuakeScreen";

        public QuakeScreen(string BSPFile)
        {
            _renderer = Renderer.Singleton();
            _textureManager = TextureManager.Singleton();
            _BSP = BSPFile;
            _camera.Position =new Vector3(-20f, 2, 12f);

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
            _camera.Components.Add(new DebugPositionComponent(_camera));
            _camera.Components.Add(new GravityComponent(_camera));


            // Skybox
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

            Components.Add(new BSPComponent(_BSP, ref _camera));
        }

        private void InitEntities()
        {
            Children.Add(_camera);

            _collisionDetector = CollisionDetector.Singleton();
            Children.Add(_collisionDetector);

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            // Add gun entitiy
            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(-28f, 2, 12f);
            gunEntity.Scale = 0.3f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI / 2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));
            _textureManager.Add("gun", "nerfrevolverMapped.bmp");
            Children.Add(gunEntity);

            // Fire Particle System
            FireParticleSystem fps = new FireParticleSystem();
            fps.Camera = _camera;
            fps.Position = new Vector3(-40, 0.5f, 4.5f);
            Children.Add(fps);
            _textureManager.Add("FireParticle", "FireParticle.png");
            _textureManager.Add("SmokeParticle", "Smoke.png");
            _renderer.RegisterMesh("FireParticle", new FireParticle().Mesh);
            _renderer.RegisterMesh("SmokeParticle", new SmokeParticle().Mesh);

            CreateWindTunnels();

            // World Floor
            //_textureManager.Add("floor", "floor_s.jpg");
            _textureManager.Add("floor", "floor_t.png");

            var floor1 = new FloorEntity(new Vector3(-110, -0.5f, -85), 200, 200, "floor1", true);
            floor1.TextureId = "floor";
            _renderer.RegisterMesh("floor1", floor1.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());
            Children.Add(floor1);

            CreateFloors();

            foreach (var entity in Children)
            {
                foreach (var component in entity.Components)
                {
                    component.Startup();
                }
            }
        }

        private void CreateWindTunnels()
        {
            List<WindTunnel> tunnels = new List<WindTunnel> {
                new WindTunnel(new Vector3(-27, 20f, 26f), new Vector3(-32, 0f, 33))
            };

            foreach (var tunnel in tunnels)
            {
                tunnel.Direction = Vector3.UnitY;
                tunnel.Force = 1f;
                Children.Add(tunnel);
            }
        }

        private void CreateFloors()
        {
            int floorCount = 1; // Big world floor
            List<FloorEntity> floors = new List<FloorEntity>
            {
                new FloorEntity(new Vector3(-39, 25.5f, 24), 70, 80, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-83, 0.5f, 0), 25, 7, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-52, 13, -38), 17, 83, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-35, 13, -32), 55, 35, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-19, 13, 3), 25, 56, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(6, 13, 1), 23, 38, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-42, 6.5f, -58), 20, 8, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(19, 13, 38), 7, 20, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(0, 9.5f, 63), 25.5f, 7.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(41, 19.5f, 21), 7, 21, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-64, 10, 31), 14, 15, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-83, 6, 12), 36, 14, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-83, 3.5f, 0), 25, 7, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-83, 25.5f, -25), 25, 12, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-77, 25.5f, -6.5f), 6.5f, 6.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-83, 25.5f, 6.5f), 6.5f, 6.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-70, 25.5f, 6), 6.5f, 6.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-64.5f, 25.5f, -6.5f), 6.5f, 6.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-70.5f, 25.5f, 19f), 7, 23.5f, "floor" + (++floorCount).ToString()),
                new FloorEntity(new Vector3(-62.5f, 25.5f, 37), 25, 8, "floor" + (++floorCount).ToString())
            };

            foreach (var floor in floors)
            {
                floor.TextureId = "floor";
                _renderer.RegisterMesh(floor.Id, floor.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());
                Children.Add(floor);
            }
        }

        private void Entity_Destroy(object sender, EventArgs e)
        {
            var entity = (IEntity)sender;
            RemoveEntity(entity);
            _collisionDetector.RemoveCollider(entity.Children.OfType<BoundingBoxEntity>().FirstOrDefault());
        }

    }
}