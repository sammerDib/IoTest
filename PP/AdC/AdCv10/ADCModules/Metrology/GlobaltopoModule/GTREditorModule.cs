using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.DataBase;

using FormatGTR;

using UnitySC.Shared.Tools;

namespace GlobaltopoModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class GTREditorModule : DatabaseEditionModule
    {
        private int wafersize_mm = 0;
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.Globaltopo_GTR);
            return Rtypes;
        }

        protected string _Filename = String.Empty;

        protected List<GTMeasure> _ListMeasure = new List<GTMeasure>();

        //=================================================================
        // Constructeur
        //=================================================================
        public GTREditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _Filename = GetResultFullPathName(ResultTypeFile.Globaltopo_GTR);

            WaferBase wafer = Recipe.Wafer;
            if (Wafer is NotchWafer)
            {
                wafersize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                wafersize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                wafersize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            GTMeasure mes = (GTMeasure)obj;
            mes.Data.m_nWaferSize_mm = wafersize_mm;

            //-------------------------------------------------------------
            // Stockage des Mesures
            //-------------------------------------------------------------
            mes.AddRef();
            lock (_ListMeasure)
                _ListMeasure.Add(mes);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("Process GTR Edition", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessGTR();

                        ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.Globaltopo_GTR, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeGTR();
                        RegisterResultInDatabase(ResultTypeFile.Globaltopo_GTR, ResultState.Error);
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.Globaltopo_GTR, ResultState.Error);
                    string msg = "GTR generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeGTR()
        {
            // Purge de la liste interne des mesure
            //......................................
            foreach (GTMeasure mes in _ListMeasure)
                mes.DelRef();
            _ListMeasure.Clear();
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessGTR()
        {
            try
            {
                //-------------------------------------------------------------
                // Write HAZE file
                //-------------------------------------------------------------
                PathString path = _Filename;
                log("Creating GTR file " + path);

                // A ce jour on ne devrait avoir qu'une seul mesure, si plsueiru warning et on ne se sert que de la première ?
                // a voir plus tard comment on gere de multiple mesure GTR
                if (_ListMeasure.Count >= 1)
                {
                    if (_ListMeasure.Count > 1)
                        logWarning("Warning : Multiple Globaltopo measure received, handle only the first one");

                    DataGTR gtResults = _ListMeasure[0].Data;
                    string err;
                    if (!gtResults.WriteFile(path, out err))
                    {
                        throw new ApplicationException(err);
                    }

                    EditGlobalTopoCSVReport(path, gtResults);
                }
                else
                {
                    // empty result
                    string err = "No GlobalTopo Measures has been received";
                    throw new ApplicationException(err);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }
            finally
            {
                PurgeGTR();
            }
        }


        //-------------------------------------------------------------------
        private void EditGlobalTopoCSVReport(string path, DataGTR GTEx)
        {
            try
            {
                string PathCSV = Path.GetFileNameWithoutExtension(path);
                PathCSV += ".csv";

                StreamWriter outStream = new StreamWriter(PathCSV);

                if (outStream != null)
                {
                    outStream.WriteLine("TWARP;" + GTEx.m_fOut_TotalWarp.ToString());
                    outStream.WriteLine("MAXPOSWARP;" + GTEx.m_fOut_MaxPosWarp.ToString());
                    outStream.WriteLine("MAXNEGWARP;" + GTEx.m_fOut_MinNegWarp.ToString());
                    outStream.WriteLine("BOWBF;" + GTEx.m_fOut_BowBF.ToString());
                    outStream.WriteLine("BOWX;" + GTEx.m_fOut_BowX.ToString());
                    outStream.WriteLine("BOWY;" + GTEx.m_fOut_BowY.ToString());
                    outStream.WriteLine("MAXBOWXY;" + GTEx.m_fOut_BowXY.ToString());
                    outStream.WriteLine("BOC;" + GTEx.m_fOut_CenterBow.ToString());

                    outStream.Close();
                }
                else
                {
                    throw new ApplicationException("Unable to save global topo CSV report in <" + PathCSV + ">");
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }
        }

    }
}
