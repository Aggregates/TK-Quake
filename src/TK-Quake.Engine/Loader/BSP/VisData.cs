using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class VisData : Directory
    {
        public struct VisDataEntry
        {
            public int    n_vecs;
            public int    sz_vecs;
            public byte[] vecs;
        }

        private const int VIS_DATA_SIZE = 8;

        private VisDataEntry visData;

        private VisData() { }
        public VisData(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[VIS_DATA_SIZE];

            // Read in the size of the visData block.
            file.Read (buf, 0, VIS_DATA_SIZE);

            visData.n_vecs  = BitConverter.ToInt32(buf, 0 * sizeof(int));
            visData.sz_vecs = BitConverter.ToInt32(buf, 1 * sizeof(int));

            // Read in the actual visData information.
            visData.vecs = new byte[visData.n_vecs * visData.sz_vecs];
            file.Read (visData.vecs, 0, (visData.n_vecs * visData.sz_vecs));
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public VisDataEntry GetVisData()
        {
            return(visData);
        }
    }
}

