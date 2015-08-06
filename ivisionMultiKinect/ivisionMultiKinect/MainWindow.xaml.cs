
namespace multiKinect
{

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading;
    using Microsoft.Kinect;
    using System.Windows.Media.Media3D;
    using ivisionMultiKinect;
    using HelixToolkit;
    using HelixToolkit.Wpf;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Collection of connected Kinect Sensors
        KinectSensorCollection sensors = KinectSensor.KinectSensors;

        //Defines how many Kinects you want to work with. Set to false to use all kinects connected.
        const bool useLessKinects = true;
        const int howManyKinects = 3;


        #region Color Lists
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private List<WriteableBitmap> colorBitmap = new List<WriteableBitmap>();
        private List<WriteableBitmap> depthBitmap = new List<WriteableBitmap>();

        private List<DepthImagePixel[]> depthPixels = new List<DepthImagePixel[]>();
        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private List<byte[]> colorPixels = new List<byte[]>();
        private List<byte[]> depthColorPixels = new List<byte[]>();
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        /// 
        #endregion

        #region Skeleton Consts
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly System.Windows.Media.Brush centerPointBrush = System.Windows.Media.Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly System.Windows.Media.Brush trackedJointBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly System.Windows.Media.Brush inferredJointBrush = System.Windows.Media.Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly System.Windows.Media.Pen trackedBonePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly System.Windows.Media.Pen inferredBonePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gray, 1);
        #endregion

        #region Skeleton Lists
        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private List<DrawingGroup> drawingGroup = new List<DrawingGroup>();
        //private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>

        private List<DrawingImage> skeImageSource = new List<DrawingImage>();
        //private DrawingImage imageSource;
        #endregion

        #region Aux Vars
        
        //Lag FIX
        int[] frameCount = new int[4] { 0, 0, 0, 0 };

        //Skeleton
        Designer designer;
        CompositeSkeleton compSkeleton;
        Skeleton2d skeleton0, skeleton1, skeleton2, skeleton3;

        //Angle Thread 
        bool updateAlive;
        
        //Stream
        int streamChoosing = 2; //initialize in IR
        
        //????
        int i, j, kid = 0;
        
        private Thread t_angleUpdate;
        
        //Transform things
        RotateTransform3D[] xrotation = new RotateTransform3D[3]; 
        RotateTransform3D[] yrotation = new RotateTransform3D[3];
        RotateTransform3D[] zrotation = new RotateTransform3D[3];
        bool transformON;
        Point3D[] workingPoints = new Point3D[3];
        
        //Transform Parsing Containers
        double[] r1, r2, r3, t1, t2, t3;

        //3D Skeleton
        private PointsVisual3D testPoint;
        private ArrowVisual3D[] arrow;
        private PipeVisual3D[] pipes;
        private List<int[]> vetores;
        private EllipsoidVisual3D head3d;

        #endregion

        #region Initialization and Closure
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            
            t_angleUpdate = new Thread(update);
            designer = new Designer(ref sensors);
            compSkeleton = new CompositeSkeleton();
            skeleton0 = new Skeleton2d();
            skeleton1 = new Skeleton2d();
            skeleton2 = new Skeleton2d();
            skeleton3 = new Skeleton2d();
            initialize3D();

            r1 = new double[3] { 0.0, 0.0, 0.0 };
            r2 = new double[3] { 0.0, 0.0, 0.0 };
            r3 = new double[3] { 0.0, 0.0, 0.0 };
            t1 = new double[3] { 0.0, 0.0, 0.0 };
            t2 = new double[3] { 0.0, 0.0, 0.0 };
            t3 = new double[3] { 0.0, 0.0, 0.0 };

            foreach (var kinect in sensors)
            {

                try
                {
                    if (useLessKinects) if (kid == howManyKinects) break;
                    
                    if (null != kinect)
                    {

                        // Turn on the color and ske stream to receive frames

                        kinect.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
                        kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                        
                        kinect.SkeletonStream.Enable();

                        this.depthColorPixels.Add(new byte[kinect.DepthStream.FramePixelDataLength * sizeof(int)]);
                        
                        //Color allocations
                        // Allocate space to put the pixels we'll receive

                        this.colorPixels.Add(new byte[kinect.ColorStream.FramePixelDataLength]);

                        // This is the bitmap we'll display on-screen
                        //this.colorBitmap.Add(new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null));
                        this.colorBitmap.Add(new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Gray16, null));
                        this.depthBitmap.Add(new WriteableBitmap(kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null));

                       

                        //Depth Allocation
                        this.depthPixels.Add(new DepthImagePixel[kinect.DepthStream.FramePixelDataLength]);

                        //Skeleton Allocations
                        // Create the drawing group we'll use for drawing
                        this.drawingGroup.Add(new DrawingGroup());

                        // Create images and setup events that cant be simply iterated
                        switch (kid)
                        {
                            case 0:
                                //rgb/ir
                                this.Image0.Source = this.colorBitmap[0];
                                this.sensors[0].ColorFrameReady += this.SensorColorFrameReady0;
                                //ske
                                this.sensors[0].SkeletonFrameReady += this.SensorSkeletonFrameReady0;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[0]));
                                Ske0.Source = this.skeImageSource[0];
                                //Depth
                                this.sensors[0].DepthFrameReady += this.SensorDepthFrameReady0;
                                break;
                            case 1:
                                //rgb/ir
                                this.Image1.Source = this.colorBitmap[1];
                                this.sensors[1].ColorFrameReady += this.SensorColorFrameReady1;
                                //ske
                                this.sensors[1].SkeletonFrameReady += this.SensorSkeletonFrameReady1;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[1]));
                                Ske1.Source = this.skeImageSource[1];
                                //Depth
                                this.sensors[1].DepthFrameReady += this.SensorDepthFrameReady1;
                                break;
                            case 2:
                                //rgb/ir
                                this.Image2.Source = this.colorBitmap[2];
                                this.sensors[2].ColorFrameReady += this.SensorColorFrameReady2;
                                //ske
                                this.sensors[2].SkeletonFrameReady += this.SensorSkeletonFrameReady2;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[2]));
                                Ske2.Source = this.skeImageSource[2];
                                //Depth
                                this.sensors[2].DepthFrameReady += this.SensorDepthFrameReady2;
                                break;     
                            default:
                                Utils.msg("wrong number in kID");
                                break;
                        }
                        kid++;


                        // Start the sensor and angle threads
                        try
                        {
                            if (kinect.Status == KinectStatus.Connected)
                                kinect.Start();

                        }
                        catch (IOException error)
                        {
                            Utils.msg("kinect id:" + kid + "failed to start: " + error.Message );
                           
                        }
                    }
                }
                catch (Exception error)
                {
                    Utils.errorMsg(error);
                    continue;
                }

                if (null == kinect)
                {
                    Utils.msg("kinect id" + kid + "not available.");
                }
            }
            updateAlive = true;
            t_angleUpdate.Start();

        }
        
        private void switchShowing(int kid)
        {
            if (streamChoosing == 1)
            {
                switch (kid)
                {
                    case 0:
                        this.Image0.Source = this.depthBitmap[0];
                        break;
                    case 1:
                        this.Image1.Source = this.depthBitmap[1];
                        break;
                    case 2:
                        this.Image2.Source = this.depthBitmap[2];
                        break;
                    default:
                        break;
                }
            }
            else if (streamChoosing == 2)
            {
                switch (kid)
                {
                    case 0: this.Image0.Source = this.colorBitmap[0];
                        break;
                    case 1: this.Image1.Source = this.colorBitmap[1];
                        break;
                    case 2: this.Image2.Source = this.colorBitmap[2];
                        break;
                    default: 
                        break;
                }
            }
            else
            { }
        }

        private void switchbt_Click(object sender, RoutedEventArgs e)
        {
            
            if (streamChoosing == 1) //ir
            {
                streamChoosing++;
                Utils.debugMsg("switching to depth");
                for (int i = 0; i < kid; i++)
                {
                    switchShowing(i);
                }
                
            }
            else if (streamChoosing == 2) //DEPTH
            {
                streamChoosing = 0;
                Utils.debugMsg("turning stream off, click switch again to turn on");     
            }
            else {
                Utils.debugMsg("switching to ir");
                streamChoosing = 1;
                for (int i = 0; i < kid; i++)
                {
                    switchShowing(i);
                } 
            }
            
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updateAlive = false;
            Thread.Sleep(600);
            Utils.debugMsg("Stoping Kinects...");
            foreach (var kinect in sensors)
                if (null != kinect)
                    kinect.Stop();
            Utils.debugMsg("Done.");

        }
        #endregion

        #region Depth Events


        private DepthColorizer colorizer = new DepthColorizer();
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e, int index)
        {
            if (streamChoosing == 1)
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame != null)
                    {
                        
                        // Copy the pixel data from the image to a temporary array
                        depthFrame.CopyDepthImagePixelDataTo(this.depthPixels[index]);

                        // Get the min and max reliable depth for the current frame
                        int minDepth = depthFrame.MinDepth;
                        int maxDepth = depthFrame.MaxDepth;

                        colorizer.ConvertDepthFrame(depthPixels[index], minDepth, maxDepth, 0, depthColorPixels[index]); //todo



                        // Write the pixel data into our bitmap
                        this.depthBitmap[index].WritePixels(
                            new Int32Rect(0, 0, this.depthBitmap[index].PixelWidth, this.depthBitmap[index].PixelHeight),
                            this.depthColorPixels[index],
                            this.depthBitmap[index].PixelWidth * sizeof(int),
                            0);
                    }
                }
            }
        }

       


        private void SensorDepthFrameReady0(object sender, DepthImageFrameReadyEventArgs e)
        {
            SensorDepthFrameReady(sender, e, 0);
        }

        private void SensorDepthFrameReady1(object sender, DepthImageFrameReadyEventArgs e)
        {
            SensorDepthFrameReady(sender, e, 1);
        }

        private void SensorDepthFrameReady2(object sender, DepthImageFrameReadyEventArgs e)
        {
            SensorDepthFrameReady(sender, e, 2);
        }

        private void SensorDepthFrameReady3(object sender, DepthImageFrameReadyEventArgs e)
        {
            SensorDepthFrameReady(sender, e, 3);
        }
        #endregion

        #region Color Events
        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 


        
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e, int index)
        {

            if (streamChoosing == 2)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame != null)
                    {
                        // Copy the pixel data from the image to a temporary array
                        

                        colorFrame.CopyPixelDataTo(this.colorPixels[index]);

                        this.colorBitmap[index].WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap[index].PixelWidth, this.colorBitmap[index].PixelHeight),
                        this.colorPixels[index],
                        this.colorBitmap[index].PixelWidth * colorFrame.BytesPerPixel,
                        0);




                    }
                }
            }
        }
 

        private void SensorColorFrameReady0(object sender, ColorImageFrameReadyEventArgs e)
        {
            SensorColorFrameReady(sender, e, 0);
        }

        private void SensorColorFrameReady1(object sender, ColorImageFrameReadyEventArgs e)
        {
            SensorColorFrameReady(sender, e, 1);
        }

        private void SensorColorFrameReady2(object sender, ColorImageFrameReadyEventArgs e)
        {
            SensorColorFrameReady(sender, e, 2);
        }

        private void SensorColorFrameReady3(object sender, ColorImageFrameReadyEventArgs e)
        {
            SensorColorFrameReady(sender, e, 3);
        }
        #endregion

        #region Skeleton Events
        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 

        //Unique Event-method
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e, int index)
        {
            Skeleton[] skeletons = new Skeleton[0];
            
            Skeleton usingSkeleton = new Skeleton();

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup[index].Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(System.Windows.Media.Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            usingSkeleton = skel;
                    }
                        designer.RenderClippedEdges(usingSkeleton, dc);

                        if (usingSkeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {

                            designer.DrawBonesAndJoints(usingSkeleton, dc, index);
                            if (Utils.checkCount(ref frameCount, index))
                            {
                                switch (index)
                                {
                                    case 0:
                                        skeleton0 = new Skeleton2d(usingSkeleton);
                                        Skel0.Text = skeleton0.getStringPoints();
                                        break;
                                    case 1:
                                        skeleton1 = new Skeleton2d(usingSkeleton);
                                        Skel1.Text = skeleton1.getStringPoints();
                                        Skel1to0.Text = transformSkeleton(1);
                                        break;
                                    case 2:
                                        skeleton2 = new Skeleton2d(usingSkeleton);
                                        Skel2.Text = skeleton2.getStringPoints();
                                        Skel2to0.Text = transformSkeleton(2);
                                        break;
                                    case 3:
                                        break;

                                }

                                if (skeleton0.safeToGetSkeleton() & skeleton1.safeToGetTransform() & skeleton2.safeToGetTransform())
                                {
                                    compSke.Text = compSkeleton.calculateCompositeSkeleton(skeleton0.getPointList(), skeleton1.getTransformedList(), skeleton2.getTransformedList());
                                    update3D();
                                }
                                else
                                    compSke.Text = "Not safe to compose Skeleton";
                                

                            }


                        }
                        else if (usingSkeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(usingSkeleton.Position, index),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    

                    // prevent drawing outside of our render area
                    this.drawingGroup[index].ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                }
            }
        }

        private void SensorSkeletonFrameReady0(object sender, SkeletonFrameReadyEventArgs e)
        {
            SensorSkeletonFrameReady(sender, e, 0);
        }



        private void SensorSkeletonFrameReady1(object sender, SkeletonFrameReadyEventArgs e)
        {
            SensorSkeletonFrameReady(sender, e, 1);
        }

        private void SensorSkeletonFrameReady2(object sender, SkeletonFrameReadyEventArgs e)
        {
            SensorSkeletonFrameReady(sender, e, 2);
        }

        private void SensorSkeletonFrameReady3(object sender, SkeletonFrameReadyEventArgs e)
        {
            SensorSkeletonFrameReady(sender, e, 3);
        }

        private void stringStream_Click(object sender, RoutedEventArgs e)
        {
            Skeleton2d.triggerStringStream();
        }
        #endregion
        
        #region GUI_UPDATE


        public void update()
        {
            try
            {
                int[] angles = new int[4] { 0, 0, 0, 0 };
                while (updateAlive)
                {
                    for (int i = 0; i < sensors.Count; i++)
                    {
                        if (sensors[i].IsRunning)
                            angles[i] = sensors[i].ElevationAngle;
                    }
                    a0.Dispatcher.Invoke(new Action<int>(a => a0.Text = a.ToString()), angles[0]);
                    a1.Dispatcher.Invoke(new Action<int>(a => a1.Text = a.ToString()), angles[1]);
                    a2.Dispatcher.Invoke(new Action<int>(a => a2.Text = a.ToString()), angles[2]);
                    a3.Dispatcher.Invoke(new Action<int>(a => a3.Text = a.ToString()), angles[3]);

                    Thread.Sleep(500);
                }
                Utils.debugMsg("Update thread terminating...");
            }
            catch (Exception e)
            {
                Utils.errorReport(e);
            }


        }
        #endregion

        #region Tilting
        private void applyAngle(object sender, RoutedEventArgs e)
        {
            int kinect, ang;

            try
            {
                kinect = Int32.Parse(kinectChooser.Text) - 1;
                ang = Int32.Parse(angle.Text);
                if (ang < sensors[kinect].MaxElevationAngle && ang > sensors[kinect].MinElevationAngle)
                    tilt(sensors[kinect], ang);
                else
                    MessageBox.Show("Ângulo muito pequeno ou muito grande!");

            }
            catch (Exception error)
            {
                MessageBox.Show("Erro capturado: " + error.Message + ", coloque valores válidos nas caixas de texto.");
            }

        }



        //Tilting method
        public void tilt(KinectSensor sensor, int ang)
        {
            sensor.ElevationAngle = ang;
            Thread.Sleep(100);
        }

        private void resetAngles(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < sensors.Count; i++)
            {
                if (sensors[i].IsRunning)
                    sensors[i].ElevationAngle = 0;
            }
        }

        private void changeKinectChooser(bool plus)
        {
            try
            {
                int kinect = Int32.Parse(kinectChooser.Text);

                if (plus)
                {

                    if ((kinect > sensors.Count - 1) | (kinect < 1))
                        Utils.msg("Kinect com esse numero não encontrado.");
                    else
                    {
                        kinect++;
                        kinectChooser.Text = (kinect).ToString();
                    }
                }
                else
                {
                    if ((kinect > sensors.Count) | (kinect < 2))
                        Utils.msg("Kinect com esse numero não encontrado.");
                    else
                    {
                        kinect--;
                        kinectChooser.Text = (kinect).ToString();
                    }
                }
                if ((kinect < sensors.Count + 1) & (kinect > 0))
                    angle.Text = sensors[kinect - 1].ElevationAngle.ToString();

            }
            catch (Exception error)
            {
                Utils.errorReport(error);
            }
        }

        private void changeAngle(bool plus)
        {
            try
            {
                int angl = Int32.Parse(angle.Text);

                if (plus)
                {

                    if (angl > 26)
                        Utils.msg("Não é possível aumentar mais que 27.");
                    else
                        angle.Text = (angl + 2).ToString();
                }
                else
                {
                    if (angl < -26)
                        Utils.msg("Não é possível diminuir mais que -27.");
                    else
                        angle.Text = (angl - 2).ToString();
                }
            }
            catch (Exception error)
            {
                Utils.errorReport(error);
            }
        }

        private void chooserPlus(object sender, RoutedEventArgs e)
        {
            changeKinectChooser(true);
        }


        private void chooserMinus(object sender, RoutedEventArgs e)
        {
            changeKinectChooser(false);
        }

        private void anglePlus(object sender, RoutedEventArgs e)
        {
            changeAngle(true);
        }

        private void angleMinus(object sender, RoutedEventArgs e)
        {
            changeAngle(false);
        }
        #endregion

        #region Screenshot
        private void Capture(object sender, RoutedEventArgs e)
        {
            int x = int.Parse(qtd.Text);
            int z = int.Parse(freq.Text);
            try
            {
                int[] pass = new int[2];
                pass[0] = int.Parse(qtd.Text);
                pass[1] = int.Parse(freq.Text);
                var t = new Thread(() => capturing(x, z));
                t.Start();
            }
            catch
            {
                MessageBox.Show("Insira valores válidos na frequência e quantidade de imagens!");
            }


        }

        private void save()
        {
            while (j < kid)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                // create frame from the writable bitmap and add to encoder
                if (streamChoosing == 2)
                {
                    encoder.Frames.Add(BitmapFrame.Create(colorBitmap[j]));
                }
                else if (streamChoosing == 1 | streamChoosing == 3)
                {
                    encoder.Frames.Add(BitmapFrame.Create(depthBitmap[j]));
                }
                //string dataset = "C:\\Users\\Public\\Kinect_Dataset\\Datatest";
                string dataset = "C:\\dataset";
                //Environment.SpecialFolder.MyPictures

                string path = "";
                if (j == 0)
                {
                    path = System.IO.Path.Combine(dataset, "cam0_image" + i +".png");
                }
                else
                {
                    if (j == 1)
                    {
                        path = System.IO.Path.Combine(dataset, "cam1_image" + i +".png");
                    }
                    else
                    {
                        if (j == 2)
                        {
                            path = System.IO.Path.Combine(dataset, "cam2_image" + i + ".png");
                        }
                        else
                        {
                            if (j==3)
                            {
                                path = System.IO.Path.Combine(dataset, "cam3_image" + i + ".png");
                            }
                        }
                    }
                }
                
                // write the new file to disk
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (IOException er)
                {
                    MessageBox.Show("Fail to Capture \n" + er.Message);
                    break;
                }
                j++;
            }

        }

        private void contador()
        {
            int cont = int.Parse(qtd.Text);
            cont--;
            qtd.Text = cont.ToString();
        }

        private void capturing(int x, int z)
        {
            i = 0;
            while (i < x)
            {
                Thread.Sleep(z);
                j = 0;
                Dispatcher.Invoke(save);
                Dispatcher.Invoke(contador);
                i++;
            }

        }
        #endregion

        #region Transform

        public String transformSkeleton(int callingKinect)
        {

           
            //Inputs
            double rx, ry, rz, tx, ty, tz;
            tx = ty = tz = rx = ry = rz = 0;

            if (transformON)
            {
                switch (callingKinect)
                {
                    case 1:
                        try
                        {
                            rx = Utils.radToDegree(Double.Parse(r11.Text));
                            ry = Utils.radToDegree(Double.Parse(r12.Text));
                            rz = Utils.radToDegree(Double.Parse(r13.Text));
                            tx = Double.Parse(t11.Text) / 1000;
                            ty = Double.Parse(t12.Text) / 1000;
                            tz = Double.Parse(t13.Text) / 1000;
                        }
                        catch (Exception e)
                        {
                            Utils.debugMsg(e.Message);
                        }

                        skeleton1.generateTransformedList(rx, ry, rz, tx, ty, tz);
                        return skeleton1.getTransformedStringPoints();
                    case 2:
                        try
                        {
                            rx = Utils.radToDegree(Double.Parse(r21.Text));
                            ry = Utils.radToDegree(Double.Parse(r22.Text));
                            rz = Utils.radToDegree(Double.Parse(r23.Text));
                            tx = Double.Parse(t21.Text)/1000;
                            ty = Double.Parse(t22.Text)/1000;
                            tz = Double.Parse(t23.Text)/1000;
                        }
                        catch (Exception e)
                        {
                            Utils.debugMsg(e.Message);
                        }

                        skeleton2.generateTransformedList(rx, ry, rz, tx, ty, tz);
                        return skeleton2.getTransformedStringPoints();
                    
                    default:
                        
                        return "error in transform";
                        
                        
                }
            }

            else return "transform disabled or error ocurred";
            

        }

        
        private double deegresToRadians(double deegres)
        {
            return deegres * (Math.PI / 180);
        }

        private double Sin(double angle)
        {
            return Math.Sin(angle);
        }

        private double Cos(double angle)
        {
            return Math.Cos(angle);
        }

        private void transformToggle(object sender, RoutedEventArgs e)
        {
            transformON = !transformON;
            transformbtn.Content = transformON ? "Transform: ON" : "Transform OFF";
        }

        #endregion

        #region CompositeSkeleton
       

        private void initialize3D()
        {

            testPoint = new PointsVisual3D();
            arrow = new ArrowVisual3D[3] { new ArrowVisual3D(), new ArrowVisual3D(), new ArrowVisual3D() };
            pipes = new PipeVisual3D[15];
            head3d = new EllipsoidVisual3D();
            vetores = new List<int[]>();
            
            //Arrows
            arrow[0].Direction = new Vector3D(0, 0, 1);
            arrow[1].Direction = new Vector3D(0, 1, 0);
            arrow[2].Direction = new Vector3D(1, 0, 0);
            foreach (ArrowVisual3D a in arrow)
            {
                a.Diameter = 0.1;
                a.HeadLength = 0.5;
            }

            //Pipes
            int pipeCount = 0;
            foreach (PipeVisual3D p in pipes)
            {
                pipes[pipeCount] = new PipeVisual3D();
                pipes[pipeCount].Diameter = 0.05;
                pipeCount++;
            }
            vetores.Add(new int[2] { 0, 2 });
            vetores.Add(new int[2] { 2, 1 });
            vetores.Add(new int[2] { 2, 3 });
            vetores.Add(new int[2] { 1, 4 });
            vetores.Add(new int[2] { 4, 6 });
            vetores.Add(new int[2] { 3, 5 });
            vetores.Add(new int[2] { 5, 7 });
            vetores.Add(new int[2] { 2, 8 });
            vetores.Add(new int[2] { 8, 10 });
            vetores.Add(new int[2] { 10, 9 });
            vetores.Add(new int[2] { 10, 11 });
            vetores.Add(new int[2] { 9, 12 });
            vetores.Add(new int[2] { 12, 14 });
            vetores.Add(new int[2] { 11, 13 });
            vetores.Add(new int[2] { 13, 15 });
            toggleVisibilityPipes(false);

            //Points
            testPoint.Size = 5;

            //Left Hand
            head3d.RadiusX = head3d.RadiusY = head3d.RadiusZ = 0.1;


            hVp3D.Children.Add(testPoint);
            hVp3D.Children.Add(head3d);
            for (int i = 0; i < 3; i++ )
                hVp3D.Children.Add(arrow[i]);

            for (int i = 0; i < 15; i++ )
                hVp3D.Children.Add(pipes[i]);


           

            //Camera
            hVp3D.CameraController.CameraUpDirection = new Vector3D(0, 1, 0);
            hVp3D.CameraController.CameraPosition = new Point3D(0, 0, 5);
            hVp3D.CameraController.CameraLookDirection = new Vector3D(0, 0, -1);
            
        }

        bool pipeOk = false;
        private void toggleVisibilityPipes(bool b)
        {
            foreach (PipeVisual3D p in pipes)
                p.Visible = b;            
        }
    
        private void update3D()
        {
            testPoint.Points = compSkeleton.points3d;

            head3d.Center = testPoint.Points[0];
            hVp3D.CameraController.CameraTarget = testPoint.Points[10];
            
            for (int i = 0; i < 15; i++)
            {
                pipes[i].Point1 = testPoint.Points[vetores[i][0]];
                pipes[i].Point2 = testPoint.Points[vetores[i][1]];
            }

            if (!pipeOk)
            {
                toggleVisibilityPipes(true);
                pipeOk = true;
            }
         

        }       

        #endregion


    }
}
