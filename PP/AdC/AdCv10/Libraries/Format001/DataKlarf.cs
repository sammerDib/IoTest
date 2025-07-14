using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Format001
{

    public class DataKlarf
    {
        #region KLARF_HEADER 
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public String FileVersion { get; set; } = "1 2";
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public DateTime FileTimeStamp { get; set; } = DateTime.MinValue;
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String InspectionStationID { get; set; } = String.Empty;
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public String SampleType { get; set; } = "WAFER";
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public DateTime ResultTimeStamp { get; set; } = DateTime.MinValue;
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String LotID { get; set; } = String.Empty;
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSampleSize SampleSize { get; set; } = new PrmSampleSize(200);
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String DeviceID { get; set; } = "RPCS_DEVICE_ID";
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSetupId SetupID { get; set; } = new PrmSetupId("000", DateTime.MinValue);
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String StepID { get; set; } = "RPCS_STEP_ID";
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmSampleOrientationMarkType SampleOrientationMarkType { get; set; } = new PrmSampleOrientationMarkType(0);
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationMarkValue OrientationMarkLocation { get; set; } = new PrmOrientationMarkValue(0);
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String WaferID { get; set; } = String.Empty;
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public int SlotID { get; set; } = 0;
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationInstruction OrientationInstructions { get; set; } = new PrmOrientationInstruction(0);
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public PrmOrientationMarkValue InspectionOrientation { get { return OrientationMarkLocation; } set { OrientationMarkLocation = value; } }
        [Category("Header"), Browsable(true), ReadOnly(false)]
        public String TiffFileName { get; set; } = String.Empty;
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat DiePitch { get; set; } = new PrmPtFloat(0.0f, 0.0f);
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat DieOrigin { get; set; } = new PrmPtFloat(0.0f, 0.0f);
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmPtFloat DieSize { get; set; } = new PrmPtFloat(0.0f, 0.0f);
        [Category("Header"), Browsable(true), ReadOnly(true)]

        public PrmPtFloat SampleCenterLocation { get; set; } = new PrmPtFloat(0.0f, 0.0f);
        [Category("Header"), Browsable(false), ReadOnly(true)]
        public int InspectionTest { get; set; } = 1;
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmYesNo CoordinatesMirrored { get; set; } = new PrmYesNo(false);
        [Category("Header"), Browsable(true), ReadOnly(true)]
        public PrmYesNo CoordinatesCentered { get; set; } = new PrmYesNo(false);
        [Category("Header"), Browsable(false), ReadOnly(true)]
        public String ClusterClassificationList { get; private set; } = "1 1 200";
        [Category("Header"), Browsable(false), ReadOnly(true)]
        public double AreaPerTest
        {
            get
            {
                if (isSquaredWafer)
                {
                    double s = (double)(SquareSizemm.x * 1000 - edgeSize_um) * (SquareSizemm.y * 1000.0 - edgeSize_um);
                    return s;
                }
                else
                {
                    if (NbDies > 0) // c'est du die
                    {
                        return NbDies * DieSize.x * DieSize.y;
                    }
                    else
                    {
                        double d = (double)(SampleSize.waferDiameter_mm * 1000.0 - edgeSize_um * 2);
                        return ((d * d * 0.25) * Math.PI);
                    }
                }
            }
        }
        #endregion

        #region SAMPLE_TEST_PLAN
        [Category("SampleTestPlan"), Browsable(true), ReadOnly(true)]
        public PrmSampleTestPlan SampleTestPlan { get; set; } = new PrmSampleTestPlan();
        #endregion

        #region DEFECTS
        [Category("Defects"), Browsable(false), ReadOnly(true)]
        public String DefectRecordSpec
        {
            get
            {
                String s = DefectListType.List.Count.ToString();
                s += " ";
                foreach (PrmDefectType dtype in DefectListType.List)
                {
                    s += dtype.ToString();
                    s += " ";
                }
                // remove last whitespece 
                s = s.Remove(s.Length - 1);
                return s;
            }
        }

        public void SortByClusterNum()
        {
            String sCat = "CLUSTERNUMBER";
            if (DefectListType.Parse(sCat) != null)
            {
                _defectList = DefectList.OrderBy(x => (int)x.Get(sCat)).ToList();

                String sCatId = "DEFECTID";
                if (DefectListType.Parse(sCatId) != null)
                {
                    int nStart = DefectList.Count % 2;
                    if (nStart == 1)
                        DefectList[0].Set(sCatId, 1);
                    for (int n = nStart; n < DefectList.Count; n += 2) // On réindex les defects de 1 = Count=
                    {
                        DefectList[n].Set(sCatId, n + 1);
                        DefectList[n + 1].Set(sCatId, n + 2);
                    }
                }
                int writingClusterNumber = 0;
                int currentclusterNumber = 0;
                int lastclusterNumber = 0;
                for (int n = 0; n < DefectList.Count; n++)
                {
                    currentclusterNumber = (int)DefectList[n].Get(sCat);
                    if (lastclusterNumber != currentclusterNumber)
                    {
                        writingClusterNumber++;
                        lastclusterNumber = currentclusterNumber;
                    }

                    DefectList[n].Set(sCat, writingClusterNumber);
                }
            }
        }

        // defect type should be added prior to defect
        private PrmDefectTypeList DefectListType = new PrmDefectTypeList();
        public void ResetDefectListType()
        {
            DefectListType = new PrmDefectTypeList();
        }
        public void AddDefectType(Type p_type, String p_name)
        {
            DefectListType.Add(p_type, p_name);
        }
        public void AddDefectType(String p_RegisteredName)
        {
            DefectListType.Add(p_RegisteredName);
        }
        public void AddDefectTypes(String[] p_RegisteredNames)
        {
            DefectListType.Add(p_RegisteredNames);
        }


        // add defect should be done after all defect types has been added
        private List<PrmDefect> _defectList = new List<PrmDefect>();
        public List<PrmDefect> DefectList
        {
            get { return _defectList; }
        }


        public PrmDefect NewDefect()
        {
            return new PrmDefect(DefectListType);
        }
        public void AddDefect(PrmDefect prm)
        {
            DefectList.Add(prm);
        }
        public void AddListDefect(List<PrmDefect> lprm)
        {
            DefectList.AddRange(lprm);
        }
        public void WriteDefectList(StreamWriter wstream)
        {
            wstream.WriteLine("DefectList");
            for (int i = 0; i < DefectList.Count - 1; i++)
                wstream.WriteLine(DefectList[i].ToString());
            if (DefectList.Count > 0)
                wstream.WriteLine(DefectList[DefectList.Count - 1]);
            wstream.WriteLine(";");
        }

        #endregion

        #region FOOTER
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public String EquipmentID { get; set; } = String.Empty;
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int TESTNO { get { return 1; } }
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDEFECT { get { return DefectList.Count; } }
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public double DEFDENSITY { get { return ((double)NDEFECT / AreaPerTest); } }
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDIE { get; set; } = 1;
        [Category("FOOTER"), Browsable(true), ReadOnly(true)]
        public int NDEFDIE { get; set; } = 1;
        public void WriteFooter(StreamWriter wstream)
        {
            wstream.Write("ProcessEquipmentIDList 1 \""); wstream.Write(EquipmentID); wstream.WriteLine("\";");
            wstream.WriteLine("SummarySpec 5");
            wstream.WriteLine("TESTNO NDEFECT DEFDENSITY NDIE NDEFDIE;");
            wstream.WriteLine("SummaryList");
            wstream.Write(TESTNO); wstream.Write(" ");
            wstream.Write(NDEFECT); wstream.Write(" ");
            wstream.Write(DEFDENSITY.ToString("E10")); wstream.Write(" ");
            wstream.Write(NDIE); wstream.Write(" ");
            wstream.Write(NDEFDIE);
            wstream.WriteLine(";");
            wstream.WriteLine("EndOfFile;");
        }
        #endregion

        #region  Specific
        public bool isSquaredWafer { get; set; } = false;   // wafer with rectangular shape
        public PrmPtFloat SquareSizemm { get; set; } = null;// wafer rectangular Size X&Y in mm (complementary information of SampleSize)
        public Double edgeSize_um { get; set; } // Wafer edge defined in module EdgeRemoveModule otherwise 0
        public int NbDies { get; set; }  // Nombre de dies pour le paterné
        #endregion
    }
}
