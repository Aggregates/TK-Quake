using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public class Vertex : Directory
    {
        public struct VertexEntry
        {
            public Vector3   position;
            public Vector2[] texCoord;
            public Vector3   normal;
            public Vector4   colour;
        }

        private const int VERTEX_SIZE = 44;

        private VertexEntry[] vertexes;

        private Vertex() { }
        public Vertex(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / VERTEX_SIZE;

            // Create vertexes array.
            vertexes = new VertexEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[VERTEX_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, VERTEX_SIZE);

                vertexes[i].texCoord    = new Vector2[2];
                vertexes[i].position    = new Vector3(BitConverter.ToSingle(buf,  0 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  1 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  2 * sizeof(float)));
                vertexes[i].texCoord[0] = new Vector2(BitConverter.ToSingle(buf,  3 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  4 * sizeof(float)));
                vertexes[i].texCoord[1] = new Vector2(BitConverter.ToSingle(buf,  5 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  6 * sizeof(float)));
                vertexes[i].normal      = new Vector3(BitConverter.ToSingle(buf,  7 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  8 * sizeof(float)),
                                                      BitConverter.ToSingle(buf,  9 * sizeof(float)));

                // Normalise colour.
                vertexes [i].colour     = new Vector4(buf [10] / 255.0f, buf [11] / 255.0f, buf [12] / 255.0f, buf [13] / 255.0f);

                // Change coordinate system to match OpenGLs.
                if (swizzle == true)
                {
                    Swizzle (ref vertexes [i].position);
                    Swizzle (ref vertexes [i].normal);
                }
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public VertexEntry[] GetVertexes()
        {
            return(vertexes);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public VertexEntry GetVertex(int vertex)
        {
            return(vertexes[vertex]);
        }
    }
}

