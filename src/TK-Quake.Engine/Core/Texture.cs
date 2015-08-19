using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Core
{
    class Texture : Directory
    {
        public struct Tex
        {
            string name;
            int    flags;       // Surface flags.
            int    contents;    // Content flags.
        }

        private const int TEXTURE_SIZE = 72;
        private const int NAME_LENGTH  = 64;

        private Tex[] textures;

        public Texture() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Create textures array.
            textures = new Tex[length / TEXTURE_SIZE];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[TEXTURE_SIZE];

            for (int i = 0; i < (length / TEXTURE_SIZE); i++)
            {
                file.Read (buf, 0, TEXTURE_SIZE);

                textures[i].name     = System.Text.Encoding.UTF8.GetString(buf, 0, NAME_LENGTH);
                textures[i].flags    = BitConverter.ToInt32(buf, NAME_LENGTH);
                textures[i].contents = BitConverter.ToInt32(buf, NAME_LENGTH + 4);
            }
        }

        public Tex[] GetTextures()
        {
            return(textures);
        }

        public Tex GetTexture(int texture)
        {
            return(textures[i]);
        }
    }
}

