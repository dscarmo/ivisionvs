using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace helixTestWpf
{
    class Skeleton3d
    {
        public List<Point3D> points3d { get; set; }
      

        public Skeleton3d()
        {
            this.points3d = new List<Point3D>();
            this.generateDebugSkeleton();
        }

        public void generateDebugSkeleton()
        {
            this.points3d = new List<Point3D>();
            for (int i = 0; i < 16; i++)
                points3d.Add(NumberGenerator.getRngPoint());
        }


        
    }

}
