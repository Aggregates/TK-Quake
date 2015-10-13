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
        private readonly Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();
        private string _BSP = null;
//        private float _gameScreenRatio;

        public CameraTestScreen(string BSPFile/*, float screenRatio*/)
        {
            _renderer = Renderer.Singleton ();
            _textureManager = TextureManager.Singleton ();
            _BSP = BSPFile;
//            _gameScreenRatio = screenRatio;

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

            Components.Add(new BSPComponent(_BSP, _camera/*, _gameScreenRatio*/));
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
            gunEntity.Components.Add(new RotateOnUpdateComponent(gunEntity, new Vector3(0, (float)Math.PI/2, 0)));
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

    class BSPComponent : IComponent
    {
        private const string ENTITY_ID = "BSP";

        private readonly Renderer _renderer = Renderer.Singleton();

        private BSPRenderer BSP;
        private Camera camera;
//        private float gameScreenRation;
        private int totalMeshes;
        private TextureManager texManager = TextureManager.Singleton ();

        private BSPComponent()
        {
            camera = null;
            BSP = null;
            totalMeshes = 0;
        }

        public BSPComponent(string BSPFile, Camera cam/*, float screenRatio*/) : this()
        {
            camera = cam;
//            gameScreenRation = screenRatio;
            ChangeBSP (BSPFile);
        }

        public void ChangeBSP(string BSPFile)
        {
            UnloadAllTextures();
            BSP = new BSPRenderer (BSPFile);
            LoadAllTextures();
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            // Make sure there are no meshes left over from the last rendering.
            for (int meshCount = 0; meshCount < totalMeshes; meshCount++)
            {
                string id = string.Format ("{0}{1}", ENTITY_ID, meshCount);

                // Remove the mesh from the system.
                if (_renderer.IsMeshRegistered (id) == true)
                {
                    _renderer.UnregisterMesh (id);
                }
            }

            // Generate, register, and render the meshes that are potentially visible to the camera.
            List<Mesh> meshes = BSP.GetMesh (camera);

            totalMeshes = meshes.Count;

            for (int meshCount = 0; meshCount < totalMeshes; meshCount++)
            {
                string id = string.Format ("{0}{1}", ENTITY_ID, meshCount);
                _renderer.RegisterMesh (id, meshes[meshCount]);

                // Create a renderable entity.
                var BSPEntity = RenderableEntity.Create ();
                BSPEntity.Id = id;
                BSPEntity.Position = camera.Position;
                BSPEntity.Scale = 0.01f;
                BSPEntity.Translation = Matrix4.Identity;
                BSPEntity.Rotation = Vector3.Zero;

                // Render the BSP entity.
                _renderer.DrawEntity (BSPEntity);
            }

            meshes.Clear ();
            meshes = null;
        }

        private void LoadAllTextures()
        {
            if (BSP == null)
            {
                return;
            }

            if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered("textures/notexture.jpg") == false))
            {
                texManager.Add ("textures/notexture.jpg", "textures/notexture.jpg");
            }

            foreach (TKQuake.Engine.Loader.BSP.Texture.TextureEntry texture in BSP.GetLoader().GetTextures())
            {
                string JPG = texture.name + ".jpg";
                string TGA = texture.name + ".tga";

                if (texture.name.Contains ("noshader") == false)
                {
                    if ((File.Exists (JPG) == true) && (texManager.Registered(JPG) == false))
                    {
                        texManager.Add (JPG, JPG);
                    }

                    else if ((File.Exists (TGA) == true) && (texManager.Registered(TGA) == false))
                    {
                        texManager.Add (TGA, TGA);
                    }
                }
            }
        }

        private void UnloadAllTextures()
        {
            if (BSP == null)
            {
                return;
            }

            if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered("textures/notexture.jpg") == false))
            {
                texManager.Remove ("textures/notexture.jpg");
            }

            foreach (TKQuake.Engine.Loader.BSP.Texture.TextureEntry texture in BSP.GetLoader().GetTextures())
            {
                string JPG = texture.name + ".jpg";
                string TGA = texture.name + ".tga";

                if (texture.name.Contains ("noshader") == false)
                {
                    if ((File.Exists (JPG) == true) && (texManager.Registered(JPG) == true))
                    {
                        texManager.Remove (JPG);
                    }

                    else if ((File.Exists (TGA) == true) && (texManager.Registered(TGA) == true))
                    {
                        texManager.Remove (TGA);
                    }
                }
            }
        }
    }
}
