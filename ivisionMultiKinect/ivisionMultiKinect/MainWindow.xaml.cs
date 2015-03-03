namespace multiKinect
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <Kinect Collection>
        /// Collection of connected Kinect Sensors
        /// </Kinect Collection>
        KinectSensorCollection sensors = KinectSensor.KinectSensors;

        #region Color Lists
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private List<WriteableBitmap> colorBitmap = new List<WriteableBitmap>();

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

        // Hand hand0, hand1, hand2, hand3;
        int[] frameCount = new int[4] { 0, 0, 0, 0 };
        Designer designer;

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
            statusUpdate();
            int kid = 0;
            designer = new Designer(ref sensors);
            foreach (var kinect in sensors)
            {
                try
                {
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


                        // Start the sensor!
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
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Utils.debugMsg("Stoping Kinects...");
            Status.Text = "Stoping Kinects...";
            foreach (var kinect in sensors)
                if (null != kinect)
                    kinect.Stop();
            Utils.debugMsg("Done.");
        }


        #region Color Events
        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady0(object sender, ColorImageFrameReadyEventArgs e)
        {
            int index = 0;
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

        private void SensorColorFrameReady1(object sender, ColorImageFrameReadyEventArgs e)
        {
            int index = 1;
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


        private void SensorColorFrameReady2(object sender, ColorImageFrameReadyEventArgs e)
        {
            int index = 2;
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


        private void SensorColorFrameReady3(object sender, ColorImageFrameReadyEventArgs e)
        {
            int index = 3;
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
        #endregion

        #region Skeleton Events
        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady0(object sender, SkeletonFrameReadyEventArgs e)
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

            using (DrawingContext dc = this.drawingGroup[0].Open())
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
                            designer.DrawBonesAndJoints(skel, dc, 0);
                            if (checkCount(ref frameCount, 0))
                            {
                                Hand0.Text = Coordinates.stringfyPositions(skel.Joints[JointType.HandLeft]);
                            }


                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(skel.Position, 0),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }

                    // prevent drawing outside of our render area
                    this.drawingGroup[0].ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                }
            }
        }



        private void SensorSkeletonFrameReady1(object sender, SkeletonFrameReadyEventArgs e)
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

            using (DrawingContext dc = this.drawingGroup[1].Open())
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
                            designer.DrawBonesAndJoints(skel, dc, 1);
                            if (checkCount(ref frameCount, 1))
                            {
                                Hand2.Text = Coordinates.stringfyPositions(skel.Joints[JointType.HandLeft]); //pog here to...
                            }
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(skel.Position, 1),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }

                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup[1].ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        private void SensorSkeletonFrameReady2(object sender, SkeletonFrameReadyEventArgs e)
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

            using (DrawingContext dc = this.drawingGroup[2].Open())
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
                            designer.DrawBonesAndJoints(skel, dc, 2);
                            if (checkCount(ref frameCount, 2))
                            {
                                Hand1.Text = Coordinates.stringfyPositions(skel.Joints[JointType.HandLeft]); //to here
                            }
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(skel.Position, 2),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }

                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup[2].ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        private void SensorSkeletonFrameReady3(object sender, SkeletonFrameReadyEventArgs e)
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

            using (DrawingContext dc = this.drawingGroup[3].Open())
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
                            designer.DrawBonesAndJoints(skel, dc, 3);
                            if (checkCount(ref frameCount, 3))
                            {
                                Hand3.Text = Coordinates.stringfyPositions(skel.Joints[JointType.HandLeft]);
                            }
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            designer.SkeletonPointToScreen(skel.Position, 3),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }


                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup[3].ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }
        #endregion

        #region AUX
        private void statusMod(int breaks, String s, String option)
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

        private void statusUpdate()
        {
            int id = 0;
            foreach (var kinect in sensors)
            {
                statusMod(1, "Kinect" + id + " status: " + kinect.Status, "add");
                id++;
            }
        }

        public bool checkCount(ref int[] frameCount, int id)
        {
            if (frameCount[id]++ == 10)
            {
                frameCount[id] = 0;
                return true;
            }
            else
                return false;
        }

        #endregion


    }
}
