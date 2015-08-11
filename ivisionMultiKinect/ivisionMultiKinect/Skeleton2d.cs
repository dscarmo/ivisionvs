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
        private List<SkeletonPoint> transformedPointList;
        public static Boolean stringEnable = false;
        private static RotateTransform3D xrotation;
        private static RotateTransform3D yrotation;
        private static RotateTransform3D zrotation;
        private bool safeGet = false;
        private bool safeGetTransform = false;

        private const int listSize = 16;

        public Skeleton2d(Skeleton skel)
        {
            this.skel = skel;
            this.pointList = new List<SkeletonPoint>();
            this.transformedPointList = new List<SkeletonPoint>();
            this.setPoints();
        }

        public Skeleton2d() {
            this.pointList = new List<SkeletonPoint>();
            this.transformedPointList = new List<SkeletonPoint>();
        }

        public String getStringPoints()
        {
            if (Skeleton2d.stringEnable & this.safeGet)
            {
                StringBuilder sbuilder = new StringBuilder();
                for (int i = 0; i < listSize; i++)
                {
                    sbuilder.Append(pointList[i].getFormatedString());
                }
                return sbuilder.ToString();
            }
            else
                return "String stream disabled, or skeleton is null.";
        }

        public String getTransformedStringPoints()
        {
            StringBuilder sbuilder = new StringBuilder();
            for (int i = 0; i < listSize; i++)
            {
                sbuilder.Append(transformedPointList[i].getFormatedStringTransform());
            }
            return sbuilder.ToString();
        }

        public bool safeToGetSkeleton()
        {
            return this.safeGet;
        }

        public bool safeToGetTransform()
        {
            return this.safeGetTransform;
        }

        public List<SkeletonPoint> getPointList()
        {
            if (this.safeGet)
                return pointList;
            else
                return null;
        }

        public List<SkeletonPoint> getTransformedList()
        {
            if (this.safeGet)
                return transformedPointList;
            else
                return null;
        }

        private void setPoints()
        {
            pointList.Add(new SkeletonPoint("head", jointToPoint3D(skel.Joints[JointType.Head]))); //0
            pointList.Add(new SkeletonPoint("shoulder_left", jointToPoint3D(skel.Joints[JointType.ShoulderLeft]))); //1
            pointList.Add(new SkeletonPoint("shoulder_center", jointToPoint3D(skel.Joints[JointType.ShoulderCenter])));//2
            pointList.Add(new SkeletonPoint("shoulder_right", jointToPoint3D(skel.Joints[JointType.ShoulderRight])));//3
            pointList.Add(new SkeletonPoint("elbow_left", jointToPoint3D(skel.Joints[JointType.ElbowLeft])));//4
            pointList.Add(new SkeletonPoint("elbow_right", jointToPoint3D(skel.Joints[JointType.ElbowRight])));//5
            pointList.Add(new SkeletonPoint("hand_left", jointToPoint3D(skel.Joints[JointType.WristLeft])));//6
            pointList.Add(new SkeletonPoint("hand_right", jointToPoint3D(skel.Joints[JointType.WristRight])));//7
            pointList.Add(new SkeletonPoint("spine", jointToPoint3D(skel.Joints[JointType.Spine])));//8
            pointList.Add(new SkeletonPoint("hip_left", jointToPoint3D(skel.Joints[JointType.HipLeft])));//9
            pointList.Add(new SkeletonPoint("hip_center", jointToPoint3D(skel.Joints[JointType.HipCenter])));//10
            pointList.Add(new SkeletonPoint("hip_right", jointToPoint3D(skel.Joints[JointType.HipRight])));//11
            pointList.Add(new SkeletonPoint("knee_left", jointToPoint3D(skel.Joints[JointType.KneeLeft])));//12
            pointList.Add(new SkeletonPoint("knee_right", jointToPoint3D(skel.Joints[JointType.KneeRight])));//13
            pointList.Add(new SkeletonPoint("ankle_left", jointToPoint3D(skel.Joints[JointType.AnkleLeft])));//14
            pointList.Add(new SkeletonPoint("ankle_right", jointToPoint3D(skel.Joints[JointType.AnkleRight])));//15
            this.safeGet = true;
        }

        public void generateTransformedList(double rx, double ry, double rz, double tx, double ty, double tz)
        {
            /*
             *Goes trough the list of points transforming each one of then following the received parameters and
             *adding in transformedListPoint
             */
            xrotation = new RotateTransform3D(new AxisAngleRotation3D(
                                      new Vector3D(1, 0, 0), rx));
            yrotation = new RotateTransform3D(new AxisAngleRotation3D(
                                      new Vector3D(0, 1, 0), ry));
            zrotation = new RotateTransform3D(new AxisAngleRotation3D(
                                      new Vector3D(0, 0, 1), rz));

            //Loop creating list here
            for (int i = 0; i < listSize; i++)
            {
                transformedPointList.Add(new SkeletonPoint(pointList[i].getJointName(), 
                                         transformPoint(rx, ry, rz, tx, ty, tz, pointList[i].getPoint())));
            }
            //
            this.safeGetTransform = true;
        }

        public Point3D transformPoint(double rx, double ry, double rz, double tx, double ty, double tz, Point3D inPoint)
        {
            Point3D workingPoint = new Point3D(inPoint.X, inPoint.Y, inPoint.Z);

            workingPoint = xrotation.Transform(workingPoint);
            workingPoint = yrotation.Transform(workingPoint);
            workingPoint = zrotation.Transform(workingPoint);
            workingPoint.X = workingPoint.X + tx;
            workingPoint.Y = workingPoint.Y + ty;
            workingPoint.Z = workingPoint.Z + tz;

            return workingPoint;
        }
        
        

        public static void triggerStringStream()
        {
            stringEnable = !stringEnable;
        }

       

        

        private Point3D jointToPoint3D(Joint inJoint)
        {
            Point3D output = new Point3D(inJoint.Position.X, inJoint.Position.Y, inJoint.Position.Z);
            return output;
        }

            
      

    }
}
