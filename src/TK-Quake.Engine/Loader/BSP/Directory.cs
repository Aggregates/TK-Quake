using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Loader.BSP
{
    public abstract class Directory
    {
        abstract public void ParseDirectoryEntry(FileStream file, int offset, int length);

        protected int size = 0;

        public int GetSize()
        {
            return(size);
        }
    }
}

