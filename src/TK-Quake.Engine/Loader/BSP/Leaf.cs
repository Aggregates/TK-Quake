using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public class Leaf : Directory
    {
        public struct LeafEntry
        {
            public int     cluster;
            public int     area;
            public Vector3 mins;
            public Vector3 maxs;
            public int     leafFace;
            public int     n_leafFaces;
            public int     leafBrush;
            public int     n_leafBrushes;
        }

        private const int LEAF_SIZE = 48;

        private LeafEntry[] leafs;

        private Leaf() { }
        public Leaf(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / LEAF_SIZE;

            // Create leafs array.
            leafs = new LeafEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LEAF_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LEAF_SIZE);

                leafs[i].cluster       = BitConverter.ToInt32(buf,  0 * sizeof(int));
                leafs[i].area          = BitConverter.ToInt32(buf,  1 * sizeof(int));
                leafs[i].mins          = new Vector3(BitConverter.ToInt32(buf,  2 * sizeof(int)), 
                                                     BitConverter.ToInt32(buf,  3 * sizeof(int)), 
                                                     BitConverter.ToInt32(buf,  4 * sizeof(int)));
                leafs[i].maxs          = new Vector3(BitConverter.ToInt32(buf,  5 * sizeof(int)), 
                                                     BitConverter.ToInt32(buf,  6 * sizeof(int)), 
                                                     BitConverter.ToInt32(buf,  7 * sizeof(int)));

                if (swizzle == true)
                {
                    Swizzle (ref leafs [i].maxs);
                    Swizzle (ref leafs [i].mins);
                }

                leafs[i].leafFace      = BitConverter.ToInt32(buf,  8 * sizeof(int));
                leafs[i].n_leafFaces   = BitConverter.ToInt32(buf,  9 * sizeof(int));
                leafs[i].leafBrush     = BitConverter.ToInt32(buf, 10 * sizeof(int));
                leafs[i].n_leafBrushes = BitConverter.ToInt32(buf, 11 * sizeof(int));
            }
        }

        public LeafEntry[] GetLeafs()
        {
            return(leafs);
        }

        public LeafEntry GetLeaf(int leaf)
        {
            return(leafs[leaf]);
        }
    }
}

