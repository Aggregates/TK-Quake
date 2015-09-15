using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Abstract;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace TKQuake.Engine.Infrastructure.Texture
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

        public void Bind(string key)
        {
            var texture = this.Get(key);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
        }

        private Texture LoadTexture(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException(filename);

            // Create a texture and bind it to the ID
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            // Load the texture as a standard bitmap
            var bmp = (filename.EndsWith("tga")) ?
                Paloma.TargaImage.LoadTargaImage(filename) :
                new Bitmap(filename);
            var rect = new Rectangle(0,0,bmp.Width, bmp.Height);

            // Lock the data in memory while we create the texture
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            // Load the textue into OpenGL
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height,
                0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Unlock the data in memory
            bmp.UnlockBits(bmpData);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return new Texture(textureId, bmp.Width, bmp.Height, filename);
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
