using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Core.BSP
{
    class Entity : Directory
    {
        private string entities = "";

        public Entity() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[length];

            // Read in the data.
            file.Read (buf, 0, length);

            // Convert entity data to a string.
            entities = System.Text.Encoding.UTF8.GetString(buf);
        }

        public string GetEntities()
        {
            return(entities);
        }
    }
}

