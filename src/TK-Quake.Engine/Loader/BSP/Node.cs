using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Node : Directory
    {
        public struct NodeEntry
        {
            public int   plane;
            public int[] children;
            public int[] mins;
            public int[] maxs;
        }

        private const int NODE_SIZE = 36;

        private NodeEntry[] nodes;

        public Node() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / NODE_SIZE;

            // Create nodes array.
            nodes = new NodeEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[NODE_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, NODE_SIZE);

                nodes[i].children    = new int[2];
                nodes[i].mins        = new int[3];
                nodes[i].maxs        = new int[3];
                nodes[i].plane       = BitConverter.ToInt32(buf, 0 * sizeof(int));
                nodes[i].children[0] = BitConverter.ToInt32(buf, 1 * sizeof(int));
                nodes[i].children[1] = BitConverter.ToInt32(buf, 2 * sizeof(int));
                nodes[i].mins[0]     = BitConverter.ToInt32(buf, 3 * sizeof(int));
                nodes[i].mins[1]     = BitConverter.ToInt32(buf, 4 * sizeof(int));
                nodes[i].mins[2]     = BitConverter.ToInt32(buf, 5 * sizeof(int));
                nodes[i].maxs[0]     = BitConverter.ToInt32(buf, 6 * sizeof(int));
                nodes[i].maxs[1]     = BitConverter.ToInt32(buf, 7 * sizeof(int));
                nodes[i].maxs[2]     = BitConverter.ToInt32(buf, 8 * sizeof(int));
            }
        }

        public NodeEntry[] GetNodes()
        {
            return(nodes);
        }

        public NodeEntry GetNode(int node)
        {
            return(nodes[node]);
        }
    }
}

