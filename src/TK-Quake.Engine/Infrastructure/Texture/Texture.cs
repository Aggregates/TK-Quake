using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Texture
{
    public struct Texture
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Path { get; set; }

        public Vector3 Center
        {
            get
            {
                float halfWidth = Width / 2;
                float halfHeight = Height / 2;
                float depth = 0;

                return new Vector3(halfWidth, halfHeight, depth);
            }
        }

        public Texture(int id, int width, int height, string path) : this()
        {
            this.Id = id;
            this.Width = width;
            this.Height = height;
            this.Path = path;
        }
    }
}
