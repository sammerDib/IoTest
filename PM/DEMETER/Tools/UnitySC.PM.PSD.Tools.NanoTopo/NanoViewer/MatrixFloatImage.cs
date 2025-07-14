using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

 using System.Diagnostics;
using System.Threading.Tasks;
     

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoViewer
{
    public class MatrixFloatImage 
    {
        private int m_nWidth;
        private int m_nHeight;
        private float[] m_fData;
        private Color[] m_ColorMapRef;

       /// <summary>
       /// Constructore
       /// </summary>
       /// <param name="p_nWidth"></param> 
       /// <param name="p_nHeight"></param>
       /// <param name="p_fData"></param>
       /// <param name="p_sColormapPath"></param>
        public MatrixFloatImage(int p_nWidth, int p_nHeight, float[] p_fData, string p_sColormapPath)
        {
            m_nWidth = p_nWidth;
            m_nHeight = p_nHeight;
            m_fData = p_fData;

            InitColorMapRef(p_sColormapPath);
        }

        /// <summary>
        /// Initialize colormap use to generate image. if colormap image cannot be loaded, default grayscale image is used
        /// </summary>
        /// <param name="p_sColormapPath"></param>
        private void InitColorMapRef(string p_sColormapPath)
        {
            Bitmap bmpClr;

            try
            {
                bmpClr = new Bitmap(p_sColormapPath);
            }
            catch (System.Exception ex)
            {
                string sMsg = "Could not load Colormap {" + p_sColormapPath + "} - Use Default Grayscale Colormap\n\n" + ex.ToString();
                MessageBox.Show(sMsg, "Colormap not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor.Current = Cursors.WaitCursor;
                bmpClr = new Bitmap(256, 1);
                for (int i = 0; i < 256; i++)
                {
                    bmpClr.SetPixel(i, 0, Color.FromArgb(i, i, i));
                }
                Cursor.Current = Cursors.Default;
            }

            int nbPaletteColors = bmpClr.Width;
            m_ColorMapRef = new Color[nbPaletteColors];
            unsafe
            {

                //lock the new bitmap in memory
                BitmapData newData = bmpClr.LockBits(new Rectangle(0, 0, bmpClr.Width, bmpClr.Height), ImageLockMode.WriteOnly, bmpClr.PixelFormat);
                int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmpClr.PixelFormat);
                int cCount = nDepth / 8;
                byte* nRow = (byte*)newData.Scan0 + (0 * newData.Stride);
                Parallel.For(0, nbPaletteColors, i =>
                {
                    int a= 255;
                    int r = 0; 
                    int g = 0; 
                    int b = 0;
                    
                     if (nDepth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                      {
                           b = nRow[i * cCount];
                           g = nRow[i * cCount + 1];
                           r = nRow[i * cCount + 2];
                           a = nRow[i * cCount + 3];
                       }
                       else if (nDepth == 24) // For 24 bpp set Red, Green and Blue
                       {
                           b = nRow[i * cCount];
                           g = nRow[i * cCount + 1];
                           r = nRow[i * cCount + 2];
                       }
                       else if (nDepth == 8) // For 8 bpp set color value (Red, Green and Blue values are the same)
                       {
                           b = g = r = nRow[i * cCount];
                       } 
                       m_ColorMapRef[i] = Color.FromArgb(a,r,g,b);
                }); // Parallel.For
                //unlock the bitmaps
                bmpClr.UnlockBits(newData);
                 
            }
            bmpClr.Dispose();
        }
        /// <summary>
        /// Create a line bmp of colormap to display
        /// </summary>
        /// <param name="bRotate90"></param>
        /// <returns></returns>
        public Bitmap GetColorMapBmp(bool bRotate90)
        {
            Bitmap bmp;
            if(bRotate90)
            {
                bmp = new Bitmap(5,m_ColorMapRef.Length);
//                 for (int i = 0; i < m_ColorMapRef.Length; i++)
//                 {
//                     bmp.SetPixel(0, i, m_ColorMapRef[m_ColorMapRef.Length - 1 - i]);
//                     bmp.SetPixel(1, i, m_ColorMapRef[m_ColorMapRef.Length - 1 - i]);
//                     bmp.SetPixel(2, i, m_ColorMapRef[m_ColorMapRef.Length - 1 - i]);
//                     bmp.SetPixel(3, i, m_ColorMapRef[m_ColorMapRef.Length - 1 - i]);
//                     bmp.SetPixel(4, i, m_ColorMapRef[m_ColorMapRef.Length - 1 - i]);
//                 }

                unsafe
                {

                    //lock the new bitmap in memory
                    BitmapData newData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                    int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
                    int cCount = nDepth / 8;
                    Parallel.For(0, m_ColorMapRef.Length, i =>
                    {
                        byte* nRow = (byte*)newData.Scan0 + (i * newData.Stride);
                        Color oClr = m_ColorMapRef[m_ColorMapRef.Length - 1 - i];
                        nRow[0 * cCount] = oClr.B;
                        nRow[0 * cCount + 1] = oClr.G;
                        nRow[0 * cCount + 2] = oClr.R;
                        nRow[0 * cCount + 3] = oClr.A;
                        nRow[1 * cCount] = oClr.B;
                        nRow[1 * cCount + 1] = oClr.G;
                        nRow[1 * cCount + 2] = oClr.R;
                        nRow[1 * cCount + 3] = oClr.A;
                        nRow[2 * cCount] = oClr.B;
                        nRow[2 * cCount + 1] = oClr.G;
                        nRow[2 * cCount + 2] = oClr.R;
                        nRow[2 * cCount + 3] = oClr.A;
                        nRow[3 * cCount] = oClr.B;
                        nRow[3 * cCount + 1] = oClr.G;
                        nRow[3 * cCount + 2] = oClr.R;
                        nRow[3 * cCount + 3] = oClr.A;
                        nRow[4 * cCount] = oClr.B;
                        nRow[4 * cCount + 1] = oClr.G;
                        nRow[4 * cCount + 2] = oClr.R;
                        nRow[4 * cCount + 3] = oClr.A;
                    }); // Parallel.For
                    //unlock the bitmaps
                    bmp.UnlockBits(newData);
                }


            }
            else
            {
                bmp = new Bitmap(m_ColorMapRef.Length,5);
//                 for (int i = 0; i < m_ColorMapRef.Length; i++)
//                 {
//                     bmp.SetPixel(i, 0, m_ColorMapRef[i]);
//                     bmp.SetPixel(i, 1, m_ColorMapRef[i]);
//                     bmp.SetPixel(i, 2, m_ColorMapRef[i]);
//                     bmp.SetPixel(i, 3, m_ColorMapRef[i]);
//                     bmp.SetPixel(i, 4, m_ColorMapRef[i]);
//                 }

                unsafe
                {

                    //lock the new bitmap in memory
                    BitmapData newData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                    int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
                    int cCount = nDepth / 8;
                    byte* nRow0 = (byte*)newData.Scan0 + (0 * newData.Stride);
                    byte* nRow1 = (byte*)newData.Scan0 + (1 * newData.Stride);
                    byte* nRow2 = (byte*)newData.Scan0 + (2 * newData.Stride);
                    byte* nRow3 = (byte*)newData.Scan0 + (3 * newData.Stride);
                    byte* nRow4 = (byte*)newData.Scan0 + (4 * newData.Stride);
                    Parallel.For(0, m_ColorMapRef.Length, i =>
                    {
                        Color oClr = m_ColorMapRef[i];
                        nRow0[i * cCount] = oClr.B;
                        nRow0[i * cCount + 1] = oClr.G;
                        nRow0[i * cCount + 2] = oClr.R;
                        nRow0[i * cCount + 3] = oClr.A;
                        nRow1[i * cCount] = oClr.B;
                        nRow1[i * cCount + 1] = oClr.G;
                        nRow1[i * cCount + 2] = oClr.R;
                        nRow1[i * cCount + 3] = oClr.A;
                        nRow2[i * cCount] = oClr.B;
                        nRow2[i * cCount + 1] = oClr.G;
                        nRow2[i * cCount + 2] = oClr.R;
                        nRow2[i * cCount + 3] = oClr.A;
                        nRow3[i * cCount] = oClr.B;
                        nRow3[i * cCount + 1] = oClr.G;
                        nRow3[i * cCount + 2] = oClr.R;
                        nRow3[i * cCount + 3] = oClr.A;
                        nRow4[i * cCount] = oClr.B;
                        nRow4[i * cCount + 1] = oClr.G;
                        nRow4[i * cCount + 2] = oClr.R;
                        nRow4[i * cCount + 3] = oClr.A;
                    }); // Parallel.For
                    //unlock the bitmaps
                    bmp.UnlockBits(newData);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Create Bitmap instance of float matrix to be displayable in viewer
        /// </summary>
        /// <param name="p_fMin"></param>
        /// <param name="p_fMax"></param>
        /// <returns></returns>
        public Bitmap GetImage(float p_fMin, float p_fMax)
        {
            Bitmap bmp = new Bitmap(m_nWidth, m_nHeight);

            int nColoMaxIdx = m_ColorMapRef.Length;
            float fColoMaxIdx = (float)nColoMaxIdx;
            float a = fColoMaxIdx / (p_fMax - p_fMin);
            float b = -p_fMin * fColoMaxIdx / (p_fMax - p_fMin);
            unsafe
            {
                //lock the new bitmap in memory
                BitmapData newData = bmp.LockBits(new Rectangle(0, 0, m_nWidth, m_nHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);

               // get source bitmap pixel format size
               int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
               // Get color components count
               int cCount = nDepth / 8;
               Parallel.For(0, m_nHeight, y =>
               //for (int y = 0; y < m_nHeight; y++)
               {
                   //get the data from the new image
                   byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                   for (int x = 0; x < m_nWidth; x++)
                   {
                       // convert float to index
                       float fVal = m_fData[y * m_nWidth + x] * a + b;
                       int nVal = (int)Math.Round(fVal);
                       if (nVal < 0)
                           nVal = 0;
                       else if (nVal >= nColoMaxIdx)
                           nVal = nColoMaxIdx - 1;

                       //set the new image's pixel with current color map

                       if (nDepth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                       {
                           nRow[x * cCount] = m_ColorMapRef[nVal].B;
                           nRow[x * cCount + 1] = m_ColorMapRef[nVal].G;
                           nRow[x * cCount + 2] = m_ColorMapRef[nVal].R;
                           nRow[x * cCount + 3] = m_ColorMapRef[nVal].A;         
                       }
                       else if (nDepth == 24) // For 24 bpp set Red, Green and Blue
                       {
                           nRow[x * cCount] = m_ColorMapRef[nVal].B;
                           nRow[x * cCount + 1] = m_ColorMapRef[nVal].G;
                           nRow[x * cCount + 2] = m_ColorMapRef[nVal].R;
                       }
                       else if (nDepth == 8) // For 8 bpp set color value (Red, Green and Blue values are the same)
                       {
                           nRow[x * cCount] = m_ColorMapRef[nVal].B;
                       }
                   }
               }); // Parallel.For

                //unlock the bitmaps
                bmp.UnlockBits(newData);
            }

            return bmp;
        }

        /// <summary>
        /// return float value in Non converted Matrix 
        /// </summary>
        /// <param name="fIdxX"></param>
        /// <param name="fIdxY"></param>
        /// <returns></returns>
        public float GetValue(float fIdxX, float fIdxY)
        {
            if (m_fData == null)
                return float.NaN;
            int x = (int)Math.Round(fIdxX, 0);
            int y = (int)Math.Round(fIdxY, 0);
            if (x < 0 || x >= m_nWidth)
                return float.NaN;
            if (y < 0 || y >= m_nHeight)
                return float.NaN;
            return m_fData[y * m_nWidth + x];
        }
    }
}
