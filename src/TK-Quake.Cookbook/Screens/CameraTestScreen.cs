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
using TKQuake.Engine.Infrastructure.Audio;
using ObjLoader.Loader.Loaders;
using OpenTK.Graphics;
using TKQuake.Engine.Infrastructure;
using Vertex = TKQuake.Engine.Infrastructure.Math.Vertex;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using OpenTK.Audio;

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
            var _audioManager = AudioManager.Singleton();
            _audioManager.UpdateListenerPosition(_camera.Position);

            InitEntities();
            InitComponents();
            
            // List loaded 'components'
            Console.WriteLine("\n++++++++++++++++++++++\nLOADED COMPONENTS\n++++++++++++++++++++++\n");
            foreach (var component in Components)
            {
                Console.WriteLine(component.ToString());
            }
            Console.WriteLine("\n++++++++++++++++++++++\nFIN.LOADED COMPONENTS\n++++++++++++++++++++++\n");

            //Apparently OpenAL should manage its own threading, however if this wasn't created as a thread the GameScreen would not run.
            //Might be to do with where I have declared it.
            //Thread th = new Thread(new ThreadStart(AudioManager.Play));
            //th.Start();
            _audioManager.UpdateListenerPosition(_camera.Position);
            var filename = Path.Combine("Audio", "PosTest.wav");
            new Thread(delegate ()
            {
                using (AudioContext context = new AudioContext())
                {
                    _audioManager.Add("bgm", filename);
                    _audioManager.PlayAtSource("bgm", new Vector3(0f, 0f, 0f));
                }
                //_audioManager.printHeader(filename);
            }).Start();

            //Setting this as a new thread will keep the camera position up to date. Probably should be placed on the renderer loop
            //rather than its own thread.
            new Thread(delegate ()
            {
                    while (true)
                    {
                        _audioManager.UpdateListenerPosition(_camera.Position);
                        Thread.Sleep(1000);
                    }
            }).Start();
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

            //register the mesh to the renderer
            var fileStream = File.OpenRead("nerfrevolver.obj");
            var mesh = _objLoader.Load(fileStream).ToMesh();

            _renderer.RegisterMesh("gun", mesh);

            // Add gun entitiy
            var gunEntity = RenderableEntity.Create();
            gunEntity.Id = "gun";
            gunEntity.Position = new Vector3(0, 0, 0);
            gunEntity.Scale = 0.05f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI / 2, 0)));
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
