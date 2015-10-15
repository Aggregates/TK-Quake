using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Texture : Directory
    {
        public struct TextureEntry
        {
            public string name;
            public int    flags;       // Surface flags.
            public int    contents;    // Content flags.
        }

        private const int TEXTURE_SIZE = 72;
        private const int NAME_LENGTH  = 64;

        private TextureEntry[] textures;

        private Texture() { }
        public Texture(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Calculate the number of elements in this directory entry.
            size = length / TEXTURE_SIZE;

            // Create textures array.
            textures = new TextureEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[TEXTURE_SIZE];

            // Read in each element of this directory entry.
            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, TEXTURE_SIZE);

                textures[i].name     = System.Text.Encoding.UTF8.GetString(buf, 0, NAME_LENGTH);
                textures[i].flags    = BitConverter.ToInt32(buf, NAME_LENGTH);
                textures[i].contents = BitConverter.ToInt32(buf, NAME_LENGTH + 4);

                // Remove non-printable characters from the textures name.
                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^ -~]");
                textures[i].name = rgx.Replace(textures[i].name, "");
            }
        }

        /// <summary>
        /// Return the array of directory entries.
        /// </summary>
        public TextureEntry[] GetTextures()
        {
            return(textures);
        }

        /// <summary>
        /// Return a particular directory entry.
        /// </summary>
        /// <param name="brush">The index of the entry to retrieve.</param>
        public TextureEntry GetTexture(int texture)
        {
            return(textures[texture]);
        }
    }
}

