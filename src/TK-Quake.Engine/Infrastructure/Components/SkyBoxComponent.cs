using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class SkyBoxComponent : IComponent
    {
        // Takes a size, takes a string related to the sky type
        private int[] skybox = new int[6];
        private int skySelection = 0;

        public void Startup()
        {
            chooseSky(skySelection);                   // Change this to alter which sky you see. Only 0 or 1 at the moment.
        }

        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            float[] difamb = { 1.0f, 0.5f, 0.3f, 1.0f };
            double size = 1000;

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, skybox[1]);
            GL.Begin(PrimitiveType.Quads);
            //back face
            //GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.End();


            //left face
            GL.BindTexture(TextureTarget.Texture2D, skybox[0]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.End();

            //front face
            GL.BindTexture(TextureTarget.Texture2D, skybox[3]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();

            //right face
            GL.BindTexture(TextureTarget.Texture2D, skybox[2]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(1.0, 0.0, 0.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();

            //top face
            GL.BindTexture(TextureTarget.Texture2D, skybox[4]);
            GL.Begin(PrimitiveType.Quads);
            GL.Normal3(0.0, 1.0, 0.0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(size / 2, size / 2, size / 2);
            GL.TexCoord2(0, 0);
            GL.Vertex3(-size / 2, size / 2, size / 2);
            GL.TexCoord2(0, 1);
            GL.Vertex3(-size / 2, size / 2, -size / 2);
            GL.TexCoord2(1, 1);
            GL.Vertex3(size / 2, size / 2, -size / 2);
            GL.End();

            //bottom face
            GL.BindTexture(TextureTarget.Texture2D, skybox[3]);
            GL.Begin(PrimitiveType.Quads);
            //GL.Normal3(0.0, -1.0, 0.0);
            GL.Vertex3(size / 2, -size / 2, size / 2);
            GL.Vertex3(-size / 2, -size / 2, size / 2);
            GL.Vertex3(-size / 2, -size / 2, -size / 2);
            GL.Vertex3(size / 2, -size / 2, -size / 2);
            GL.End();
        }

        int loadTexture(string filename)  //load the filename named texture
        {

            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // Increase or decrease the size of the texture
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Repeat the pixels in the edge of the texture, it will hide that 1px wide line at the edge of the cube
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Clamp);

            // Create bmp
            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Make texture... at least that's what I think this does...
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            return id;     //and we return the id
        }

        //load all of the textures, to the skybox array
        private void chooseSky(int choice)
        {
            string selection;
            switch (choice)
            {
                case 0: selection = "anotherworld"; break;
                case 1: selection = "space"; break;
                default: selection = ""; break;
            }

            // Location of textures *root of build* / skybox / selection / position.bmp
            skybox[0] = loadTexture(Path.Combine("skybox", selection, "left.bmp"));
            skybox[1] = loadTexture(Path.Combine("skybox", selection, "back.bmp"));
            skybox[2] = loadTexture(Path.Combine("skybox", selection, "right.bmp"));
            skybox[3] = loadTexture(Path.Combine("skybox", selection, "front.bmp"));
            skybox[4] = loadTexture(Path.Combine("skybox", selection, "top.bmp"));
            skybox[5] = loadTexture(Path.Combine("skybox", selection, "top.bmp")); // bottom?
        }
    }
}
