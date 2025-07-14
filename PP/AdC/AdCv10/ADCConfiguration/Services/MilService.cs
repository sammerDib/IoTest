using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace ADCConfiguration.Services
{
    public class MilService
    {
        private const double WaferPositionTolerance = 0.1;
        private const double FitCircleCoverageMin = 0.6;
        private const string folder = @".\";
        private const string logfile = folder + "WaferCenter.log";
        private MIL_ID _systemId;
        private MilImage _workImage;
        private MilImage _debugImage;
        private MilGraphicsContext _debugGraphicContext;

        //-----------------------------------------------------------------
        // Variables internes
        //-----------------------------------------------------------------
        private MilEdgeFinder _edgeFinder;
        private MilEdgeFinderResult _edgeFinderResult;


        public bool LicenseMilIsAvailable()
        {
            return (Mil.Instance.LicenseModules & MIL.M_LICENSE_IM) != 0;
        }

        //=================================================================
        // 
        //=================================================================
        public BitmapSource GetImageFromMilModel(byte[] MemPtr)
        {
            using (MilModelFinder milModel = new MilModelFinder())
            using (MilImage milImage = new MilImage())
            using (MilGraphicsContext milGC = new MilGraphicsContext())
            {
                milModel.Stream(MemPtr, Mil.Instance.HostSystem, MIL.M_RESTORE, MIL.M_MEMORY);
                milImage.AllocColor(Mil.Instance.HostSystem, 3, milModel.AllocSizeX, milModel.AllocSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                milImage.Clear();
                milGC.Alloc(Mil.Instance.HostSystem);
                milGC.Image = milImage;
                milGC.Color = MIL.M_COLOR_MAGENTA;
                milModel.Draw(milGC, MIL.M_DRAW_IMAGE);
                milModel.Draw(milGC, MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES);

                BitmapSource bitmapSource = milImage.ConvertToWpfBitmapSource();
                return bitmapSource;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public WaferCenterResult ProcessWaferCenter(string waferImagePath, bool debug = false)
        {
            WaferCenterResult result = new WaferCenterResult();
            using (MilImage milImage = new MilImage())
            {
                milImage.Restore(waferImagePath);
                _systemId = milImage.OwnerSystem;
                _workImage = milImage;

                try
                {
                    InitEdgeFinder();
                    result = FindWaferCenter();
                    if (debug)
                        DrawDebug();
                }
                finally
                {
                    Free();
                }
            }
            return result;
        }

        //=================================================================
        // 
        //=================================================================
        private void InitEdgeFinder()
        {
            _edgeFinder = new MilEdgeFinder();
            _edgeFinder.Alloc(_systemId, MIL.M_CONTOUR);
            _edgeFinder.Control(MIL.M_CIRCLE_FIT_COVERAGE + MIL.M_SORT1_DOWN, MIL.M_ENABLE);
            _edgeFinder.Control(MIL.M_CIRCLE_FIT_RADIUS + MIL.M_SORT2_DOWN, MIL.M_ENABLE);
            _edgeFinder.Control(MIL.M_CIRCLE_FIT_ERROR + MIL.M_SORT3_UP, MIL.M_ENABLE);
            _edgeFinder.Control(MIL.M_CIRCLE_FIT_CENTER_X, MIL.M_ENABLE);
            _edgeFinder.Control(MIL.M_CIRCLE_FIT_CENTER_Y, MIL.M_ENABLE);
        }

        //=================================================================
        // 
        //=================================================================
        private WaferCenterResult FindWaferCenter()
        {

            WaferCenterResult waferCenterResult = new WaferCenterResult();

            //---------------------------------------------------------
            // Find edges
            //---------------------------------------------------------
            _edgeFinderResult = new MilEdgeFinderResult(_edgeFinder);
            _edgeFinderResult.AllocResult();
            _edgeFinderResult.Calculate(_workImage);

            // Remove edges that not fit with wafer size circle approx
            double radius_min_pix = (_workImage.SizeX * 0.8) / 2;
            double radius_max_pix = _workImage.SizeX / 2.0;
            _edgeFinderResult.Select(MIL.M_DELETE, MIL.M_CIRCLE_FIT_RADIUS, MIL.M_OUT_RANGE, radius_min_pix, radius_max_pix);

            double waferCenterX = _workImage.SizeX / 2.0;
            // Supression des centres qui sont hors tolérence
            double pos_min_pix = waferCenterX - (_workImage.SizeX * WaferPositionTolerance);
            double pos_max_pix = waferCenterX + (_workImage.SizeX * WaferPositionTolerance);
            _edgeFinderResult.Select(MIL.M_DELETE, MIL.M_CIRCLE_FIT_CENTER_X, MIL.M_OUT_RANGE, pos_min_pix, pos_max_pix);

            double waferCenterY = _workImage.SizeY / 2.0;
            // Supression des centres qui sont hors tolérence
            pos_min_pix = waferCenterY - (_workImage.SizeY * WaferPositionTolerance);
            pos_max_pix = waferCenterY + (_workImage.SizeY * WaferPositionTolerance);
            _edgeFinderResult.Select(MIL.M_DELETE, MIL.M_CIRCLE_FIT_CENTER_Y, MIL.M_OUT_RANGE, pos_min_pix, pos_max_pix);

            // Remove edges that not cover at least xxx% of circle edge
            _edgeFinderResult.Select(MIL.M_DELETE, MIL.M_CIRCLE_FIT_COVERAGE, MIL.M_LESS, FitCircleCoverageMin, MIL.M_NULL);

            waferCenterResult.WaferSize.Height = _workImage.SizeY;
            waferCenterResult.WaferSize.Width = _workImage.SizeX;

            if (_edgeFinderResult.NumberOfChains == 0)
            {
                waferCenterResult.WaferCenter = new Point(waferCenterResult.WaferSize.Width / 2, waferCenterResult.WaferSize.Height / 2);
                return waferCenterResult;
            }

            //---------------------------------------------------------
            // Compute Center
            //---------------------------------------------------------
            PointF center = _edgeFinderResult.CircleFitCenter;
            waferCenterResult.Radius = _edgeFinderResult.CircleFitRadius;
            waferCenterResult.WaferCenter = new Point((int)center.X, (int)center.Y);
            waferCenterResult.IsValid = true;

            return waferCenterResult;
        }

        //=================================================================
        // 
        //=================================================================
        private void DrawDebug()
        {
            _debugImage = new MilImage();
            _debugImage.AllocColor(_systemId, 3, _workImage.SizeX, _workImage.SizeY, _workImage.Type, _workImage.Attribute);
            MilImage.Copy(_workImage, _debugImage);

            _debugGraphicContext = new MilGraphicsContext();
            _debugGraphicContext.Alloc(_systemId);
            _debugGraphicContext.Image = _debugImage;

            if (_edgeFinderResult != null)
            {
                //_debugGraphicContext.Color = MIL.M_COLOR_LIGHT_BLUE;
                //_edgeFinderResult.Draw(_debugMilGraphic, MIL.M_DRAW_EDGE);
                _debugGraphicContext.Color = MIL.M_COLOR_BLUE;
                _edgeFinderResult.Draw(_debugGraphicContext, MIL.M_DRAW_CIRCLE_FIT);
                _debugGraphicContext.Color = MIL.M_COLOR_DARK_BLUE;
                for (int i = 0; i < _edgeFinderResult.NumberOfChains; i++)
                    _debugGraphicContext.Cross((int)_edgeFinderResult.CirclesFitCenter[i].X, (int)_edgeFinderResult.CirclesFitCenter[i].Y, 10);

                if (_edgeFinderResult.NumberOfChains >= 1)
                {
                    //_debugGraphicContext.Color = MIL.M_COLOR_LIGHT_GREEN;
                    //_edgeFinderResult.Draw(_debugMilGraphic, MIL.M_DRAW_EDGE, 0);
                    _debugGraphicContext.Color = MIL.M_COLOR_GREEN;
                    _edgeFinderResult.Draw(_debugGraphicContext, MIL.M_DRAW_CIRCLE_FIT, 0);
                    _debugGraphicContext.Color = MIL.M_COLOR_DARK_GREEN;
                    _debugGraphicContext.Cross((int)_edgeFinderResult.CirclesFitCenter[0].X, (int)_edgeFinderResult.CirclesFitCenter[0].Y, 10);
                }
            }

            string filename = String.Format("EdgeFinderPositionCorrector_{0:yyyyMMdd_HHmmssfff}.tif", DateTime.Now);
            Directory.CreateDirectory(folder);
            _debugImage.Save(folder + filename);

            string text = "";
            if (_edgeFinderResult == null)
            {
                text = filename + "\t⚠\n";
            }
            else if (_edgeFinderResult.NumberOfChains == 0)
            {
                text = filename + "\t∅\n";
            }
            else
            {
                for (int i = 0; i < _edgeFinderResult.NumberOfChains; i++)
                    text += filename + "\t" + i + "\t" + _edgeFinderResult.CirclesFitCenter[i].X + "\t" + _edgeFinderResult.CirclesFitCenter[i].Y + "\t" + _edgeFinderResult.CirclesFitRadius[i] + "\t" + _edgeFinderResult.CirclesFitCoverage[i] + "\n";
            }
            text += "\n";
            File.AppendAllText(logfile, text);
        }

        //=================================================================
        // 
        //=================================================================
        private void Free()
        {
            _workImage = null;

            if (_debugImage != null)
            {
                _debugImage.Dispose();
                _debugImage = null;
            }

            if (_debugGraphicContext != null)
            {
                _debugGraphicContext.Dispose();
                _debugGraphicContext = null;
            }

            if (_edgeFinder != null)
            {
                _edgeFinder.Dispose();
                _edgeFinder = null;
            }

            if (_edgeFinderResult != null)
            {
                _edgeFinderResult.Dispose();
                _edgeFinderResult = null;
            }
        }

    }

    public class WaferCenterResult
    {
        public bool IsValid;
        /// <summary>
        /// Wafer center pixel
        /// </summary>
        public Point WaferCenter;

        /// <summary>
        /// Wafer size
        /// </summary>
        public Size WaferSize = new Size();

        /// <summary>
        /// Radius pixel
        /// </summary>
        public double Radius { get; set; }
    }
}
