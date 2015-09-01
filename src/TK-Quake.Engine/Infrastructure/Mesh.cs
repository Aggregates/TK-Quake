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
        public static Mesh ToMesh(this LoadResult objLoaderResult)
        {
            var mesh = new Mesh
            {
                Vertices = objLoaderResult.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(),
                Normals = objLoaderResult.Normals.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(),
                Textures = objLoaderResult.Textures.Select(t => new Vector2(t.X, t.Y)).ToArray(),
            };

            var indiceList = new List<uint>();
            foreach (var group in objLoaderResult.Groups)
            {
                foreach (var face in group.Faces)
                {
                    for (var i = 0; i < face.Count; ++i)
                    {
                        indiceList.Add((uint)face[i].VertexIndex);
                    }
                }
            }
            mesh.Indices = indiceList.ToArray();

            return mesh;
        }
    }
}
