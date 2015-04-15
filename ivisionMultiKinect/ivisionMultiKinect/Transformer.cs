using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
namespace multiKinect
{
    public class Transformer
    {

        public String transformPoint(Joint inJoint /*double[] inJoint*/, ref double[] translateVector, ref double[] R)
        {
            //Inputs
            double x, y, z, rx, ry, rz;
            //Outputs
            double X, Y, Z;

            System.Console.WriteLine(R[0]);
            //Convert angle to radian
            for (int i = 0; i < 3; i++)
            {
                //output[i] = inJoint[i] + translateVector[i];
                //routput[i] = Math.Sin(inJoint[i]);
                //Convert rotations to radians
                R[i] = deegresToRadians(R[i]);
            }

            //Coordinates as in the formula
            //x = inJoint[0];
            //y = inJoint[1];
            //z = inJoint[2];
            x = inJoint.Position.X;
            y = inJoint.Position.Y;
            z = inJoint.Position.Z;
            //Rotations as in the formula
            rx = R[0];
            ry = R[1];
            rz = R[2];

            //the final calculationnnnnn

            //X
            X = x * (Cos(rz) * Cos(ry)) + y * (Cos(rz) * Sin(ry) * Sin(rx) - Sin(rz) * Cos(rx)) +
                z * (Cos(rz) * Sin(ry) * Sin(rx) - Sin(rz) * Cos(rx)) + translateVector[0];

            Y = x * (Sin(rz) * Cos(ry)) + y * (Sin(rz) * Sin(ry) * Sin(rx) + Cos(rz) * Cos(rx)) +
                z * (Sin(rz) * Sin(ry) * Cos(rx) - Cos(rz) * Sin(rx)) + translateVector[1];

            Z = x * (-Sin(ry)) + y * (Cos(ry) * Sin(rx)) + z * (Cos(ry) * Cos(rx)) + translateVector[2];
            //

            return Coordinates.stringfyPositions(X, Y, Z);
        }

        private double deegresToRadians(double deegres)
        {
            return deegres * (Math.PI / 180);
        }

        private double Sin(double angle)
        {
            return Math.Sin(angle);
        }

        private double Cos(double angle)
        {
            return Math.Cos(angle);
        }
    }
}
