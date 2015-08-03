using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Texture
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float red, float green, float blue, float alpha) : this()
        {
            this.R = red;
            this.B = blue;
            this.G = green;
            this.A = alpha;
        }

        // Quick Colours
        public static Color Red   { get { return new Color(1f, 0f, 0f, 1f); } }
        public static Color Green { get { return new Color(0f, 1f, 0f, 1f); } }
        public static Color Blue  { get { return new Color(0f, 0f, 1f, 1f); } }
        public static Color Black { get { return new Color(0f, 0f, 0f, 1f); } }
        public static Color White { get { return new Color(1f, 1f, 1f, 1f); } }

    }
}
