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

        protected Vector3 Swizzle(Vector3 vector)
        {
            return(new Vector3(vector[0], vector[2], -vector[1]));
        }

        protected Vector4 Swizzle(Vector4 vector)
        {
            return(new Vector4(vector[0], vector[2], -vector[1], vector[3]));
        }

        protected float[] Swizzle(float[] vector)
        {
            return(new float[3] {vector[0], vector[2], -vector[1]});
        }

        protected int[] Swizzle(int[] vector)
        {
            return(new int[3] {vector[0], vector[2], -vector[1]});
        }

        protected void Swizzle(ref Vector3 vector)
        {   
            vector = Swizzle(vector);
        }

        protected void Swizzle(ref Vector4 vector)
        {
            vector = Swizzle(vector);
        }

        protected void Swizzle(ref int[] vector)
        {   
            vector = Swizzle(vector);
        }

        protected void Swizzle(ref float[] vector)
        {   
            vector = Swizzle(vector);
        }
    }
}

