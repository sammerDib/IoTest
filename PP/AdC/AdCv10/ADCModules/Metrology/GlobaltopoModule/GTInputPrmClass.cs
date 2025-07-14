using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace GlobaltopoModule
{
    public enum lpType
    {
        None = 0,
        Smooth = 1,
        Uniform = 2,
        Gaussian = 3
    }

    public class GTInputPrmClass : Serializable, IValueComparer
    {

        [XmlAttribute] public float EdgeExcusion_mm { get; set; }

        // Region 
        //                ^
        //                | 
        //                Y2 
        //                | 
        //                |
        //   ------ X1----+------X2---->     X1< WaferCenter < X2
        //                |
        //                |
        //               Y1
        //                |
        //       Y1 < WaferCenter < Y2
        //
        [XmlAttribute] public float X1_mm { get; set; } // en mm par rapport centre wafer (repere wafer) - default -25
        [XmlAttribute] public float X2_mm { get; set; } // en mm par rapport centre wafer (repere wafer) - default +25
        [XmlAttribute] public float Y1_mm { get; set; } // en mm par rapport centre wafer (repere wafer) - default -25
        [XmlAttribute] public float Y2_mm { get; set; } // en mm par rapport centre wafer (repere wafer) - default +25
        [XmlAttribute] public int NbSamples { get; set; } // echantillonage pour calcule du best fit plane - default 10
        [XmlAttribute] public float RadiusCenterBow { get; set; }   // en mm
        [XmlAttribute] public lpType LowPassKernelType { get; set; } //0: none; 1:Smooth; 2: uniform; 3:Gaussian - default 0 
        [XmlAttribute] public int LowPassKernelSize { get; set; }  // 3<=K<=21 //doit etre impaire !! - default 3
        [XmlAttribute] public float LowPassGaussianSigma { get; set; } // only for Low pass gaussian type =>  sigma used for gaussian - Default 1.0f

        [XmlArray("ExcludeAreas", IsNullable = true), XmlArrayItem(typeof(RectangleF), ElementName = "rcf")]
        public List<RectangleF> ExcludeAreasList { get; set; }

        public GTInputPrmClass()
        {
            EdgeExcusion_mm = 0.0f;
            X1_mm = -25.0f;
            X2_mm = +25.0f;
            Y1_mm = -25.0f;
            Y2_mm = +25.0f;
            NbSamples = 10;
            RadiusCenterBow = 10.0f;
            LowPassKernelType = lpType.None;
            LowPassKernelSize = 3;
            LowPassGaussianSigma = 1.0f;
            ExcludeAreasList = new List<RectangleF>();
        }

        static public bool ExcludeAreasEquals(List<RectangleF> ListA, List<RectangleF> ListB)
        {
            if (ListA == null)
            {
                return (ListB == null);
            }

            if (ListB == null)
                return false;

            if (ListA.Count != ListB.Count)
                return false;

            // here ListA and B have the same size
            for (int i = 0; i < ListA.Count; i++)
            {
                if (ListA[i] != ListB[i])
                    return false;
            }

            return true;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as GTInputPrmClass;
            if (@class != null)
            {
                bool IsSameExclusionsBoxes = ExcludeAreasEquals(ExcludeAreasList, @class.ExcludeAreasList); // check boxes exlusion

                return IsSameExclusionsBoxes &&
                  EdgeExcusion_mm == @class.EdgeExcusion_mm &&
                  X1_mm == @class.X1_mm &&
                  X2_mm == @class.X2_mm &&
                  Y1_mm == @class.Y1_mm &&
                  Y2_mm == @class.Y2_mm &&
                  NbSamples == @class.NbSamples &&
                  RadiusCenterBow == @class.RadiusCenterBow &&
                  LowPassKernelType == @class.LowPassKernelType &&
                  LowPassKernelSize == @class.LowPassKernelSize &&
                  LowPassGaussianSigma == @class.LowPassGaussianSigma;
            }
            else
                return false;
        }
    }
}
