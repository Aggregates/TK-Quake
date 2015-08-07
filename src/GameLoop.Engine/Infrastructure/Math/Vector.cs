using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Math
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector(double x, double y, double z) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector operator+ (Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator- (Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Cross Product
        /// </summary>
        /// <returns>Orthogonal Vector</returns>
        public static Vector operator* (Vector a, Vector b)
        {
            double x = (a.Y * b.Z) - (a.Z * b.Y);
            double y = (a.X * b.Z) - (a.Z * b.X);
            double z = (a.X * b.Y) - (a.Y * b.X);

            return new Vector(x, -y, z);
        }

        public double Dot(Vector b)
        {
            return (this.X * b.X) + (this.Y * b.Y) + (this.Z * b.Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                Vector other = (Vector)obj;
                return this.X == other.X &&
                    this.Y == other.Y &&
                    this.Z == other.Z;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
