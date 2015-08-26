using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    class VisData : Directory
    {
        public struct VisDataEntry
        {
            public int    n_vecs;
            public int    sz_vecs;
            public byte[] vecs;
        }

        private const int VIS_DATA_SIZE = 8;

        private VisDataEntry[] visDatas;

        public VisData() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Create textures array.
            visDatas = new VisDataEntry[length / VIS_DATA_SIZE];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[VIS_DATA_SIZE];

            for (int i = 0; i < (length / VIS_DATA_SIZE); i++)
            {
                file.Read (buf, 0, VIS_DATA_SIZE);

                visDatas[i].n_vecs  = BitConverter.ToInt32(buf, 0 * sizeof(int));
                visDatas[i].sz_vecs = BitConverter.ToInt32(buf, 1 * sizeof(int));

                visDatas[i].vecs = new byte[visDatas[i].n_vecs * visDatas[i].sz_vecs];
                file.Read (visDatas[i].vecs, 0, (visDatas[i].n_vecs * visDatas[i].sz_vecs));
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

