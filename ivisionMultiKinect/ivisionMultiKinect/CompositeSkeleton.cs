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
        public List<Point3D> points3d {get;set;}

        public String calculateCompositeSkeleton(List<SkeletonPoint> ske1, List<SkeletonPoint> ske2, List<SkeletonPoint> ske3)
        {
            /* 
            *  Get 3 already transformed skeletons lists and calculates the average point, adding
            * then to pointList.
            */
            String pointName;
            Point3D averagePoint;
            this.pointList = new List<SkeletonPoint>();
            this.points3d = new List<Point3D>();
           // int i = 0;
            for (int i = 0; i < listSize; i++)
            {
                pointName = ske1[i].getJointName();
                averagePoint = getAveragePoint(ske1[i].getPoint(), ske2[i].getPoint(), ske3[i].getPoint());
                points3d.Add(amplifyPoint(10, averagePoint));
                pointList.Add(new SkeletonPoint(pointName, averagePoint));
            }

            return this.getStringPoints();
        }


        public Point3D getAveragePoint(Point3D p1, Point3D p2, Point3D p3)
        {
            //P1 has more impact in the average due to more reliability
            double x = (2*p1.X + p2.X + p3.X)/5;
            double y = (2*p1.Y + p2.Y + p3.Y)/5;
            double z = (2*p1.Z + p2.Z + p3.Z)/5;
            return new Point3D(x, y, z);
        }

        private Point3D amplifyPoint(int x, Point3D point)
        {
            return new Point3D(point.X*x, point.Y*x, point.Z*x);
        }

        public String getStringPoints()
        {
            if (Skeleton2d.stringEnable)
            {
                StringBuilder sbuilder = new StringBuilder();
                for (int i = 0; i < listSize; i++)
                {
                    sbuilder.Append(pointList[i].getFormatedString());
                }
                return sbuilder.ToString();
            }
            else
                return "String stream disabled.";
        }


        
    }
}
