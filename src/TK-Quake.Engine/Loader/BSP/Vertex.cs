using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    class Vertex : Directory
    {
        public struct VertexEntry
        {
            public float[]  position;
            public float[,] texcoord;
            public float[]  normal;
            public byte[]   colour;
        }

        private const int VERTEX_SIZE = 12;

        private VertexEntry[] vertexes;

        public Vertex() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Create textures array.
            vertexes = new VertexEntry[length / VERTEX_SIZE];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[VERTEX_SIZE];

            for (int i = 0; i < (length / VERTEX_SIZE); i++)
            {
                file.Read (buf, 0, VERTEX_SIZE);

                vertexes[i].position = new float[3];
                vertexes[i].texcoord = new float[2, 2];
                vertexes[i].normal   = new float[3];
                vertexes[i].colour   = new byte[4];
                vertexes[i].position[0]    = BitConverter.ToSingle(buf,  0 * sizeof(float));
                vertexes[i].position[1]    = BitConverter.ToSingle(buf,  1 * sizeof(float));
                vertexes[i].position[2]    = BitConverter.ToSingle(buf,  2 * sizeof(float));
                vertexes[i].texcoord[0, 0] = BitConverter.ToSingle(buf,  3 * sizeof(float));
                vertexes[i].texcoord[0, 1] = BitConverter.ToSingle(buf,  4 * sizeof(float));
                vertexes[i].texcoord[1, 0] = BitConverter.ToSingle(buf,  5 * sizeof(float));
                vertexes[i].texcoord[1, 1] = BitConverter.ToSingle(buf,  6 * sizeof(float));
                vertexes[i].normal[0]      = BitConverter.ToSingle(buf,  7 * sizeof(float));
                vertexes[i].normal[1]      = BitConverter.ToSingle(buf,  8 * sizeof(float));
                vertexes[i].normal[2]      = BitConverter.ToSingle(buf,  9 * sizeof(float));
                Array.Copy(buf, 10 * sizeof(float), vertexes[i].colour, 0, 4);
            }
        }

        public VertexEntry[] GetVertexes()
        {
            return(vertexes);
        }

        public VertexEntry GetVertex(int vertex)
        {
            return(vertexes[vertex]);
        }
    }
}

