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
        private readonly SkyBoxComponent skyBox = new SkyBoxComponent();

        public CameraTestScreen(Renderer renderer)
        {
            _renderer = renderer;
            _textureManager = new TextureManager();
            _renderer.TextureManager = _textureManager;

            InitEntities();
            Components.Add(new UserInputComponent(_camera));

            skyBox.Startup();
            Components.Add(skyBox);

            // List loaded 'components
            Console.WriteLine("\n++++++++++++++++++++++\nLOADED COMPONENTS\n++++++++++++++++++++++\n");
            foreach (var component in Components)
            {
                Console.WriteLine(component.ToString());
            }
            Console.WriteLine("\n++++++++++++++++++++++\nFIN.LOADED COMPONENTS\n++++++++++++++++++++++\n");

        }

        // This isnt used / doesnt work yet
        public void ChangeSkyBox(int choice)
        {
            Components.Remove(Components.ElementAt(2));
            SkyBoxComponent skyBox = new SkyBoxComponent();
            skyBox.Startup();
            skyBox.chooseSky(choice);
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

                indices.AddRange(new []
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

    class SkyBoxComponent : IComponent
    {
        // Takes a size, takes a string related to the sky type
        private int[] skybox = new int [6];

        int loadTexture(string filename)  //load the filename named texture
        {

            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // Increase or decrease the size of the texture
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Repeat the pixels in the edge of the texture, it will hide that 1px wide line at the edge of the cube
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Clamp);

            // Create bmp
            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Make texture... at least that's what I think this does...
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            return id;     //and we return the id
        }

        //load all of the textures, to the skybox array
        public void chooseSky(int choice)
        {
            string selection;

            switch (choice)
            {
                case 0: selection = "anotherworld"; break;
                case 1: selection = "space"; break;
                default: selection = ""; break;
            }

            // Location of textures *root of build* / skybox / selection / position.bmp
            skybox[0] = loadTexture(Path.Combine("skybox", selection, "left.bmp"));
            skybox[1] = loadTexture(Path.Combine("skybox", selection, "back.bmp"));
            skybox[2] = loadTexture(Path.Combine("skybox", selection, "right.bmp"));
            skybox[3] = loadTexture(Path.Combine("skybox", selection, "front.bmp"));
            skybox[4] = loadTexture(Path.Combine("skybox", selection, "top.bmp"));
            skybox[5] = loadTexture(Path.Combine("skybox", selection, "top.bmp")); // bottom?
        }

        public void Startup()
        {
            chooseSky(0);                   // Change this to alter which sky you see. Only 0 or 1 at the moment.

        }
        public void Shutdown() { }
        public void Update(double elapsedTime)
        {
            float[] difamb = { 1.0f, 0.5f, 0.3f, 1.0f };
            double size = 1000;

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, skybox[1]);
            GL.Begin(PrimitiveType.Quads);
            //back face
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
            GL.Normal3(0.0, 1.0, 0.0);
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
