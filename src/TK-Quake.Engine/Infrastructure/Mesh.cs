using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vertex = TKQuake.Engine.Infrastructure.Math.Vertex;

namespace TKQuake.Engine.Infrastructure
{
    public class Mesh
    {
        public Vertex[] Vertices { get; set; }
        public int[] Indices { get; set; }
        public int VaoId { get; set; }
        public int VboId { get; set; }
        public int EboId { get; set; }
        public Texture.Texture texture;
        public Texture.Texture lightMap;

        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

    }

    public static class MeshExtensions
    {
        /// <summary>
        /// This converts from the ObjLoader format to our Mesh format
        /// </summary>
        /// <param name="res">The result from ObjLoader</param>
        /// <returns></returns>
        public static Mesh ToMesh(this LoadResult res)
        {
            //create mesh
            //note that each array is the length of vertices
            var mesh = new Mesh();
            var inPositions = new List<Vector3>();
            var inNormals = new List<Vector3>();
            var inTextures = new List<Vector2>();
            

            // Mins and Maxs
            float minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
            float maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;

            var indiceList = new List<int>();
            foreach (var group in res.Groups)
            {
                foreach (var face in group.Faces)
                {
                    for (var i = 0; i < face.Count; i++)
                    {
                        var vertexIndex = face[i].VertexIndex - 1;
                        var normalIndex = face[i].NormalIndex - 1;
                        var textureIndex = face[i].TextureIndex - 1;

                        inPositions.Add(res.Vertices[vertexIndex].ToVector3());
                        inNormals.Add(res.Normals[normalIndex].ToVector3());
                        inTextures.Add(res.Textures[textureIndex].ToVector2());
                    }
                }
            }

            //build index
            var hash = new Dictionary<Vertex, int>();
            var indicies = new List<int>();
            var vertices = new List<Vertex>();
            for (var i = 0; i < inPositions.Count; i++)
            {
                var v = new Vertex(inPositions[i], inNormals[i], inTextures[i], new Vector2(0, 0));
                //v.Position = new Vector3(v.Position.X, v.Position.Z, v.Position.Y);
                //v.Normal = new Vector3(v.Normal.X, v.Normal.Z, v.Normal.Y);

                if (hash.ContainsKey(v))
                    indicies.Add(hash[v]);
                else
                {
                    vertices.Add(v);
                    // Update Min/Max vertices for Bounding Box
                    minX = (v.Position.X < minX) ? v.Position.X : minX;
                    minY = (v.Position.Y < minY) ? v.Position.Y : minY;
                    minZ = (v.Position.Z < minZ) ? v.Position.Z : minZ;

                    maxX = (v.Position.X > maxX) ? v.Position.X : maxX;
                    maxY = (v.Position.Y > maxY) ? v.Position.Y : maxY;
                    maxZ = (v.Position.Z > maxZ) ? v.Position.Z : maxZ;

                    var index = vertices.Count - 1;
                    indicies.Add(index);
                    hash.Add(v, index);
                }
            }

            //set mesh
            mesh.Indices = indicies.ToArray();
            mesh.Vertices = vertices.ToArray();

            mesh.Min = new Vector3(minX, minY, minZ);
            mesh.Max = new Vector3(maxX, maxY, maxZ);

            return mesh;
        }

        public static Vector3 ToVector3(this ObjLoader.Loader.Data.VertexData.Vertex v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector3 ToVector3(this Normal n)
        {
            return new Vector3(n.X, n.Y, n.Z);
        }

        public static Vector2 ToVector2(this ObjLoader.Loader.Data.VertexData.Texture t)
        {
            return new Vector2(t.X, t.Y);
        }
    }
}
