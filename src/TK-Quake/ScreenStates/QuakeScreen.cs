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
            fps.Position = new Vector3(-9, 1, 16);
            Children.Add(fps);
            _textureManager.Add("FireParticle", "FireParticle.png");
            _textureManager.Add("SmokeParticle", "Smoke.png");
            _renderer.RegisterMesh("FireParticle", new FireParticle().Mesh);
            _renderer.RegisterMesh("SmokeParticle", new SmokeParticle().Mesh);

            //// Wind Tunnel
            WindTunnel tunnel1 = new WindTunnel(new Vector3(-27, 25f, 26f), new Vector3(-32, 0f, 33));
            tunnel1.Direction = Vector3.UnitY;
            tunnel1.Force = 1f;
            Children.Add(tunnel1);

            // Big Floor
            var floor1 = new FloorEntity(new Vector3(-85, -0.5f, -60), 150, 150, "floor", true);
            var floor2 = new FloorEntity(new Vector3(-85, 24.5f, -60), 150, 150, "floor", true);
            _textureManager.Add("floor", "floor_t.png");
            _renderer.RegisterMesh("floor", floor1.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());
            Children.Add(floor1);
            Children.Add(floor2);

            foreach (var entity in Children)
            {
                foreach (var component in entity.Components)
                {
                    component.Startup();
                }
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