using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public abstract class Directory
    {
        abstract public void ParseDirectoryEntry(FileStream file, int offset, int length);

        protected int size = 0;
        protected bool swizzle = false;

        public int GetSize()
        {
            return(size);
        }

        protected void Swizzle(ref Vector3 vector)
        {   
            float temp;

            temp      = vector[1];
            vector[1] = vector[2];
            vector[2] = -temp;
        }

        protected void Swizzle(ref int[] vector)
        {   
            int temp;

            temp      = vector[1];
            vector[1] = vector[2];
            vector[2] = -temp;
        }

        protected void Swizzle(ref float[] vector)
        {   
            float temp;

            temp      = vector[1];
            vector[1] = vector[2];
            vector[2] = -temp;
        }
    }
}

