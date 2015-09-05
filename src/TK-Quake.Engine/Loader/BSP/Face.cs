using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Loader.BSP
{
    public class Face : Directory
    {
        public struct FaceEntry
        {
            public int       texture;
            public int       effect;
            public int       type;
            public int       vertex;
            public int       n_vertexes;
            public int       meshVert;
            public int       n_meshVerts;
            public int       lm_index;
            public int[]     lm_start;
            public int[]     lm_size;
            public Vector3   lm_origin;
            public Vector3[] lm_vecs;
            public Vector3   normal;
            public int[]     size;
        }

        private const int FACE_SIZE = 104;

        private FaceEntry[] faces;

        public Face() { }

        public override void ParseDirectoryEntry(FileStream file, int offset, int length)
        {
            size = length / FACE_SIZE;

            // Create faces array.
            faces = new FaceEntry[size];

            // Seek to the specified offset within the file.
            file.Seek (offset, SeekOrigin.Begin);

            // Create buffer to hold data.
            byte[] buf = new byte[FACE_SIZE];

            for (int i = 0; i < size; i++)
            {
                file.Read (buf, 0, FACE_SIZE);

                faces[i].lm_start    = new int[2];
                faces[i].lm_size     = new int[2];
                faces[i].lm_vecs     = new Vector3[2];
                faces[i].size        = new int[2];
                faces[i].texture     = BitConverter.ToInt32(buf,   0 * sizeof(int));
                faces[i].effect      = BitConverter.ToInt32(buf,   1 * sizeof(int));
                faces[i].type        = BitConverter.ToInt32(buf,   2 * sizeof(int));
                faces[i].vertex      = BitConverter.ToInt32(buf,   3 * sizeof(int));
                faces[i].n_vertexes  = BitConverter.ToInt32(buf,   4 * sizeof(int));
                faces[i].meshVert    = BitConverter.ToInt32(buf,   5 * sizeof(int));
                faces[i].n_meshVerts = BitConverter.ToInt32(buf,   6 * sizeof(int));
                faces[i].lm_index    = BitConverter.ToInt32(buf,   7 * sizeof(int));
                faces[i].lm_start[0] = BitConverter.ToInt32(buf,   8 * sizeof(int));
                faces[i].lm_start[1] = BitConverter.ToInt32(buf,   9 * sizeof(int));
                faces[i].lm_size[0]  = BitConverter.ToInt32(buf,  10 * sizeof(int));
                faces[i].lm_size[1]  = BitConverter.ToInt32(buf,  11 * sizeof(int));
                faces[i].lm_origin   = new Vector3(BitConverter.ToSingle(buf, 12 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 13 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 14 * sizeof(float)));
                faces[i].lm_vecs[0]  = new Vector3(BitConverter.ToSingle(buf, 15 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 16 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 17 * sizeof(float)));
                faces[i].lm_vecs[1]  = new Vector3(BitConverter.ToSingle(buf, 18 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 19 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 20 * sizeof(float)));
                faces[i].normal      = new Vector3(BitConverter.ToSingle(buf, 21 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 22 * sizeof(float)),
                                                   BitConverter.ToSingle(buf, 23 * sizeof(float)));
                faces[i].size[0]     = BitConverter.ToInt32(buf,  24 * sizeof(int));
                faces[i].size[1]     = BitConverter.ToInt32(buf,  25 * sizeof(int));
            }
        }

        public FaceEntry[] GetFaces()
        {
            return(faces);
        }

        public FaceEntry GetFace(int face)
        {
            return(faces[face]);
        }
    }
}

