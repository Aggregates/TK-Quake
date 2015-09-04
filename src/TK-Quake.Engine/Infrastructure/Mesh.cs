using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using ObjLoader.Loader.Loaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKQuake.Engine.Infrastructure
{
    public class Mesh
    {
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector2[] Textures { get; set; }
        public uint[] Indices { get; set; }
    }

    public static class MeshExtensions
    {
        /// <summary>
        /// This converts from the ObjLoader format to our Mesh format
        /// </summary>
        /// <param name="objLoaderResult">The result from ObjLoader</param>
        /// <returns></returns>
        public static Mesh ToMesh(this LoadResult objLoaderResult)
        {
            //create mesh
            //note that each array is the length of vertices
            var mesh = new Mesh
            {
                Vertices = new Vector3[objLoaderResult.Vertices.Count],
                Normals = new Vector3[objLoaderResult.Vertices.Count],
                Textures = new Vector2[objLoaderResult.Vertices.Count]
            };

            var indiceList = new List<uint>();
            foreach (var group in objLoaderResult.Groups)
            {
                foreach (var face in group.Faces)
                {
                    for (var i = 0; i < face.Count; ++i)
                    {
                        //get the face
                        var faceVertex = face[i];

                        //get vertex index
                        var vertIndex = faceVertex.VertexIndex - 1;

                        //load data
                        var vertex = objLoaderResult.Vertices[vertIndex];
                        var normal = objLoaderResult.Normals[faceVertex.NormalIndex - 1];
                        var texture = objLoaderResult.Textures[faceVertex.TextureIndex - 1];

                        //save to mesh
                        //this rearranges the data so that indices[i] is contiguous across
                        //verticies, normals and textures
                        mesh.Vertices[vertIndex] = new Vector3(vertex.X, vertex.Y, vertex.Z);
                        mesh.Normals[vertIndex] = new Vector3(normal.X, normal.Y, normal.Z);
                        mesh.Textures[vertIndex] = new Vector2(texture.X, texture.Y);

                        indiceList.Add((uint)vertIndex);
                    }
                }
            }
            //set the mesh indices
            mesh.Indices = indiceList.ToArray();

            return mesh;
        }
    }
}
