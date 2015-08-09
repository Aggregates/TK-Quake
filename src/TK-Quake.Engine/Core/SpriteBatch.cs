using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Core
{
    public class SpriteBatch
    {
        public const int MaxVertexNumber = 1000;
        private Vector[] _positions = new Vector[MaxVertexNumber];
        private Color[] _colors = new Color[MaxVertexNumber];
        private Point[] _uvs = new Point[MaxVertexNumber];
        private int _currentBatchSize = 0;

        private const int PositionDimensions = 3;
        private const int ColorDimensions = 4;
        private const int UVDimensions = 2;

        private Texture _texture;

        public void AddSprite(Sprite2 sprite)
        {

            // If the RenderBatch is full, render what is already there and
            // then start again
            if (sprite.Vertices.Count + _currentBatchSize > MaxVertexNumber || sprite.Texture.Id != _texture.Id)
            {
                Draw();
            }

            //  Add current Sprite to the batch
            for(int i = 0; i < sprite.Vertices.Count; i++)
            {
                _positions[_currentBatchSize + i] = sprite.Vertices[i].Position;
                _colors[_currentBatchSize + i] = sprite.Vertices[i].Color;
                _uvs[_currentBatchSize + i] = sprite.Vertices[i].UVs;
            }

            _texture = sprite.Texture;
            _currentBatchSize += sprite.Vertices.Count;
        }

        public void Draw()
        {
            if (_currentBatchSize == 0) return;

            // Setup the GL Pointers for drawin arrays
            SetupPointers();

            // Enable Transparency by Binding to the Texture
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _texture.Id);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.DrawArrays(PrimitiveType.Triangles, 0, _currentBatchSize); // Only draws up to current vertex

            _currentBatchSize = 0;
        }

        private void SetupPointers()
        {
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.VertexPointer(PositionDimensions, VertexPointerType.Double, 0, _positions);
            GL.ColorPointer(ColorDimensions, ColorPointerType.Float, 0, _colors);
            GL.TexCoordPointer(UVDimensions, TexCoordPointerType.Float, 0, _uvs);
        }
    }
}
