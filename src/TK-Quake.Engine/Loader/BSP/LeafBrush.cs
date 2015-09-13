using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class LeafBrush : Directory
    {
        public struct LeafBrushEntry
        {
            public int brush;
        }

        private const int LEAF_BRUSH_SIZE = 4;

        private LeafBrushEntry[] leafBrushes;

        private LeafBrush() { }
        public LeafBrush(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / LEAF_BRUSH_SIZE;

            // Create leafbrushes array.
            leafBrushes = new LeafBrushEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LEAF_BRUSH_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LEAF_BRUSH_SIZE);

                leafBrushes[i].brush = BitConverter.ToInt32(buf,  0 * sizeof(int));
            }
        }

        public LeafBrushEntry[] GetLeafBrushes()
        {
            return(leafBrushes);
        }

        public LeafBrushEntry GetLeafBrush(int leaf)
        {
            return(leafBrushes[leaf]);
        }
    }
}

