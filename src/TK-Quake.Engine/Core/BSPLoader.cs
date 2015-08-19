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

        private Directory[] directoryParsers = new Directory[NUM_DIRECTORIES];

        private string BSPFile = "";

        public BSPLoader()
        {
            directoryParsers[ 0] = new Entity();
            directoryParsers[ 1] = new Texture();
            //directoryParsers[ 2] = new Plane();
            //directoryParsers[ 3] = new Node();
            //directoryParsers[ 4] = new Leaf();
            //directoryParsers[ 5] = new Leafface();
            //directoryParsers[ 6] = new Leafbrush();
            //directoryParsers[ 7] = new Model();
            //directoryParsers[ 8] = new Brush();
            //directoryParsers[ 9] = new Brushside();
            //directoryParsers[10] = new Vertex();
            ////directoryParsers[11] = new Meshvert();
            //directoryParsers[12] = new Effect();
            //directoryParsers[13] = new Face();
            //directoryParsers[14] = new Lightmap();
            //directoryParsers[15] = new Lightvol();
            //directoryParsers[16] = new Visdata();
        }

        public BSPLoader(string file)
        {
            BSPFile = file;
        }

        public void SetBSPFile(string file)
        {
            BSPFile = file;
        }

        public string GetBSPFile()
        {
            return (BSPFile);
        }

        public bool LoadFile()
        {
            try
            {
                FileStream file = File.Open(BSPFile, FileMode.Open);

                // Extract the magic bytes.
				byte[] buf = new byte[4];
				file.Read (buf, 0, 4);
                string magic = System.Text.Encoding.UTF8.GetString(buf);

                // Verify magic bytes.
                if (magic.CompareTo(MAGIC_STRING) != 0)
                {
                    return (false);
                }

				// Extract version number.
  				file.Read (buf, 0, 4);
				int version = BitConverter.ToInt32 (buf, 0);

                // Verify version number.
				if (version != BSP_VERSION)
                {
                    return (false);
                }

                // Read in the directory information.
                for (int i = 0; i < NUM_DIRECTORIES; i++)
                {
                    int offset, length;

                    file.Read (buf, 0, 4);
                    offset = BitConverter.ToInt32 (buf, 0);

                    file.Read (buf, 0, 4);
                    length = BitConverter.ToInt32 (buf, 0);

                    // Save the current position in the file.
                    long pos = file.Position;

                    directoryParsers[i].ParseDirectoryEntry(file, offset, length);

                    // Restore original position in the file.
                    file.Seek (pos, SeekOrigin.Begin);
                }

                file.Close ();
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

			return (true);
        }

        public string GetEntities()
        {
            return(((Entity)directoryParsers[0]).GetEntities());
        }
    }
}
