using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOcean
{
    public struct Vector
    {
        public readonly int x;
        public readonly int y;

        public static Vector up
        {
            get { return new Vector(0, 1); }
        }

        public static Vector down
        {
            get { return new Vector(0, -1); }
        }

        public static Vector left
        {
            get { return new Vector(-1, 0); }
        }

        public static Vector right
        {
            get { return new Vector(1, 0); }
        }

        public static Vector[] cardinals
        {
            get { return new Vector[] { Vector.up, Vector.down, Vector.left, Vector.right }; }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Vector))
                return false;
            Vector v = (Vector)obj;
            return x == v.x && y == v.y;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }

        public override string ToString()
        {
            return "Vector (" + x + ", " + y + ")";
        }

        public int sqrMagnitude
        {
            // https://docs.unity3d.com/ScriptReference/Vector-sqrMagnitude.html
            get { return (x * x) + (y * y); }
        }

        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
