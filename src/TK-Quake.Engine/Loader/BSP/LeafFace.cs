using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class LeafFace : Directory
    {
        public struct LeafFaceEntry
        {
            public int face;
        }

        private const int LEAF_FACE_SIZE = 4;

        private LeafFaceEntry[] leafFaces;

        public LeafFace() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / LEAF_FACE_SIZE;

            // Create leaffaces array.
            leafFaces = new LeafFaceEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LEAF_FACE_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LEAF_FACE_SIZE);

                leafFaces[i].face = BitConverter.ToInt32(buf,  0 * sizeof(int));
            }
        }

        public LeafFaceEntry[] GetLeafFaces()
        {
            return(leafFaces);
        }

        public LeafFaceEntry GetLeafFace(int leaf)
        {
            return(leafFaces[leaf]);
        }
    }
}

