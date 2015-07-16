using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ivisionMultiKinect
{
    class CompositeSkeleton 
    {
        /*
         * Constructor gets each transformed skeleton2d and calls private methods to generate the only and one skeleton.
         * 
         */
        private List<SkeletonPoint> pointList;
        private const int listSize = 16;

        public CompositeSkeleton(Skeleton2d ske1, Skeleton2d ske2, Skeleton2d ske3)
        {
            String pointName;
            this.pointList = new List<SkeletonPoint>();
            for (int i = 0; i < listSize; i++)
            {
               //TO DO
            }    
        }

        /*private Point3D pointAverage(Point3D p1, Point3D p2, Point3D p3){
            // TO DO
        }*/
    }
}
