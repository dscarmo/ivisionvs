using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace transformTest
{
    class Transformer
    {
        /// <summary>
        /// Translate a given coordinate by previously defined input offset
        /// Translated Vector = Original Vector + OffSet
        /// </summary>
        /// <param name="inJoint"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public void translatePoint(/*Joint inJoint*/ double[] inJoint, ref double[] offSet)
        {
            /*For final version
            double[] inVector = new double[3] { 0.0, 0.0, 0.0 };
            inVector[0] = inJoint.Position.X;
            inVector[1] = inJoint.Position.Y;
            inVector[2] = inJoint.Position.Z;
            */

            //For Debug
            for (int i = 0; i < 3; i++) {offSet[i] = offSet[i] + inJoint[i]; Console.WriteLine(offSet[i]);}
        
        }

        public void rotatePoint(double[] offSet, ref double[] rotatedPoint)
        {

        }
    }
}
