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
        //private Point3D teaPos;
        private EllipsoidVisual3D esfera;
        private PointsVisual3D testPoint;
        private Skeleton3d testSkeleton;
        private List<Point3D> pointList;
        private ArrowVisual3D[] arrow;
        private PipeVisual3D[] pipes;
        
        public MainWindow()
        {
            this.InitializeComponent();
            initialize3D();
        }

        private void initialize3D()
        {
            int pipeCount = 0;
            //esfera = new EllipsoidVisual3D();

            //Test skeleton
            testSkeleton = new Skeleton3d();
            testSkeleton.generateDebugSkeleton();

            //Point Cloud
            testPoint = new PointsVisual3D();
            testPoint.Size = 3;

            //Origin Arrows
            arrow = new ArrowVisual3D[3] { new ArrowVisual3D(), new ArrowVisual3D(), new ArrowVisual3D() };

            arrow[0].Direction = new Vector3D(0, 0, 3);
            arrow[1].Direction = new Vector3D(0, 3, 0);
            arrow[2].Direction = new Vector3D(3, 0, 0);
           
            foreach (ArrowVisual3D a in arrow){
                a.Diameter = 0.3;
                a.HeadLength = 1;
                a.Origin = new Point3D(0, 0, 0);
            }

            //Limbs
            pipes = new PipeVisual3D[15];
            foreach (PipeVisual3D p in pipes)
            {
                pipes[pipeCount] = new PipeVisual3D();
                pipeCount++;
            }
            pointList = testSkeleton.points3d;




            pipes[0].Point1 = pointList[0];

            pipes[0].Point2 = pointList[1];


            //hVp3D.Children.Add(esfera);
            hVp3D.Children.Add(testPoint);
            hVp3D.Children.Add(arrow[0]);
            hVp3D.Children.Add(arrow[1]);
            hVp3D.Children.Add(arrow[2]);
            hVp3D.Children.Add(pipes[0]);
        }
       
        private void fly_Click_1(object sender, RoutedEventArgs e)
        {            
            testSkeleton.generateDebugSkeleton();
            testPoint.Points = testSkeleton.points3d;

            pipes[0].Point1 = testPoint.Points[0];

            pipes[0].Point2 = testPoint.Points[1];
            /*esfera.Center = NumberGenerator.getRngPoint();
            esfera.RadiusX = NumberGenerator.getDouble();
            esfera.RadiusY = NumberGenerator.getDouble();
            esfera.RadiusZ = NumberGenerator.getDouble();    */   
        }

        private void connectDots()
        {

        }
    }
}
