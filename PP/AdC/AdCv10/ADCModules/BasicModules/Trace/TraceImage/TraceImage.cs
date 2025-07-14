using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace BasicModules.Trace
{
    ///////////////////////////////////////////////////////////////////////
    // Classe qui englobe un IImage pour pouvoir l'afficher
    // dans le module de trace
    ///////////////////////////////////////////////////////////////////////
    public class TraceImage : DisposableObject
    {
        public IImage image;
        public ModuleBase sourceModule;
        private BitmapSource _wpfBitmap;

        private static ProcessingClassMil3D _processClass3D = new ProcessingClassMil3D();

        //=================================================================
        // Constructeur
        //=================================================================
        public TraceImage(ModuleBase sourceModule, IImage image)
        {
            image.AddRef();
            this.image = image;
            this.sourceModule = sourceModule;
        }

        //=================================================================
        // From DisposableObject
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (image != null)
            {
                image.DelRef();
                image = null;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // 
        //=================================================================
        public string Name
        {
            get
            {
                if (image == null)
                    return "";
                else
                    return image.ToString();
            }
        }

        public int SourceModuleId
        {
            get
            {
                if (image == null)
                    return -1;
                else
                    return sourceModule.Id;
            }
        }

        public override string ToString()
        {
            if (image == null)
                return "";
            else
                return image.ToString() + "\t" + sourceModule.ToString();
        }

        //=================================================================
        // Proprietees Browsable
        //=================================================================
        [Category("Image"), Browsable(true), ExpandableObject]
        public IImage Image { get { return image; } }
        [Category("Source"), Browsable(true)]
        public string SourceModule { get { return sourceModule.ToString(); } }
        [Category("Source"), Browsable(true)]
        public LayerBase Layer { get { return image.Layer; } }

        //=================================================================
        // 
        //=================================================================
        public MilImage MilImage
        {
            get
            {
                if (image == null)
                    return null;
                else
                    return image.CurrentProcessingImage.GetMilImage();
            }
        }

        //=================================================================
        // Conversion image MIL => image WPF
        //=================================================================
        public BitmapSource WpfBitmap
        {
            get
            {
                if (_wpfBitmap == null)
                    _wpfBitmap = image.CurrentProcessingImage.ConvertToWpfBitmapSource();

                return _wpfBitmap;
            }
        }

        //=================================================================
        // Conversion image MIL => image WPF
        // retourne true si bitmapSource a été réalloué 
        //=================================================================
        public bool CopyToWriteableBitmap(ref WriteableBitmap writeableBitmap)
        {
            ProcessingImage img8bit = null;
            MilImage milImage = null;

            bool reallocated = false;
            try
            {
                //---------------------------------------------------------
                // Get MilImage
                //---------------------------------------------------------
                milImage = image.CurrentProcessingImage.GetMilImage();
                if (milImage == null || milImage.MilId == 0)
                {
                    reallocated = (writeableBitmap == null);
                    writeableBitmap = null;
                    return reallocated;
                }

                // Conversion 32float -> 8bit si besoin
                //.....................................
                img8bit = _processClass3D.ConvertTo8bit(image.CurrentProcessingImage);
                milImage = img8bit.GetMilImage();

                int width = milImage.SizeX;
                int height = milImage.SizeY;
                int pitch = milImage.Pitch;

                //---------------------------------------------------------
                // Reuse or reallocate the BitmapSource
                //---------------------------------------------------------
                reallocated = reallocated || (writeableBitmap == null);
                reallocated = reallocated || (writeableBitmap.Width != width);
                reallocated = reallocated || (writeableBitmap.Height != height);
                reallocated = reallocated || (writeableBitmap.Format != PixelFormats.Gray8);

                if (reallocated)
                    writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);

                //-----------------------------------------------------
                // Copie des pixels
                //-----------------------------------------------------

                // Lock source image
                //..................
                milImage.Lock(MIL.M_READ);
                long pSrc = milImage.HostAddress;

                // Copy
                //.....
                int bufferSize = pitch * height;
                writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), new IntPtr(pSrc), bufferSize, pitch);
            }
            finally
            {
                // Unlock source image
                //....................
                if (milImage.IsLocked)
                    milImage.Unlock();

                if (img8bit != null)
                    img8bit.Dispose();
            }

            return reallocated;
        }

        //=================================================================
        // Sauvegarde qui gère les exceptions
        //=================================================================
        public bool Save(string filename)
        {
            try
            {
                image.CurrentProcessingImage.GetMilImage().Save(filename);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show("Error while saving \"" + filename + "\"", ex);
                return false;
            }
        }

        //=================================================================
        // 
        //=================================================================
        private MilImage Convert16bitTo8bit(MilImage milSource)
        {
            if (milSource.SizeBit != 16)
                throw new ApplicationException("invalid image depth: " + milSource.SizeBit + " expected: 16");

            using (var milStat = new MilImageResult())
            {
                milStat.AllocResult(milSource.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(milSource, MIL.M_STAT_MIN , MIL.M_STAT_MAX);
                int min = (int) milStat.GetResult(MIL.M_STAT_MIN);
                int max = (int) milStat.GetResult(MIL.M_STAT_MAX);

                using (MilImage milDest = new MilImage())
                {
                    milDest.Alloc2d(milSource.OwnerSystem, milSource.SizeX, milSource.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);

                    if (max < 255)
                    {
                        MilImage.Copy(milSource, milDest);
                    }
                    else
                    {
                        using (MilImage milTemp = (MilImage)milSource.DeepClone())
                        {
                            milTemp.Arith(min, MIL.M_SUB_CONST);
                            double factor = (max - min) / 256.0;
                            milTemp.Arith(factor, MIL.M_MULT_CONST);
                            MilImage.Copy(milTemp, milDest);
                        }
                    }

                    milDest.AddRef();
                    return milDest;
                }
            }
        }

    }
}
