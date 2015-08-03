using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Texture
{
    public struct Texture
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Path { get; set; }

        public Vector Center
        {
            get
            {
                double halfWidth = Width / 2;
                double halfHeight = Height / 2;
                double depth = 0;

                return new Vector(halfWidth, halfHeight, depth);
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
