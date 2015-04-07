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
            //MessageBox.Show(e.Message);
            AutoClosingMessageBox.Show(e.Message, "Error", 2000);
        }
    }
}
