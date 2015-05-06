using System;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace multiKinect
{
    class Coordinates
    {
        public static String stringfyPositions(Joint joint)
        {
            return "x: " + joint.Position.X.ToString("F2") + " | y: " + joint.Position.Y.ToString("F2") + " | z: " + joint.Position.Z.ToString("F2");
        }

        public static String stringfyPositions(double X, double Y, double Z)
        {
            //System.Console.WriteLine(0.14 * (Math.Cos(0) * Math.Cos(Math.PI / 2)) - 0.01 * (Math.Cos(0) * Math.Sin(Math.PI / 2) * Math.Sin(0) - Math.Sin(0) * Math.Cos(0)) + 2.3 * (Math.Cos(0) * Math.Sin(Math.PI / 2) * Math.Sin(0) - Math.Sin(0) * Math.Cos(0)));
            return "x: " + X.ToString("F4") + "|y: " + Y.ToString("F4") + "|z: " + Z.ToString("F4");
        }

        public static String stringfyPositions(Point3D p)
        {
            return "x: " + p.X.ToString("F2") + " | y: " + p.Y.ToString("F2") + " | z: " + p.Z.ToString("F2");
        }
    }
}