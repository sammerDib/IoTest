using System;
using System.Collections.Generic;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.DataBase;

using FormatHAZE;

using UnitySC.Shared.Tools;

namespace HazeLSModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HazeLSEditorModule : DatabaseEditionModule
    {
        private int wafersize_mm = 0;
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.HazeLS_HAZE);
            return Rtypes;
        }

        protected string _Filename = String.Empty;

        protected List<HazeLSMeasure> _ListMeasure = new List<HazeLSMeasure>(3);

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeLSEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _Filename = GetResultFullPathName(ResultTypeFile.HazeLS_HAZE);

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

            HazeLSMeasure mes = (HazeLSMeasure)obj;

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

            Scheduler.StartSingleTask("ProcessLSHaze", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessLSHaze();

                        ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.HazeLS_HAZE, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeLSHaze();
                        RegisterResultInDatabase(ResultTypeFile.HazeLS_HAZE, ResultState.Error);
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.HazeLS_HAZE, ResultState.Error);
                    string msg = "HAZE LS generation failed: " + ex.Message;
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
        private void PurgeLSHaze()
        {
            // Purge de la liste interne des mesure
            //......................................
            foreach (HazeLSMeasure mes in _ListMeasure)
                mes.DelRef();
            _ListMeasure.Clear();
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessLSHaze()
        {
            try
            {
                //-------------------------------------------------------------
                // Write HAZE file
                //-------------------------------------------------------------
                PathString path = _Filename;
                log("Creating LS HAZE file " + path);

                _ListMeasure.Sort((x, y) => x.Data.nId.CompareTo(y.Data.nId));

                LSHazeResults result = new LSHazeResults();
                foreach (HazeLSMeasure mes in _ListMeasure)
                {
                    result.Data.Add(mes.Data);
                }
                string err;
                if (!result.WriteInFile(path, out err))
                {
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
                PurgeLSHaze();
            }
        }
    }
}
