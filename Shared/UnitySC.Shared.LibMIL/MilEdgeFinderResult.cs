using System;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilEdgeFinderResult : AMilId
    {
        public MilEdgeFinder ParentEdgeFinder { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public MilEdgeFinderResult(MilEdgeFinder parentEdgeFinder)
        {
            ParentEdgeFinder = parentEdgeFinder;
        }

        //=================================================================
        // Alloc Model Result
        //=================================================================
        public void AllocResult()
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing edge finder result");

            MIL.MedgeAllocResult(ParentEdgeFinder.OwnerSystem, MIL.M_DEFAULT, ref _milId);
            AMilId.checkMilError("Failed to allocate edge finder result");
        }

        //=================================================================
        // Calculate
        //=================================================================
        public void Calculate(MilImage sourceImage)
        {
            Calculate(sourceImage, MIL.M_NULL, MIL.M_NULL, MIL.M_NULL, MIL.M_DEFAULT);
        }

        public void Calculate(MilImage sourceImage, MIL_ID sourceDeriv1Id, MIL_ID sourceDeriv2Id, MIL_ID sourceDeriv3Id, long controlFlag)
        {
            MIL.MedgeCalculate(ParentEdgeFinder.MilId, sourceImage, sourceDeriv1Id, sourceDeriv2Id, sourceDeriv3Id, _milId, controlFlag);
            AMilId.checkMilError("Failed to calculate edge finder result");
        }

        //=================================================================
        // Select
        //=================================================================
        public void Select(long operation, long selectionCriterion, long condition, double param1, double param2)
        {
            MIL.MedgeSelect(_milId, operation, selectionCriterion, condition, param1, param2);
            AMilId.checkMilError("Failed select edge finder feature");
        }

        //=================================================================
        // Draw
        //=================================================================
        public void Draw(MilGraphicsContext milGC,
                 int operation = MIL.M_DRAW_EDGES + MIL.M_DRAW_POSITION,
                 int index = MIL.M_DEFAULT,
                 int controlFlag = MIL.M_DEFAULT
                )
        {
            MIL.MedgeDraw(milGC.MilId, _milId, milGC.Image.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw edge finder result");
        }

        //=================================================================
        // Results : M_NUMBER_OF_CHAINS
        //=================================================================
        public int NumberOfChains
        {
            get
            {
                int nb = -1;
                MIL.MedgeGetResult(_milId, MIL.M_DEFAULT, MIL.M_NUMBER_OF_CHAINS + MIL.M_TYPE_MIL_INT, ref nb, MIL.M_NULL);
                return nb;
            }
        }

        //=================================================================
        // Results : M_BOX_X/Y_MIN/MAX
        //=================================================================

        //-----------------------------------------------------------------
        //
        //-----------------------------------------------------------------
        public double BoxXMin
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_BOX_X_MIN, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _boxesXMin;

        public double[] BoxesXMin
        {
            get
            {
                if (_boxesXMin == null)
                {
                    _boxesXMin = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_BOX_X_MIN, _boxesXMin, MIL.M_NULL);
                }
                return _boxesXMin;
            }
        }

        //-----------------------------------------------------------------
        //
        //-----------------------------------------------------------------
        public double BoxXMax
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_BOX_X_MAX, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _boxesXMax;

        public double[] BoxesXMax
        {
            get
            {
                if (_boxesXMax == null)
                {
                    _boxesXMax = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_BOX_X_MAX, _boxesXMax, MIL.M_NULL);
                }
                return _boxesXMax;
            }
        }

        //-----------------------------------------------------------------
        //
        //-----------------------------------------------------------------
        public double BoxYMin
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_BOX_Y_MIN, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _boxesYMin;

        public double[] BoxesYMin
        {
            get
            {
                if (_boxesYMin == null)
                {
                    _boxesYMin = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_BOX_X_MIN, _boxesYMin, MIL.M_NULL);
                }
                return _boxesYMin;
            }
        }

        //-----------------------------------------------------------------
        //
        //-----------------------------------------------------------------
        public double BoxYMax
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_BOX_Y_MAX, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _boxesYMax;

        public double[] BoxesYMax
        {
            get
            {
                if (_boxesYMax == null)
                {
                    _boxesYMax = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_BOX_Y_MAX, _boxesYMax, MIL.M_NULL);
                }
                return _boxesYMax;
            }
        }

        //=================================================================
        // Results : M_CIRCLE_FIT_CENTER_X/Y
        //=================================================================
        public PointF CircleFitCenter
        {
            get
            {
                double x = double.NaN;
                double y = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_CIRCLE_FIT_CENTER_X, ref x, MIL.M_NULL);
                MIL.MedgeGetResult(_milId, 0, MIL.M_CIRCLE_FIT_CENTER_Y, ref y, MIL.M_NULL);
                return new PointF((float)x, (float)y);
            }
        }

        private PointF[] _circlesFitCenter;

        public PointF[] CirclesFitCenter
        {
            get
            {
                if (_circlesFitCenter == null)
                {
                    _circlesFitCenter = new PointF[NumberOfChains];

                    double[] x = new double[NumberOfChains];
                    double[] y = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_CIRCLE_FIT_CENTER_X, x, MIL.M_NULL);
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_CIRCLE_FIT_CENTER_Y, y, MIL.M_NULL);

                    for (int i = 0; i < NumberOfChains; i++)
                        _circlesFitCenter[i] = new PointF((float)x[i], (float)y[i]);
                }
                return _circlesFitCenter;
            }
        }

        //=================================================================
        // Results : M_CIRCLE_FIT_RADIUS
        //=================================================================
        public double CircleFitRadius
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_CIRCLE_FIT_RADIUS, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _circlesFitRadius;

        public double[] CirclesFitRadius
        {
            get
            {
                if (_circlesFitRadius == null)
                {
                    _circlesFitRadius = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_CIRCLE_FIT_RADIUS, _circlesFitRadius, MIL.M_NULL);
                }
                return _circlesFitRadius;
            }
        }

        //=================================================================
        // Results : M_CIRCLE_FIT_COVERAGE
        //=================================================================
        public double CircleFitCoverage
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_CIRCLE_FIT_COVERAGE, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _circlesFitCoverage;

        public double[] CirclesFitCoverage
        {
            get
            {
                if (_circlesFitCoverage == null)
                {
                    _circlesFitCoverage = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_CIRCLE_FIT_COVERAGE, _circlesFitCoverage, MIL.M_NULL);
                }
                return _circlesFitCoverage;
            }
        }

        //=================================================================
        // Results : M_NUMBER_OF_CHAINED_EDGELS
        //=================================================================
        public double NumberOfChainedEdgels
        {
            get
            {
                double d = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_NUMBER_OF_CHAINED_EDGELS, ref d, MIL.M_NULL);
                return d;
            }
        }

        private double[] _numberOfChainedEdgelsTable;

        public double[] NumberOfChainedEdgelsTable
        {
            get
            {
                if (_numberOfChainedEdgelsTable == null)
                {
                    _numberOfChainedEdgelsTable = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_NUMBER_OF_CHAINED_EDGELS, _numberOfChainedEdgelsTable, MIL.M_NULL);
                }
                return _numberOfChainedEdgelsTable;
            }
        }

        //=================================================================
        // Results : M_CHAIN
        //=================================================================
        public PointF Chain
        {
            get
            {
                double x = double.NaN, y = double.NaN;
                MIL.MedgeGetResult(_milId, 0, MIL.M_CHAIN, ref x, ref y);
                return new PointF((float)x, (float)y);
            }
        }

        private PointF[] _chains;

        public PointF[] Chains
        {
            get
            {
                if (_chains == null)
                {
                    _chains = new PointF[NumberOfChains];

                    double[] x = new double[NumberOfChains];
                    double[] y = new double[NumberOfChains];
                    MIL.MedgeGetResult(_milId, MIL.M_ALL, MIL.M_CHAIN, x, y);

                    for (int i = 0; i < NumberOfChains; i++)
                        _chains[i] = new PointF((float)x[i], (float)y[i]);
                }
                return _chains;
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MedgeFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }
    }
}