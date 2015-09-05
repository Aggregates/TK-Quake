using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public class Model : Directory
    {
        public struct ModelEntry
        {
            public Vector3 mins;
            public Vector3 maxs;
            public int     face;
            public int     n_faces;
            public int     brush;
            public int     n_brushes;
        }

        private const int MODEL_SIZE = 40;

        private ModelEntry[] models;

        public Model() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / MODEL_SIZE;

            // Create models array.
            models = new ModelEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[MODEL_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, MODEL_SIZE);

                models[i].mins      = new Vector3(BitConverter.ToSingle(buf, 0 * sizeof(float)),
                                                  BitConverter.ToSingle(buf, 1 * sizeof(float)),
                                                  BitConverter.ToSingle(buf, 2 * sizeof(float)));
                models[i].maxs      = new Vector3(BitConverter.ToSingle(buf, 3 * sizeof(float)),
                                                  BitConverter.ToSingle(buf, 4 * sizeof(float)),
                                                  BitConverter.ToSingle(buf, 5 * sizeof(float)));
                models[i].face      = BitConverter.ToInt32(buf,  6 * sizeof(int));
                models[i].n_faces   = BitConverter.ToInt32(buf,  7 * sizeof(int));
                models[i].brush     = BitConverter.ToInt32(buf,  8 * sizeof(int));
                models[i].n_brushes = BitConverter.ToInt32(buf,  9 * sizeof(int));
            }
        }

        public ModelEntry[] GetModels()
        {
            return(models);
        }

        public ModelEntry GetModel(int model)
        {
            return(models[model]);
        }
    }
}

