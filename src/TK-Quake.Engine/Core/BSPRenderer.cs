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
                // Add all the vertexes for the face to the mesh.
                int index = vertices.Count;
                int offset = BSP.GetFace (face).vertex;

                for (int vertex = 0; vertex < BSP.GetFace (face).n_vertexes; vertex++)
                {
                    vertices.Add (BSP.GetVertex (offset + vertex).position);
                    normals.Add  (BSP.GetVertex (offset + vertex).normal);
                    textures.Add (BSP.GetVertex (offset + vertex).texCoord[0]);
                }

                // Add all the vertex indices for the face to the mesh.
                index = indices.Count;
                offset = BSP.GetFace (face).meshVert;

                for (int vertex = 0; vertex < BSP.GetFace (face).n_meshVerts; vertex++)
                {
                    // MeshVert offset is relative to the first vertex of the face.
                    indices.Add(index + BSP.GetMeshVert (offset + vertex).offset);
                }
            }

            BSPMesh.Vertices = vertices.ToArray ();
            BSPMesh.Normals  = normals.ToArray ();
            BSPMesh.Textures = textures.ToArray ();
            BSPMesh.Indices  = indices.ToArray ();

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
    }
}

