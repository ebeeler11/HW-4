using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSV_Assignment
    //Eric Beeler
{
    public partial class Form1 : Form
    {
        //initialize bars
        private int _HmaxBar = 33;
        private int _HminBar = 17;
        private int _SmaxBar = 255;
        private int _SminBar = 93;
        private int _VmaxBar = 255;
        private int _VminBar = 151;
        private int _BinaryTrackBar = 150;

        private VideoCapture _capture;
        private Thread _captureThread;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //setup values and webcam
            _capture = new VideoCapture(0);
            _captureThread = new Thread(DisplayWebcam);
            _captureThread.Start();
            BinaryTrackBar.Value = _BinaryTrackBar;
            HminBar.Value = _HminBar;
            HmaxBar.Value = _HmaxBar;
            SminBar.Value = _SminBar;
            SmaxBar.Value = _SmaxBar;
            VminBar.Value = _VminBar;
            HmaxBar.Value = _VmaxBar;

   
        }
       

        private void DisplayWebcam()
        {
            while (_capture.IsOpened)
            {
                Mat frame = _capture.QueryFrame();
                // create new box size
                int newHeight = (frame.Size.Height * RawPictureBox.Size.Width) / frame.Size.Width;
                Size newSize = new Size(RawPictureBox.Size.Width, newHeight);
                CvInvoke.Resize(frame, frame, newSize);

                RawPictureBox.Image = frame.Bitmap;
                // convert to and display binary box
                Mat grayFrame = new Mat();
                Mat binaryFrame = new Mat();
                CvInvoke.CvtColor(frame, grayFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.Threshold(grayFrame, binaryFrame, _BinaryTrackBar, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
               
                BinaryPictureBox.Image = binaryFrame.Bitmap;

                Mat hsvFrame = new Mat();
                CvInvoke.CvtColor(frame, hsvFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
                //setup hsv
                Mat[] hsvChannels = hsvFrame.Split();
                //complete h,s,and v channels and their respective boxes
                Mat hueFilter = new Mat();
                CvInvoke.InRange(hsvChannels[0], new ScalarArray(_HminBar), new ScalarArray(_HmaxBar), hueFilter);
                Invoke(new Action(() => { HpictureBox.Image = hueFilter.Bitmap; }));

                Mat saturationFilter = new Mat();
                CvInvoke.InRange(hsvChannels[1], new ScalarArray(_SminBar), new ScalarArray(_SmaxBar), saturationFilter);
                Invoke(new Action(() => { SpictureBox.Image = saturationFilter.Bitmap; }));

                Mat valueFilter = new Mat();
                CvInvoke.InRange(hsvChannels[2], new ScalarArray(_VminBar), new ScalarArray(_VmaxBar), valueFilter);
                Invoke(new Action(() => { VpictureBox.Image = valueFilter.Bitmap; }));
                //assemble into combined image
                Mat combinedImage = new Mat();
                CvInvoke.BitwiseAnd(hueFilter, saturationFilter, combinedImage);
                CvInvoke.BitwiseAnd(combinedImage, valueFilter, combinedImage);
                Invoke(new Action(() => { RedPictureBox.Image = combinedImage.Bitmap; }));
            }
        }

        private void Form1_FormClosing(object sender, FormClosedEventArgs e)
        {
            _captureThread.Abort();
        }

        //Set up scroll bars
        private void BinaryTrackBar_Scroll(object sender, EventArgs e)
        {
            _BinaryTrackBar = BinaryTrackBar.Value;
        }

        private void HminBar_Scroll(object sender, EventArgs e)
        {
            _HminBar = HminBar.Value;
        }

        private void HmaxBar_Scroll(object sender, EventArgs e)
        {
            _HmaxBar = HmaxBar.Value;
        }

        private void SminBar_Scroll(object sender, EventArgs e)
        {
            _SminBar = SminBar.Value;
        }

        private void SmaxBar_Scroll(object sender, EventArgs e)
        {
            _SmaxBar = SmaxBar.Value;

        }

        private void VminBar_Scroll(object sender, EventArgs e)
        {
            _VminBar = VminBar.Value;
        }

        private void VmaxBar_Scroll(object sender, EventArgs e)
        {
            _VmaxBar = VmaxBar.Value;
        }

     
    }       
}
