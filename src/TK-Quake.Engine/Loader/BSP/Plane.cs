using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public class Plane : Directory
    {
        public struct PlaneEntry
        {
            public Vector4 plane;
        }

        private const int PLANE_SIZE = 16;

        private PlaneEntry[] planes;

        private Plane() { }
        public Plane(bool swizzle) { this.swizzle = swizzle; }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / PLANE_SIZE;

            // Create planes array.
            planes = new PlaneEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[PLANE_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, PLANE_SIZE);

                planes [i].plane = new Vector4(
                    BitConverter.ToSingle(buf, 0 * sizeof(float)),
                    BitConverter.ToSingle(buf, 1 * sizeof(float)),
                    BitConverter.ToSingle(buf, 2 * sizeof(float)),
                    BitConverter.ToSingle(buf, 3 * sizeof(float)));

                if (swizzle == true)
                {
                    Swizzle (ref planes [i].plane);
                }
            }
        }

        public PlaneEntry[] GetPlanes()
        {
            return(planes);
        }

        public PlaneEntry GetPlane(int plane)
        {
            return(planes[plane]);
        }
    }
}

