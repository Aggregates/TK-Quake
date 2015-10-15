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

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / LEAF_BRUSH_SIZE;

            // Create leafbrushes array.
            leafBrushes = new LeafBrushEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LEAF_BRUSH_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LEAF_BRUSH_SIZE);

                leafBrushes[i].brush = BitConverter.ToInt32(buf,  0 * sizeof(int));
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public LeafBrushEntry[] GetLeafBrushes()
        {
            return(leafBrushes);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public LeafBrushEntry GetLeafBrush(int leaf)
        {
            return(leafBrushes[leaf]);
        }
    }
}

