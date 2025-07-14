using System;
using System.Drawing;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
    public class IVDrawEngine
    {
		private	Graphics graphics;
		private Bitmap memoryBitmap;
		private	int	width;
		private	int	height;

        public void Dispose()
        {
            if (graphics != null)
            {
                graphics.Dispose();
            }

            if (memoryBitmap != null)
            {
                memoryBitmap.Dispose();
            }
        }

        public IVDrawEngine()
		{
            try
            {
			    width = 0;
			    height = 0;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public bool CreateDoubleBuffer(Graphics g, int width, int height)
        {
            try
            {
                if (memoryBitmap != null)
                {
                    memoryBitmap.Dispose();
                    memoryBitmap = null;
                }

                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }

                if (width == 0 || height == 0)
                    return false;

                this.width = width;
                this.height = height;

                memoryBitmap = new Bitmap(width, height);
                graphics = Graphics.FromImage(memoryBitmap);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}

		public void Render(Graphics g)
		{
            try
            {
                if (memoryBitmap != null)
                {
                    g.DrawImage(memoryBitmap, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
            }
		}

		public bool CanDoubleBuffer()
		{
            try
            {
                return graphics != null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("ImageViewer error: " + ex.ToString());
                return false;
            }
		}

		public Graphics g 
		{
			get 
			{ 
				return graphics; 
			}
		}
    }
}
