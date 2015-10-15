using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Brush : Directory
    {
        // The structure of a brush entry.
        public struct BrushEntry
        {
            public int brushSide;
            public int n_brushSides;
            public int texture;
        }

        // The length of a brush entry.
        private const int BRUSH_SIZE = 12;

        private BrushEntry[] brushes;

        private Brush() { }
        public Brush(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / BRUSH_SIZE;

            // Create brushes array.
            brushes = new BrushEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[BRUSH_SIZE];

            // Read in each element in the directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, BRUSH_SIZE);

                brushes[i].brushSide    = BitConverter.ToInt32(buf,  0 * sizeof(int));
                brushes[i].n_brushSides = BitConverter.ToInt32(buf,  1 * sizeof(int));
                brushes[i].texture      = BitConverter.ToInt32(buf,  2 * sizeof(int));
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public BrushEntry[] GetBrushes()
        {
            return(brushes);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public BrushEntry GetBrush(int brush)
        {
            return(brushes[brush]);
        }
    }
}

