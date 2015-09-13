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
        public struct BrushEntry
        {
            public int brushSide;
            public int n_brushSides;
            public int texture;
        }

        private const int BRUSH_SIZE = 12;

        private BrushEntry[] brushes;

        private Brush() { }
        public Brush(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / BRUSH_SIZE;

            // Create brushes array.
            brushes = new BrushEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[BRUSH_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, BRUSH_SIZE);

                brushes[i].brushSide    = BitConverter.ToInt32(buf,  0 * sizeof(int));
                brushes[i].n_brushSides = BitConverter.ToInt32(buf,  1 * sizeof(int));
                brushes[i].texture      = BitConverter.ToInt32(buf,  2 * sizeof(int));
            }
        }

        public BrushEntry[] GetBrushes()
        {
            return(brushes);
        }

        public BrushEntry GetBrush(int brush)
        {
            return(brushes[brush]);
        }
    }
}

