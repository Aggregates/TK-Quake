using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class LightVol : Directory
    {
        public struct LightVolEntry
        {
            public byte[] ambient;
            public byte[] directional;
            public byte[] dir;
        }

        private const int LIGHT_VOL_SIZE = 8;

        private LightVolEntry[] lightVols;

        private LightVol() { }
        public LightVol(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / LIGHT_VOL_SIZE;

            // Create lightvols array.
            lightVols = new LightVolEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[LIGHT_VOL_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, LIGHT_VOL_SIZE);

                lightVols[i].ambient        = new byte[3];
                lightVols[i].directional    = new byte[3];
                lightVols[i].dir            = new byte[2];

                lightVols[i].ambient[0]     = buf[0];
                lightVols[i].ambient[1]     = buf[1];
                lightVols[i].ambient[2]     = buf[2];
                lightVols[i].directional[0] = buf[3];
                lightVols[i].directional[1] = buf[4];
                lightVols[i].directional[2] = buf[5];
                lightVols[i].dir[0]         = buf[6];
                lightVols[i].dir[1]         = buf[7];
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public LightVolEntry[] GetLightVols()
        {
            return(lightVols);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public LightVolEntry GetLightVol(int lightVol)
        {
            return(lightVols[lightVol]);
        }
    }
}

