﻿using OpenTK;
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

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private Camera _camera = new Camera();
        private readonly IObjLoader _objLoader = new ObjLoaderFactory().Create();
        private string _BSP = null;

        public CameraTestScreen(string BSPFile)
        {
            _renderer = Renderer.Singleton ();
            _textureManager = TextureManager.Singleton ();
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
                    new Vertex(v1, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                    new Vertex(v2, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                    new Vertex(v3, Vector3.Zero, Vector2.Zero, Vector2.Zero),
                    new Vertex(v4, Vector3.Zero, Vector2.Zero, Vector2.Zero),
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
        // The base ID for every mesh that is generated by this component.
        private const string ENTITY_ID = "BSP";

        // The system renderer.
        private readonly Renderer _renderer = Renderer.Singleton();

        // The system texture manager.
        private readonly TextureManager texManager = TextureManager.Singleton ();

        // The BSP loader and renderer.
        private BSPLoader loader;
        private BSPRenderer renderer;

        // The total number of meshes in the current BSP file.
        private int totalMeshes;

        // The renderable entities.
        Dictionary<int, List<RenderableEntity>> BSPEntities;

        // The camera.
        private Camera camera;

        private BSPComponent()
        {
            camera = null;
            loader = null;
            renderer = null;
            totalMeshes = 0;
            BSPEntities = null;
        }

        /// <summary>
        /// Construct the BSPComponent.
        /// </summary>
        /// <param name="BSPFile">The BSP file to render.</param>
        /// <param name="camera">The camera.</param>
        public BSPComponent(string BSPFile, ref Camera cam) : this()
        {
            camera = cam;
            ChangeBSP (BSPFile);
        }

        /// <summary>
        /// Loads a new BSP file for rendering.
        /// </summary>
        /// <param name="BSPFile">The BSP file to render.</param>
        public void ChangeBSP(string BSPFile)
        {
            // Make sure there are no meshes or textures loaded from the previous BSP file.
            UnloadAllMeshes();
            UnloadAllTextures();

            // Create the new loader and parse the file.
            loader = new BSPLoader(BSPFile, true);
            loader.ParseFile ();

            // TODO: This can be removed once "debugging" is finished.
            loader.DumpBSP ();

            // Create the new renderer.
            renderer = new BSPRenderer(loader);

            // Load all of the textures and meshes for the current BSP file.
            // Order is important here.
            LoadAllTextures();
            LoadAllMeshes();
        }

        public void Startup() { }
        public void Shutdown() { }

        /// <summary>
        /// Render all of the faces that are visible from the current camera position.
        /// </summary>
        /// <param name="elapsedTime">Not used.</param>
        public void Update(double elapsedTime)
        {
//            var visibleFaces = renderer.GetVisibleFaces(camera);
            var visibleFaces = BSPEntities.Keys;
            foreach (var face in visibleFaces)
            {
                BSPEntities[face].ForEach(_renderer.DrawEntity);
            }

            // Get the list of visible faces.
//            List<int> visibleFaces = renderer.GetVisibleFaces(camera);
//
//            // Iterate over all of the visible faces.
//            foreach (int face in visibleFaces)
//            {
//                // Iterate over all of the renderable entities for each visible face.
//                try
//                {
//                    foreach (RenderableEntity BSPEntity in BSPEntities[face])
//                    {
//                        // Render the BSP entity.
//                        _renderer.DrawEntity (BSPEntity);
//                    }
//                }
//
//                catch (KeyNotFoundException)
//                {
//                    Console.WriteLine ("KeyNotFoundException thrown for face {0}", face);
//                }
//            }
        }

        /// <summary>
        /// Load all meshes for this BSP file.
        /// </summary>
        private void LoadAllMeshes()
        {
            // Generate and register all of the meshes in the BSP file.
            Dictionary<int, List<Mesh>> meshes = renderer.GetAllMeshes();

            // Reset the mesh count.
            totalMeshes = 0;

            // Create a new list of renderable entities.
            BSPEntities = new Dictionary<int, List<RenderableEntity>>();

            // Register each mesh with the renderer.
            foreach (KeyValuePair<int, List<Mesh>> meshSet in meshes)
            {
                // Make sure the entry in the dictuinary exists for this face.
                BSPEntities [meshSet.Key] = new List<RenderableEntity> ();

                // Create renderable entities.
                foreach (Mesh mesh in meshSet.Value)
                {
                    string id = string.Format ("{0}{1}", ENTITY_ID, totalMeshes);

                    // Make sure we haven't registered this mesh already.
                    if (_renderer.IsMeshRegistered (id) == false)
                    {
                        // Register the mesh.
                        _renderer.RegisterMesh (id, mesh);

                        // Create a renderable entity for this mesh.
                        RenderableEntity BSPEntity = RenderableEntity.Create ();
                        BSPEntity.Id = id;
                        BSPEntity.Position = camera.Position;
                        BSPEntity.Scale = 0.01f;
                        BSPEntity.Translation = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
                        BSPEntity.Rotation = Vector3.Zero;
                        BSPEntity.Transform = Matrix4.CreateTranslation(camera.Position)*Matrix4.CreateScale(BSPEntity.Scale);

                        // Add it to our renderable entity dictionary.
                        BSPEntities[meshSet.Key].Add(BSPEntity);

                        // Update the mesh count.
                        totalMeshes++;
                    }
                }
            }
        }

        /// <summary>
        /// Unload all meshes for this BSP file.
        /// </summary>
        private void UnloadAllMeshes()
        {
            if (BSPEntities != null)
            {
                BSPEntities.Clear ();
                BSPEntities = null;
            }

            // Unload all of the meshes from the previous BSP file.
            for (int meshCount = 0; meshCount < totalMeshes; meshCount++)
            {
                string id = string.Format ("{0}{1}", ENTITY_ID, meshCount);

                // Remove the mesh from the renderer.
                if (_renderer.IsMeshRegistered (id) == true)
                {
                    _renderer.UnregisterMesh (id);
                }
            }
        }

        /// <summary>
        /// Load all textures for this BSP file.
        /// </summary>
        private void LoadAllTextures()
        {
            // Make sure a BSP file has been loaded.
            if ((loader == null) || (renderer == null))
            {
                return;
            }

            // Load the notexture texture.
            if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered("textures/notexture.jpg") == false))
            {
                texManager.Add ("textures/notexture.jpg", "textures/notexture.jpg");
            }

            // Iterate through every texture in the BSP and load it.
            foreach (TKQuake.Engine.Loader.BSP.Texture.TextureEntry texture in loader.GetTextures())
            {
                string JPG = texture.name + ".jpg";
                string TGA = texture.name + ".tga";

                // Ignore the noshader textures.
                // TODO: What are these anyway?
                if (texture.name.Contains ("noshader") == false)
                {
                    // Make the sure the texture exists and make sure it has not already been loaded.
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

            for (int face = 0; face < loader.GetFaces().Length; face++)
            {
                Face.FaceEntry currentFace = loader.GetFace(face);

                if (currentFace.lm_index >= 0)
                {
                    byte[,,] lightMap = new byte[currentFace.lm_size[0], currentFace.lm_size[1], 3];

                    LightMap.LightMapEntry map = loader.GetLightMap(currentFace.lm_index);

                    // Make a copy of the relevant part of the light map.
                    for (int x = 0; x < currentFace.lm_size[0]; x++)
                    {
                        for (int y = 0; y < currentFace.lm_size[1]; y++)
                        {
                            lightMap[x, y, 0] = map.map[x + currentFace.lm_start[0], y + currentFace.lm_start[1], 0];
                            lightMap[x, y, 1] = map.map[x + currentFace.lm_start[0], y + currentFace.lm_start[1], 1];
                            lightMap[x, y, 2] = map.map[x + currentFace.lm_start[0], y + currentFace.lm_start[1], 2];
                        }
                    }

                    texManager.AddUV(lightMap, loader.GetFace (face).lm_size, string.Format ("FACE{0}", face));
                }
            }
        }

        /// <summary>
        /// Unload all textures that were loaded for this BSP file.
        /// </summary>
        private void UnloadAllTextures()
        {
            // Make sure a BSP file has been loaded.
            if ((loader == null) || (renderer == null))
            {
                return;
            }

            // Unload the notexture texture.
            if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered("textures/notexture.jpg") == false))
            {
                texManager.Remove ("textures/notexture.jpg");
            }

            // Iterate through every texture in the BSP and unload it.
            foreach (TKQuake.Engine.Loader.BSP.Texture.TextureEntry texture in loader.GetTextures())
            {
                string JPG = texture.name + ".jpg";
                string TGA = texture.name + ".tga";

                // Ignore the noshader textures.
                // TODO: What are these anyway?
                if (texture.name.Contains ("noshader") == false)
                {
                    // Make the sure the texture exists and make sure it has already been loaded.
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
