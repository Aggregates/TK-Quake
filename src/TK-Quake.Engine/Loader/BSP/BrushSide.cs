using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    class BrushSide : Directory
    {
        public struct BrushSideEntry
        {
            public int plane;
            public int texture;
        }

        private const int BRUSHSIDE_SIZE = 8;

        private BrushSideEntry[] brushSides;

        public BrushSide() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Create textures array.
            brushSides = new BrushSideEntry[length / BRUSHSIDE_SIZE];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[BRUSHSIDE_SIZE];

            for (int i = 0; i < (length / BRUSHSIDE_SIZE); i++)
            {
                file.Read (buf, 0, BRUSHSIDE_SIZE);

                brushSides[i].plane   = BitConverter.ToInt32(buf,  0 * sizeof(int));
                brushSides[i].texture = BitConverter.ToInt32(buf,  2 * sizeof(int));
            }
        }

        public BrushSideEntry[] GetBrushSidees()
        {
            return(brushSides);
        }

        public BrushSideEntry GetBrushSide(int brushSide)
        {
            return(brushSides[brushSide]);
        }
    }
}

