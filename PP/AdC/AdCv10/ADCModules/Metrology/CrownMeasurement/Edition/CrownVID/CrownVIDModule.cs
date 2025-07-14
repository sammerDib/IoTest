using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using ADCEngine;
using AdcTools;
using CrownMeasurementModule.Objects;
using LibProcessing;
using AdcBasicObjects;
using CrownMeasurementModule.CrownProfile;
using static CrownMeasurementModule.Objects.ProfileMeasure;
using static CrownMeasurementModule.Objects.ProfileMeasure.CrownProfileModule_ProfileStat;
using BasicModules.Edition.DataBase;
using FormatCRW;
using BasicModules.VidReport;
using AdcRobotExchange;
using Database.Service;
using GalaSoft.MvvmLight.Ioc;
using BasicModules;

namespace CrownMeasurementModule.CrownEditor
{
    public class CrownVIDModule : VidReportModule
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();


        bool _GlassDiameters;
        bool _TaikoRing;
        bool _EBR;
        bool _RingScan;
        bool _TaikoShoulder;
        
        //public VidReportParameter paramVID;
        

        private CustomExceptionDictionary<int, VidEdgeMeasure> vidMap;

        IToolService toolService = SimpleIoc.Default.GetInstance<IToolService>();

        //=================================================================
        // Constructeur
        //=================================================================
        public CrownVIDModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            //paramCategories = new VidReportParameter(this, "Categories");      
        }
        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // Création de la liste des VIDs
            //-------------------------------------------------------------
            vidMap = new CustomExceptionDictionary<int, VidEdgeMeasure>(exceptionKeyName: "VID");
            foreach (ReportClass cat in paramCategories.ReportClasses.Values)
            {
                VidEdgeMeasure vid;
                bool exists = vidMap.TryGetValue(cat.VID, out vid);
                if (!exists)
                {
                    vid = new VidEdgeMeasure();
                    vid.VidNumber = cat.VID;
                    vid.VidLabel = cat.VidLabel;
                    vidMap[cat.VID] = vid;
                }
            }

            //-------------------------------------------------------------
            // Envoie de la liste des inputs
            //-------------------------------------------------------------
            SendInputList();

        }
        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            SendVids();
            base.OnStopping(oldState);
        }
        //=================================================================
        // 
        //=================================================================
        private void SendInputList()
        {
            //-------------------------------------------------------------
            // Construction de la liste des inputs
            //-------------------------------------------------------------
            List<AdcInput> adcInputList = new List<AdcInput>();
            foreach (var input in Recipe.InputInfoList.OfType<BasicModules.DataLoader.InspectionInputInfoBase>())
            {
                AdcInput adcInput = new AdcInput();
                adcInputList.Add(adcInput);

                adcInput.ModuleID = (int)input.ModuleID;
                adcInput.ChannelID = (int)input.ChannelID;
                adcInput.InputPictureDirectory = input.Folder;
            }

            //-------------------------------------------------------------
            // Envoi
            //-------------------------------------------------------------
            ADC.Instance.TransferToRobotStub.TransferInputList(Recipe.Toolname, Recipe.Wafer.UniqueID, adcInputList);
        }

        //=================================================================
        // 
        //=================================================================
        private void SendVids()
        {
            if (vidMap != null)
            {
                List<VidBase> list = vidMap.Values.Cast<VidBase>().ToList();
                ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, Recipe.Wafer.UniqueID, list);
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

            ProcessChildren(obj);
        }

    }
}
