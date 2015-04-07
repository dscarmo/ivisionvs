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

        public static bool checkCount(ref int[] frameCount, int id)
        {
            if (frameCount[id]++ == 10)
            {
                frameCount[id] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Deprecated
        /*Deprectated
        public void statusMod(int breaks, String s, String option)
        {
            switch (option)
            {
                case "add":
                    for (int i = 0; i < breaks; i++)
                        s = System.Environment.NewLine + s;
                    Status.Text += s;
                    break;
                case "clear":
                    Status.Text = "";
                    break;
                default:
                    Utils.msg("Fatal error: wrong message in statusMod");
                    break;
            }

        }

        public void statusUpdate()
        {
            int id = 0;
            foreach (var kinect in sensors)
            {
                statusMod(1, "Kinect" + id + " status: " + kinect.Status, "add");
                id++;
            }
        }

        */
        #endregion
    }
}
