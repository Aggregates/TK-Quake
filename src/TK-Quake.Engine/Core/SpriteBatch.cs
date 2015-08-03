using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Engine.Core
{
    public class SpriteBatch
    {
        public const int MaxVertexNumber = 1000;
        private Vertex[] _vertices = new Vertex[MaxVertexNumber];
        private int _currentBatchSize = 0;

        public void AddSprite(Sprite2 sprite)
        {
            
            // If the RenderBatch is full, render what is already there and
            // then start again
            if (sprite.Vertices.Count + _currentBatchSize > MaxVertexNumber)
            {
                Draw();
            }

            //  Add current Sprite to the batch
            for(int i = 0; i < sprite.Vertices.Count; i++)
            {
                _vertices[_currentBatchSize + 1] = sprite.Vertices[i];
            }

            _currentBatchSize += sprite.Vertices.Count;
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
