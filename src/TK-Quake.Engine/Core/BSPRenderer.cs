using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Loader;
using TKQuake.Engine.Loader.BSP;
using TKQuake.Engine.Infrastructure;

namespace TKQuake.Engine.Core
{
    public class BSPRenderer
    {
        private string BSPFile;
        private Loader.BSPLoader BSP;

        public BSPRenderer ()
        {
            BSP = null;
        }

        public BSPRenderer (string file) : this()
        {
            SetFile (file);
        }

        public void SetFile (string file)
        {
            BSPFile = file;

            // Delete old BSP.
            if (BSP != null)
            {
                BSP = null;
            }

            BSP = new Loader.BSPLoader (BSPFile);
            BSP.LoadFile ();
            BSP.DumpBSP ();
        }

        public string GetFile ()
        {
            return(BSPFile);
        }

        public Mesh GetMesh(Vector3 cameraPosition)
        {
            Mesh BSPMesh = new Mesh();

            // Find the cluster that the camera is currently in.
            int cameraCluster = BSP.GetLeaf (FindCameraLeaf (cameraPosition)).cluster;

            bool[] alreadyVisible = new bool[BSP.GetFaces ().Length];
            int[]  visibleFaces   = new int[1];

            // Find all leafs that are visible from cameraCluster.
            foreach (Loader.BSP.Leaf.LeafEntry leaf in BSP.GetLeafs ())
            {
                if (IsClusterVisible (cameraCluster, leaf.cluster) == true)
                {
                    for (int face = leaf.leafFace; face < leaf.n_leafFaces; face++)
                    {
                        // If we haven't already added this face to the list of visible faces then add it.
                        if (alreadyVisible [face] == false)
                        {
                            alreadyVisible [face] = true;

                            // Resize array.
                            if (visibleFaces.Length != 1)
                            {
                                Array.Resize<int> (ref visibleFaces, visibleFaces.Length + 1);
                            }

                            visibleFaces [visibleFaces.Length - 1] = face;
                        }
                    }
                }
            }

            // Resize the vertices array so we can add in the new vertices.
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals  = new List<Vector3>();
            List<Vector2> textures = new List<Vector2>();
            List<int>     indices  = new List<int>();

            // Iterate through all the visible faces and collect all of the vertices.
            foreach (int face in visibleFaces)
            {
                Face.FaceEntry currentFace = BSP.GetFace (face);

                switch(BSP.GetFace (face).type)
                {
                    case Face.FaceType.POLYGON:
                    {
                        Vector3[] vertexes, norms;
                        Vector2[] texCoords;
                        int[]     indexes;

                        RenderPolygon (currentFace, out vertexes, out norms, out texCoords, out indexes);

                        foreach (Vector3 vertex in vertexes)
                        {
                            vertices.Add (vertex);
                        }

                        foreach (Vector3 norm in norms)
                        {
                            normals.Add (norm);
                        }

                        foreach (Vector2 texCoord in texCoords)
                        {
                            textures.Add (texCoord);
                        }

                        int offset = indices.Count;
                        foreach (int index in indexes)
                        {
                            indices.Add (offset + index);
                        }

                        break;
                    }

                    case Face.FaceType.PATCH:
                    {
                        Vector3[] vertexes;
                        int[]     indexes;

                        TessellateBezierPatch (currentFace, 7, out vertexes, out indexes);

                        foreach (Vector3 vertex in vertexes)
                        {
                            vertices.Add (vertex);
                        }

                        int offset = indices.Count;
                        foreach (int index in indexes)
                        {
                            indices.Add (index);
                        }

                        break;
                    }

                    case Face.FaceType.MESH:
                    {
                        Vector3[] vertexes, norms;
                        Vector2[] texCoords;
                        int[]     indexes;

                        RenderPolygon (currentFace, out vertexes, out norms, out texCoords, out indexes);

                        foreach (Vector3 vertex in vertexes)
                        {
                            vertices.Add (vertex);
                        }

                        foreach (Vector3 norm in norms)
                        {
                            normals.Add (norm);
                        }

                        foreach (Vector2 texCoord in texCoords)
                        {
                            textures.Add (texCoord);
                        }

                        int offset = indices.Count;
                        foreach (int index in indexes)
                        {
                            indices.Add (offset + index);
                        }

                        break;
                    }

                    case Face.FaceType.BILLBOARD:
                    {
                        Console.WriteLine("BSPRenderer: Billboards are currently not supported.");
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("BSPRenderer: Unknown face type detected.");
                        break;
                    }
                }

                BSPMesh.Vertices = vertices.ToArray ();
                BSPMesh.Normals  = normals.ToArray ();
                BSPMesh.Textures = textures.ToArray ();
                BSPMesh.Indices  = indices.ToArray ();
            }

            return(BSPMesh);
        }

        private int FindCameraLeaf(Vector3 cameraPosition)
        {
            int index = 0;

            while (index >= 0)
            {
                Node.NodeEntry   node  = BSP.GetNode (index);
                Plane.PlaneEntry plane = BSP.GetPlane (node.plane);

                // Distance from point to a plane
                double distance = Vector3.Dot (plane.normal, cameraPosition) - plane.dist;

                if (distance >= 0)
                {
                    index = node.children[0];
                }

                else
                {
                    index = node.children[1];
                }
            }

            return(-index - 1);
        }

        private bool IsClusterVisible(int visCluster, int testCluster)
        {
            // Make sure we have something to test.
            if ((BSP.GetVisData (0).vecs == null) || (visCluster < 0))
            {
                return(true);
            }

            // Find the relevant section in the visData bit array.
            int i = (visCluster * BSP.GetVisData (0).sz_vecs) + (testCluster >> 3);
            byte visSet = BSP.GetVisData (0).vecs[i];

            // Check to see if the testCluster is visible from visCluster
            return((visSet & (1 << (testCluster & 7))) != 0);
        }

        private void TessellateBezierPatch(Face.FaceEntry face, int level, out Vector3[] vertices, out int[] indices)
        {
            // The amount of increments we need to make for each dimension, so we have the (potentially) shared points between patches
            int stepWidth  = (face.size[0] - 1) / 2;
            int stepHeight = (face.size[1] - 1) / 2;

            int c = 0;

            Vector3[] controlPoints = new Vector3[9];

            // Generate the control points.
            for (int i = 0; i < face.size[0]; i += stepWidth)
            {
                for ( int j = 0; j < face.size[1]; j += stepHeight)
                {
                    controlPoints[c++] = BSP.GetVertex (face.vertex + j * face.size[0] + i).position;
                }
            }

            // The number of vertices along a side is 1 + num edges
            int level1 = level + 1;

            Vector3[] vertex = new Vector3[level1 * level1];

            // Compute the vertices
            for (int i = 0; i <= level; i++)
            {
                float a = (float)i / level;
                float b = 1 - a;

                vertex[i] = Vector3.Multiply(controlPoints[0], (b * b)) + Vector3.Multiply(controlPoints[3], (2 * b * a)) + Vector3.Multiply(controlPoints[6], (a * a));
            }

            for (int i = 1; i <= level; i++)
            {
                float a = (float)i / level;
                float b = 1 - a;

                Vector3[] temp = new Vector3[3];

                for (int j = 0; j < 3; j++)
                {
                    int k = 3 * j;

                    temp[j] = Vector3.Multiply(controlPoints[k + 0], (b * b)) + Vector3.Multiply(controlPoints[k + 1], (2 * b * a)) + Vector3.Multiply(controlPoints[k + 2], (a * a));
                }

                for (int j = 0; j <= level; j++)
                {
                    float n = (float)j / level;
                    float m = 1 - n;

                    vertex[i * level1 + j] = Vector3.Multiply(temp[0], (m * m)) + Vector3.Multiply(temp[1], (2 * m * n)) + Vector3.Multiply(temp[2], (n * n));
                }
            }

            // Compute the indices
            int[] indexes = new int[level * (level + 1) * 2];

            for (int row = 0; row < level; ++row)
            {
                for(int col = 0; col <= level; ++col)
                {
                    indexes[(row * (level + 1) + col) * 2 + 1] = row       * level1 + col;
                    indexes[(row * (level + 1) + col) * 2]     = (row + 1) * level1 + col;
                }
            }

            int[]    trianglesPerRow = new int[level];
//            IntPtr[] rowIndexes      = new IntPtr[level];

            for (int row = 0; row < level; ++row)
            {
                trianglesPerRow[row] = 2 * level1;
//                rowIndexes[row]      = &indexes[row * 2 * level1];
            }

            vertices = vertex;
            indices  = indexes;
        }

        private void RenderPolygon(Face.FaceEntry face, out Vector3[] vertices, out Vector3[] normals, out Vector2[] texCoords, out int[] indices)
        {
            vertices  = new Vector3[face.n_vertexes];
            normals   = new Vector3[face.n_vertexes];
            texCoords = new Vector2[face.n_vertexes];

            // Add all the vertexes for the face to the mesh.
            int offset = face.vertex;

            for (int vertex = 0; vertex < face.n_vertexes; vertex++)
            {
                Vertex.VertexEntry currentVertex = BSP.GetVertex (offset + vertex);
                vertices[vertex]  = currentVertex.position;
                normals[vertex]   = currentVertex.normal;
                texCoords[vertex] = currentVertex.texCoord[0];
            }

            // Add all the vertex indices for the face to the mesh.
            indices = new int[face.n_meshVerts];
            offset  = face.meshVert;

            for (int vertex = 0; vertex < face.n_meshVerts; vertex++)
            {
                // MeshVert offset is relative to the first vertex of the face.
                indices[vertex] = BSP.GetMeshVert (offset + vertex).offset;
            }
        }
    }
}

