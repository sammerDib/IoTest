using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CV = Emgu.CV;

using UnitySC.PM.WOTAN.Common;
using UnitySC.PM.WOTAN.Common.Converters;
using UnitySC.PM.WOTAN.Processing.Types;


namespace UnitySC.PM.WOTAN.Processing
{
    public class BareWaferLaneAligner : IBareWaferLaneAligner
    {
        private readonly Wafer _wafer;
        private readonly int _laneNumber;
        private readonly LaneImage _laneImage;
        private readonly CV.Mat _filterKernel;

        private IList<Tuple<int, int>> _validAngularAreas;
        private CV.Mat _polarImage, _polarImageDebug;
        private CV.Mat _grayCartesianImage, _cartesianImageDebug;

        private IList<Point> _edgePointsPolar, _edgePointsCartesian;

        public BareWaferLaneAligner(Wafer wafer, int laneNumber)
        {
            _wafer = wafer;
            _laneNumber = laneNumber;
            _laneImage = _wafer.AlignImages[_laneNumber];

            int kernelSize = GetKernelLeftSize() + 1 + GetKernelRightSize();

            _filterKernel = new CV.Mat(
                rows: 1,
                cols: kernelSize,
                type: CV.CvEnum.DepthType.Cv32F,
                channels: 1);

            InitializeKernelFilter(kernelSize);
        }

        private void InitializeKernelFilter(int kernelSize)
        {
            unsafe
            {
                float* _kernelData = (float*)_filterKernel.GetDataPointer().ToPointer();
                int index;
                for (index = 0; index < GetKernelLeftSize(); index++)
                    _kernelData[index] = 1f / kernelSize;
                _kernelData[index] = 0;
                index++;
                for (index = index; index < kernelSize; index++)
                    _kernelData[index] = -1f / kernelSize;
            }
        }

        public AlignResult Align()
        {
            var timeStart = DateTime.Now;

            _grayCartesianImage = new CV.Mat();
            CV.CvInvoke.Transform(
                src: _laneImage.Image,
                dst: _grayCartesianImage,
                m: CV.Mat.Ones(1, _laneImage.Image.NumberOfChannels, CV.CvEnum.DepthType.Cv8U, 1));

            _grayCartesianImage *= _laneImage.Image.NumberOfChannels;

            Ellipse theoriticalEllipse = CreateTheoriticalEllipse();

            PolarConverter polarConverter = new PolarConverter(
                pixelSize: _laneImage.PixelSize,
                angularSteps: PolarAngularSteps(waferRadius: _wafer.Radius),
                innerMarginUm: GetInnerMargin(),
                outerMarginUm: GetOuterMargin(),
                ellipse: theoriticalEllipse);

            _polarImage = polarConverter.ConvertCartesianToPolarImage(cartesianImage: _grayCartesianImage);

            _validAngularAreas = getValidAngularAreas(theoriticalEllipse, polarConverter);

            _edgePointsPolar = getEdgePointsPolar(edgePointsDistance: GetEdgePixelsDistance());

            _edgePointsCartesian = new List<Point>();
            foreach (Point edgePointPolar in _edgePointsPolar)
                _edgePointsCartesian.Add(polarConverter.ConvertPolarToCartesianPoint(edgePointPolar));

            Ellipse foundEllipse = GetEllipse(_edgePointsCartesian);

            IList<MarkTypeCandidate> markTypeCandidates = findMarkTypeCandidates(ellipse: foundEllipse);

            AlignResult alignResult = new AlignResult(_laneImage, _polarImage.Height)
            {
                ShiftXPx = (int)Math.Round(theoriticalEllipse.CenterX - foundEllipse.CenterX),
                ShiftYPx = (int)Math.Round(theoriticalEllipse.CenterY - foundEllipse.CenterY),
            };

            double candidateScore;
            foreach (MarkTypeCandidate candidate in markTypeCandidates)
            {
                candidate.ComputeMatching(_polarImage);
                candidateScore = candidate.MatchingScore();
                if (candidateScore < alignResult.MarkTypeScore)
                {
                    alignResult.MarkTypeScore = candidateScore;
                    alignResult.AngularPosition = candidate.MatchingPositionPolar.Y;
                }
            }

            Console.WriteLine($"L{_laneNumber}: Alignment Time: {DateTime.Now - timeStart}");

            if (IsDebug())
            {
                _cartesianImageDebug = _laneImage.Image.Clone();

                //_polarImageDebug = polarConverter.ConvertCartesianToPolarImage(cartesianImage: _cartesianImageDebug);
                //_polarImageDebug *= 3;
                _polarImageDebug = _polarImage.Clone();
                CV.CvInvoke.CvtColor(_polarImageDebug, _polarImageDebug, CV.CvEnum.ColorConversion.Gray2Bgr);

                // cartesian image drawings

                CV.CvInvoke.Circle(
                    img: _cartesianImageDebug,
                    center: theoriticalEllipse.CenterInt,
                    radius: 10,
                    color: new CV.Structure.MCvScalar(0, 0, 255),
                    thickness: 2,
                    lineType: CV.CvEnum.LineType.AntiAlias);

                CV.CvInvoke.Ellipse(
                    img: _cartesianImageDebug,
                    box: theoriticalEllipse.AsCvEllipse(),
                    color: new CV.Structure.MCvScalar(0, 0, 255),
                    thickness: 2,
                    lineType: CV.CvEnum.LineType.AntiAlias);

                CV.CvInvoke.Circle(
                   img: _cartesianImageDebug,
                   center: foundEllipse.CenterInt,
                   radius: 10,
                   color: new CV.Structure.MCvScalar(255, 0, 0),
                   thickness: 2,
                   lineType: CV.CvEnum.LineType.AntiAlias);

                CV.CvInvoke.Ellipse(
                img: _cartesianImageDebug,
                box: foundEllipse.AsCvEllipse(),
                color: new CV.Structure.MCvScalar(255, 0, 0),
                thickness: 2,
                lineType: CV.CvEnum.LineType.AntiAlias);

                // polar image drawings

                CV.CvInvoke.Line(
                    img: _polarImageDebug,
                    pt1: new Point((int)_laneImage.ToPixels(GetInnerMargin()), 0),
                    pt2: new Point((int)_laneImage.ToPixels(GetInnerMargin()), _polarImageDebug.Height),
                    color: new CV.Structure.MCvScalar(0, 255, 0),
                    thickness: 2,
                    lineType: CV.CvEnum.LineType.AntiAlias);

                // valid angular areas drawings
                CV.Mat cartesianMask = _cartesianImageDebug.Clone();
                CV.Mat polarMask = _polarImageDebug.Clone();

                int areaStart;
                int areaEnd = _validAngularAreas.Last().Item2;
                double maskStart, maskEnd;

                foreach (Tuple<int, int> validAngularArea in _validAngularAreas)
                {

                    areaStart = validAngularArea.Item1;

                    // polar image

                    if (areaEnd < areaStart)
                    {
                        CV.CvInvoke.Rectangle(
                            img: polarMask,
                            rect: new Rectangle(x: 0, y: areaEnd, width: polarMask.Width, height: areaStart - areaEnd),
                            color: new CV.Structure.MCvScalar(0, 0, 255),
                            thickness: -1);
                    }
                    else
                    {
                        CV.CvInvoke.Rectangle(
                            img: polarMask,
                            rect: new Rectangle(x: 0, y: areaEnd, width: polarMask.Width, height: polarMask.Height - areaEnd),
                            color: new CV.Structure.MCvScalar(0, 0, 255),
                            thickness: -1);

                        CV.CvInvoke.Rectangle(
                            img: polarMask,
                            rect: new Rectangle(x: 0, y: 0, width: polarMask.Width, height: areaStart),
                            color: new CV.Structure.MCvScalar(0, 0, 255),
                            thickness: -1);
                    }

                    // cartesian image

                    maskStart = polarConverter.ConvertAngularPositionToAngle(areaEnd, AngleUnit.Deg);
                    maskEnd = polarConverter.ConvertAngularPositionToAngle(areaStart, AngleUnit.Deg);

                    if (maskStart > maskEnd) // we need to ensure that maskStart < maskEnd (% 360) or openCV will swap them
                    {
                        maskEnd = -maskEnd;
                        maskStart = 360 - maskStart;
                    }

                    CV.CvInvoke.Ellipse(
                        img: cartesianMask,
                        center: theoriticalEllipse.CenterInt,
                        axes: theoriticalEllipse.SizeTwice,
                        angle: theoriticalEllipse.AngleDeg,
                        startAngle: maskStart,
                        endAngle: maskEnd,
                        color: new CV.Structure.MCvScalar(0, 0, 255),
                        thickness: -1,
                        lineType: CV.CvEnum.LineType.AntiAlias);

                    areaEnd = validAngularArea.Item2;

                }

                CV.CvInvoke.AddWeighted(
                    src1: _cartesianImageDebug, alpha: 0.6,
                    src2: cartesianMask, beta: 0.4,
                    gamma: 0,
                    dst: _cartesianImageDebug);

                CV.CvInvoke.AddWeighted(
                    src1: _polarImageDebug, alpha: 0.6,
                    src2: polarMask, beta: 0.4,
                    gamma: 0,
                    dst: _polarImageDebug);

                // edge points cartesian & polar
                double dist;
                CV.Structure.MCvScalar color; 
                foreach (var polarCartesian in _edgePointsPolar.Zip(_edgePointsCartesian, (first, second) => new Tuple<Point, Point>(first, second)))
                {
                    polarCartesian.Deconstruct(out Point polarPoint, out Point cartesianPoint);

                    dist = Math.Abs(
                        Math.Sqrt(
                            Math.Pow(cartesianPoint.X - foundEllipse.CenterX, 2) 
                            + Math.Pow(cartesianPoint.Y - foundEllipse.CenterY, 2)) 
                        - foundEllipse.AverageRadius);

                    if (dist > _laneImage.ToPixels(GetMarkTypeThreshold())) color = new CV.Structure.MCvScalar(0, 0, 255);
                    else color = new CV.Structure.MCvScalar(0, 255, 255);

                    CV.CvInvoke.Circle(
                        img: _cartesianImageDebug,
                        center: cartesianPoint,
                        radius: 5,
                        color: color,
                        lineType: CV.CvEnum.LineType.AntiAlias);

                    CV.CvInvoke.Circle(
                        img: _polarImageDebug,
                        center: polarPoint,
                        radius: 5,
                        color: color,
                        lineType: CV.CvEnum.LineType.AntiAlias);
                }


                // notch bouding & position
                int candidateNumber = 0;
                foreach(MarkTypeCandidate candidate in markTypeCandidates)
                {
                    CV.Mat debugMatchingImage = new CV.Mat();

                    CV.CvInvoke.Normalize(
                        src: candidate.MatchingResult,
                        dst: debugMatchingImage,
                        alpha: 0, beta: 255,
                        normType: CV.CvEnum.NormType.MinMax,
                        dType: CV.CvEnum.DepthType.Cv8U);

                    //CV.CvInvoke.ApplyColorMap(debugMatchingImage, debugMatchingImage, CV.CvEnum.ColorMapType.Jet);
                    CV.CvInvoke.Imwrite(string.Format("D:/L{0}_candidate{1}.tif", _laneNumber, candidateNumber), debugMatchingImage);
                    Console.WriteLine($"L{_laneNumber}, candidate{candidateNumber} : score: {candidate.MatchingScore()}");

                    CV.CvInvoke.Rectangle(_polarImageDebug, candidate.Patch, new CV.Structure.MCvScalar(255, 0, 255));

                    CV.CvInvoke.Circle(
                        img: _polarImageDebug, 
                        center: candidate.MatchingPositionPolar, 
                        radius: 5, 
                        color: new CV.Structure.MCvScalar(0, 128, 255), 
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _polarImageDebug,
                        center: candidate.MatchingPositionPolar,
                        radius: 8, 
                        color: new CV.Structure.MCvScalar(0, 128, 255), 
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _polarImageDebug,
                        center: candidate.MatchingPositionPolar,
                        radius: 13, 
                        color: new CV.Structure.MCvScalar(0, 128, 255), 
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _polarImageDebug,
                        center: candidate.MatchingPositionPolar,
                        radius: 21, 
                        color: new CV.Structure.MCvScalar(0, 128, 255), 
                        lineType: CV.CvEnum.LineType.AntiAlias);

                    Point matchingPositionCartesian = polarConverter.ConvertPolarToCartesianPoint(candidate.MatchingPositionPolar);

                    CV.CvInvoke.Circle(
                        img: _cartesianImageDebug,
                        center: matchingPositionCartesian,
                        radius: 5,
                        color: new CV.Structure.MCvScalar(0, 128, 255),
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _cartesianImageDebug,
                        center: matchingPositionCartesian,
                        radius: 8,
                        color: new CV.Structure.MCvScalar(0, 128, 255),
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _cartesianImageDebug,
                        center: matchingPositionCartesian,
                        radius: 13,
                        color: new CV.Structure.MCvScalar(0, 128, 255),
                        lineType: CV.CvEnum.LineType.AntiAlias);
                    CV.CvInvoke.Circle(
                        img: _cartesianImageDebug,
                        center: matchingPositionCartesian,
                        radius: 21,
                        color: new CV.Structure.MCvScalar(0, 128, 255),
                        lineType: CV.CvEnum.LineType.AntiAlias);

                    candidateNumber++;
                }

                // alignResult tracing
                CV.CvInvoke.Line(
                    img: _polarImageDebug,
                    pt1: new Point(0, alignResult.AngularPosition - 1),
                    pt2: new Point(_polarImageDebug.Width, alignResult.AngularPosition - 1),
                    color: new CV.Structure.MCvScalar(255, 0, 255));

                CV.CvInvoke.Line(
                    img: _polarImageDebug,
                    pt1: new Point(0, alignResult.AngularPosition + 1),
                    pt2: new Point(_polarImageDebug.Width, alignResult.AngularPosition + 1),
                    color: new CV.Structure.MCvScalar(255, 0, 255));

                Point cartesianNotch = theoriticalEllipse.CartesianPointFromAngle(
                    -theoriticalEllipse.AngleRad - alignResult.AngleCorrection(AngleUnit.Rad), 
                    AngleUnit.Rad);

                CV.CvInvoke.Line(
                    img: _cartesianImageDebug,
                    pt1: foundEllipse.CenterInt,
                    pt2: cartesianNotch,
                    color: new CV.Structure.MCvScalar(255, 0, 255),
                    thickness: 4,
                    lineType: CV.CvEnum.LineType.AntiAlias);

                CV.CvInvoke.Imwrite(string.Format("D:/DummyPolar{0}.tif", _laneNumber), _polarImageDebug);
                CV.CvInvoke.Imwrite(string.Format("D:/DummyCartesian{0}.tif", _laneNumber), _cartesianImageDebug);
            }

            return alignResult;
        }

        private Ellipse CreateTheoriticalEllipse()
        {
            PointF theoriticalCenter = _wafer.TheoriticalWaferCenter(
                    _laneImage.LaneNumber,
                    _laneImage.GrabDirection,
                    _laneImage.Side);

            double theoriticalRadiusPx = _laneImage.ToPixels(um: _wafer.Radius);

            Ellipse theoriticalEllipse = new Ellipse(
                centerX: theoriticalCenter.X,
                centerY: theoriticalCenter.Y,
                halfAxisX: theoriticalRadiusPx,
                halfAxisY: theoriticalRadiusPx,
                angle: 0);

            Console.WriteLine(string.Format("lane {0}: theoritical ellipse: (center: {1}, size: {2}, angle: {3}",
                _laneNumber, theoriticalEllipse.Center, theoriticalEllipse.Size, theoriticalEllipse.AngleDeg));

            return theoriticalEllipse;
        }

        private IList<Tuple<int, int>> getValidAngularAreas(Ellipse ellipse, PolarConverter polarConverter)
        {
            IList<Tuple<int, int>> validAngularAreas = new List<Tuple<int, int>>();

            float laneCount = _wafer.AlignImages.Count;

            double deltaLeft = Math.Sqrt(Math.Pow(ellipse.AverageRadius, 2)
                - Math.Pow((laneCount / 2 - _laneNumber - 1) * _grayCartesianImage.Width, 2));

            double deltaRight = Math.Sqrt(Math.Pow(ellipse.AverageRadius, 2) 
                - Math.Pow((laneCount / 2 - _laneNumber) * _grayCartesianImage.Width, 2));

            if (laneCount == 1)
            {
                validAngularAreas.Add(new Tuple<int, int>(0, _polarImage.Height));
            }
            else
            {
                if (0 < _laneNumber &&  _laneNumber < laneCount - 1) // central image
                {

                    int xLeft, xRight;

                    if (_laneNumber < (float)laneCount / 2)
                    {
                        xLeft = 0;
                        xRight = _grayCartesianImage.Width;
                    }
                    else if (_laneNumber > (float)laneCount / 2)
                    {
                        xLeft = _grayCartesianImage.Width;
                        xRight = 0;
                    }
                    else
                    {
                        throw new Exception();
                    }

                    Point pointBotLeft = new Point(
                        x: xLeft,
                        y: (int)(_grayCartesianImage.Height / 2 + deltaLeft));

                    Point pointBotRight = new Point(
                        x: xRight,
                        y: (int)(_grayCartesianImage.Height / 2 + deltaRight));

                    validAngularAreas.Add(new Tuple<int, int>(
                       polarConverter.ConvertCartesianToPolarPoint(pointBotRight).Y,
                       polarConverter.ConvertCartesianToPolarPoint(pointBotLeft).Y));

                    Point pointTopLeft = new Point(
                        x: xLeft,
                        y: (int)(_grayCartesianImage.Height / 2- deltaLeft));

                    Point pointTopRight = new Point(
                        x: xRight,
                        y:(int)(_grayCartesianImage.Height / 2 - deltaRight));

                    validAngularAreas.Add(new Tuple<int, int>(
                       polarConverter.ConvertCartesianToPolarPoint(pointTopLeft).Y,
                       polarConverter.ConvertCartesianToPolarPoint(pointTopRight).Y));

                }
                else // border image
                {
                    int xPosition = _laneNumber == 0 ? 0 : _grayCartesianImage.Width;

                    double delta = _laneNumber == 0 ? deltaLeft : deltaRight;

                    Point p1 = new Point(
                       x: xPosition,
                       y: (int)(_grayCartesianImage.Height / 2 - delta));

                    Point p2 = new Point(
                        x: xPosition,
                        y: (int)(_grayCartesianImage.Height / 2 + delta));

                    Point polarP1 = polarConverter.ConvertCartesianToPolarPoint(p1);
                    Point polarP2 = polarConverter.ConvertCartesianToPolarPoint(p2);

                    validAngularAreas.Add(new Tuple<int, int>(
                        _laneNumber == 0 ? polarP1.Y : polarP2.Y,
                        _laneNumber == 0 ? polarP2.Y : polarP1.Y));
                }
            }

            foreach (Tuple<double, double> maskedArea in GetBracketsAngles())
            {
                maskedArea.Deconstruct(out double angleStart, out double angleEnd);

                // switch to ClockWise because of the orientation of the polar image
                double CWangleStart = 360 - angleEnd;
                double CWangleEnd = 360 - angleStart;

                int maskStart = polarConverter.ConvertAngleToAngularPosition(CWangleStart, AngleUnit.Deg);
                int maskEnd = polarConverter.ConvertAngleToAngularPosition(CWangleEnd, AngleUnit.Deg);

                Point pointStart = ellipse.CartesianPointFromAngle(CWangleStart, AngleUnit.Deg);
                Point pointEnd = ellipse.CartesianPointFromAngle(CWangleEnd, AngleUnit.Deg);

                IList<Tuple<int, int>> maskedAreas = new List<Tuple<int, int>>();

                foreach (Tuple<int, int> angularArea in validAngularAreas)
                {
                    angularArea.Deconstruct(out int angularStart, out int angularEnd);

                    if ((angularStart < maskStart && maskEnd < angularEnd)
                        || (angularStart > angularEnd && (angularStart < maskStart || maskEnd < angularEnd)))
                    {
                        maskedAreas.Add(new Tuple<int, int>(angularStart, maskStart));
                        maskedAreas.Add(new Tuple<int, int>(maskEnd, angularEnd));
                    }
                    else if (maskStart < angularStart && angularStart < maskEnd)
                    {
                        maskedAreas.Add(new Tuple<int, int>(maskEnd, angularEnd));
                    }
                    else if (maskStart < angularEnd && angularEnd < maskEnd)
                    {
                        maskedAreas.Add(new Tuple<int, int>(angularStart, maskStart));
                    }
                    else
                    {
                        maskedAreas.Add(angularArea);
                    }
                }
                validAngularAreas = maskedAreas;
            }
            return validAngularAreas;
        }

        private IList<Point> getEdgePointsPolar(int edgePointsDistance)
        {
            IList<Point> polarEdgePoints = new List<Point>();

            List<int> positions = new List<int>();

            foreach (Tuple<int, int> validAngularArea in _validAngularAreas)
            {
                validAngularArea.Deconstruct(out int areaStart, out int areaEnd);

                if (areaStart < areaEnd)
                {
                    for (int position = areaStart; position < areaEnd; position += edgePointsDistance) positions.Add(position);
                }
                else // areaStart > areaEnd
                {
                    for (int position = areaStart; position < _polarImage.Height; position += edgePointsDistance) positions.Add(position);
                    for (int position = 0; position < areaEnd; position += edgePointsDistance) positions.Add(position);
                }
            }

            foreach (int row in positions)
            {
                CV.Mat lineImage = _polarImage.Row(row);
                int col = FindEdgePosition(lineImage);
                if (col != 0)
                {
                    polarEdgePoints.Add(new Point(col, row));
                }
            }
            
            return polarEdgePoints;
        }

        private int FindEdgePosition(CV.Mat lineImage)
        {
            CV.Mat transitionPoints = new CV.Mat(
                size: lineImage.Size, 
                type: lineImage.Depth, 
                channels: lineImage.NumberOfChannels);

            CV.CvInvoke.Filter2D(
                src: lineImage,
                dst: transitionPoints,
                kernel: _filterKernel,
                anchor: new Point(-1, -1),
                delta: 0,
                borderType: CV.CvEnum.BorderType.Replicate);

            double[] minValues, maxValues;
            Point[] minPositions, maxPositions;
            transitionPoints.MinMax(out minValues, out maxValues, out minPositions, out maxPositions);

            return maxValues[0] > 20 ? maxPositions[0].X : 0;
        }

        private Ellipse GetEllipse(IList<Point> edgePointsCartesian)
        {
            CV.Util.VectorOfPoint edgePointsCartesianCV = new CV.Util.VectorOfPoint(edgePointsCartesian.ToArray());
            CV.Structure.RotatedRect cvEllipse = CV.CvInvoke.FitEllipse(edgePointsCartesianCV);

            Console.WriteLine($"lane {_laneNumber}: found ellipse: (center: {cvEllipse.Center}, size: {cvEllipse.Size}, angle: {cvEllipse.Angle}");

            return new Ellipse(cvEllipse);
        }

        private IList<MarkTypeCandidate> findMarkTypeCandidates(Ellipse ellipse)
        {
            IList<MarkTypeCandidate> candidates = new List<MarkTypeCandidate>();

            bool inCandidate = false;
            double dist;

            MarkTypeCandidate candidate = new MarkTypeCandidate(markTypePadding: GetMarkTypePadding());
            Rectangle patch = candidate.Patch;
            foreach (var polarCartesian in _edgePointsPolar.Zip(_edgePointsCartesian, (first, second) => new Tuple<Point, Point>(first, second)))
            {
                polarCartesian.Deconstruct(out Point polarPoint, out Point cartesianPoint);

                dist = Math.Abs(
                        Math.Sqrt(
                            Math.Pow(cartesianPoint.X - ellipse.CenterX, 2)
                            + Math.Pow(cartesianPoint.Y - ellipse.CenterY, 2))
                        - ellipse.AverageRadius);

                if (dist > _laneImage.ToPixels(GetMarkTypeThreshold()))
                {
                    if (inCandidate)    // extend the boundary of the patch if necessary
                    {
                        if (polarPoint.X < patch.X) patch.X = polarPoint.X;
                        if (polarPoint.Y < patch.Y) patch.Y = polarPoint.Y;

                        if (polarPoint.X - patch.X > patch.Width) patch.Width = polarPoint.X - patch.X;
                        if (polarPoint.Y - patch.Y > patch.Height) patch.Height = polarPoint.Y - patch.Y;
                    }
                    else  // new candidate
                    {
                        patch.X = polarPoint.X;
                        patch.Y = polarPoint.Y;
                        inCandidate = true;
                    }
                }
                if (inCandidate && dist < _laneImage.ToPixels(GetMarkTypeThreshold()))
                {
                    inCandidate = false;
                    if (patch.Width > GetEdgePixelsDistance() * 5 && patch.Width > GetEdgePixelsDistance() * 5) // check if the candidate patch is large enough 
                    {
                        patch.X = Math.Max(0, patch.X - GetMarkTypePadding());
                        patch.Y = Math.Max(0, patch.Y - GetMarkTypePadding()); ;
                        patch.Width = Math.Min(_polarImage.Width - patch.X, patch.Width + GetMarkTypePadding() * 2);
                        patch.Height = Math.Min(_polarImage.Height - patch.Y, patch.Height + GetMarkTypePadding() * 2);
                        candidate.Patch = patch;
                        Console.WriteLine(string.Format("Lane {0}: marktype candidate: {1}", _laneNumber, candidate.Patch));
                        candidates.Add(candidate);
                    }
                    candidate = new MarkTypeCandidate(markTypePadding: GetMarkTypePadding());
                    patch = candidate.Patch;
                }
            }

            return candidates;
        }

        private bool IsDebug()
        {
            return true;
        }
        private int PolarAngularSteps(int waferRadius)
        {
            // assuming we want a 1:1 ratio between the cartesian and polar images for the pixels on the (theoritical) edge of the wafer
            int polarAngularSteps = (int)Math.Round(2 * Math.PI * _laneImage.ToPixels(waferRadius));
            #warning limiting the maximum value to maximum short because of the limitation of CV::Remap
            return Math.Min(polarAngularSteps, short.MaxValue - 1);
        }
        private int GetInnerMargin()
        {
            #warning implementation of dynamic inner margin (with a xml file or so) required
            return 5000;
        }
        private int GetOuterMargin()
        {
            #warning implementation of dynamic outer margin (with a xml file or so) required
            return 5000;
        }
        private int GetKernelLeftSize()
        {
            return 100;
        }
        private int GetKernelRightSize()
        {
            return 100;
        }
        private int GetEdgePixelsDistance()
        {
            #warning implementation of sampling (with a xml file or so) required
            return 5;
        }
        private int GetMarkTypeThreshold()
        {
            return 500;
        }
        private int GetMarkTypePadding()
        {
            return 10;
        }
        private IList<Tuple<double, double>> GetBracketsAngles()
        {
            IList<Tuple<double, double>> bracketAngles = new List<Tuple<double, double>>();

            //bracketAngles.Add(new Tuple<double, double>(2.5, 17.5));
            //bracketAngles.Add(new Tuple<double, double>(53.5, 68.5));
            //bracketAngles.Add(new Tuple<double, double>(113.5, 128.5));
            //bracketAngles.Add(new Tuple<double, double>(164.5, 179.5));
            //bracketAngles.Add(new Tuple<double, double>(214.5, 229.5));
            //bracketAngles.Add(new Tuple<double, double>(311.5, 326.5));

            return bracketAngles;
        }
    }
}
