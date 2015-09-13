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

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / BRUSH_SIDE_SIZE;

            // Create brushsides array.
            brushSides = new BrushSideEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[BRUSH_SIDE_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, BRUSH_SIDE_SIZE);

                brushSides[i].plane   = BitConverter.ToInt32(buf,  0 * sizeof(int));
                brushSides[i].texture = BitConverter.ToInt32(buf,  1 * sizeof(int));
            }
        }

        public BrushSideEntry[] GetBrushSides()
        {
            return(brushSides);
        }

        public BrushSideEntry GetBrushSide(int brushSide)
        {
            return(brushSides[brushSide]);
        }
    }
}

