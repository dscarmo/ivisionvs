using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ivisionMultiKinect
{
    class SpeedCalculator
    {
        private double deltat;
        private bool isDt1;
        private double[] dt, dx, dy, dz;

        public SpeedCalculator()
        {
            this.deltat = 0;
            this.isDt1 = true;
            this.dt = new double[2] { 0, 0 };
            this.dx = new double[2] { 0, 0 };
            this.dy = new double[2] { 0, 0 };
            this.dz = new double[2] { 0, 0 };            
        }

        private double TimeDifferential()
        {
            if (isDt1)
            {
                dt[1] = Environment.TickCount;
                isDt1 = false;
            }
            else
            {
                dt[0] = Environment.TickCount;
                isDt1 = true;
            }

            if (isDt1)
                deltat = dt[0] - dt[1];
            else
                deltat = dt[1] - dt[0];

            return deltat;
        }

        private double CoordVelocity(double[] coord)
        {
            double aux, auy, auz, deltax, deltay, deltaz;
            double[] Velocidade = new double[3] { 0, 0, 0 };

            dx[1] = coord[0];
            dy[1] = coord[1];
            dz[1] = coord[2];

            aux = dx[1];
            dx[1] = dx[0];
            dx[0] = aux;

            auy = dy[1];
            dy[1] = dy[0];
            dy[0] = auy;

            auz = dz[1];
            dz[1] = dz[0];
            dz[0] = auz;

            deltax = dx[1] - dx[0];
            deltay = dy[1] - dy[0];
            deltaz = dz[1] - dz[0];

            Velocidade[0] = (deltax) / (deltat / 1000);
            Velocidade[1] = (deltay) / (deltat / 1000);
            Velocidade[2] = (deltaz) / (deltat / 1000);
            return Modulo(Velocidade);
        }

        private double Modulo(double[] vetor)
        {
            return (Math.Sqrt((vetor[0] * vetor[0]) + (vetor[1] * vetor[1]) + (vetor[2] * vetor[2])));
        }

        public double getTime(){
             return TimeDifferential();
        }

        public double calcSpeed(Point3D ponto)
        {
            double [] coord = new double[3] {ponto.X, ponto.Y, ponto.Z};
            return CoordVelocity(coord);
        }
    }
}
