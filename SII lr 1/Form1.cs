using System;
using System.Drawing;
using System.Windows.Forms;
using VisioForge.Shared.DirectShowLib;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SII_lr_1
{
    public partial class Form1 : Form
    {
        static VideoCapture videoCapture = null;
        static DsDevice[] dsDevice = null;
        static readonly CascadeClassifier cascadeEye = new CascadeClassifier("haarcascade_eye.xml");
        static readonly CascadeClassifier cascadeFace = new CascadeClassifier("haarcascade_frontalface_default.xml");
        int SelectedCamera = 0;
        static bool startWebCam = false;
        public Form1()
        {
            InitializeComponent();
            dsDevice = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i =0; i < dsDevice.Length; i++)
            {
                comboBox1.Items.Add(dsDevice[i].Name);
            }

            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedCamera = comboBox1.SelectedIndex;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            startWebCam = !startWebCam;

            videoCapture = new VideoCapture(SelectedCamera);
            videoCapture.ImageGrabbed += VideoCapture_ImageGrabbed;
            videoCapture.Start();

            if (startWebCam == false) videoCapture.Stop();

        }

        private void VideoCapture_ImageGrabbed(object sender, EventArgs e)
        {
            Mat mat = new Mat();
            videoCapture.Retrieve(mat);
            var image = mat.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal);
            var GrayImage = image.Convert<Gray, byte>();
            
            var Eyes = cascadeEye.DetectMultiScale(GrayImage, 1.1, 10, Size.Empty);
            var Faces = cascadeFace.DetectMultiScale(GrayImage, 1.1, 10, Size.Empty);
            foreach (var item in Eyes)
            {
                image.Draw(item, new Bgr(Color.Red), 2);
            }
            foreach (var item in Faces)
            {
                image.Draw(item, new Bgr(Color.Blue), 2);
            }

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            pictureBox1.Image = image.ToBitmap();

            image.Dispose();
            GrayImage.Dispose();
            mat.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var bmpBase = new Bitmap(openFileDialog.FileName);
            var bmp = new Bitmap(bmpBase, new Size(pictureBox1.Width, pictureBox1.Height));

            Image<Bgr, byte> image = new Image<Bgr, byte>(bmp);
            Image<Gray, Byte> grayImg = image.Convert<Gray, Byte>();

            var Eyes = cascadeEye.DetectMultiScale(grayImg, 1.1, 10, Size.Empty);
            var Faces = cascadeFace.DetectMultiScale(grayImg, 1.1, 10, Size.Empty);
            foreach (var item in Eyes)
            {
                image.Draw(item, new Bgr(Color.Red), 2);
            }
            foreach (var item in Faces)
            {
                image.Draw(item, new Bgr(Color.Blue), 2);
            }

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }

            pictureBox1.Image = image.ToBitmap();

            image.Dispose();
            bmpBase.Dispose();
            bmp.Dispose();
            grayImg.Dispose();
        }
    }
}
