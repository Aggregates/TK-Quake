using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    class Leaf : Directory
    {
        public struct LeafEntry
        {
            public int   cluster;
            public int   area;
            public int[] mins;
            public int[] maxs;
            public int   leaffaces;
            public int   n_leaffaces;
            public int   leafbrushes;
            public int   n_leafbrushes;
        }

        private const int LEAF_SIZE = 48;

        private LeafEntry[] leafs;

        public Leaf() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Create textures array.
            leafs = new LeafEntry[length / LEAF_SIZE];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LEAF_SIZE];

            for (int i = 0; i < (length / LEAF_SIZE); i++)
            {
                file.Read (buf, 0, LEAF_SIZE);

                leafs[i].mins          = new int[3];
                leafs[i].maxs          = new int[3];
                leafs[i].cluster       = BitConverter.ToInt32(buf,  0 * sizeof(int));
                leafs[i].area          = BitConverter.ToInt32(buf,  1 * sizeof(int));
                leafs[i].mins[0]       = BitConverter.ToInt32(buf,  2 * sizeof(int));
                leafs[i].mins[1]       = BitConverter.ToInt32(buf,  3 * sizeof(int));
                leafs[i].mins[2]       = BitConverter.ToInt32(buf,  4 * sizeof(int));
                leafs[i].maxs[0]       = BitConverter.ToInt32(buf,  5 * sizeof(int));
                leafs[i].maxs[1]       = BitConverter.ToInt32(buf,  6 * sizeof(int));
                leafs[i].maxs[2]       = BitConverter.ToInt32(buf,  7 * sizeof(int));
                leafs[i].leaffaces     = BitConverter.ToInt32(buf,  8 * sizeof(int));
                leafs[i].n_leaffaces   = BitConverter.ToInt32(buf,  9 * sizeof(int));
                leafs[i].leafbrushes   = BitConverter.ToInt32(buf, 10 * sizeof(int));
                leafs[i].n_leafbrushes = BitConverter.ToInt32(buf, 11 * sizeof(int));
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
