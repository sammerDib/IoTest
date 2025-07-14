using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.WOTAN.Common
{
    public class Wafer
    {
        private int _radius;        
        private MarkType _marktype;
        private IList<LaneImage> _alignImages, _acquisationFrontImages, _acquisationBackImages; // Acquisation Images defined on SCAN NODE Servers

        public Wafer()
        {
            _alignImages = new List<LaneImage>();
            _acquisationFrontImages = new List<LaneImage>();
            _acquisationBackImages = new List<LaneImage>();
        }

        public int Radius { get => _radius; set => _radius = value; }
        public MarkType Marktype { get => _marktype; set => _marktype = value; }
        public IList<LaneImage> AlignImages { get => _alignImages; set => _alignImages = value; }
        public IList<LaneImage> AcquisationFrontImages { get => _acquisationFrontImages; set => _acquisationFrontImages = value; }
        public IList<LaneImage> AcquisationBackImages { get => _acquisationBackImages; set => _acquisationBackImages = value; }

        public PointF TheoriticalWaferCenter(int laneNumber, GrabDirection grabDirection, Side side)
        {
            IList<LaneImage> laneImages = null;
            switch (grabDirection)
            {
                case GrabDirection.ALIGNMENT:
                    laneImages = _alignImages;
                    break;
                case GrabDirection.ACQUISATION:
                    switch (side)
                    {
                        case Side.FRONT:
                            laneImages = _acquisationFrontImages;
                            break;
                        case Side.BACK:
                            laneImages = _acquisationBackImages;
                            break;
                    };
                    break;
                default:
                    throw new Exception();
            }
            LaneImage laneImage = laneImages[laneNumber];

            float expectedWidth = laneImage.Image.Width / 2
                + (grabDirection == GrabDirection.ALIGNMENT ? -1 : 1)                   // switch direction according to the direction of the acquisition 
                * laneImage.Image.Width * ((laneImages.Count - 1) / 2 - laneNumber);    // 1 offset for compensating 0-indexing
            
            float expectedHeight = laneImage.Image.Height / 2;

            return new PointF(
                x: expectedWidth,
                y: expectedHeight);
        }
    }
}
