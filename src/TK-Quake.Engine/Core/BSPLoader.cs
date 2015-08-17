using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Core
{
    class BSPLoader
    {
        private const string MAGIC_STRING    = "IBSP";
        private const int    BSP_VERSION     = 0x2E;
        private const int    NUM_DIRECTORIES = 17;

        private struct Directory
        {
            public int offset;
            public int length;
        }

        private string BSPFile = "";

        public BSPLoader() { }

        public BSPLoader(string BSPFile)
        {
            BSPFile = file;
        }

        public void setBSPFile(string file)
        {
            BSPFile = file;
        }

        public string getBSPFile()
        {
            return(BSPFile);
        }

        public bool loadFile()
        {
            try
            {
                BinaryReader file = new BinaryReader(File.Open(BSPFile, FileMode.Open));

                // Load the header.
                string magic = System.Text.Encoding.UTF8.GetString(file.ReadBytes(4));
                int version = file.ReadInt32();

                // Verify header.
                if (magic.CompareTo(MAGIC_STRING) != 0)
                {
                    return (false);
                }

                if (version != BSP_VERSION)
                {
                    return (false);
                }

                // Read in the directory information.
                Directory[] directories = new Directory[17];

                for (int i = 0; i < NUM_DIRECTORIES; i++)
                {
                    directories[i].offset = file.ReadInt32();
                    directories[i].length = file.ReadInt32();
                }

                // Parse each directory.
                for (int i = 0; i < NUM_DIRECTORIES; i++)
                {
                }
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("File '" + BSPFile + "' does not exist.");
                return (false);
            }

            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                return (false);
            }
        }
    }
}
