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

        private VisDataEntry[] visDatas;

        private VisData() { }
        public VisData(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[VIS_DATA_SIZE];

            // Keep track of the number of bytes read so far.
            int bytesRead = 0;

            for (size = 0; bytesRead < length; size++)
            {
                // Resize array.
                Array.Resize<VisDataEntry> (ref visDatas, size + 1);

                file.Read (buf, 0, VIS_DATA_SIZE);

                visDatas[size].n_vecs  = BitConverter.ToInt32(buf, 0 * sizeof(int));
                visDatas[size].sz_vecs = BitConverter.ToInt32(buf, 1 * sizeof(int));

                visDatas[size].vecs = new byte[visDatas[size].n_vecs * visDatas[size].sz_vecs];
                file.Read (visDatas[size].vecs, 0, (visDatas[size].n_vecs * visDatas[size].sz_vecs));

                bytesRead += VIS_DATA_SIZE + (visDatas[size].n_vecs * visDatas[size].sz_vecs);
            }
        }

        public VisDataEntry[] GetVisDatas()
        {
            return(visDatas);
        }

        public VisDataEntry GetVisData(int visData)
        {
            return(visDatas[visData]);
        }
    }
}

