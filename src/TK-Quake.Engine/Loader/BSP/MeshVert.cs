using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class MeshVert : Directory
    {
        public struct MeshVertEntry
        {
            public int offset;
        }

        private const int MESH_VERT_SIZE = 4;

        private MeshVertEntry[] meshVerts;

        public MeshVert() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / MESH_VERT_SIZE;

            // Create meshverts array.
            meshVerts = new MeshVertEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[MESH_VERT_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, MESH_VERT_SIZE);

                meshVerts[i].offset = BitConverter.ToInt32(buf,  0 * sizeof(int));
            }
        }

        public MeshVertEntry[] GetMeshVerts()
        {
            return(meshVerts);
        }

        public MeshVertEntry GetMeshVert(int meshVert)
        {
            return(meshVerts[meshVert]);
        }
    }
}

