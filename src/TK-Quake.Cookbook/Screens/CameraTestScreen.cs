﻿using OpenTK;
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

        public CameraTestScreen(string BSPFile)
        {
            _renderer = Renderer.Singleton();

            InitEntities();
            Components.Add(new UserInputComponent(_camera));
            Components.Add(new FloorGridComponent());
            Components.Add(new BSPComponent(BSPFile, _renderer, _camera));
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
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));

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

    class BSPComponent : IComponent
    {
        private const string ENTITY_ID = "BSP";

        private BSPRenderer BSP;
        private Renderer renderer;
        private Camera camera;

        public BSPComponent(string BSPFile, Renderer render, Camera cam)
        {
            renderer = render;
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
            Mesh mesh = BSP.GetMesh (camera.Position);

            if (renderer.IsMeshRegister (ENTITY_ID) == true)
            {
                renderer.UnregisterMesh (ENTITY_ID);
            }

            renderer.RegisterMesh(ENTITY_ID, mesh);

            var BSPEntity = RenderableEntity.Create();
            BSPEntity.Id = ENTITY_ID;
            BSPEntity.Position = new Vector3(0, 0, 0);
            BSPEntity.Scale = 1.0f;
        }
    }
}
