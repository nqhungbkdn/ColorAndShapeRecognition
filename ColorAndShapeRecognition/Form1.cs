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
        Image<Bgr, byte> img;
        Image<Bgr, byte> imgClone;

        public static int threshold_Red = 0;
        public static int threshold_Blue = 0;
        public static int ObjectSize = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    img = new Image<Bgr, byte>(dialog.FileName);
                    imgClone = img.Clone();
                    pictureBox1.Image = img.AsBitmap();
                    RedProcessing();
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

                    CvInvoke.DrawContours(imgClone, contours, i, new MCvScalar(0, 0, 0), 2);

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(img, "Red Triangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size == 4)
                    {
                        CvInvoke.PutText(img, "Red Rectangle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size == 6)
                    {
                        CvInvoke.PutText(img, "Red Hexagon", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                    else if (approx.Size > 6)
                    {
                        CvInvoke.PutText(img, "Red Circle", new Point(x, y), FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 0), 2);
                    }
                }
            }
        }

        private void reToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
