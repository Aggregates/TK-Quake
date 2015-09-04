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

        public CameraTestScreen(string BSPFile)
        {
            _renderer = renderer;
            _textureManager = new TextureManager();
            _renderer.TextureManager = _textureManager;

            InitEntities();
            Components.Add(new UserInputComponent(_camera));
            Components.Add(new FloorGridComponent());
            Components.Add(new BSPComponent(BSPFile, ref _camera));

            SkyBoxComponent skyBox = new SkyBoxComponent();
            skyBox.Startup();
            Components.Add(skyBox);
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
            gunEntity.Position = new Vector3(0, 0, 0);
            gunEntity.Scale = 0.05f;
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI/2, 0)));
            gunEntity.Components.Add(new BobComponent(gunEntity, speed: 2, scale: 2));
            _textureManager.Add("gun", "nerfrevolverMapped.bmp");

            Children.Add(gunEntity);

            var floor = RenderableEntity.Create();
            floor.Id = "floor";
            floor.Position = Vector3.Zero;
            //Children.Add(floor);

            //_renderer.RegisterMesh("floor", FloorGridEntity.Mesh());
            //_textureManager.Add("floor", "nerfrevolverMapped.bmp");
        }
    }

    static class FloorGridEntity
    {
        public static Mesh Mesh()
        {
            var lineLength = 100f;
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

    class BSPComponent : IComponent
    {
        private const string ENTITY_ID = "BSP";

        private readonly Renderer _renderer = Renderer.Singleton();

        private BSPRenderer BSP;
        private Camera camera;

        public BSPComponent(string BSPFile, ref Camera cam)
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
            // Check to see if a mesh has already been registered for the BSP component.
            // If so, unregister it.
            if (_renderer.IsMeshRegister (ENTITY_ID) == true)
            {
                _renderer.UnregisterMesh (ENTITY_ID);
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

            // Generate and register the mesh that is potentially visible to the camera.
            _renderer.RegisterMesh (ENTITY_ID, BSP.GetMesh (camera.Position, Matrix4.Mult (projection, modelView)));

            // Create a renderable entity.
            var BSPEntity = RenderableEntity.Create ();
            BSPEntity.Id = ENTITY_ID;
            BSPEntity.Position = camera.Position;
            BSPEntity.Scale = 1.0f;

            // Render the BSP entity.
            _renderer.DrawEntity (BSPEntity);
        }
    }

    // Step 1: Draw a cube
    class SkyBoxComponent : IComponent
    {
        private int[] skybox = new int [6];

        int loadTexture(string filename)  //load the filename named texture
        {

            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);
            //int [] num = new int[0];       //the id for the texture
            //glGenTextures(1, num);  //we generate a unique one
            int id = GL.GenTexture();

            //GL.GenTextures(1, num);
            //SDL_Surface* img = SDL_LoadBMP(filename); //load the bmp image

            GL.BindTexture(TextureTarget.Texture2D, id);

            //glBindTexture(GL_TEXTURE_2D, num);       //and use the texture, we have just generated

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR); //same if the image bigger
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP);      //we repeat the pixels in the edge of the texture, it will hide that 1px wide line at the edge of the cube, which you have seen in the video
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP);      //we do it for vertically and horizontally (previous line)
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, img->w, img->h, 0, GL_RGB, GL_UNSIGNED_SHORT_5_6_5, img->pixels);        //we make the actual texture
            //SDL_FreeSurface(img);   //we delete the image, we don't need it anymore
            return id;     //and we return the id
        }

        //load all of the textures, to the skybox array
        void initskybox()
        {
            skybox[0] = loadTexture("left.bmp");
            skybox[1] = loadTexture("back.bmp");
            skybox[2] = loadTexture("right.bmp");
            skybox[3] = loadTexture("front.bmp");
            skybox[4] = loadTexture("top.bmp");
            skybox[5] = loadTexture("top.jpg"); // bottom?
        }

        public void Startup()
        {
            initskybox();

        }
        public void Shutdown() { }
        public void Update(double elapsedTime)
        {
            //bool b1 = GL.IsEnabled(EnableCap.Texture2D);

            float[] difamb = { 1.0f, 0.5f, 0.3f, 1.0f };
            double size = 1000;

            GL.Disable(EnableCap.Lighting);
            //GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, skybox[1]);
            GL.Begin(PrimitiveType.Quads);
            //back face
            //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, difamb);
            //GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.End();


            //left face
            GL.BindTexture(TextureTarget.Texture2D, skybox[0]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.End();

            //front face
            GL.BindTexture(TextureTarget.Texture2D, skybox[3]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();

            //right face
            GL.BindTexture(TextureTarget.Texture2D, skybox[2]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(1.0, 0.0, 0.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();

            //top face
            GL.BindTexture(TextureTarget.Texture2D, skybox[4]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(0.0, 1.0, 0.0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.End();

            //bottom face
            GL.BindTexture(TextureTarget.Texture2D, skybox[3]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(0.0, -1.0, 0.0);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();

        }
    }
}
