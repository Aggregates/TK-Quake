using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y) : this()
        {
            this.X = x;
            this.Y = y;
        }
    }
}
