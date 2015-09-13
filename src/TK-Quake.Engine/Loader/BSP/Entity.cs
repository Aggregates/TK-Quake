using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Entity : Directory
    {
        public struct EntityEntry
        {
            public string entities;
        }

        private EntityEntry entities;

        private Entity() { }
        public Entity(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length;

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[length];

            // Read in the data.
            file.Read (buf, 0, length);

            // Convert entity data to a string.
            entities.entities = System.Text.Encoding.UTF8.GetString(buf);

            // Remove non-printable characters from the entities.
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^\\n -~]");
            entities.entities = rgx.Replace(entities.entities, "");
        }

        public EntityEntry GetEntities()
        {
            return(entities);
        }
    }
}

