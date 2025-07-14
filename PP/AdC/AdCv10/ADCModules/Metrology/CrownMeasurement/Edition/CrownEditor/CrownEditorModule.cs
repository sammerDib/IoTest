using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.DataBase;

using CrownMeasurementModule.Objects;

using FormatCRW;

using LibProcessing;

using static CrownMeasurementModule.Objects.ProfileMeasure;

namespace CrownMeasurementModule.CrownEditor
{
    public class CrownEditorModule : DatabaseEditionModule
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        private string _crownfFilename;

        private bool _GlassDiameters;
        private bool _TaikoRing;
        private bool _EBR;
        private bool _RingScan;
        private bool _TaikoShoulder;

        public readonly EnumColorBoxParameter paramColorEBR_Scan;
        public readonly EnumColorBoxParameter paramColorEdge_Scan;
        public readonly EnumColorBoxParameter paramColorTaikoRing;
        public readonly EnumColorBoxParameter paramColorTaikoShoulder;

        static private readonly object SavingLock = new object();

        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.Crown_CRW);
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public CrownEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramColorEBR_Scan = new EnumColorBoxParameter(this, "EBRScan");
            paramColorEBR_Scan.Value = EnumColorList.Red;

            paramColorEdge_Scan = new EnumColorBoxParameter(this, "EdgeScan");
            paramColorEdge_Scan.Value = EnumColorList.Blue;

            paramColorTaikoRing = new EnumColorBoxParameter(this, "TaikoRing");
            paramColorTaikoRing.Value = EnumColorList.Yellow;

            paramColorTaikoShoulder = new EnumColorBoxParameter(this, "TaikoShoulder");
            paramColorTaikoShoulder.Value = EnumColorList.Green;


        }
        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _crownfFilename = GetResultFullPathName(ResultTypeFile.Crown_CRW);
            // on détruit ce fichier car il d
            if (File.Exists(_crownfFilename))
            {
                File.Delete(_crownfFilename);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ProfileMeasure profileMeasure = (ProfileMeasure)obj;

            _GlassDiameters = profileMeasure.glassDiameters;
            _EBR = profileMeasure.EBR;
            _RingScan = profileMeasure.ringScan;
            _TaikoRing = profileMeasure.taikoRing;
            _TaikoShoulder = profileMeasure.taikoShoulder;

            lock (SavingLock)
            {
                WriteEdgeResult(_crownfFilename, profileMeasure._profile, profileMeasure);
            }

            //ProcessChildren(obj);
        }

        //-----------------------------------------------------
        private void WriteEdgeResult(String sEdgeResultFile, List<CrownProfileModule_ProfileStat> pListOfProfils, ProfileMeasure profileMeasure)
        {
            List<ProfilScan> vListProfil_IN = new List<ProfilScan>();
            List<DataMeasure> vListData_IN = new List<DataMeasure>();

            if (File.Exists(sEdgeResultFile)) // déjà créé par un autre module CrownEditor
            {
                CRWXmlReader oReader = new CRWXmlReader(sEdgeResultFile);
                vListProfil_IN = oReader.ProfilScans;
                vListData_IN = oReader.DataMeasures;
            }

            #region CALCUL_EBR
            if (_EBR)
            {
                ProfilScan scan1;
                scan1.sLabel = "EBR_Scan";
                scan1.cColor = Color.FromName(paramColorEBR_Scan.Value.ToString());
                scan1.DeltaList = new List<double>();
                foreach (CrownProfileModule_ProfileStat pProfile in pListOfProfils)
                {
                    scan1.DeltaList.Add(pProfile.valueCrownSizeAverageEBR);
                }
                vListProfil_IN.Add(scan1);
            }
            #endregion

            #region CALCUL_TAIKO_RING
            if (_TaikoRing)
            {
                ProfilScan scanRing;
                scanRing.sLabel = "TAIKO_Ring";
                scanRing.cColor = Color.FromName(paramColorTaikoRing.Value.ToString());
                scanRing.DeltaList = new List<double>();

                foreach (CrownProfileModule_ProfileStat pProfile in pListOfProfils)
                {
                    scanRing.DeltaList.Add(pProfile.valueRingSizeAverageTaiko);
                }
                vListProfil_IN.Add(scanRing);

                ProfilScan scanShoulder;
                scanShoulder.sLabel = "TAIKO_Shoulder";
                scanShoulder.cColor = Color.FromName(paramColorTaikoShoulder.Value.ToString());
                scanShoulder.DeltaList = new List<double>();

                foreach (CrownProfileModule_ProfileStat pProfile in pListOfProfils)
                {
                    scanShoulder.DeltaList.Add(pProfile.valueShoulderSizeAverageTaiko);
                }
                vListProfil_IN.Add(scanShoulder);
            }
            #endregion

            #region CALCUL_EDGESCAN
            if (_RingScan)
            {
                ProfilScan scan1;
                scan1.sLabel = "EDGE_Scan";
                scan1.cColor = Color.FromName(paramColorEdge_Scan.Value.ToString());
                scan1.DeltaList = new List<double>();
                // tracé du profils EBR
                foreach (CrownProfileModule_ProfileStat pProfile in pListOfProfils)
                {
                    scan1.DeltaList.Add(pProfile.valueCrownSizeAverageEdg);
                }
                vListProfil_IN.Add(scan1);
            }
            #endregion

            //foreach (KeyValuePair<string, CVIDProcessMeasurement> pKeyPair in pDictonaryToAdd)
            //{
            //    DataMeasure mes;
            //    mes.sLabel = pKeyPair.Key;
            //    mes.sValue = pKeyPair.Value.m_lfMeasurementValue.ToString("#0.00");
            //    vListData_IN.Add(mes);
            //}


            CRWXmlWriter oWriter = new CRWXmlWriter(vListProfil_IN, vListData_IN);

            oWriter.Save(sEdgeResultFile);
        }

    }
}
