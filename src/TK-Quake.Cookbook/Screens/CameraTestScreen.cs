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
            gunEntity.Scale = 0.5f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI/2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, 10, 1));

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
}
