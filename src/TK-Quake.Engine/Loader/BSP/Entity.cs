using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Entity : Directory
    {
        public struct EntityEntry
        {
            public string entity;
        }

        private EntityEntry[] entities;

        private Entity() { }
        public Entity(bool swizzle) { this.swizzle = swizzle; }

        /// <summary>
        /// Parses the directory entry.
        /// </summary>
        /// <param name="file">The file to read the directory entry from.</param>
        /// <param name="offset">The offset within the file that the directory entry starts at.</param>
        /// <param name="offset">The length of the directory entry.</param>
        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[length];

            // Read in the data.
            file.Read (buf, 0, length);

            // Convert entity data to a string.
            string entityLump = System.Text.Encoding.UTF8.GetString(buf);

            // Remove non-printable characters from the entities.
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^\\n -~]");
            entityLump = rgx.Replace(entityLump, "");

            // Split into an array.
            Regex re = new Regex(@"{(.*\s*)*?}");
            MatchCollection mc = re.Matches(entityLump);

            // Calculate the number of elements in this directory entry.
            size = mc.Count;

            entities = new EntityEntry[size];

            for (int i = 0; i < size; i++)
            {
                entities [i].entity = mc [i].Groups [0].Value;
            }
        }

        /// <summary>
        /// Return the entities.
        /// </summary>
        public EntityEntry[] GetEntities()
        {
            return(entities);
        }

        /// <summary>
        /// Return a particular entity.
        /// </summary>
        /// <param name="entity">The index of the entity to retrieve.</param>
        public EntityEntry GetEntity(int entity)
        {
            return(entities[entity]);
        }
    }
}

