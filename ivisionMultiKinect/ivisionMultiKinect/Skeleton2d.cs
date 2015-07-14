using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace ivisionMultiKinect
{
    class Skeleton2d
    {
        private Skeleton skel;
        /*private Point3D head, left_shoulder, neck, right_shoulder, left_elbow, right_elbow, left_hand,
                        right_hand, spine, center, left_center, right_center, left_leg, right_leg, 
                        left_foot, right_foot;*/
        private List<SkeletonPoint> pointList;
        private static Boolean stringEnable = false;

        const int listSize = 16;

        public Skeleton2d(Skeleton skel)
        {
            this.skel = skel;
            this.pointList = new List<SkeletonPoint>();
        }

        public static void triggerStringStream()
        {
            stringEnable = !stringEnable;
        }

        public String getStringPoints (){
            if (stringEnable)
            {
                StringBuilder sbuilder = new StringBuilder();
                this.setPoints();
                for (int i = 0; i < listSize; i++)
                {
                    sbuilder.Append(pointList[i].getFormatedString());
                }
                return sbuilder.ToString();
            }
            else
                return "String stream disabled.";
        }

        private void setPoints()
        {
            pointList.Add(new SkeletonPoint("head", jointToPoint3D(skel.Joints[JointType.Head])));
            pointList.Add(new SkeletonPoint("shoulder left",jointToPoint3D(skel.Joints[JointType.ShoulderLeft])));
            pointList.Add(new SkeletonPoint("shoulder center",jointToPoint3D(skel.Joints[JointType.ShoulderCenter])));
            pointList.Add(new SkeletonPoint("shoulder right",jointToPoint3D(skel.Joints[JointType.ShoulderRight])));
            pointList.Add(new SkeletonPoint("elbow left",jointToPoint3D(skel.Joints[JointType.ElbowLeft])));
            pointList.Add(new SkeletonPoint("elbow right",jointToPoint3D(skel.Joints[JointType.ElbowRight])));
            pointList.Add(new SkeletonPoint("hand left",jointToPoint3D(skel.Joints[JointType.HandLeft])));
            pointList.Add(new SkeletonPoint("hand right",jointToPoint3D(skel.Joints[JointType.HandRight])));
            pointList.Add(new SkeletonPoint("spine",jointToPoint3D(skel.Joints[JointType.Spine])));
            pointList.Add(new SkeletonPoint("hip left",jointToPoint3D(skel.Joints[JointType.HipLeft])));
            pointList.Add(new SkeletonPoint("hip center",jointToPoint3D(skel.Joints[JointType.HipCenter])));
            pointList.Add(new SkeletonPoint("hip right",jointToPoint3D(skel.Joints[JointType.HipRight])));
            pointList.Add(new SkeletonPoint("knee left",jointToPoint3D(skel.Joints[JointType.KneeLeft])));
            pointList.Add(new SkeletonPoint("knee right",jointToPoint3D(skel.Joints[JointType.KneeRight])));
            pointList.Add(new SkeletonPoint("ankle left",jointToPoint3D(skel.Joints[JointType.AnkleLeft])));
            pointList.Add(new SkeletonPoint("ankle right",jointToPoint3D(skel.Joints[JointType.AnkleRight])));
        }

        private Point3D jointToPoint3D(Joint inJoint)
        {
            Point3D output = new Point3D(inJoint.Position.X, inJoint.Position.Y, inJoint.Position.Z);
            return output;
        }

            
      //TODO create points 3d to storage all skeleton joints

    }
}
