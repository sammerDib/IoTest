using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CV = Emgu.CV;

namespace UnitySC.PM.WOTAN.Processing.Types
{
    class MarkTypeCandidate
    {
        private int _markTypePadding;

        private Rectangle _patch;
        private CV.Mat _matchingResult;
        private Point _matchingPositionPatch, _matchingPositionPolar;
        private double _minValue, _maxValue;

        public MarkTypeCandidate(int markTypePadding)
        {
            _markTypePadding = markTypePadding;

            _patch = new Rectangle();
            _matchingResult = new CV.Mat();
            _matchingPositionPatch = new Point();
            _matchingPositionPolar = new Point();
            _minValue = double.MaxValue;
            _maxValue = double.MinValue;
        }

        public Rectangle Patch { get => _patch; set => _patch = value; }
        public CV.Mat MatchingResult { get => _matchingResult; }
        //public double MatchingScore { get => _matchingScore; set => _matchingScore = value; }
        public Point MatchingPositionPatch { get => _matchingPositionPatch; set => _matchingPositionPatch = value; }
        public Point MatchingPositionPolar { get => _matchingPositionPolar; set => _matchingPositionPolar = value; }

        public Rectangle BoundedSearchArea(Size boundary)
        {
            Rectangle searchArea = new Rectangle();
            searchArea.X = Math.Max(_patch.X - _patch.Width, 0);
            searchArea.Y = Math.Max(_patch.Y - _patch.Height, 0);
            searchArea.Width = Math.Min(_patch.Width * 3, boundary.Width - searchArea.X);
            searchArea.Height = Math.Min(_patch.Height * 3, boundary.Height - searchArea.Y);

            return searchArea;
        }

        public void ComputeMatching(CV.Mat image)
        {
            Rectangle searchArea = BoundedSearchArea(image.Size);
            CV.Mat searchImage = new CV.Mat(
                mat: image,
                roi: searchArea);

            CV.Mat patchImage = new CV.Mat(
                mat: image,
                roi: _patch);

            CV.CvInvoke.Flip(
                src: patchImage,
                dst: patchImage,
                flipType: CV.CvEnum.FlipType.Vertical);

            CV.CvInvoke.MatchTemplate(
                image: searchImage,
                templ: patchImage,
                _matchingResult,
                method: CV.CvEnum.TemplateMatchingType.CcorrNormed);

            Point minLoc = new Point();

            CV.CvInvoke.MinMaxLoc(_matchingResult, ref _minValue, ref _maxValue, ref minLoc, ref _matchingPositionPatch);

            _matchingPositionPolar.X = _matchingPositionPatch.X + searchArea.X + _markTypePadding;
            _matchingPositionPolar.Y = _matchingPositionPatch.Y + searchArea.Y + _patch.Height / 2;
        }

        public double MatchingScore()
        {
            double halfWidth = 1;
            double threshold = (_minValue + _maxValue) / 2;

            int y;
            double lastValue;

            CV.Mat maxLine = _matchingResult.T().Row(_matchingPositionPatch.X);
            unsafe
            {
                float* lineArray = (float*)maxLine.GetDataPointer().ToPointer();
                lastValue = _maxValue;
                y = _matchingPositionPatch.Y - 1;
                while (y >= 0 
                    && lineArray[y] > threshold 
                    //&& lineArray[y] < lastValue
                    )
                {
                    lastValue = lineArray[y];
                    halfWidth++;
                    y--;
                }

                lastValue = _maxValue;
                y = _matchingPositionPatch.X + 1;
                while (y < maxLine.Height 
                    && lineArray[y] > threshold 
                    //&& lineArray[y] < lastValue
                    )
                {
                    lastValue = lineArray[y];
                    halfWidth++;
                    y++;
                }
            }

            return halfWidth;
        }
    }
}
