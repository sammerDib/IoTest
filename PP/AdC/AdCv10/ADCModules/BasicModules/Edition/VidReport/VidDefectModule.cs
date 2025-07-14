using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcRobotExchange;

using AdcTools;

namespace BasicModules.VidReport
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class VidDefectModule : VidReportModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Autres membres
        //=================================================================
        private CustomExceptionDictionary<int, VidDefect> vidMap;

        //=================================================================
        // Constructeur
        //=================================================================
        public VidDefectModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
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
            vidMap = new CustomExceptionDictionary<int, VidDefect>(exceptionKeyName: "Defect");
            foreach (ReportClass cat in paramCategories.ReportClasses.Values)
            {
                VidDefect vid;
                bool exists = vidMap.TryGetValue(cat.VID, out vid);
                if (!exists)
                {
                    vid = new VidDefect();
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
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Cluster cluster = (Cluster)obj;
            bool report = ComputeVid(cluster);

            // Ce module est un pass-through !
            if (report)
                ProcessChildren(obj);
        }

        //=================================================================
        /// <summary>
        /// Range un cluster dans le VID correspondant
        /// </summary>
        /// <returns>true si le cluster est rangé dans un "vrai" VID, 
        /// false si le cluster est dans un VID négatif. </returns>
        //=================================================================
        private bool ComputeVid(Cluster cluster)
        {
            //-------------------------------------------------------------
            // Vid
            //-------------------------------------------------------------
            ReportClass cat = paramCategories.ReportClasses[cluster.DefectClass];
            VidDefect vid = vidMap[cat.VID];

            if (vid.VidNumber <= 0)
            {
                logDebug("cluster: " + cluster + " VID:" + vid.VidNumber + " " + vid.VidLabel);
                return false;   // false alarm :-)
            }

            //-------------------------------------------------------------
            // Calcul du bin
            //-------------------------------------------------------------
            double measure = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];

            int i;
            for (i = 0; i < cat.Bin2.Length; i++)
            {
                if (measure < cat.Bin2[i])
                    break;
            }

            logDebug("cluster: " + cluster + " VID:" + vid.VidNumber + " " + vid.VidLabel + " bin: " + i);
            vid.DefectSizePerBin[i] += measure;
            vid.DefectCountPerBin[i]++;
            return true;
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
            foreach (var input in Recipe.InputInfoList.OfType<DataLoader.InspectionInputInfoBase>())
            {
                AdcInput adcInput = new AdcInput();
                adcInput.InputResultType = (int)input.ResultType;
                adcInput.InputPictureDirectory = input.Folder;
                // obsolete
                //adcInput.ActorTypeId = (int)input.ActorTypeId;
                //adcInput.ChannelID = (int)input.ChannelID;
                adcInputList.Add(adcInput);
            }

            //-------------------------------------------------------------
            // Envoi
            //-------------------------------------------------------------
            ADC.Instance.TransferToRobotStub.TransferInputList(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", adcInputList);
        }

        //=================================================================
        // 
        //=================================================================
        private void SendVids()
        {
            if (vidMap != null)
            {
                List<VidBase> list = vidMap.Values.Cast<VidBase>().ToList();
                ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", list);
            }
        }

    }
}
