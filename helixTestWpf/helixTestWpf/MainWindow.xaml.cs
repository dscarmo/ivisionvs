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
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Threading;


namespace helixTestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Point3D teaPos;
        private EllipsoidVisual3D esfera;
        private PointsVisual3D testPoint;
        private Skeleton3d testSkeleton;
        private ArrowVisual3D[] arrow;
        
        public MainWindow()
        {
            this.InitializeComponent();
            initializeThings();
        }

        private void initializeThings()
        { 
            esfera = new EllipsoidVisual3D();
            testPoint = new PointsVisual3D();
            testSkeleton = new Skeleton3d();
            arrow = new ArrowVisual3D[3] { new ArrowVisual3D(), new ArrowVisual3D(), new ArrowVisual3D() };
            
            testPoint.Size = 3;

            arrow[0].Direction = new Vector3D(0, 0, 10);
            arrow[1].Direction = new Vector3D(0, 10, 0);
            arrow[2].Direction = new Vector3D(10, 0, 0);

            hVp3D.Children.Add(esfera);
            hVp3D.Children.Add(testPoint);
            hVp3D.Children.Add(arrow[0]);
            hVp3D.Children.Add(arrow[1]);
            hVp3D.Children.Add(arrow[2]);
        }
       
        private void fly_Click_1(object sender, RoutedEventArgs e)
        {            
            testSkeleton.generateDebugSkeleton();
            testPoint.Points = testSkeleton.points3d;
            System.Console.WriteLine(teaPos.ToString());
            esfera.Center = NumberGenerator.getRngPoint();
            esfera.RadiusX = NumberGenerator.getDouble();
            esfera.RadiusY = NumberGenerator.getDouble();
            esfera.RadiusZ = NumberGenerator.getDouble();       
        }       
    }
}
