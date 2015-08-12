using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ivisionMultiKinect;
namespace multiKinect
{
    class Utils
    {

        public static void errorMsg()
        {
            MessageBox.Show("Ocorreu um erro ao inicializar os kinects, por favor desconecte e reconecte os cabos USB e reinicie o programa.");
        }

        public static void errorMsg(Exception e)
        {
            MessageBox.Show("Ocorreu um erro ao inicializar os kinects, por favor desconecte e reconecte os cabos USB e reinicie o programa. \n" + e.Message);
        }

        public static void msg(String s)
        {
            MessageBox.Show(s);
        }

        public static void debugMsg(String s)
        {
            System.Console.WriteLine(s);
        }

        public static void errorReport(Exception e)
        {
            MessageBox.Show(e.Message);
        }

        public static bool checkCount(ref int[] frameCount, int id)
        {
            if (frameCount[id]++ == 1)
            {
                frameCount[id] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double radToDegree(double rad)
        {
            return rad * 180 / Math.PI;
        }

        private static double[] generateSinCos(double i)
        {
            double[] result = new double[2] { 0.0, 0.0 };
            result[0] = 5*Math.Sin(i);
            result[1] = 5 * Math.Cos(i) + 2;
            return result;
        }

        public static List<double[]> generateCircle()
        {
            List<double[]> circulo = new List<double[]>();

            for (double i = 0; i <= 6.28; i = i + 0.02)
                circulo.Add(generateSinCos(i));
      
            return circulo;
        }
    }
}
