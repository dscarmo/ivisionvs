﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ivisionMultiKinect
{
    class SkeletonPoint
    {
        private String jointName;
        private Point3D point;
        

        public SkeletonPoint(String jointName, Point3D point)
        {
            this.jointName = jointName;
            this.point = point;
        }

        

        public String getFormatedString()
        {
            Point3D p = this.getPoint();
            return this.getJointName() + ": x: " + p.X.ToString("F2") + " | y: " + p.Y.ToString("F2") + " | z: " + p.Z.ToString("F2") + " - ";
            
        }

        public Point3D getPoint(){
            return this.point;
        }
        
        public String getJointName(){
            return this.jointName;
        }
    }
}
