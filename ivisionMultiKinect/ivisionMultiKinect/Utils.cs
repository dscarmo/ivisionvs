using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace multiKinect
{
    class Utils
    {


        public static void errorMsg()
        {
            MessageBoxResult counted = MessageBox.Show("Ocorreu um erro ao inicializar os kinects, por favor desconecte e reconecte os cabos USB e reinicie o programa.");
        }

        public static void msg(String s)
        {
            MessageBoxResult counted = MessageBox.Show(s);
        }

        public static void debugMsg(String s)
        {
            System.Console.WriteLine(s);
        }
    }
}
