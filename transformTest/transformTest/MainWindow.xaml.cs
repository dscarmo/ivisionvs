using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace transformTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         
                translate();
           
        }

        private void translate()
        {
            double[] inJoint, translateVector;
            Transformer transformer = new Transformer();
            try { 
                inJoint = new double[3] { Double.Parse(x.Text), Double.Parse(y.Text), Double.Parse(z.Text) };
                translateVector = new double[3] { Double.Parse(t1.Text), Double.Parse(t2.Text), Double.Parse(t3.Text) };
                transformer.translatePoint(inJoint, ref translateVector);
                xl.Text = translateVector[0].ToString();
                yl.Text = translateVector[1].ToString();
                zl.Text = translateVector[2].ToString();
            } catch (Exception) {
                MessageBox.Show("Insira coordenadas nas entradas.");
            }
        }

        

        
    }
    }

