using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class BrushSide : Directory
    {
        public struct BrushSideEntry
        {
            public int plane;
            public int texture;
        }

        private const int BRUSH_SIDE_SIZE = 8;

        private BrushSideEntry[] brushSides;

        private BrushSide() { }
        public BrushSide(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / BRUSH_SIDE_SIZE;

            // Create brushsides array.
            brushSides = new BrushSideEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[BRUSH_SIDE_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, BRUSH_SIDE_SIZE);

                brushSides[i].plane   = BitConverter.ToInt32(buf,  0 * sizeof(int));
                brushSides[i].texture = BitConverter.ToInt32(buf,  1 * sizeof(int));
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public BrushSideEntry[] GetBrushSides()
        {
            return(brushSides);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brushSide">The index of the entry to retrieve.</param>
        public BrushSideEntry GetBrushSide(int brushSide)
        {
            return(brushSides[brushSide]);
        }
    }
}

