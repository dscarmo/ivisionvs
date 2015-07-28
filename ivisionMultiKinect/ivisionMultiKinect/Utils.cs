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
            if (frameCount[id]++ == 40)
            {
                frameCount[id] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
