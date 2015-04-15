using System;
using Microsoft.Kinect;

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
            return "x: " + X.ToString("F2") + " | y: " + Y.ToString("F2") + " | z: " + Z.ToString("F2");
        }
    }
}