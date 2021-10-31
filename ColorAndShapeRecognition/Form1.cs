using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace ColorAndShapeRecognition
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> imgOriginal;
        Image<Bgr, byte> imgResult;

        public static int threshold_Red = 40;
        public static int threshold_Green = 50;
        public static int ObjectSize = 1000;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imgOriginal = new Image<Bgr, byte>(dialog.FileName);
                    pictureBox1.Image = imgOriginal.AsBitmap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RedProcessing()
        {
            Image<Gray, byte> imgSubtract;
            Image<Gray, byte> imgBinary;
            Image<Bgr, byte> imgClone = imgOriginal.Clone();
            Image<Gray, byte> imgCloneGray;

            imgCloneGray = imgClone.Convert<Gray, byte>();
            imgSubtract = imgClone[2] - imgCloneGray;

            imgBinary = imgSubtract.ThresholdBinary(new Gray(threshold_Red), new Gray(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat m = new Mat();
            CvInvoke.FindContours(imgBinary, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) >= ObjectSize)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();

                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    CvInvoke.DrawContours(imgResult, contours, i, new MCvScalar(0, 0, 0), 2);

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(imgResult, "Red Triangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size == 4)
                    {
                        CvInvoke.PutText(imgResult, "Red Rectangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size > 6)
                    {
                        CvInvoke.PutText(imgResult, "Red Circle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else
                    {
                        CvInvoke.PutText(imgResult, "This type of red shape was not defined yet", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                }
            }
        }


        private void GreenProcessing()
        {
            Image<Gray, byte> imgSubtract;
            Image<Gray, byte> imgBinary;
            Image<Bgr, byte> imgClone = imgOriginal.Clone();
            Image<Gray, byte> imgCloneGray;

            imgCloneGray = imgClone.Convert<Gray, byte>();
            imgSubtract = imgClone[1] - imgCloneGray;

            imgBinary = imgSubtract.ThresholdBinary(new Gray(threshold_Green), new Gray(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat m = new Mat();
            CvInvoke.FindContours(imgBinary, contours, m, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) >= ObjectSize)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();

                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    CvInvoke.DrawContours(imgResult, contours, i, new MCvScalar(0, 0, 0), 2);

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(imgResult, "Green Triangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size == 4)
                    {
                        CvInvoke.PutText(imgResult, "Green Rectangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size > 6)
                    {
                        CvInvoke.PutText(imgResult, "Green Circle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else
                    {
                        CvInvoke.PutText(imgResult, "This type of Green shape was not defined yet", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                }
            }
        }


        private void reToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imgResult = imgOriginal.Clone();
            RedProcessing();
            GreenProcessing();
            pictureBox2.Image = imgResult.AsBitmap();
        }

        private void tbRedThreshold_Scroll(object sender, EventArgs e)
        {
            threshold_Red = tbRedThreshold.Value;
            imgResult = imgOriginal.Clone();
            RedProcessing();
            GreenProcessing();
            pictureBox2.Image = imgResult.AsBitmap();
        }

        private void tbGreenThreshold_Scroll(object sender, EventArgs e)
        {
            threshold_Green = tbGreenThreshold.Value;
            imgResult = imgOriginal.Clone();
            RedProcessing();
            GreenProcessing();
            pictureBox2.Image = imgResult.AsBitmap();
        }

        private void tbObjectSize_Scroll(object sender, EventArgs e)
        {
            ObjectSize = tbObjectSize.Value;
            imgResult = imgOriginal.Clone();
            RedProcessing();
            GreenProcessing();
            pictureBox2.Image = imgResult.AsBitmap();
        }
    }
}
