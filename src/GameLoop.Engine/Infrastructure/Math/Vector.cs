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
        public static Vector Zero { get { return new Vector(0, 0, 0); } }
        public static Vector UnitX { get { return new Vector(1, 0, 0); } }
        public static Vector UnitY { get { return new Vector(0, 1, 0); } }
        public static Vector UnitZ { get { return new Vector(0, 0, 1); } }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Length
        {
            get { return System.Math.Sqrt(this.LengthSquared()); }
        }


        public Vector(double x, double y, double z) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector Add(Vector v)
        {
            return new Vector(X + v.X, Y + v.Y, Z + v.Z);
        }

        public Vector Subtract(Vector v)
        {
            return new Vector(X - v.X, Y - v.Y, Z - v.Z);
        }

        public Vector CrossProduct(Vector v)
        {
            double x = (Y * v.Z) - (Z * v.Y);
            double y = (X * v.Z) - (Z * v.X);
            double z = (X * v.Y) - (Y * v.X);

            return new Vector(x, -y, z);
        }

        public double Dot(Vector b)
        {
            return (this.X * b.X) + (this.Y * b.Y) + (this.Z * b.Z);
        }

        /// <summary>
        /// Cross Product
        /// </summary>
        /// <returns>Orthogonal Vector</returns>
        public Vector Scale(double scale)
        {
            return new Vector(X * scale, Y * scale, Z * scale);
        }

        /// <summary>
        /// Gets the angle between two vectors in Radians
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The angle between this Vector and the other</returns>
        public double Angle(Vector v)
        {
            return System.Math.Acos(this.Dot(v) / this.Length / v.Length);
        }

        public void Normalise()
        {
            // Store length to avoid having to recalculate it
            double len = this.Length;

            if (len == 0) this = Vector.Zero;
            else this = new Vector(X / len, Y / len, Z / len );
        }

        public double LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public bool Equals(Vector v)
        {
           return this.X == v.X &&
                    this.Y == v.Y &&
                    this.Z == v.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
                return this.Equals((Vector)obj);
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Z;
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Z: {2}", X, Y, Z);
        }

        public static bool operator == (Vector v1, Vector v2)
        {
            if (System.Object.ReferenceEquals(v1, v2)) return true;
            if (v1 == null || v2 == null) return false;
            return v1.Equals(v2);
        }

        public static bool operator != (Vector v1, Vector v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector operator + (Vector a, Vector b)
        {
            return a.Add(b);
        }

        public static Vector operator - (Vector a, Vector b)
        {
            return a.Subtract(b);
        }

        public static Vector operator * (Vector a, Vector b)
        {
            return a.CrossProduct(b);
        }

        public static Vector operator * (Vector a, double scale)
        {
            return a.Scale(scale);
        }
    }
}
