using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;

namespace helixTestWpf
{
    class NumberGenerator
    {
        private static Random rng = new Random();

        public static Point3D getRngPoint()
        {
            double db1 = rng.NextDouble() * 10;
            double db2 = rng.NextDouble() * 10;
            double db3 = rng.NextDouble() * 10;
            return new Point3D(db1, db2, db3);
        }

        public static double getDouble()
        {
            return rng.NextDouble();
        }

    }
}
