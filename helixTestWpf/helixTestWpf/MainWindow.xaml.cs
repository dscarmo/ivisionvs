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
            

            //Test skeleton
            testSkeleton = new Skeleton3d();
            testSkeleton.generateDebugSkeleton();

            //Point Cloud
            testPoint = new PointsVisual3D();
            testPoint.Size = 3;

            //Origin Arrows
            arrow = new ArrowVisual3D[3] { new ArrowVisual3D(), new ArrowVisual3D(), new ArrowVisual3D() };

            arrow[0].Direction = new Vector3D(0, 0, 1);
            arrow[1].Direction = new Vector3D(0, 1, 0);
            arrow[2].Direction = new Vector3D(1, 0, 0);
           
            foreach (ArrowVisual3D a in arrow){
                a.Diameter = 0.1;
                a.HeadLength = 0.5;
                a.Origin = new Point3D(0, 0, 0);
            }

            //Limbs
            int pipeCount = 0;
            pipes = new PipeVisual3D[15];
            foreach (PipeVisual3D p in pipes)
            {
                pipes[pipeCount] = new PipeVisual3D();
                pipes[pipeCount].Diameter = 0.05;
                pipeCount++;
            }
            
            pointList = testSkeleton.points3d;

            pipes[0].Point1 = pointList[0];
            pipes[0].Point2 = pointList[1];

            hVp3D.Children.Add(testPoint);

            for (int i = 0; i < 3; i++)
                hVp3D.Children.Add(arrow[i]);

            for (int i = 0; i < 5;i++ )
                hVp3D.Children.Add(pipes[i]);
        }
       
        private void fly_Click_1(object sender, RoutedEventArgs e)
        {            
            testSkeleton.generateDebugSkeleton();
            testPoint.Points = testSkeleton.points3d;


            pipes[0].Point1 = testPoint.Points[0];
            pipes[0].Point2 = testPoint.Points[1];
            pipes[1].Point1 = testPoint.Points[1];
            pipes[1].Point2 = testPoint.Points[2];
            pipes[2].Point1 = testPoint.Points[2];
            pipes[2].Point2 = testPoint.Points[3];
            pipes[3].Point1 = testPoint.Points[3];
            pipes[3].Point2 = testPoint.Points[4];
            pipes[4].Point1 = testPoint.Points[4];
            pipes[4].Point2 = testPoint.Points[5];
        }

        private void connectDots()
        {

        }
    }
}
