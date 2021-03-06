﻿using OpenTK;
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
using TKQuake.Engine.Infrastructure.Physics;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;
using ObjLoader.Loader.Loaders;
using OpenTK.Graphics;
using TKQuake.Engine.Infrastructure;
using Vertex = TKQuake.Engine.Infrastructure.Math.Vertex;
using System.Drawing;
using System.Drawing.Imaging;
using Isg.Range;
using TKQuake.Components;
using TKQuake.Cookbook.Entities;
using TKQuake.Engine.Debug;
using TKQuake.Physics;

namespace TKQuake.Cookbook.Screens
{
    public class CollisionTestScreen : GameScreen
    {
        private readonly Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();
        private readonly CollisionDetector collisionDetector;

        public CollisionTestScreen(Renderer renderer)
        {
            _renderer = renderer;
            _textureManager = TextureManager.Singleton();// TextureManager();
            //_renderer.TextureManager = _textureManager;

            collisionDetector = CollisionDetector.Singleton();
            Children.Add(collisionDetector);

            _camera.Position = new Vector3(0, 10, 0);
            //_camera.Rotation = new Vector3(0, MathHelper.PiOver2, 0);
            _camera.Components.Add(new GravityComponent(_camera));
            _camera.Components.Add(new DebugPositionComponent(_camera));

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
        }

        private void InitEntities()
        {
            Children.Add(_camera);

            var floor1 = new FloorEntity(new Vector3(0, 0, 0), 10, 10, "floor", true);
            var floor2 = new FloorEntity(new Vector3(0, -2, 10), 10, 10, "floor", true);
            var floor3 = new FloorEntity(new Vector3(10, -2, 0), 10, 10, "floor", true);
            var floor4 = new FloorEntity(new Vector3(-10, -2, 0), 10, 10, "floor", true);
            var floor5 = new FloorEntity(new Vector3(0, -2, -10), 10, 10, "floor", true);

            var floor6 = new FloorEntity(new Vector3(-10, -2, 10), 10, 10, "floor", true);
            var floor7 = new FloorEntity(new Vector3(10, -2, 10), 10, 10, "floor", true);
            var floor8 = new FloorEntity(new Vector3(-10, -2, -10), 10, 10, "floor", true);
            var floor9 = new FloorEntity(new Vector3(10, -2, -10), 10, 10, "floor", true);

            _renderer.RegisterMesh("floor", floor1.Children.OfType<BoundingBoxEntity>().FirstOrDefault().ToMesh());

            Children.Add(floor1);
            Children.Add(floor2);
            Children.Add(floor3);
            Children.Add(floor4);
            Children.Add(floor5);
            Children.Add(floor6);
            Children.Add(floor7);
            Children.Add(floor8);
            Children.Add(floor9);

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(5, 1, 5);
            gunEntity.Scale = 1f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI / 2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));

            _textureManager.Add("gun", "nerfrevolverMapped.bmp");
            _textureManager.Add("floor", "floor.jpg");
            _textureManager.Add("FireParticle", "FireParticle.png");
            _textureManager.Add("SmokeParticle", "Smoke.png");

            gunEntity.Components.Add(new GravityComponent(gunEntity));

            BoundingBoxEntity box = new BoundingBoxEntity(gunEntity, mesh.Max, mesh.Min, true);
            gunEntity.Children.Add(box);
            gunEntity.Components.Add(new PickupComponent(gunEntity));
            
            box.Collided += Box_Collided;

            Children.Add(gunEntity);

            //// Wind Tunnel
            WindTunnel tunnel1 = new WindTunnel(new Vector3(15, 10, 10), new Vector3(-10, -2, 20));
            tunnel1.Direction = Vector3.UnitX;
            tunnel1.Force = 0.1f;
            Children.Add(tunnel1);

            WindTunnel tunnel2 = new WindTunnel(new Vector3(20, 10, -15), new Vector3(10, -2, 20));
            tunnel2.Direction = -Vector3.UnitZ;
            tunnel2.Force = 0.1f;
            Children.Add(tunnel2);

            WindTunnel tunnel3 = new WindTunnel(new Vector3(20, 10, -10), new Vector3(-5, -2, 0));
            tunnel3.Direction = -Vector3.UnitX;
            tunnel3.Force = 0.1f;
            Children.Add(tunnel3);

            WindTunnel tunnel4 = new WindTunnel(new Vector3(0, 10, -10), new Vector3(-10, -2, 10));
            tunnel4.Direction = Vector3.UnitZ;
            tunnel4.Force = 0.1f;
            Children.Add(tunnel4);

            FireParticleSystem fps = new FireParticleSystem();
            fps.Camera = _camera;
            fps.Position = new Vector3(5,0,5);
            Children.Add(fps);
            _renderer.RegisterMesh("FireParticle", new FireParticle().Mesh);
            _renderer.RegisterMesh("SmokeParticle", new SmokeParticle().Mesh);


            foreach (var entity in Children)
            {
                foreach (var component in entity.Components)
                {
                    component.Startup();
                }

                entity.Destroy += Entity_Destroy;
            }
        }

        private void Entity_Destroy(object sender, EventArgs e)
        {
            var entity = (IEntity) sender;
            RemoveEntity(entity);
            collisionDetector.RemoveCollider(entity.Children.OfType<BoundingBoxEntity>().FirstOrDefault());
        }

        private void Box_Collided(object sender, CollisionEventArgs e)
        {
            // Do World-Scope collision detection
        }
    }

}
