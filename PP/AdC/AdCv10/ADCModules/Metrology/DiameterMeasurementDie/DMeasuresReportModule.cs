using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Edition.DataBase;

using UnitySC.Shared.Tools;

namespace DiameterMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DMeasuresReportModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster2DDieDM> _clusterList = new List<Cluster2DDieDM>();
        protected PathString _Filename;

        protected int _nSampleSize_mm = 0;

        //=================================================================
        // Database results registration - in this specific cas there will be no registration 
        //=================================================================
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.HeightMeasurement_AHM); // en réalité c'est un csv,  mais ce type là n'est pas register
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public DMeasuresReportModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {


        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            // Find ancestor of die height measurement
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is DiameterMeasurementDieModule);
            if (AncestorHMmodule.Count == 0)
                return "No 2d diameter measurement die module has been set above this module";

            DiameterMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as DiameterMeasurementDieModule;
            // check if data is coorectly computed by ancestor

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            String sFileName = String.Format("{0}_DM_{1}{2}", Wafer.Basename, RunIter, ".csv");
            _Filename = DestinationDirectory / sFileName;

            if (Wafer is NotchWafer)
            {
                _nSampleSize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                _nSampleSize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                _nSampleSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }

        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if ((obj is Cluster2DDieDM) == false)
                throw new ApplicationException("Wrong type of parent data sent, Cluster2DDieDM is expected");

            Cluster2DDieDM cluster = (Cluster2DDieDM)obj;

            //-------------------------------------------------------------
            // Stockage des clusters
            //-------------------------------------------------------------
            cluster.AddRef();
            lock (_clusterList)
                _clusterList.Add(cluster);
        }


        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessCSV", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                        ProcessCSV();
                }
                catch (Exception ex)
                {
                    string msg = "CSV generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    PurgeCSV();
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessCSV()
        {
            //-------------------------------------------------------------
            // Write CSV file - reporting all Height Measure
            //-------------------------------------------------------------
            log("Creating 2D Measure report file " + _Filename);

            int nFooter = 512;
            int NbDies = _clusterList.Count;
            int NbMeasuresPerDie = 0;

            if (NbDies > 0)
            {
                NbMeasuresPerDie = _clusterList[0].blobList.Count;
            }

            // on sort les die cluster de manière à les enregistré s dans le même ordre
            _clusterList.Sort((a, b) =>
            {
                var n = b.DieIndexY.CompareTo(a.DieIndexY);
                if (n == 0)
                    n = a.DieIndexX.CompareTo(b.DieIndexX);
                return n;
            });

            StringBuilder sb = new StringBuilder(NbDies * NbMeasuresPerDie * 128 + nFooter);

            DateTime dtNow = DateTime.Now;
            sb.AppendLine("2D Measures Report");
            sb.AppendFormat("Job ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ToolRecipe));
            sb.AppendFormat("Lot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.LotID));
            sb.AppendFormat("Wafer ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.WaferID));
            int nSlotId = 0;
            if (int.TryParse(Wafer.GetWaferInfo(eWaferInfo.SlotID), out nSlotId))
            {
                sb.AppendFormat("Slot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.SlotID));

            }
            sb.AppendFormat("Recipe;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName));
            sb.AppendFormat("Unique ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.Basename));
            sb.AppendFormat("Wafer Size;{0}\n", _nSampleSize_mm);
            sb.AppendFormat("Equipment ID;{0}-{1}-{2}\n", "UNITYSC", "4See", Recipe.Toolname);
            sb.AppendLine();

            sb.AppendLine("2D Measures Details");
            sb.AppendLine("Die Col;Die Row;Measure ID;Diameter (um); Offset (um); DeltaX (um); DeltaY (um);");

            foreach (Cluster2DDieDM cluster in _clusterList)
            {
                if (State == eModuleState.Aborting)
                {
                    break;
                }

                int nDieIdxX = cluster.DieIndexX;
                int nDieIdxY = cluster.DieIndexY;
                foreach (Blob blob in cluster.blobList)
                {
                    if (blob.characteristics.ContainsKey(Blob2DCharacteristics.isMissing) && (double)blob.characteristics[Blob2DCharacteristics.isMissing] != 0.0)
                        sb.AppendFormat("{0};{1};{2};;;;;\n", nDieIdxX, nDieIdxY, blob.Index);
                    else
                        sb.AppendFormat("{0};{1};{2};{3:#0.00};{4:#0.00};{5:#0.00};{6:#0.00};\n", nDieIdxX, nDieIdxY, blob.Index,
                            blob.characteristics[Blob2DCharacteristics.Diameter],
                            blob.characteristics[Blob2DCharacteristics.OffsetPos],
                            blob.characteristics[Blob2DCharacteristics.DeltaTargetX],
                            blob.characteristics[Blob2DCharacteristics.DeltaTargetY]);
                }

                Interlocked.Increment(ref nbObjectsOut);
            }
            sb.AppendLine();

            if (State != eModuleState.Aborting)
            {
                using (StreamWriter SW = new StreamWriter(_Filename, false))
                {
                    SW.Write(sb.ToString());
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeCSV()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster2DDieDM cluster in _clusterList)
                cluster.DelRef();
            _clusterList.Clear();
        }
    }
}
