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
        public Vector3[] Positions { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] Textures { get; set; }
        public uint[] Indices { get; set; }
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

            var outPositions = new List<Vector3>();
            var outNormals = new List<Vector3>();
            var outTextures = new List<Vector2>();

            //build index
            var hash = new Dictionary<Vertex, uint>();
            var indicies = new List<uint>();
            for (var i = 0; i < inPositions.Count; i++)
            {
                var v = new Vertex(inPositions[i], inNormals[i], inTextures[i]);
                if (hash.ContainsKey(v))
                    indicies.Add(hash[v]);
                else
                {
                    outPositions.Add(v.Position);
                    outNormals.Add(v.Normal);
                    outTextures.Add(v.TexCoord);

                    var index = (uint) outPositions.Count - 1;
                    indicies.Add(index);
                    hash.Add(v, index);
                }
            }

            //set mesh
            mesh.Indices = indicies.ToArray();
            mesh.Positions = outPositions.ToArray();
            mesh.Normals = outNormals.ToArray();
            mesh.Textures = outTextures.ToArray();

            using (var output = File.OpenWrite("mesh-output.txt"))
            {
                output.Seek(0, SeekOrigin.Begin);
                using (var writer = new StreamWriter(output))
                {
                    foreach (var index in mesh.Indices)
                    {
                        var v = mesh.Positions[index];
                        var n = mesh.Normals[index];
                        var t = mesh.Textures[index];

                        writer.WriteLine("v {0}, {1}, {2}", v.X, v.Y, v.Z);
                        writer.WriteLine("n {0}, {1}, {2}", n.X, n.Y, n.Z);
                        writer.WriteLine("t {0}, {1}", t.X, t.Y);
                    }
                }
            }

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
