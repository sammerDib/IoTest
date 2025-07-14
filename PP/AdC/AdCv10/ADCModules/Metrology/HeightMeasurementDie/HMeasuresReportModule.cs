using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.Edition.DataBase;

using UnitySC.Shared.Tools;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HMeasuresReportModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster3DDieHM> _clusterList = new List<Cluster3DDieHM>();
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
        public HMeasuresReportModule(IModuleFactory factory, int id, Recipe recipe)
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
            List<ModuleBase> AncestorHMmodule = FindAncestors(mod => mod is HeightMeasurementDieModule);
            if (AncestorHMmodule.Count == 0)
                return "No Height measurement die module has been set above this module";

            HeightMeasurementDieModule DirectAncestor = AncestorHMmodule[0] as HeightMeasurementDieModule;
            // check if data is coorectly computed by ancestor

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            String sFileName = String.Format("{0}_HM_{1}{2}", Wafer.Basename, RunIter, ".csv");
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

            if ((obj is Cluster3DDieHM) == false)
                throw new ApplicationException("Wrong type of parent data sent, Cluster3DDieHM is expected");

            Cluster3DDieHM cluster = (Cluster3DDieHM)obj;

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
            log("Creating H Measure report file " + _Filename);

            int NbDies = _clusterList.Count;
            int NbMeasuresPerDie = 0;

            if (NbDies > 0)
            {
                NbMeasuresPerDie = _clusterList[0]._dieHMresult.NbMeasures;
            }

            // on sort les die cluster de manière à les enregistré s dans le même ordre
            _clusterList.Sort((a, b) =>
            {
                var n = b.DieIndexY.CompareTo(a.DieIndexY);
                if (n == 0)
                    n = a.DieIndexX.CompareTo(b.DieIndexX);
                return n;
            });

            using (StreamWriter stream = new StreamWriter(_Filename, false))
            {
                DateTime dtNow = DateTime.Now;
                stream.WriteLine("Height Measures Report");
                stream.WriteFormat("Job ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.JobID));
                stream.WriteFormat("Lot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.LotID));
                stream.WriteFormat("Wafer ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.WaferID));
                int nSlotId = 0;
                if (int.TryParse(Wafer.GetWaferInfo(eWaferInfo.SlotID), out nSlotId))
                    stream.WriteFormat("Slot ID;{0}\n", Wafer.GetWaferInfo(eWaferInfo.SlotID));

                stream.WriteFormat("Recipe;{0}\n", Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName));
                stream.WriteFormat("BaseName;{0}\n", Wafer.GetWaferInfo(eWaferInfo.Basename));
                stream.WriteFormat("Wafer Size;{0}\n", _nSampleSize_mm);
                stream.WriteFormat("Equipment ID;{0}-{1}-{2}\n", "UNITYSC", "4See", Recipe.Toolname);
                stream.WriteLine();

                stream.WriteLine("Height Measures Details");
                stream.WriteLine("Die Col;Die Row; Die Cx (µm); DieCy (µm); Measure ID;Height (um);");

                foreach (Cluster3DDieHM cluster in _clusterList)
                {
                    if (State == eModuleState.Aborting)
                        break;

                    int nDieIdxX = cluster.DieIndexX;
                    int nDieIdxY = cluster.DieIndexY;

                    RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                    PointF Mid = rect_um.Middle();

                    foreach (Blob blob in cluster.blobList)
                    {
                        stream.WriteFormat("{0};{1};{2:#0.00};{3:#0.00};{4};{5:#0.00};\n", nDieIdxX, nDieIdxY, Mid.X, Mid.Y, blob.Index, blob.characteristics[Blob3DCharacteristics.HeightMicron]);
                    }

                    Interlocked.Increment(ref nbObjectsOut);
                }

                stream.WriteLine();
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeCSV()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster3DDieHM cluster in _clusterList)
                cluster.DelRef();
            _clusterList.Clear();
        }
    }
}
