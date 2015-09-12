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
        private const int TESSELLATION_LEVEL = 7;

        private string BSPFile;
        private Loader.BSPLoader BSP;

        private Vector4[] frustum;

        public BSPRenderer ()
        {
            BSP = null;
            frustum = new Vector4[6];
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

            BSP = new Loader.BSPLoader (BSPFile, true);
            BSP.LoadFile ();
            BSP.DumpBSP ();
        }

        public string GetFile ()
        {
            return(BSPFile);
        }

        public Mesh GetMesh(Vector3 cameraPosition, Matrix4 clip)
        {
            Mesh BSPMesh = new Mesh();

            // Find the cluster that the camera is currently in.
            int cameraCluster = BSP.GetLeaf (FindCameraLeaf (cameraPosition)).cluster;

            bool[]    alreadyVisible = new bool[BSP.GetFaces ().Length];
            List<int> visibleFaces   = new List<int> ();

            // Set the view frustum.
            SetViewFrustum (clip);

            // Find all leafs that are visible from cameraCluster.
            foreach (Loader.BSP.Leaf.LeafEntry leaf in BSP.GetLeafs ())
            {
                if ((IsClusterVisible (cameraCluster, leaf.cluster) == true) && (IsBoxInsideViewFrustum (leaf.mins, leaf.maxs)))
                {
                    for (int leafFace = leaf.leafFace; leafFace < (leaf.leafFace + leaf.n_leafFaces); leafFace++)
                    {
                        int face = BSP.GetLeafFace (leafFace).face;

                        // If we haven't already added this face to the list of visible faces then add it.
                        if (alreadyVisible [face] == false)
                        {
                            alreadyVisible [face] = true;
                            visibleFaces.Add (face);
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

        private void SetViewFrustum(Matrix4 clip)
        {
            /* Extract the numbers for the RIGHT plane */
            frustum [0] = Vector4.Normalize (Vector4.Subtract (clip.Column3, clip.Column0));

            /* Extract the numbers for the LEFT plane */
            frustum [1] = Vector4.Normalize (Vector4.Add (clip.Column3, clip.Column0));

            /* Extract the BOTTOM plane */
            frustum [2] = Vector4.Normalize (Vector4.Add (clip.Column3, clip.Column1));

            /* Extract the TOP plane */
            frustum [3] = Vector4.Normalize (Vector4.Subtract (clip.Column3, clip.Column1));

            /* Extract the FAR plane */
            frustum [4] = Vector4.Normalize (Vector4.Subtract (clip.Column3, clip.Column2));

            /* Extract the NEAR plane */
            frustum [5] = Vector4.Normalize (Vector4.Add (clip.Column3, clip.Column2));
        }

        private bool IsBoxInsideViewFrustum(Vector3 min, Vector3 max)
        {
            // Test the bounds of every the bounding box with every plane in the view frustum.
            foreach (Vector4 plane in frustum)
            {
                bool[] dots = new bool[8];

                // Test the bounds of the bounding box formed by min and max with the current plane.
                dots [0] = (Vector4.Dot (plane, new Vector4 (min[0], min[1], min[2], 1.0f))) >= 0.0f;
                dots [1] = (Vector4.Dot (plane, new Vector4 (max[0], min[1], min[2], 1.0f))) >= 0.0f;
                dots [2] = (Vector4.Dot (plane, new Vector4 (min[0], max[1], min[2], 1.0f))) >= 0.0f;
                dots [3] = (Vector4.Dot (plane, new Vector4 (max[0], max[1], min[2], 1.0f))) >= 0.0f;
                dots [4] = (Vector4.Dot (plane, new Vector4 (min[0], min[1], max[2], 1.0f))) >= 0.0f;
                dots [5] = (Vector4.Dot (plane, new Vector4 (max[0], min[1], max[2], 1.0f))) >= 0.0f;
                dots [6] = (Vector4.Dot (plane, new Vector4 (min[0], max[1], max[2], 1.0f))) >= 0.0f;
                dots [7] = (Vector4.Dot (plane, new Vector4 (max[0], max[1], max[2], 1.0f))) >= 0.0f;

                // If all tests fail then the box is not inside the view frustum.
                if (!(dots[0] || dots[1] || dots[2] || dots[3] || dots[4] || dots[5] || dots[6] || dots[7]))
                {
                    return(false);
                }
            }

            // At least one test passed for every plane. So at least some part of the box
            // is inside the view frustum.
            return(true);
        }

        private int FindCameraLeaf(Vector3 cameraPosition)
        {
            int index = 0;
            Vector4 cam = new Vector4 (cameraPosition [0], cameraPosition [1], cameraPosition [2], 1.0f);

            // Leaf nodes have negative indexes.
            while (index >= 0)
            {
                Node.NodeEntry   node  = BSP.GetNode (index);
                Plane.PlaneEntry plane = BSP.GetPlane (node.plane);

                // Distance from point to the plane
                double distance = Vector4.Dot (plane.plane, cam);
                //double distance = Vector3.Dot (plane.normal, cameraPosition) - plane.dist;

                // Determine which branch of the tree to traverse down.
                if (distance >= 0)
                {
                    index = node.children[1];
                }

                else
                {
                    index = node.children[0];
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

        private void TessellateBezierPatch (Face.FaceEntry face, int level, ref List<Vector3> vertices, ref List<Vector2> texCoords, ref List<int> indices)
        {
            // Preserve index of the first vertex in this patch.
            int vertexIndexOffset = vertices.Count;

            // The amount of increments we need to make for each dimension, so we have the (potentially) shared points between patches
            int stepWidth  = (face.size[0] - 1) / 2;
            int stepHeight = (face.size[1] - 1) / 2;
            int numPatches = stepHeight * stepWidth;

            for (int patch = 0; patch < numPatches; patch++)
            {
                //Calculate how many patches there are using size[]
                //There are n_patchesX by n_patchesY patches in the grid, each of those
                //starts at a vert (i,j) in the overall grid
                //We don't actually need to know how many are on the Y length
                //but the forumla is here for historical/academic purposes
                int n_patchesX = stepWidth;
                //int n_patchesY = stepHeight;


                //Calculate what [n,m] patch we want by using an index
                //called patchNumber  Think of patchNumber as if you 
                //numbered the patches left to right, top to bottom on
                //the grid in a piece of paper.
                int pxStep = 0;
                int pyStep = 0;
                for (int i = 0; i < patch; i++)
                {
                    pxStep++;
                    if (pxStep == n_patchesX)
                    {
                        pxStep = 0;
                        pyStep++;
                    }
                }
                    
                //Create an array the size of the grid, which is given by
                //size[] on the face object.
                Vertex.VertexEntry[,] vertGrid = new Vertex.VertexEntry[face.size [0], face.size [1]];

                //Read the verts for this face into the grid, making sure
                //that the final shape of the grid matches the size[] of
                //the face.
                int gridXstep = 0;
                int gridYstep = 0;
                int vertStep = face.vertex;
                for (int i = 0; i < face.n_vertexes; i++)
                {
                    vertGrid [gridXstep, gridYstep] = BSP.GetVertex (vertStep);
                    vertStep++;
                    gridXstep++;
                    if (gridXstep == face.size [0])
                    {
                        gridXstep = 0;
                        gridYstep++;
                    }
                }

                //We now need to pluck out exactly nine vertexes to pass to our
                //teselate function, so lets calculate the starting vertex of the
                //3x3 grid of nine vertexes that will make up our patch.
                //we already know how many patches are in the grid, which we have
                //as n and m.  There are n by m patches.  Since this method will
                //create one gameobject at a time, we only need to be able to grab
                //one.  The starting vertex will be called vi,vj think of vi,vj as x,y
                //coords into the grid.
                int vi = 2 * pxStep;
                int vj = 2 * pyStep;
                //Now that we have those, we need to get the vert at [vi,vj] and then
                //the two verts at [vi+1,vj] and [vi+2,vj], and then [vi,vj+1], etc.
                //the ending vert will at [vi+2,vj+2]

                Vector3[] vertControls  = new Vector3[9];
                Vector2[] textControls  = new Vector2[9];
                Vector2[] lightControls = new Vector2[9];

                //Top row
                vertControls [0] = vertGrid [vi    , vj].position;
                vertControls [1] = vertGrid [vi + 1, vj].position;
                vertControls [2] = vertGrid [vi + 2, vj].position;

                textControls [0] = vertGrid [vi    , vj].texCoord[0];
                textControls [1] = vertGrid [vi + 1, vj].texCoord[0];
                textControls [2] = vertGrid [vi + 2, vj].texCoord[0];

                lightControls [0] = vertGrid [vi    , vj].texCoord[1];
                lightControls [1] = vertGrid [vi + 1, vj].texCoord[1];
                lightControls [2] = vertGrid [vi + 2, vj].texCoord[1];

                //Middle row
                vertControls [3] = vertGrid [vi    , vj + 1].position;
                vertControls [4] = vertGrid [vi + 1, vj + 1].position;
                vertControls [5] = vertGrid [vi + 2, vj + 1].position;

                textControls [3] = vertGrid [vi    , vj + 1].texCoord[0];
                textControls [4] = vertGrid [vi + 1, vj + 1].texCoord[0];
                textControls [5] = vertGrid [vi + 2, vj + 1].texCoord[0];

                lightControls [3] = vertGrid [vi    , vj + 1].texCoord[1];
                lightControls [4] = vertGrid [vi + 1, vj + 1].texCoord[1];
                lightControls [5] = vertGrid [vi + 2, vj + 1].texCoord[1];

                //Bottom row
                vertControls [6] = vertGrid [vi    , vj + 2].position;
                vertControls [7] = vertGrid [vi + 1, vj + 2].position;
                vertControls [8] = vertGrid [vi + 2, vj + 2].position;

                textControls [6] = vertGrid [vi    , vj + 2].texCoord[0];
                textControls [7] = vertGrid [vi + 1, vj + 2].texCoord[0];
                textControls [8] = vertGrid [vi + 2, vj + 2].texCoord[0];

                lightControls [6] = vertGrid [vi    , vj + 2].texCoord[1];
                lightControls [7] = vertGrid [vi + 1, vj + 2].texCoord[1];
                lightControls [8] = vertGrid [vi + 2, vj + 2].texCoord[1];

                // The incoming list is 9 entires, 
                // referenced as p0 through p8 here.

                // Generate extra rows to tessellate
                // each row is three control points
                // start, curve, end
                // The "lines" go as such
                // p0s from p0 to p3 to p6 ''
                // p1s from p1 p4 p7
                // p2s from p2 p5 p8

                List<Vector2> p0suv2, p1suv2, p2suv2;
                List<Vector2> p0suv, p1suv, p2suv;
                List<Vector3> p0s, p1s, p2s;
                p0s    = Tessellate(level, vertControls [0], vertControls [3], vertControls [6]);
                p0suv  = TessellateUV(level, textControls [0], textControls [3], textControls [6]);
                p0suv2 = TessellateUV(level, lightControls [0], lightControls [3], lightControls [6]);

                p1s    = Tessellate(level, vertControls [1], vertControls [4], vertControls [7]);
                p1suv  = TessellateUV(level, textControls [1], textControls [4], textControls [7]);
                p1suv2 = TessellateUV(level, lightControls [1], lightControls [4], lightControls [7]);

                p2s    = Tessellate(level, vertControls [2], vertControls [5], vertControls [8]);
                p2suv  = TessellateUV(level, textControls [2], textControls [5], textControls [8]);
                p2suv2 = TessellateUV(level, lightControls [2], lightControls [5], lightControls [8]);

                // Tessellate all those new sets of control points and pack
                // all the results into our vertex array, which we'll return.
                // Make our uvs list while we're at it.
                for (int i = 0; i <= level; i++)
                {
                    vertices.AddRange (Tessellate (level, p0s [i], p1s [i], p2s [i]));
                    texCoords.AddRange (TessellateUV (level, p0suv [i], p1suv [i], p2suv [i]));
//                    lightCoords.AddRange (TessellateUV (level, p0suv2 [i], p1suv2 [i], p2suv2 [i]));
                }

                // This will produce (tessellationLevel + 1)^2 verts
                int numVerts = (level + 1) * (level + 1);

                // Compute triangle indexes for forming a mesh.
                // The mesh will be tessellationlevel + 1 verts
                // wide and tall.
                int xStep = 1;
                int width = level + 1;

                for (int i = vertexIndexOffset; i < (vertexIndexOffset + (numVerts - width)); i++)
                {
                    //on left edge
                    if (xStep == 1)
                    {
                        indices.Add (i);
                        indices.Add (i + width);
                        indices.Add (i + 1);

                        xStep++;

                        continue;
                    } 

                    else if (xStep == width) //on right edge
                    {
                        indices.Add (i);
                        indices.Add (i + (width - 1));
                        indices.Add (i + width);

                        xStep = 1;

                        continue;
                    }

                    else // not on an edge, so add two
                    {
                        indices.Add (i);
                        indices.Add (i + (width - 1));
                        indices.Add (i + width);
                        indices.Add (i);
                        indices.Add (i + width);
                        indices.Add (i + 1);

                        xStep++;

                        continue;
                    }
                }

                // TODO: Figure out how to calculate vertex normals.
            }
        }

        // This takes a tessellation level and three vector3
        // p0 is start, p1 is the midpoint, p2 is the endpoint
        // The returned list begins with p0, ends with p2, with
        // the tessellated verts in between.
        private List<Vector3> Tessellate(int level, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            List<Vector3> vects = new List<Vector3>();

            float stepDelta = 1.0f / level;
            float step = stepDelta;

            vects.Add (p0);

            for (int i = 1; i < level; i++)
            {
                vects.Add (BezCurve (step, p0, p1, p2));
                step += stepDelta;
            }

            vects.Add (p2);

            return(vects);
        }

        // Same as above, but for UVs
        private List<Vector2> TessellateUV(int level, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            List<Vector2> vects = new List<Vector2>();

            float stepDelta = 1.0f / level;
            float step = stepDelta;

            vects.Add (p0);

            for (int i = 1; i < level; i++)
            {
                vects.Add (BezCurveUV (step, p0, p1, p2));
                step += stepDelta;
            }

            vects.Add (p2);

            return vects;
        }

        // Calculate a vector3 at point t on a bezier curve between
        // p0 and p2 via p1.  
        private Vector3 BezCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float a = 1f - t;
            float tt = t * t;

            float[] tPoints = new float[3];

            for (int i = 0; i < 3; i++)
            {
                tPoints [i] = ((a * a) * p0 [i]) + (2 * a) * (t * p1 [i]) + (tt * p2 [i]);
            }

            return(new Vector3(tPoints [0], tPoints [1], tPoints [2]));
        }

        // Calculate UVs for our tessellated vertices 
        private Vector2 BezCurveUV(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float a = 1f - t;
            float tt = t * t;

            float[] tPoints = new float[2];

            for (int i = 0; i < 2; i++)
            {
                tPoints [i] = ((a * a) * p0 [i]) + (2 * a) * (t * p1 [i]) + (tt * p2 [i]);
            }

            return(new Vector2(tPoints [0], tPoints [1]));
        }

        private void RenderPolygon(Face.FaceEntry face, ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> texCoords, ref List<int> indices)
        {
            // Add all the vertex indices for the face to the mesh.
            for (int meshVert = face.meshVert; meshVert < (face.meshVert + face.n_meshVerts); meshVert++)
            {
                // MeshVert offset is relative to the first vertex of the face.
                indices.Add (vertices.Count + BSP.GetMeshVert (meshVert).offset);
            }

            // Add all the vertexes for the face to the mesh.
            for (int vertex = face.vertex; vertex < (face.vertex + face.n_vertexes); vertex++)
            {
                Vertex.VertexEntry currentVertex = BSP.GetVertex (vertex);
                vertices.Add (currentVertex.position);
                normals.Add (currentVertex.normal);
                texCoords.Add (currentVertex.texCoord [0]);
//                lightCoords.Add (currentVertex.texCoord [1]);
            }
        }
    }
}

