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
            double[] inJoint, translateVector, rotation, output, routput;
            Transformer transformer = new Transformer(  );
            try { 
                inJoint = new double[3] { Double.Parse(x.Text), Double.Parse(y.Text), Double.Parse(z.Text) };
                translateVector = new double[3] { Double.Parse(t1.Text), Double.Parse(t2.Text), Double.Parse(t3.Text) };
                rotation = new double[3] { Double.Parse(r1.Text), Double.Parse(r2.Text), Double.Parse(r3.Text) };
                output = new double[3] { 0.0, 0.0, 0.0 };
                routput = new double[3] { 0.0, 0.0, 0.0 };

                transformer.transformPoint(inJoint, translateVector, rotation, ref output);
                xl.Text = output[0].ToString("0.##");
                yl.Text = output[1].ToString("0.##");
                zl.Text = output[2].ToString("0.##");



            } catch (Exception) {
                MessageBox.Show("Insira coordenadas nas entradas.");
            }

            //pROCRUST DISTANCE
        }

        

        
    }
    }

