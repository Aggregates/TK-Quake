using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public class Effect : Directory
    {
        public struct EffectEntry
        {
            public string name;
            public int    brush;
            public int    unknown;
        }

        private const int EFFECT_SIZE = 72;
        private const int NAME_LENGTH = 64;

        private EffectEntry[] effects;

        private Effect() { }
        public Effect(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / EFFECT_SIZE;

            // Create effects array.
            effects = new EffectEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[EFFECT_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, EFFECT_SIZE);

                effects[i].name    = System.Text.Encoding.UTF8.GetString(buf, 0, NAME_LENGTH);
                effects[i].brush   = BitConverter.ToInt32(buf, NAME_LENGTH);
                effects[i].unknown = BitConverter.ToInt32(buf, NAME_LENGTH + 4);
            }
        }

        public EffectEntry[] GetEffects()
        {
            return(effects);
        }

        public EffectEntry GetEffect(int effect)
        {
            return(effects[effect]);
        }
    }
}

