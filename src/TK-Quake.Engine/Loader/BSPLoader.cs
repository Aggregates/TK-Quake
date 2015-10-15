using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader
{
    public class BSPLoader
    {
        private const string MAGIC_STRING    = "IBSP";
        private const int    BSP_VERSION     = 0x2E;
        private const int    NUM_DIRECTORIES = 17;

        // The structure of each lump entry.
        private struct LumpEntry
        {
            public int offset;
            public int length;
        }

        // The structure of the BSP header block.
        private struct HeaderEntry
        {
            public string      magic;
            public int         version;
            public LumpEntry[] lumps;
        }

        // The supported firstory types.
        private enum DirectoryTypes : byte
        {
            Entity,
            Texture,
            Plane,
            Node,
            Leaf,
            LeafFace,
            LeafBrush,
            Model,
            Brush,
            BrushSide,
            Vertex,
            MeshVert,
            Effect,
            Face,
            LightMap,
            LightVol,
            VisData
        };

        private BSP.Directory[] directoryParsers = new BSP.Directory[NUM_DIRECTORIES];
        HeaderEntry header;

        private string BSPFile = "";

        /// <summary>
        /// Constructor that allows vector swizzling to be enabled.
        /// </summary>
        /// <param name="swizzle">Sets whether vector components should be swizzled.</param>
        public BSPLoader(bool swizzle)
        {
            header.lumps = new LumpEntry[NUM_DIRECTORIES];

            // Instantiate all of the directory parsers.
            directoryParsers[(int)DirectoryTypes.Entity]    = new BSP.Entity(swizzle);
            directoryParsers[(int)DirectoryTypes.Texture]   = new BSP.Texture(swizzle);
            directoryParsers[(int)DirectoryTypes.Plane]     = new BSP.Plane(swizzle);
            directoryParsers[(int)DirectoryTypes.Node]      = new BSP.Node(swizzle);
            directoryParsers[(int)DirectoryTypes.Leaf]      = new BSP.Leaf(swizzle);
            directoryParsers[(int)DirectoryTypes.LeafFace]  = new BSP.LeafFace(swizzle);
            directoryParsers[(int)DirectoryTypes.LeafBrush] = new BSP.LeafBrush(swizzle);
            directoryParsers[(int)DirectoryTypes.Model]     = new BSP.Model(swizzle);
            directoryParsers[(int)DirectoryTypes.Brush]     = new BSP.Brush(swizzle);
            directoryParsers[(int)DirectoryTypes.BrushSide] = new BSP.BrushSide(swizzle);
            directoryParsers[(int)DirectoryTypes.Vertex]    = new BSP.Vertex(swizzle);
            directoryParsers[(int)DirectoryTypes.MeshVert]  = new BSP.MeshVert(swizzle);
            directoryParsers[(int)DirectoryTypes.Effect]    = new BSP.Effect(swizzle);
            directoryParsers[(int)DirectoryTypes.Face]      = new BSP.Face(swizzle);
            directoryParsers[(int)DirectoryTypes.LightMap]  = new BSP.LightMap(swizzle);
            directoryParsers[(int)DirectoryTypes.LightVol]  = new BSP.LightVol(swizzle);
            directoryParsers[(int)DirectoryTypes.VisData]   = new BSP.VisData(swizzle);
        }

        /// <summary>
        /// Constructor that accepts a BSP file to load and allows vector swizzling to be enabled.
        /// </summary>
        /// <param name="file">The BSP file to load.</param>
        /// <param name="swizzle">Sets whether vector components should be swizzled.</param>
        public BSPLoader(string file, bool swizzle) : this(swizzle)
        {
            SetBSPFile(file);
        }

        /// <summary>
        /// Sets the BSP file that should be loaded.
        /// </summary>
        /// <param name="file">The BSP file to load.</param>
        public void SetBSPFile(string file)
        {
            BSPFile = file;
        }

        /// <summary>
        /// Gets the loaded BSP file.
        /// </summary>
        public string GetBSPFile()
        {
            return (BSPFile);
        }

        /// <summary>
        /// Parses the contents of the specified BSP file.
        /// </summary>
        public bool ParseFile()
        {
            try
            {
                FileStream file = File.Open(BSPFile, FileMode.Open);

                // Extract the magic bytes.
				byte[] buf = new byte[4];
				file.Read (buf, 0, 4);
                header.magic = System.Text.Encoding.UTF8.GetString(buf);

                // Verify magic bytes.
                if (header.magic.CompareTo(MAGIC_STRING) != 0)
                {
                    return (false);
                }

				// Extract version number.
  				file.Read (buf, 0, 4);
				header.version = BitConverter.ToInt32 (buf, 0);

                // Verify version number.
                if (header.version != BSP_VERSION)
                {
                    return (false);
                }

                // Read in the directory information.
                for (int i = 0; i < NUM_DIRECTORIES; i++)
                {
                    file.Read (buf, 0, 4);
                    header.lumps[i].offset = BitConverter.ToInt32 (buf, 0);

                    file.Read (buf, 0, 4);
                    header.lumps[i].length = BitConverter.ToInt32 (buf, 0);

                    // Save the current position in the file.
                    long pos = file.Position;

                    directoryParsers[i].ParseDirectoryEntry(file, header.lumps[i].offset, header.lumps[i].length);

                    // Restore original position in the file.
                    file.Seek (pos, SeekOrigin.Begin);
                }

                file.Close ();
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("File '" + BSPFile + "' does not exist.");
                return (false);
            }

            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                return (false);
            }

			return (true);
        }

        /// <summary>
        /// Dumps the contents of the parsed BSP file into a human-readable format.
        /// TODO: Create as print functions in the directory parsers.
        /// </summary>
        public void DumpBSP()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("./BSPDump.txt");

            file.WriteLine("********* Header *********");
            file.WriteLine(String.Format("Magic Number: {0}", header.magic));
            file.WriteLine(String.Format("Version: {0}", header.version));

            for (int i = 0; i < NUM_DIRECTORIES; i++)
            {
                file.WriteLine(String.Format("Lump {0}", i));
                file.WriteLine(String.Format("\tOffset : {0}", header.lumps[i].offset));
                file.WriteLine(String.Format("\tLength : {0}", header.lumps[i].length));
            }

            file.WriteLine("");

            file.WriteLine("********* Entity Lump *********");
            file.WriteLine(String.Format("Size : {0}", directoryParsers[(int)DirectoryTypes.Entity].GetSize()));

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Entity].GetSize(); i++)
            {
                file.WriteLine(String.Format("Entity {0}", i));
                file.WriteLine(String.Format("{0}\n", GetEntity(i).entity));
            }

            file.WriteLine("");

            file.WriteLine("********* Texture Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Texture].GetSize(); i++)
            {
                file.WriteLine(String.Format("Texture {0}", i));
                file.WriteLine(String.Format("\tName : {0}", GetTexture(i).name));
                file.WriteLine(String.Format("\tFlags : {0}", GetTexture(i).flags));
                file.WriteLine(String.Format("\tContents : {0}", GetTexture(i).contents));
            }

            file.WriteLine("");

            file.WriteLine("********* Plane Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Plane].GetSize(); i++)
            {
                file.WriteLine(String.Format("Plane {0}", i));
                file.WriteLine(String.Format("\tVector form : {0}", GetPlane(i).plane));
            }

            file.WriteLine("");

            file.WriteLine("********* Node Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Node].GetSize(); i++)
            {
                file.WriteLine(String.Format("Node {0}", i));
                file.WriteLine(String.Format("\tPlane index : {0}", GetNode(i).plane));
                file.WriteLine(String.Format("\tChildren index : {0} {1}", GetNode(i).children[0], GetNode(i).children[1]));
                file.WriteLine(String.Format("\tMin Bounding Box : {0}", GetNode(i).mins));
                file.WriteLine(String.Format("\tMax Bounding Box : {0}", GetNode(i).maxs));
            }

            file.WriteLine("");

            file.WriteLine("********* Leaf Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Leaf].GetSize(); i++)
            {
                file.WriteLine(String.Format("Leaf {0}", i));
                file.WriteLine(String.Format("\tCluster {0}", GetLeaf(i).cluster));
                file.WriteLine(String.Format("\tMin Bounding Box : {0}", GetLeaf(i).mins));
                file.WriteLine(String.Format("\tMax Bounding Box : {0}", GetLeaf(i).maxs));
                file.WriteLine(String.Format("\tLeafFace {0}", GetLeaf(i).leafFace));
                file.WriteLine(String.Format("\tNb LeafFace {0}", GetLeaf(i).n_leafFaces));
                file.WriteLine(String.Format("\tLeafBrush {0}", GetLeaf(i).leafBrush));
                file.WriteLine(String.Format("\tNb LeafBrushes {0}", GetLeaf(i).n_leafBrushes));
            }

            file.WriteLine("");

            file.WriteLine("********* LeafFace Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.LeafFace].GetSize(); i++)
            {
                file.WriteLine(String.Format("LeafFace {0}", i));
                file.WriteLine(String.Format("\tFaceIndex {0}", GetLeafFace(i).face));
            }

            file.WriteLine("");

            file.WriteLine("********* LeafBrush Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.LeafBrush].GetSize(); i++)
            {
                file.WriteLine(String.Format("LeafBrush {0}", i));
                file.WriteLine(String.Format("\tBrushIndex {0}", GetLeafBrush(i).brush));
            }

            file.WriteLine("");

            file.WriteLine("********* Model Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Model].GetSize(); i++)
            {
                file.WriteLine(String.Format("Model {0}", i));
                file.WriteLine(String.Format("\tMin Bounding Box : {0}", GetModel(i).mins));
                file.WriteLine(String.Format("\tMax Bounding Box : {0}", GetModel(i).maxs));
                file.WriteLine(String.Format("\tFace {0}", GetModel(i).face));
                file.WriteLine(String.Format("\tNbFaces {0}", GetModel(i).n_faces));
                file.WriteLine(String.Format("\tBrush {0}", GetModel(i).brush));
                file.WriteLine(String.Format("\tNbBrushes {0}", GetModel(i).n_brushes));
            }

            file.WriteLine("");

            file.WriteLine("********* Brush Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Brush].GetSize(); i++)
            {
                file.WriteLine(String.Format("Brush {0}", i));
                file.WriteLine(String.Format("\tBrushSide {0}", GetBrush(i).brushSide));
                file.WriteLine(String.Format("\tNbBrushSides {0}", GetBrush(i).n_brushSides));
                file.WriteLine(String.Format("\tTextureIndex {0}", GetBrush(i).texture));
            }

            file.WriteLine("");

            file.WriteLine("********* BrushSide Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.BrushSide].GetSize(); i++)
            {
                file.WriteLine(String.Format("BrushSide {0}", i));
                file.WriteLine(String.Format("\tPlaneIndex {0}", GetBrushSide(i).plane));
                file.WriteLine(String.Format("\tTextureIndex {0}", GetBrushSide(i).texture));
            }

            file.WriteLine("");

            file.WriteLine("********* Vertex Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Vertex].GetSize(); i++)
            {
                file.WriteLine(String.Format("Vertex {0}", i));
                file.WriteLine(String.Format("\tPosition : {0}", GetVertex(i).position));
                file.WriteLine(String.Format("\tTexCoord0 : {0}", GetVertex(i).texCoord[0]));
                file.WriteLine(String.Format("\tTexCoord1 : {0}", GetVertex(i).texCoord[1]));
                file.WriteLine(String.Format("\tNormal : {0}", GetVertex(i).normal));
                file.WriteLine(String.Format("\tColor : {0}", GetVertex(i).colour));
            }

            file.WriteLine("");

            file.WriteLine("********* MeshVert Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.MeshVert].GetSize(); i++)
            {
                file.WriteLine(String.Format("MeshVert {0}", i));
                file.WriteLine(String.Format("\tVertex Index : {0}", GetMeshVert(i).offset));
            }

            file.WriteLine("");

            file.WriteLine("********* Effect Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Effect].GetSize(); i++)
            {
                file.WriteLine(String.Format("Effect {0}", i));
                file.WriteLine(String.Format("\tName : {0}", GetEffect(i).name));
                file.WriteLine(String.Format("\tBrush : {0}", GetEffect(i).brush));
                file.WriteLine(String.Format("\tUnknown : {0}", GetEffect(i).unknown));
            }

            file.WriteLine("");

            file.WriteLine("********* Face Lump *********");
            file.WriteLine(String.Format("Num Faces {0}", directoryParsers[(int)DirectoryTypes.Face].GetSize()));

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.Face].GetSize(); i++)
            {
                file.WriteLine(String.Format("Face {0}", i));
                file.WriteLine(String.Format("\tTextureIndex : {0}", GetFace(i).texture));
                file.WriteLine(String.Format("\tEffectIndex : {0}", GetFace(i).effect));
                file.WriteLine(String.Format("\tType : {0}", GetFace(i).type));
                file.WriteLine(String.Format("\tVertex : {0}", GetFace(i).vertex));
                file.WriteLine(String.Format("\tNbVertices : {0}", GetFace(i).n_vertexes));
                file.WriteLine(String.Format("\tMeshVertex : {0}", GetFace(i).meshVert));
                file.WriteLine(String.Format("\tNbMeshVertices : {0}", GetFace(i).n_meshVerts));
                file.WriteLine(String.Format("\tLightMapIndex : {0}", GetFace(i).lm_index));
                file.WriteLine(String.Format("\tLightMapCorner : {0} {1}", GetFace(i).lm_start[0], GetFace(i).lm_start[1]));
                file.WriteLine(String.Format("\tLightmapSize : {0} {1}", GetFace(i).lm_size[0], GetFace(i).lm_size[1]));
                file.WriteLine(String.Format("\tLightmapOrigin : {0}", GetFace(i).lm_origin));
                file.WriteLine(String.Format("\tLightmapVecs S : {0}", GetFace(i).lm_vecs[0]));
                file.WriteLine(String.Format("\tLightmapVecs T : {0}", GetFace(i).lm_vecs[1]));
                file.WriteLine(String.Format("\tNormal : {0}", GetFace(i).normal.ToString ()));
                file.WriteLine(String.Format("\tPatchSize : {0} {1}", GetFace(i).size[0], GetFace(i).size[1]));
            }

            file.WriteLine("");

            file.WriteLine("********* LightMap Lump *********");
            file.WriteLine(String.Format("NbLightMaps {0}", directoryParsers[(int)DirectoryTypes.LightMap]));
            file.WriteLine("");

            file.WriteLine("********* LightVol Lump *********");

            for (int i = 0; i < directoryParsers[(int)DirectoryTypes.LightVol].GetSize(); i++)
            {
                file.WriteLine(String.Format("LightVol {0}", i));
                file.WriteLine(String.Format("\tAmbient : {0} {1} {2}", GetLightVol(i).ambient[0], GetLightVol(i).ambient[1], GetLightVol(i).ambient[2]));
                file.WriteLine(String.Format("\tDirectional : {0} {1} {2}", GetLightVol(i).directional[0], GetLightVol(i).directional[1], GetLightVol(i).directional[2]));
                file.WriteLine(String.Format("\tDir : {0} {1}", GetLightVol(i).dir[0], GetLightVol(i).dir[1]));
            }

            file.WriteLine("");

            file.WriteLine("********* VisData Lump *********");

            if (directoryParsers[(int)DirectoryTypes.LightVol].GetSize() > 0)
            {
                file.WriteLine(String.Format("\tNbCluster {0}", GetVisData().n_vecs));
                file.WriteLine(String.Format("\tBytePerCluster {0}", GetVisData().sz_vecs));
            }

            file.WriteLine("");

            file.Close();
        }

        /// <summary>
        /// Return the entities from the parsed file.
        /// </summary>
        public BSP.Entity.EntityEntry[] GetEntities()
        {
            return(((BSP.Entity)directoryParsers[(int)DirectoryTypes.Entity]).GetEntities());
        }

        /// <summary>
        /// Return an entity from the parsed file.
        /// </summary>
        /// <param name="entity">The index of the entity to retrieve.</param>
        public BSP.Entity.EntityEntry GetEntity(int entity)
        {
            return(((BSP.Entity)directoryParsers[(int)DirectoryTypes.Entity]).GetEntity(entity));
        }

        /// <summary>
        /// Return the textures from the parsed file.
        /// </summary>
        public BSP.Texture.TextureEntry[] GetTextures()
        {
            return(((BSP.Texture)directoryParsers[(int)DirectoryTypes.Texture]).GetTextures());
        }

        /// <summary>
        /// Return a texture from the parsed file.
        /// </summary>
        /// <param name="texture">The index of the texture to retrieve.</param>
        public BSP.Texture.TextureEntry GetTexture(int texture)
        {
            return(((BSP.Texture)directoryParsers[(int)DirectoryTypes.Texture]).GetTexture (texture));
        }

        /// <summary>
        /// Return the planes from the parsed file.
        /// </summary>
        public BSP.Plane.PlaneEntry[] GetPlanes()
        {
            return(((BSP.Plane)directoryParsers[(int)DirectoryTypes.Plane]).GetPlanes());
        }

        /// <summary>
        /// Return a plane from the parsed file.
        /// </summary>
        /// <param name="plane">The index of the plane to retrieve.</param>
        public BSP.Plane.PlaneEntry GetPlane(int plane)
        {
            return(((BSP.Plane)directoryParsers[(int)DirectoryTypes.Plane]).GetPlane(plane));
        }

        /// <summary>
        /// Return the nodes from the parsed file.
        /// </summary>
        public BSP.Node.NodeEntry[] GetNodes()
        {
            return(((BSP.Node)directoryParsers[(int)DirectoryTypes.Node]).GetNodes());
        }

        /// <summary>
        /// Return a node from the parsed file.
        /// </summary>
        /// <param name="node">The index of the node to retrieve.</param>
        public BSP.Node.NodeEntry GetNode(int node)
        {
            return(((BSP.Node)directoryParsers[(int)DirectoryTypes.Node]).GetNode(node));
        }

        /// <summary>
        /// Return the leafs from the parsed file.
        /// </summary>
        public BSP.Leaf.LeafEntry[] GetLeafs()
        {
            return(((BSP.Leaf)directoryParsers[(int)DirectoryTypes.Leaf]).GetLeafs());
        }

        /// <summary>
        /// Return a leaf from the parsed file.
        /// </summary>
        /// <param name="leaf">The index of the leaf to retrieve.</param>
        public BSP.Leaf.LeafEntry GetLeaf(int leaf)
        {
            return(((BSP.Leaf)directoryParsers[(int)DirectoryTypes.Leaf]).GetLeaf(leaf));
        }

        /// <summary>
        /// Return the leaffaces from the parsed file.
        /// </summary>
        public BSP.LeafFace.LeafFaceEntry[] GetLeafFaces()
        {
            return(((BSP.LeafFace)directoryParsers[(int)DirectoryTypes.LeafFace]).GetLeafFaces());
        }

        /// <summary>
        /// Return a leafface from the parsed file.
        /// </summary>
        /// <param name="leafface">The index of the leafface to retrieve.</param>
        public BSP.LeafFace.LeafFaceEntry GetLeafFace(int leafFace)
        {
            return(((BSP.LeafFace)directoryParsers[(int)DirectoryTypes.LeafFace]).GetLeafFace(leafFace));
        }

        /// <summary>
        /// Return the leafbrushes from the parsed file.
        /// </summary>
        public BSP.LeafBrush.LeafBrushEntry[] GetLeafBrushes()
        {
            return(((BSP.LeafBrush)directoryParsers[(int)DirectoryTypes.LeafBrush]).GetLeafBrushes());
        }

        /// <summary>
        /// Return a leafbrush from the parsed file.
        /// </summary>
        /// <param name="leafbrush">The index of the leafbrush to retrieve.</param>
        public BSP.LeafBrush.LeafBrushEntry GetLeafBrush(int leafBrush)
        {
            return(((BSP.LeafBrush)directoryParsers[(int)DirectoryTypes.LeafBrush]).GetLeafBrush(leafBrush));
        }

        /// <summary>
        /// Return the models from the parsed file.
        /// </summary>
        public BSP.Model.ModelEntry[] GetModels()
        {
            return(((BSP.Model)directoryParsers[(int)DirectoryTypes.Model]).GetModels());
        }

        /// <summary>
        /// Return a model from the parsed file.
        /// </summary>
        /// <param name="model">The index of the model to retrieve.</param>
        public BSP.Model.ModelEntry GetModel(int model)
        {
            return(((BSP.Model)directoryParsers[(int)DirectoryTypes.Model]).GetModel(model));
        }

        /// <summary>
        /// Return the brushes from the parsed file.
        /// </summary>
        public BSP.Brush.BrushEntry[] GetBrushes()
        {
            return(((BSP.Brush)directoryParsers[(int)DirectoryTypes.Brush]).GetBrushes());
        }

        /// <summary>
        /// Return a brush from the parsed file.
        /// </summary>
        /// <param name="brush">The index of the brush to retrieve.</param>
        public BSP.Brush.BrushEntry GetBrush(int brush)
        {
            return(((BSP.Brush)directoryParsers[(int)DirectoryTypes.Brush]).GetBrush(brush));
        }

        /// <summary>
        /// Return the brushsides from the parsed file.
        /// </summary>
        public BSP.BrushSide.BrushSideEntry[] GetBrushSides()
        {
            return(((BSP.BrushSide)directoryParsers[(int)DirectoryTypes.BrushSide]).GetBrushSides());
        }

        /// <summary>
        /// Return a brushside from the parsed file.
        /// </summary>
        /// <param name="brushside">The index of the brushside to retrieve.</param>
        public BSP.BrushSide.BrushSideEntry GetBrushSide(int brushSide)
        {
            return(((BSP.BrushSide)directoryParsers[(int)DirectoryTypes.BrushSide]).GetBrushSide(brushSide));
        }

        /// <summary>
        /// Return the vertexes from the parsed file.
        /// </summary>
        public BSP.Vertex.VertexEntry[] GetVertexes()
        {
            return(((BSP.Vertex)directoryParsers[(int)DirectoryTypes.Vertex]).GetVertexes());
        }

        /// <summary>
        /// Return a vertex from the parsed file.
        /// </summary>
        /// <param name="vertex">The index of the vertex to retrieve.</param>
        public BSP.Vertex.VertexEntry GetVertex(int vertex)
        {
            return(((BSP.Vertex)directoryParsers[(int)DirectoryTypes.Vertex]).GetVertex(vertex));
        }

        /// <summary>
        /// Return the meshverts from the parsed file.
        /// </summary>
        public BSP.MeshVert.MeshVertEntry[] GetMeshVerts()
        {
            return(((BSP.MeshVert)directoryParsers[(int)DirectoryTypes.MeshVert]).GetMeshVerts());
        }

        /// <summary>
        /// Return a meshvert from the parsed file.
        /// </summary>
        /// <param name="meshvert">The index of the meshvert to retrieve.</param>
        public BSP.MeshVert.MeshVertEntry GetMeshVert(int meshVert)
        {
            return(((BSP.MeshVert)directoryParsers[(int)DirectoryTypes.MeshVert]).GetMeshVert(meshVert));
        }

        /// <summary>
        /// Return the effects from the parsed file.
        /// </summary>
        public BSP.Effect.EffectEntry[] GetEffects()
        {
            return(((BSP.Effect)directoryParsers[(int)DirectoryTypes.Effect]).GetEffects());
        }

        /// <summary>
        /// Return a effect from the parsed file.
        /// </summary>
        /// <param name="effect">The index of the effect to retrieve.</param>
        public BSP.Effect.EffectEntry GetEffect(int effect)
        {
            return(((BSP.Effect)directoryParsers[(int)DirectoryTypes.Effect]).GetEffect(effect));
        }

        /// <summary>
        /// Return the faces from the parsed file.
        /// </summary>
        public BSP.Face.FaceEntry[] GetFaces()
        {
            return(((BSP.Face)directoryParsers[(int)DirectoryTypes.Face]).GetFaces());
        }

        /// <summary>
        /// Return a face from the parsed file.
        /// </summary>
        /// <param name="face">The index of the face to retrieve.</param>
        public BSP.Face.FaceEntry GetFace(int face)
        {
            return(((BSP.Face)directoryParsers[(int)DirectoryTypes.Face]).GetFace(face));
        }

        /// <summary>
        /// Return the lightmaps from the parsed file.
        /// </summary>
        public BSP.LightMap.LightMapEntry[] GetLightMaps()
        {
            return(((BSP.LightMap)directoryParsers[(int)DirectoryTypes.LightMap]).GetLightMaps());
        }

        /// <summary>
        /// Return a lightmap from the parsed file.
        /// </summary>
        /// <param name="lightmap">The index of the lightmap to retrieve.</param>
        public BSP.LightMap.LightMapEntry GetLightMap(int lightMap)
        {
            return(((BSP.LightMap)directoryParsers[(int)DirectoryTypes.LightMap]).GetLightMap(lightMap));
        }

        /// <summary>
        /// Return the lightvols from the parsed file.
        /// </summary>
        public BSP.LightVol.LightVolEntry[] GetLightVols()
        {
            return(((BSP.LightVol)directoryParsers[(int)DirectoryTypes.LightVol]).GetLightVols());
        }

        /// <summary>
        /// Return a lightvol from the parsed file.
        /// </summary>
        /// <param name="lightvol">The index of the lightvol to retrieve.</param>
        public BSP.LightVol.LightVolEntry GetLightVol(int lightVol)
        {
            return(((BSP.LightVol)directoryParsers[(int)DirectoryTypes.LightVol]).GetLightVol(lightVol));
        }

        /// <summary>
        /// Return the visdata from the parsed file.
        /// </summary>
        public BSP.VisData.VisDataEntry GetVisData()
        {
            return(((BSP.VisData)directoryParsers[(int)DirectoryTypes.VisData]).GetVisData());
        }
    }
}
