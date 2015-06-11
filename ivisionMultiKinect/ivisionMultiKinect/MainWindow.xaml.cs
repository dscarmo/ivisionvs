
namespace multiKinect
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading;
    using Microsoft.Kinect;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Collection of connected Kinect Sensors
        KinectSensorCollection sensors = KinectSensor.KinectSensors;
        //Defines how many Kinects you want to work with. Set to false to use all kinects connected.
        const bool useLessKinects = true;
        const int howManyKinects = 4;
        //Back one month
        //Yan test

        #region Color Lists
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private List<WriteableBitmap> colorBitmap = new List<WriteableBitmap>();


        private List<DepthImagePixel[]> depthPixels = new List<DepthImagePixel[]>();
        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private List<byte[]> colorPixels = new List<byte[]>();

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
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
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
        int[] frameCount = new int[4] { 0, 0, 0, 0 };
        double[] r1, r2, r3, t1, t2, t3;

        Designer designer;
        bool updateAlive;
        int i, j, kid = 0;
        Joint hand1, hand2, hand3; //To be transformed to hand0;
        private Thread t_angleUpdate;
        Transformer transformer;
        RotateTransform3D[] xrotation = new RotateTransform3D[3]; 
        RotateTransform3D[] yrotation = new RotateTransform3D[3];
        RotateTransform3D[] zrotation = new RotateTransform3D[3];
        Point3D[] workingPoints = new Point3D[3];



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


            designer = new Designer(ref sensors);
            transformer = new Transformer();
            t_angleUpdate = new Thread(update);
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
                        kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                        kinect.SkeletonStream.Enable();

                        //Color allocations
                        // Allocate space to put the pixels we'll receive
                        this.colorPixels.Add(new byte[kinect.ColorStream.FramePixelDataLength]);

                        // This is the bitmap we'll display on-screen
                        this.colorBitmap.Add(new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null));


                        //Skeleton Allocations
                        // Create the drawing group we'll use for drawing
                        this.drawingGroup.Add(new DrawingGroup());

                        // Create images and setup events that cant be simply iterated
                        switch (kid)
                        {
                            case 0:
                                //img
                                this.Image0.Source = this.colorBitmap[0];
                                this.sensors[0].ColorFrameReady += this.SensorColorFrameReady0;
                                //ske
                                this.sensors[0].SkeletonFrameReady += this.SensorSkeletonFrameReady0;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[0]));
                                Ske0.Source = this.skeImageSource[0];
                                // hand0 = new Hand(0);
                                break;
                            case 1:
                                this.Image1.Source = this.colorBitmap[1];
                                this.sensors[1].ColorFrameReady += this.SensorColorFrameReady1;
                                //ske
                                this.sensors[1].SkeletonFrameReady += this.SensorSkeletonFrameReady1;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[1]));
                                Ske1.Source = this.skeImageSource[1];
                                // hand1 = new Hand(1);
                                break;
                            case 2:
                                this.Image2.Source = this.colorBitmap[2];
                                this.sensors[2].ColorFrameReady += this.SensorColorFrameReady2;
                                //ske
                                this.sensors[2].SkeletonFrameReady += this.SensorSkeletonFrameReady2;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[2]));
                                Ske2.Source = this.skeImageSource[2];
                                // hand2 = new Hand(2);
                                break;
                            case 3:
                                this.Image3.Source = this.colorBitmap[3];
                                this.sensors[3].ColorFrameReady += this.SensorColorFrameReady3;
                                //ske
                                this.sensors[3].SkeletonFrameReady += this.SensorSkeletonFrameReady3;
                                this.skeImageSource.Add(new DrawingImage(this.drawingGroup[3]));
                                Ske3.Source = this.skeImageSource[3];
                                // hand3 = new Hand(3);
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
                        catch (IOException)
                        {
                            Utils.msg("kinect id:" + kid + "failed to start.");
                        }
                    }
                }
                catch
                {
                    Utils.errorMsg();
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

        #region Color Events
        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 


        //Unique event-method
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e, int index)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels[index]);

                    // Write the pixel data into our bitmap
                    this.colorBitmap[index].WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap[index].PixelWidth, this.colorBitmap[index].PixelHeight),
                        this.colorPixels[index],
                        this.colorBitmap[index].PixelWidth * sizeof(int),
                        0);
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
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        designer.RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            designer.DrawBonesAndJoints(skel, dc, index);
                            if (Utils.checkCount(ref frameCount, index))
                            {
                                switch (index)
                                {
                                    case 0:
                                        Hand0.Text = Coordinates.stringfyPositions(skel.Joints[JointType.HandLeft]);
                                        break;
                                    case 1:
                                        hand1 = skel.Joints[JointType.HandLeft];
                                        //Hand1to0.Text = transformer.transformPoint(hand1, ref t1, ref r1);
                                        Hand1to0.Text = transformPoint1(hand1);
                                        Hand1.Text = Coordinates.stringfyPositions(hand1);
                                        break;
                                    case 2:
                                        hand2 = skel.Joints[JointType.HandLeft];
                                        //Hand2to0.Text = transformer.transformPoint(hand2, ref t2, ref r2);
                                        Hand2to0.Text = transformPoint2(hand2);
                                        Hand2.Text = Coordinates.stringfyPositions(hand2);
                                        break;
                                    case 3:
                                        hand3 = skel.Joints[JointType.HandLeft];
                                        //Hand3to0.Text = transformer.transformPoint(hand3, ref t3, ref r3);
                                        Hand3to0.Text = transformPoint3(hand3);
                                        Hand3.Text = Coordinates.stringfyPositions(hand3);
                                        break;

                                }
                            }


                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(skel.Position, index),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
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
        #endregion

        #region GUI_UPDATE


        public void update()
        {
            try
            {
                int[] angles = new int[4] { 0, 0, 0, 0 };
                while (updateAlive)
                {
                    //statusUpdate();
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
            //Thread Capturion = new Thread(new ParameterizedThreadStart(capturing));
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

            //t.Start();
            //return t;
            //Capturion.Start(int.Parse(qtd.Text), int.Parse(freq.Text));
        }

        private void save()
        {
            while (j < kid)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                // create frame from the writable bitmap and add to encoder
                encoder.Frames.Add(BitmapFrame.Create(colorBitmap[j]));

                string myPhotos = "C:\\Users\\Public\\Kinect Dataset";
                //Environment.SpecialFolder.MyPictures

                string path = "";
                if (j == 0)
                {
                    path = System.IO.Path.Combine(myPhotos, "cam0_image" + i +".png");
                }
                else
                {
                    if (j == 1)
                    {
                        path = System.IO.Path.Combine(myPhotos, "cam1_image" + i +".png");
                    }
                    else
                    {
                        if (j == 2)
                        {
                            path = System.IO.Path.Combine(myPhotos, "cam2_image" + i + ".png");
                        }
                        else
                        {
                            if (j==3)
                            {
                                path = System.IO.Path.Combine(myPhotos, "cam3_image" + i + ".png");
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
                catch (IOException)
                {
                    MessageBox.Show("Fail to Capture");
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
        private void updateParameters(object sender, RoutedEventArgs e)
        {
            try
            {
                t1[0] = Double.Parse(t11.Text);
                t1[1] = Double.Parse(t12.Text);
                t1[2] = Double.Parse(t13.Text);
                r1[0] = Double.Parse(r11.Text);
                r1[1] = Double.Parse(r12.Text);
                r1[2] = Double.Parse(r13.Text);

                t2[0] = Double.Parse(t21.Text);
                t2[1] = Double.Parse(t22.Text);
                t2[2] = Double.Parse(t23.Text);
                r2[0] = Double.Parse(r21.Text);
                r2[1] = Double.Parse(r22.Text);
                r2[2] = Double.Parse(r23.Text);

                t3[0] = Double.Parse(t31.Text);
                t3[1] = Double.Parse(t32.Text);
                t3[2] = Double.Parse(t33.Text);
                r3[0] = Double.Parse(r31.Text);
                r3[1] = Double.Parse(r32.Text);
                r3[2] = Double.Parse(r33.Text);
            }
            catch (Exception error)
            {
                Utils.errorReport(error);
            }
        }

        public String transformPoint1(Joint inJoint)
        {
            //Inputs
            double rx, ry, rz, tx, ty, tz;
            int i = 0;
            workingPoints[i] = new Point3D(inJoint.Position.X, inJoint.Position.Y, inJoint.Position.Z);
            

            tx = ty = tz = rx = ry = rz = 0;
            try
            {
                rx = Double.Parse(r11.Text);
                ry = Double.Parse(r12.Text);
                rz = Double.Parse(r13.Text);
                tx = Double.Parse(t11.Text);
                ty = Double.Parse(t12.Text);
                tz = Double.Parse(t13.Text);
            }
            catch (Exception) { }

            xrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(1, 0, 0), rx));
            yrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 1, 0), ry));
            zrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 0, 1), rz));

            workingPoints[i] = xrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = yrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = zrotation[i].Transform(workingPoints[i]);
            workingPoints[i].X = workingPoints[i].X + tx;
            workingPoints[i].Y = workingPoints[i].Y + ty;
            workingPoints[i].Z = workingPoints[i].Z + tz;
            
            

            return Coordinates.stringfyPositions(workingPoints[i]);

        }
        public String transformPoint2(Joint inJoint)
        {
            //Inputs
            double rx, ry, rz, tx, ty, tz;
            int i = 1;
            workingPoints[i] = new Point3D(inJoint.Position.X, inJoint.Position.Y, inJoint.Position.Z);


            tx = ty = tz = rx = ry = rz = 0;
            try
            {
                rx = Double.Parse(r21.Text);
                ry = Double.Parse(r22.Text);
                rz = Double.Parse(r23.Text);
                tx = Double.Parse(t21.Text);
                ty = Double.Parse(t22.Text);
                tz = Double.Parse(t23.Text);
            }
            catch (Exception) { }

            xrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(1, 0, 0), rx));
            yrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 1, 0), ry));
            zrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 0, 1), rz));

            workingPoints[i] = xrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = yrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = zrotation[i].Transform(workingPoints[i]);
            workingPoints[i].X = workingPoints[i].X + tx;
            workingPoints[i].Y = workingPoints[i].Y + ty;
            workingPoints[i].Z = workingPoints[i].Z + tz;



            return Coordinates.stringfyPositions(workingPoints[i]);
            
        }

        
        public String transformPoint3(Joint inJoint)
        {
            //Inputs
            double rx, ry, rz, tx, ty, tz;
            int i = 2;
            workingPoints[i] = new Point3D(inJoint.Position.X, inJoint.Position.Y, inJoint.Position.Z);


            tx = ty = tz = rx = ry = rz = 0;
            try
            {
                rx = Double.Parse(r31.Text);
                ry = Double.Parse(r32.Text);
                rz = Double.Parse(r33.Text);
                tx = Double.Parse(t31.Text);
                ty = Double.Parse(t32.Text);
                tz = Double.Parse(t33.Text);
            }
            catch (Exception) { }

            xrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(1, 0, 0), rx));
            yrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 1, 0), ry));
            zrotation[i] = new RotateTransform3D(new AxisAngleRotation3D(
                                  new Vector3D(0, 0, 1), rz));

            workingPoints[i] = xrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = yrotation[i].Transform(workingPoints[i]);
            workingPoints[i] = zrotation[i].Transform(workingPoints[i]);
            workingPoints[i].X = workingPoints[i].X + tx;
            workingPoints[i].Y = workingPoints[i].Y + ty;
            workingPoints[i].Z = workingPoints[i].Z + tz;



            return Coordinates.stringfyPositions(workingPoints[i]);

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


        #endregion
    }


}
