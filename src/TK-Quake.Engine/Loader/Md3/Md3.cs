using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using TKQuake.Engine.Infrastructure;
using TKQuake.Engine.Infrastructure.Math;
using D = System.Diagnostics.Debug;
using static System.Math;

namespace TKQuake.Engine.Loader.Md3
{
    public class Md3
    {
        public string Id { get; set; }
        public int Version { get; set; }
        string FileName { get; set; }
        int Flags { get; set; }
        int NumFrames { get; set; }
        int NumTags { get; set; }
        int NumSurfaces { get; set; }
        int NumSkins { get; set; }
        int FramesStart { get; set; }
        int TagsStart { get; set; }
        int SurfacesStart { get; set; }
        int Eof { get; set; }

        public IList<Md3Tag> Tags { get; set; }
        public IList<Md3Frame> Frames { get; set; }
        public IList<Md3Surface> Surfaces { get; set; }

        #region Constants
        private const int IdLength = 4;
        private const int NameLength = 68;

        private const int FrameNameLength = 16;
        private const int TagNameLength = 64;

        private const int SurfaceIdLength = 4;
        private const int SurfaceNameLength = 64;

        private const int ShaderNameLength = 64;

        private const int VertexNormalsLength = 2;

        #endregion

        private Md3() { }

        public static Md3 FromFile(string fileName)
        {
            D.Assert(File.Exists(fileName));

            var md3 = new Md3();
            var bytes = File.ReadAllBytes(fileName);
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream, Encoding.Default))
                {
                    md3.Id = new string(reader.ReadChars(IdLength));
                    md3.Version = reader.ReadInt32();
                    var fName = new string(reader.ReadChars(NameLength));
                    md3.FileName = fName.Substring(0, fName.IndexOf('\0'));
                    //md3.Flags = reader.ReadInt32();
                    md3.NumFrames = reader.ReadInt32();
                    md3.NumTags = reader.ReadInt32();
                    md3.NumSurfaces = reader.ReadInt32();
                    md3.NumSkins = reader.ReadInt32();
                    md3.FramesStart = reader.ReadInt32();
                    md3.TagsStart = reader.ReadInt32();
                    md3.SurfacesStart = reader.ReadInt32();
                    md3.Eof = reader.ReadInt32();

                    //load frames
                    md3.Frames = new List<Md3Frame>(md3.NumFrames);
                    reader.BaseStream.Seek(md3.FramesStart, SeekOrigin.Begin);
                    for (var i = 0; i < md3.NumFrames; i++)
                    {
                        var frame = new Md3Frame
                        {
                            Min = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                            Max = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                            Radius = reader.ReadSingle()
                        };

                        var name = new string(reader.ReadChars(FrameNameLength));
                        frame.Name = name.Substring(0, name.IndexOf('\0'));

                        md3.Frames.Add(frame);
                    }

                    //load tags
                    md3.Tags = new List<Md3Tag>(md3.NumTags * md3.NumFrames);
                    reader.BaseStream.Seek(md3.TagsStart, SeekOrigin.Begin);
                    for (var i = 0; i < md3.NumTags * md3.NumFrames; i++)
                    {
                        var name = new string(reader.ReadChars(TagNameLength));
                        var tag = new Md3Tag
                        {
                            Name = name.Substring(0, name.IndexOf('\0')),
                            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                            Rotation = new Matrix3
                            {
                                Row0 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                Row1 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                                Row2 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                            }
                        };

                        md3.Tags.Add(tag);
                    }

                    //load surfaces
                    md3.Surfaces = new List<Md3Surface>(md3.NumSurfaces);
                    reader.BaseStream.Seek(md3.SurfacesStart, SeekOrigin.Begin);
                    for (var i = 0; i < md3.NumSurfaces; i++)
                    {
                        var currentPosition = reader.BaseStream.Position;

                        var id = new string(reader.ReadChars(SurfaceIdLength));
                        var name = new string(reader.ReadChars(SurfaceNameLength));
                        var surface = new Md3Surface
                        {
                            Id = id,
                            Name = name.Substring(0, name.IndexOf('\0')),
                            Flags = reader.ReadInt32(),
                            NumFrames = reader.ReadInt32(),
                            NumShaders = reader.ReadInt32(),
                            NumVerts = reader.ReadInt32(),
                            NumTriangles = reader.ReadInt32(),
                            TrianglesStart = reader.ReadInt32(),
                            ShadersStart = reader.ReadInt32(),
                            TexturesStart = reader.ReadInt32(),
                            VerticesStart = reader.ReadInt32(),
                            SurfaceEnd = reader.ReadInt32()
                        };

                        reader.BaseStream.Seek(currentPosition + surface.ShadersStart, SeekOrigin.Begin);
                        surface.Shaders = new Md3Shader[surface.NumShaders];
                        for (var j = 0; j < surface.NumShaders; j++)
                        {
                            var shaderName = new string(reader.ReadChars(ShaderNameLength));
                            var shader = new Md3Shader
                            {
                                Name = shaderName.Substring(0, shaderName.IndexOf('\0')),
                                Index = reader.ReadInt32()
                            };

                            surface.Shaders[j] = shader;
                        }

                        reader.BaseStream.Seek(currentPosition + surface.TrianglesStart, SeekOrigin.Begin);
                        surface.Triangles = new Md3Triangle[surface.NumTriangles];
                        for (var j = 0; j < surface.NumTriangles; j++)
                        {
                            var triangle = new Md3Triangle
                            {
                                Index = new [] {reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()}
                            };

                            surface.Triangles[j] = triangle;
                        }

                        reader.BaseStream.Seek(currentPosition + surface.TexturesStart, SeekOrigin.Begin);
                        surface.TexCoords = new Vector2[surface.NumVerts];
                        for (var j = 0; j < surface.NumVerts; j++)
                        {
                            surface.TexCoords[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        }

                        reader.BaseStream.Seek(currentPosition + surface.VerticesStart, SeekOrigin.Begin);
                        surface.Vertices = new Md3Vertex[surface.NumVerts];
                        for (var j = 0; j < surface.NumVerts; j++)
                        {
                            const float scalar = 1.0f/64;
                            surface.Vertices[j] = new Md3Vertex
                            {
                                Position = -(new Vector3(reader.ReadInt16() * scalar, reader.ReadInt16() * scalar, reader.ReadInt16() * scalar))
                            };

                            var normal = reader.ReadInt16();
                            var lat = ((normal >> 8) & 255)*MathHelper.TwoPi/255;
                            var lng = (normal & 255)*MathHelper.TwoPi/255;

                            surface.Vertices[j].Normal = new Vector3(
                                (float)(Cos(lat) * Sin(lng)),
                                (float)(Sin(lat) * Sin(lng)),
                                (float)Cos(lng)
                                );
                        }

                        md3.Surfaces.Add(surface);
                    }
                }
            }

            return md3;
        }
    }

    public class Md3Tag
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Matrix3 Rotation { get; set; }
    }

    public class Md3Frame
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public string Name { get; set; }
    }

    public class Md3Surface
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Flags { get; set; }

        public int NumFrames { get; set; }
        public int NumShaders { get; set; }
        public int NumVerts { get; set; }
        public int NumTriangles { get; set; }

        public int TrianglesStart { get; set; }
        public int ShadersStart { get; set; }
        public int TexturesStart { get; set; }
        public int VerticesStart { get; set; }
        public int SurfaceEnd { get; set; }

        public Md3Shader[] Shaders { get; set; }
        public Md3Triangle[] Triangles { get; set; }
        public Vector2[] TexCoords { get; set; }
        public Md3Vertex[] Vertices { get; set; }
    }

    public class Md3Triangle
    {
        public int[] Index { get; set; }
    }

    public class Md3Shader
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class Md3Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
    }

    public static class Md3Extensions
    {
        public static IEnumerable<Mesh> ToMesh(this Md3 md3)
        {
            foreach (var surface in md3.Surfaces)
            {
                var mesh = new Mesh {Vertices = new Vertex[surface.NumVerts]};

                for (var i = 0; i < surface.NumVerts; i++)
                {
                    var vertex = new Vertex(surface.Vertices[i].Position, surface.Vertices[i].Normal, surface.TexCoords[i]);
                    mesh.Vertices[i] = vertex;
                }

                var indices = new List<int>();
                foreach (var triangle in surface.Triangles)
                {
                    indices.AddRange(new[] {triangle.Index[0], triangle.Index[1], triangle.Index[2]});
                }

                mesh.Indices = indices.ToArray();

                yield return mesh;
            }
        }
    }
}
