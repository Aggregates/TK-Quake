using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class LightMap : Directory
    {
        public struct LightMapEntry
        {
            public byte[,,] map;
        }

        private const int LIGHT_MAP_SIZE = 49152;
        private const int X_RESOLUTION   = 128;
        private const int Y_RESOLUTION   = 128;
        private const int RGB_RESOLUTION = 3;

        private LightMapEntry[] lightMaps;

        private LightMap() { }
        public LightMap(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / LIGHT_MAP_SIZE;

            // Create lightmaps array.
            lightMaps = new LightMapEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LIGHT_MAP_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LIGHT_MAP_SIZE);

                lightMaps[i].map = new byte[X_RESOLUTION, Y_RESOLUTION, 3];

                for (int x = 0; x < X_RESOLUTION; x++)
                {
                    for (int y = 0; y < Y_RESOLUTION; y++)
                    {
                        lightMaps[i].map[x, y, 0] = buf[(x * Y_RESOLUTION * RGB_RESOLUTION) + (y * RGB_RESOLUTION) + 0];
                        lightMaps[i].map[x, y, 1] = buf[(x * Y_RESOLUTION * RGB_RESOLUTION) + (y * RGB_RESOLUTION) + 1];
                        lightMaps[i].map[x, y, 2] = buf[(x * Y_RESOLUTION * RGB_RESOLUTION) + (y * RGB_RESOLUTION) + 2];
                    }
                }
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public LightMapEntry[] GetLightMaps()
        {
            return(lightMaps);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public LightMapEntry GetLightMap(int lightMap)
        {
            return(lightMaps[lightMap]);
        }
    }
}

