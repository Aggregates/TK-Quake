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

        private BSP.Directory[] directoryParsers = new BSP.Directory[NUM_DIRECTORIES];

        private string BSPFile = "";

        public BSPLoader()
        {
            directoryParsers[ 0] = new BSP.Entity();
            directoryParsers[ 1] = new BSP.Texture();
            directoryParsers[ 2] = new BSP.Plane();
            //directoryParsers[ 3] = new BSP.Node();
            //directoryParsers[ 4] = new BSP.Leaf();
            //directoryParsers[ 5] = new BSP.Leafface();
            //directoryParsers[ 6] = new BSP.Leafbrush();
            //directoryParsers[ 7] = new BSP.Model();
            //directoryParsers[ 8] = new BSP.Brush();
            //directoryParsers[ 9] = new BSP.Brushside();
            //directoryParsers[10] = new BSP.Vertex();
            //directoryParsers[11] = new BSP.Meshvert();
            //directoryParsers[12] = new BSP.Effect();
            //directoryParsers[13] = new BSP.Face();
            //directoryParsers[14] = new BSP.Lightmap();
            //directoryParsers[15] = new BSP.Lightvol();
            //directoryParsers[16] = new BSP.Visdata();
        }

        public BSPLoader(string file)
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
            return(((BSP.Entity)directoryParsers[0]).GetEntities());
        }

        public BSP.Texture.TextureEntry[] GetTextures()
        {
            return(((BSP.Texture)directoryParsers[0]).GetTextures());
        }

        public BSP.Texture.TextureEntry GetTexture(int texture)
        {
            return(((BSP.Texture)directoryParsers [0]).GetTexture (texture));
        }

        public BSP.Plane.PlaneEntry[] GetPlanes()
        {
            return(((BSP.Plane)directoryParsers[0]).GetPlanes());
        }

        public BSP.Plane.PlaneEntry GetPlane(int plane)
        {
            return(((BSP.Plane)directoryParsers[0]).GetPlane(plane));
        }
    }
}
