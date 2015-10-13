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
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Engine.Core
{
    public class BSPRenderer
    {
        private const int   TESSELLATION_LEVEL = 5;
        private const float NEAR_DISTANCE = 0.1f;
        private const float FAR_DISTANCE = 20.0f;
        private const float ASPECT_RATIO = 800.0f / 600.0f;
        private const float FOV_Y = MathHelper.PiOver4;
        private const float FOV_X = 1.009191290f; //2 * Math.Atan(Math.Tan(FOV_Y / 2) * ASPECT_RATIO);

        private string BSPFile;
        private Loader.BSPLoader BSP;

        private Vector4[] frustum;

        private TextureManager texManager;

        public BSPRenderer ()
        {
            BSP = null;
            frustum = new Vector4[6];
            texManager = new TextureManager ();
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

        public Loader.BSPLoader GetLoader()
        {
            return(BSP);
        }

        //public List<Mesh> GetMesh(Vector3 cameraPosition, Matrix4 clip)
        public List<Mesh> GetMesh(Camera camera)
        {
            List<Mesh> BSPMeshes = new List<Mesh>();

            // Find the cluster that the camera is currently in.
            int cameraCluster = BSP.GetLeaf (FindCameraLeaf (camera.Position)).cluster;

            bool[]    alreadyVisible = new bool[BSP.GetFaces ().Length];
            List<int> visibleFaces   = new List<int> ();

            // Set the view frustum.
            //SetViewFrustum (clip);
            SetViewFrustum (camera);

            // Find all leafs that are visible from cameraCluster.
            for (int i = (BSP.GetLeafs().Length - 1); i >= 0; i--)
            {
                Leaf.LeafEntry leaf = BSP.GetLeaf (i);

                if ((IsClusterVisible (cameraCluster, leaf.cluster) == true) && (IsBoxInsideViewFrustum (leaf.mins, leaf.maxs)))
                {
                    for (int leafFace = (leaf.leafFace + leaf.n_leafFaces - 1); leafFace >= leaf.leafFace; leafFace--)
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

            // Iterate through all the visible faces and collect all of the vertices.
            foreach (int face in visibleFaces)
            {
                Face.FaceEntry currentFace = BSP.GetFace (face);

                switch(currentFace.type)
                {
                    case Face.FaceType.POLYGON:
                    {
                        BSPMeshes.Add (RenderPolygon (currentFace));

                        break;
                    }

                    case Face.FaceType.PATCH:
                    {
                        BSPMeshes.AddRange (TessellateBezierPatch (currentFace));

                        break;
                    }

                    case Face.FaceType.MESH:
                    {
                        BSPMeshes.Add (RenderPolygon (currentFace));

                        break;
                    }

                    case Face.FaceType.BILLBOARD:
                    {
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("BSPRenderer: Unknown face type detected.");
                        break;
                    }
                }
            }

            return(BSPMeshes);
        }

        private void SetViewFrustum(Camera camera)
        {
            Vector3 view  = Vector3.Normalize (camera.ViewDirection);
            Vector3 up    = Vector3.UnitY;
            Vector3 right = Vector3.Cross (view, up);

            // Half the height of the near plane.
            float nearHeight = NEAR_DISTANCE * (float)Math.Tan (FOV_Y / 2.0f);

            // Half the width of the near plane.
            float nearWidth = NEAR_DISTANCE * (float)Math.Tan (FOV_X / 2.0f);

            // Find the center of the far clipping plane.
            Vector3 farCenter = camera.Position + FAR_DISTANCE * view;

            // Find the center of the near clipping plane.
            Vector3 nearCenter = camera.Position + NEAR_DISTANCE * view;

            // Find the far clipping plane.
            frustum [0] = new Vector4 (-view, -Vector3.Dot (-view, farCenter));

            // Find the near clipping plane.
            frustum [1] = new Vector4 ( view, -Vector3.Dot (view, nearCenter));

            Vector3 aux, normal;

            // Find the top clipping plane.
            aux = Vector3.Normalize ((nearCenter + nearHeight * up) - camera.Position);
            normal = Vector3.Cross (aux, up);
            frustum [2] = new Vector4 (-normal, -Vector3.Dot (-normal, (nearCenter + nearHeight * up)));

            // Find the bottom clipping plane.
            aux = Vector3.Normalize ((nearCenter - nearHeight * up) - camera.Position);
            frustum [3] = new Vector4 ( normal, -Vector3.Dot ( normal, (nearCenter - nearHeight * up)));

            // Find the left clipping plane.
            aux = Vector3.Normalize ((nearCenter + nearWidth * right) - camera.Position);
            normal = Vector3.Cross (aux, right);
            frustum [4] = new Vector4 ( normal, -Vector3.Dot ( normal, (nearCenter + nearWidth * right)));

            // Find the right clipping plane.
            aux = Vector3.Normalize ((nearCenter - nearWidth * right) - camera.Position);
            frustum [5] = new Vector4 (-normal, -Vector3.Dot (-normal, (nearCenter - nearWidth * right)));
        }

        private void SetViewFrustum(Matrix4 clip)
        {
            if (clip == Matrix4.Zero)
            {
                frustum [0] = Vector4.Zero;
                frustum [1] = Vector4.Zero;
                frustum [2] = Vector4.Zero;
                frustum [3] = Vector4.Zero;
                frustum [4] = Vector4.Zero;
                frustum [5] = Vector4.Zero;
            }

            else
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
        }

        private bool IsBoxInsideViewFrustum(Vector3 min, Vector3 max)
        {
            // If the view frustum has not been defined then dont even bother.
            if ((frustum[0] == Vector4.Zero) || (frustum[1] == Vector4.Zero) || (frustum[2] == Vector4.Zero) ||
                (frustum[3] == Vector4.Zero) || (frustum[4] == Vector4.Zero) || (frustum[5] == Vector4.Zero))
            {
                return(true);
            }

            // Put all of the corners of the bounding box into an array for easier processing.
            Vector4[] vertices = new Vector4[8];
            vertices [0] = new Vector4 (min [0], min [1], min [2], 1.0f);
            vertices [1] = new Vector4 (max [0], min [1], min [2], 1.0f);
            vertices [2] = new Vector4 (min [0], max [1], min [2], 1.0f);
            vertices [3] = new Vector4 (max [0], max [1], min [2], 1.0f);
            vertices [4] = new Vector4 (min [0], min [1], max [2], 1.0f);
            vertices [5] = new Vector4 (max [0], min [1], max [2], 1.0f);
            vertices [6] = new Vector4 (min [0], max [1], max [2], 1.0f);
            vertices [7] = new Vector4 (max [0], max [1], max [2], 1.0f);

            int inside = 0, outside = 0;

            // Test the bounds of every the bounding box with every plane in the view frustum.
            foreach (Vector4 plane in frustum)
            {
                // Check each vertex of the bounding box to see which side of the plane it is on.
                // Stop checking as soon as we have at least on point on each side of the plane.
                for (int i = 0; i < 8 && (inside == 0 || outside == 0); i++)
                {
                    if (Vector4.Dot (plane, vertices [i]) < 0)
                    {
                        outside++;
                    }

                    else
                    {
                        inside++;
                    }
                }

                // If all of the vertices are on the wrong side of this plane then we can give up now.
                if (inside == 0)
                {
                    return(false);
                }
            }

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
            VisData.VisDataEntry visData = BSP.GetVisData (0);

            // Make sure we have something to test.
            if ((visData.vecs == null) || (visCluster < 0) || (testCluster < 0))
            {
                return(true);
            }

            // Find the relevant section in the visData bit array.
            int i = (visCluster * visData.sz_vecs) + (testCluster >> 3);
            byte visSet = visData.vecs[i];

            // Check to see if the testCluster is visible from visCluster
            return((visSet & (1 << (testCluster & 7))) > 0);
        }

        private List<Mesh> TessellateBezierPatch (Face.FaceEntry face)
        {
            List<Mesh>                       meshes = new List<Mesh> ();
            List<Infrastructure.Math.Vertex> vertices;
            List<int>                        indices;
            Mesh                             mesh;
            Vector3[]                        vertControls  = new Vector3[9];
            Vector2[]                        textControls  = new Vector2[9];
            Vector2[]                        lightControls = new Vector2[9];

            // The amount of increments we need to make for each dimension, so we have the (potentially) shared points between patches
            int stepWidth  = (face.size[0] - 1) / 2;
            int stepHeight = (face.size[1] - 1) / 2;
            int numPatches = stepHeight * stepWidth;

            for (int patch = 0; patch < numPatches; patch++)
            {
                mesh      = new Mesh ();
                vertices  = new List<Vector3>();
                texCoords = new List<Vector2>();
                indices   = new List<int>();

                //Calculate how many patches there are using size[]
                //There are n_patchesX by n_patchesY patches in the grid, each of those
                //starts at a vert (i,j) in the overall grid
                //We don't actually need to know how many are on the Y length
                //but the forumla is here for historical/academic purposes
                int n_patchesX = stepWidth;
                int n_patchesY = stepHeight;

                //Calculate what [n,m] patch we want by using an index
                //called patchNumber. Think of patchNumber as if you
                //numbered the patches left to right, top to bottom on
                //the grid in a piece of paper.
                int pxStep = patch % n_patchesX;
                int pyStep = (patch / n_patchesX) % n_patchesY;

                //Create an array the size of the grid, which is given by
                //size[] on the face object.
                Loader.BSP.Vertex.VertexEntry[,] vertGrid = new Loader.BSP.Vertex.VertexEntry[face.size [0], face.size [1]];

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

                //Top row
                vertControls [0]  = vertGrid [vi + 0, vj + 0].position;
                vertControls [1]  = vertGrid [vi + 1, vj + 0].position;
                vertControls [2]  = vertGrid [vi + 2, vj + 0].position;

                textControls [0]  = vertGrid [vi + 0, vj + 0].texCoord[0];
                textControls [1]  = vertGrid [vi + 1, vj + 0].texCoord[0];
                textControls [2]  = vertGrid [vi + 2, vj + 0].texCoord[0];

                lightControls [0] = vertGrid [vi + 0, vj + 0].texCoord[1];
                lightControls [1] = vertGrid [vi + 1, vj + 0].texCoord[1];
                lightControls [2] = vertGrid [vi + 2, vj + 0].texCoord[1];

                //Middle row
                vertControls [3]  = vertGrid [vi + 0, vj + 1].position;
                vertControls [4]  = vertGrid [vi + 1, vj + 1].position;
                vertControls [5]  = vertGrid [vi + 2, vj + 1].position;

                textControls [3]  = vertGrid [vi + 0, vj + 1].texCoord[0];
                textControls [4]  = vertGrid [vi + 1, vj + 1].texCoord[0];
                textControls [5]  = vertGrid [vi + 2, vj + 1].texCoord[0];

                lightControls [3] = vertGrid [vi + 0, vj + 1].texCoord[1];
                lightControls [4] = vertGrid [vi + 1, vj + 1].texCoord[1];
                lightControls [5] = vertGrid [vi + 2, vj + 1].texCoord[1];

                //Bottom row
                vertControls [6]  = vertGrid [vi + 0, vj + 2].position;
                vertControls [7]  = vertGrid [vi + 1, vj + 2].position;
                vertControls [8]  = vertGrid [vi + 2, vj + 2].position;

                textControls [6]  = vertGrid [vi + 0, vj + 2].texCoord[0];
                textControls [7]  = vertGrid [vi + 1, vj + 2].texCoord[0];
                textControls [8]  = vertGrid [vi + 2, vj + 2].texCoord[0];

                lightControls [6] = vertGrid [vi + 0, vj + 2].texCoord[1];
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

                List<Vector3> p0s, p1s, p2s;
                List<Vector2> p0suv, p1suv, p2suv;
                //List<Vector2> p0suv2, p1suv2, p2suv2;

                p0s    = Tessellate  (vertControls  [0], vertControls  [3], vertControls  [6]);
                p0suv  = TessellateUV(textControls  [0], textControls  [3], textControls  [6]);
                //p0suv2 = TessellateUV(lightControls [0], lightControls [3], lightControls [6]);

                p1s    = Tessellate  (vertControls  [1], vertControls  [4], vertControls  [7]);
                p1suv  = TessellateUV(textControls  [1], textControls  [4], textControls  [7]);
                //p1suv2 = TessellateUV(lightControls [1], lightControls [4], lightControls [7]);

                p2s    = Tessellate  (vertControls  [2], vertControls  [5], vertControls  [8]);
                p2suv  = TessellateUV(textControls  [2], textControls  [5], textControls  [8]);
                //p2suv2 = TessellateUV(lightControls [2], lightControls [5], lightControls [8]);

                // Tessellate all those new sets of control points and pack
                // all the results into our vertex array, which we'll return.
                // Make our uvs list while we're at it.
                List<Vector3> points      = new List<Vector3> ();
                List<Vector2> texs        = new List<Vector2> ();
                //List<Vector2> lightCoords = new List<Vector2> ();

                for (int i = 0; i <= TESSELLATION_LEVEL; i++)
                {
                    points.AddRange      (Tessellate   (p0s    [i], p1s    [i], p2s    [i]));
                    texs.AddRange        (TessellateUV (p0suv  [i], p1suv  [i], p2suv  [i]));
                    //lightCoords.AddRange (TessellateUV (p0suv2 [i], p1suv2 [i], p2suv2 [i]));
                }

                // Reformat the tesselated points.
                for (int j = 0; j < points.Count; j++)
                {
                    Infrastructure.Math.Vertex point;
                    point.Position = points [j];
                    point.Normal   = Vector3.UnitZ;                  // FIXME: Calculate the actual normal for this surface at this point.
                    point.TexCoord = texs [j];
                    vertices.Add (point);
                }

                // This will produce (TESSELLATION_LEVEL + 1)^2 verts
                int numVerts = (TESSELLATION_LEVEL + 1) * (TESSELLATION_LEVEL + 1);

                // Compute triangle indexes for forming a mesh.
                // The mesh will be (TESSELLATION_LEVEL + 1) verts
                // wide and tall.
                int xStep = 1;
                int width = TESSELLATION_LEVEL + 1;

                for (int i = 0; i < (numVerts - width); i++)
                {
                    //on left edge
                    if (xStep == 1)
                    {
                        indices.Add (i);
                        indices.Add (i + width);
                        indices.Add (i + 1);

                        xStep++;
                    }

                    //on right edge
                    else if (xStep == width)
                    {
                        indices.Add (i);
                        indices.Add (i + (width - 1));
                        indices.Add (i + width);

                        xStep = 1;
                    }

                    // not on an edge, so add two
                    else
                    {
                        indices.Add (i);
                        indices.Add (i + (width - 1));
                        indices.Add (i + width);

                        indices.Add (i);
                        indices.Add (i + width);
                        indices.Add (i + 1);

                        xStep++;
                    }
                }

                // Add mesh to list of meshes.
                mesh.Vertices = vertices.ToArray ();
                mesh.Normals  = new Vector3[0];
                mesh.Textures = texCoords.ToArray ();
                mesh.Indices  = indices.ToArray ();
                mesh.tex      = GetTexture (face);
                meshes.Add (mesh);

                vertices.Clear ();
                indices.Clear ();
                vertices = null;
                indices = null;
            }

            return(meshes);
        }

        // This takes a tessellation level and three vector3
        // p0 is start, p1 is the midpoint, p2 is the endpoint
        // The returned list begins with p0, ends with p2, with
        // the tessellated verts in between.
        private List<Vector3> Tessellate(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            List<Vector3> vects = new List<Vector3>();

            float stepDelta = 1.0f / TESSELLATION_LEVEL;
            float step = stepDelta;

            vects.Add (p0);

            for (int i = 0; i < (TESSELLATION_LEVEL - 1); i++)
            {
                vects.Add (BezCurve (step, p0, p1, p2));
                step += stepDelta;
            }

            vects.Add (p2);

            return(vects);
        }

        // Same as above, but for UVs
        private List<Vector2> TessellateUV(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            List<Vector2> vects = new List<Vector2>();

            float stepDelta = 1.0f / TESSELLATION_LEVEL;
            float step = stepDelta;

            vects.Add (p0);

            for (int i = 0; i < (TESSELLATION_LEVEL - 1); i++)
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
            float a = 1.0f - t;
            float tt = t * t;

            Vector3 p0s = Vector3.Multiply (p0, a * a);
            Vector3 p1s = Vector3.Multiply (p1, 2 * a * t);
            Vector3 p2s = Vector3.Multiply (p2, tt);
            return(Vector3.Add (Vector3.Add (p0s, p1s), p2s));
        }

        // Calculate UVs for our tessellated vertices
        private Vector2 BezCurveUV(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float a = 1.0f - t;
            float tt = t * t;

            Vector2 p0s = Vector2.Multiply (p0, a * a);
            Vector2 p1s = Vector2.Multiply (p1, 2 * a * t);
            Vector2 p2s = Vector2.Multiply (p2, tt);
            return(Vector2.Add (Vector2.Add (p0s, p1s), p2s));
        }

        private Mesh RenderPolygon(Face.FaceEntry face)
        {
            List<Vector3> vertices  = new List<Vector3> ();
            List<Vector3> normals   = new List<Vector3> ();
            List<Vector2> texCoords = new List<Vector2> ();
            List<int>     indices   = new List<int> ();

           // Add all the vertexes for the face to the mesh.
            for (int vertex = face.vertex; vertex < (face.vertex + face.n_vertexes); vertex++)
            {
                Loader.BSP.Vertex.VertexEntry currentVertex = BSP.GetVertex (vertex);
                vertices.Add (currentVertex.position);
                normals.Add (currentVertex.normal);
                texCoords.Add (currentVertex.texCoord [0]);
//                lightCoords.Add (currentVertex.texCoord [1]);
            }

            // Add all the vertex indices for the face to the mesh.
            for (int meshVert = face.meshVert; meshVert < (face.meshVert + face.n_meshVerts); meshVert++)
            {
                // MeshVert offset is relative to the first vertex of the face.
                indices.Add (BSP.GetMeshVert (meshVert).offset);
            }

            Mesh mesh = new Mesh ();
            mesh.Vertices = vertices.ToArray ();
            mesh.Normals  = normals.ToArray ();
            mesh.Textures = texCoords.ToArray ();
            mesh.Indices  = indices.ToArray ();
            mesh.tex      = GetTexture (face);
            return(mesh);
         }

        private Infrastructure.Texture.Texture GetTexture(Face.FaceEntry face)
        {
            Infrastructure.Texture.Texture texture = null;

            string textureName = BSP.GetTexture (face.texture).name;
            string JPG = textureName + ".jpg";
            string TGA = textureName + ".tga";

            if (textureName.Contains ("noshader") == false)
            {
                if ((File.Exists (JPG) == true) && (texManager.Registered (JPG) == true))
                {
                    texture = texManager.Get (JPG);
                }

                else if ((File.Exists (TGA) == true) && (texManager.Registered (TGA) == true))
                {
                    texture = texManager.Get (TGA);
                }

                else if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered ("textures/notexture.jpg") == true))
                {
                    texture = texManager.Get ("textures/notexture.jpg");
                }
            }

            else if ((File.Exists ("textures/notexture.jpg") == true) && (texManager.Registered ("textures/notexture.jpg") == true))
            {
                texture = texManager.Get ("textures/notexture.jpg");
            }

            return (texture);
        }
    }
}

