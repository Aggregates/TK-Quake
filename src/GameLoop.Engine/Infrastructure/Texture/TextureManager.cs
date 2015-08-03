using GameLoop.Engine.Infrastructure.Abstract;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Texture
{
    public class TextureManager : ResourceManager<Texture>, IDisposable
    {

        public TextureManager() : base()
        {
        }

        public void Add(string textureName, string filename)
        {
            Texture text = LoadTexture(filename);
            base.Add(textureName, text);
        }

        public override void Add(string key, Texture data)
        {
            base.Add(key, data);
        }

        public override Texture Get(string key)
        {
            return base.Get(key);
        }

        public override bool Registered(string key)
        {
            return base.Registered(key);
        }

        public override void Remove(string key)
        {
            Texture text = Get(key);

            base.Remove(key);
            GL.DeleteTexture(text.Id);
        }

        private Texture LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename) || Path.GetFullPath(filename) == null )
                throw new ArgumentException(filename);

            // Create a texture and bind it to the ID
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Load the texture as a standard bitmap
            Bitmap bmp = (filename.Contains("tga")) ?
                Paloma.TargaImage.LoadTargaImage(filename) :
                new Bitmap(filename);
            Rectangle rect = new Rectangle(0,0,bmp.Width, bmp.Height);

            // Lock the data in memory while we create the texture
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            // Load the textue into OpenGL
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            // Unlock the data in memory
            bmp.UnlockBits(bmpData);

            Texture text = new Texture(textureId, bmp.Width, bmp.Height, filename);
            return text;
        }

        public void Dispose()
        {
            foreach(string keyVal in Database.Keys)
            {
                Remove(keyVal);
            }
        }
    }
}
