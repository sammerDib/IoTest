using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.DataLoader;
using BasicModules.Edition.DataBase;

using FormatAHM;

using LibProcessing;

using UnitySC.Shared.Tools;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class AhmDieEditorModule : DatabaseEditionModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramUseHeightMap;
        public readonly DoubleParameter paramMinHeight;
        public readonly DoubleParameter paramMaxHeight;
        public readonly DoubleParameter paramStepHeight;
        public readonly BoolParameter paramUseCoplanarityMap;
        public readonly DoubleParameter paramMinCoplanarity;
        public readonly DoubleParameter paramMaxCoplanarity;
        public readonly DoubleParameter paramStepCoplanarity;
        public readonly BoolParameter paramUseSubstrateCoplanarity;
        public readonly DoubleParameter paramMinSubstrateCoplanarity;
        public readonly DoubleParameter paramMaxSubstrateCoplanarity;
        public readonly DoubleParameter paramStepSubstrateCoplanarity;

        //=================================================================
        // Autres Champs
        //=================================================================
        private List<Cluster3DDieHM> _clusterList = new List<Cluster3DDieHM>();
        protected PathString _AHMFilename;

        private static ProcessingClass _processClass = new ProcessingClassMil();
        private MatrixBase viewerMatrix;
        private HeightMapResults.eWaferType hmwafertype;
        private float lfWaferSizeX_µm = 0.0f;
        private float lfWaferSizeY_µm = 0.0f;

        //=================================================================
        // Database results registration
        //=================================================================
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.HeightMeasurement_AHM);
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public AhmDieEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {

            paramMinHeight = new DoubleParameter(this, "RangeMinHeight");
            paramMinHeight.Value = 0.0;

            paramMaxHeight = new DoubleParameter(this, "RangeMaxHeight");
            paramMaxHeight.Value = 50.0;

            paramStepHeight = new DoubleParameter(this, "RangeStepHeight");
            paramStepHeight.Value = 10.0;

            paramUseHeightMap = new BoolParameter(this, "UseHeightMap");
            paramUseHeightMap.ValueChanged +=
                (use) =>
                {
                    paramMinHeight.IsEnabled = use;
                    paramMaxHeight.IsEnabled = use;
                    paramStepHeight.IsEnabled = use;
                };
            paramUseHeightMap.Value = true;

            paramMinHeight.IsEnabled = paramUseHeightMap.Value;
            paramMaxHeight.IsEnabled = paramUseHeightMap.Value;
            paramStepHeight.IsEnabled = paramUseHeightMap.Value;

            paramMinCoplanarity = new DoubleParameter(this, "RangeMinCopla");
            paramMinCoplanarity.Value = 0.0;

            paramMaxCoplanarity = new DoubleParameter(this, "RangeMaxCopla");
            paramMaxCoplanarity.Value = 30.0;

            paramStepCoplanarity = new DoubleParameter(this, "RangeStepCopla");
            paramStepCoplanarity.Value = 10.0;

            paramUseCoplanarityMap = new BoolParameter(this, "UseCoplaMap");
            paramUseCoplanarityMap.ValueChanged +=
                (use) =>
                {
                    paramMinCoplanarity.IsEnabled = use;
                    paramMaxCoplanarity.IsEnabled = use;
                    paramStepCoplanarity.IsEnabled = use;
                };
            paramUseCoplanarityMap.Value = false;

            paramMinCoplanarity.IsEnabled = paramUseCoplanarityMap.Value;
            paramMaxCoplanarity.IsEnabled = paramUseCoplanarityMap.Value;
            paramStepCoplanarity.IsEnabled = paramUseCoplanarityMap.Value;

            paramMinSubstrateCoplanarity = new DoubleParameter(this, "RangeMinSubstrateCopla");
            paramMinSubstrateCoplanarity.Value = 0.0;

            paramMaxSubstrateCoplanarity = new DoubleParameter(this, "RangeMaxSubstrateCopla");
            paramMaxSubstrateCoplanarity.Value = 30.0;

            paramStepSubstrateCoplanarity = new DoubleParameter(this, "RangeStepSubstrateCopla");
            paramStepSubstrateCoplanarity.Value = 10.0;

            paramUseSubstrateCoplanarity = new BoolParameter(this, "UseSubstrateCoplaMap");
            paramUseSubstrateCoplanarity.ValueChanged +=
                (use) =>
                {
                    paramMinSubstrateCoplanarity.IsEnabled = use;
                    paramMaxSubstrateCoplanarity.IsEnabled = use;
                    paramStepSubstrateCoplanarity.IsEnabled = use;
                };
            paramUseSubstrateCoplanarity.Value = false;

            paramMinSubstrateCoplanarity.IsEnabled = paramUseSubstrateCoplanarity.Value;
            paramMaxSubstrateCoplanarity.IsEnabled = paramUseSubstrateCoplanarity.Value;
            paramStepSubstrateCoplanarity.IsEnabled = paramUseSubstrateCoplanarity.Value;


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

            // height map
            if (paramUseHeightMap.Value && !DirectAncestor.paramUseHeightMap)
                return String.Format("Height Map is not computed ! check <{0}> Module parameters", DirectAncestor.Name);

            // coplanarity
            if (paramUseCoplanarityMap.Value && !DirectAncestor.paramUseCoplanarityMap)
                return String.Format("Coplanarity Map is not computed ! check <{0}> Module parameters", DirectAncestor.Name);

            // substrate coplanarity
            if (paramUseSubstrateCoplanarity.Value && !DirectAncestor.paramUseSubstrateCoplanarity)
                return String.Format("Substrate Coplanarity Map is not computed ! check <{0}> Module parameters", DirectAncestor.Name);

            if (paramUseHeightMap.Value && paramMinHeight >= paramMaxHeight)
                return "inconsistant min/max Height Map range";

            if (paramUseHeightMap.Value && ((paramStepHeight <= 0.0) || (paramStepHeight > (paramMaxHeight.Value - paramMinHeight.Value))))
                return "inconsistant Step Height Map range  ]0.0 (Max-Min)]";

            if (paramUseCoplanarityMap.Value && paramMinCoplanarity >= paramMaxCoplanarity)
                return "inconsistant min/max Coplanarity Map range";

            if (paramUseCoplanarityMap.Value && ((paramStepCoplanarity <= 0.0) || (paramStepCoplanarity > (paramMaxCoplanarity.Value - paramMinCoplanarity.Value))))
                return "inconsistant Step Coplanarity Map range  ]0.0 (Max-Min)]";

            if (paramUseSubstrateCoplanarity.Value && paramMinSubstrateCoplanarity >= paramMaxSubstrateCoplanarity)
                return "inconsistant min/max Substrate Coplanarity Map range";

            if (paramUseSubstrateCoplanarity.Value && ((paramStepSubstrateCoplanarity <= 0.0) || (paramStepSubstrateCoplanarity > (paramMaxSubstrateCoplanarity.Value - paramMinSubstrateCoplanarity.Value))))
                return "inconsistant Step Substrate Coplanarity Map range  ]0.0 (Max-Min)]";

            if (!paramUseHeightMap.Value && !paramUseCoplanarityMap.Value && !paramUseSubstrateCoplanarity)
                return "No measures computation has been set";

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _AHMFilename = GetResultFullPathName(ResultTypeFile.HeightMeasurement_AHM);

            WaferBase wafer = Recipe.Wafer;
            if (Wafer is NotchWafer)
            {
                hmwafertype = HeightMapResults.eWaferType.Round_Notch;
                lfWaferSizeX_µm = (int)((Wafer as NotchWafer).Diameter);
                lfWaferSizeY_µm = (int)((Wafer as NotchWafer).Diameter);
            }
            else if (Wafer is FlatWafer)
            {
                hmwafertype = HeightMapResults.eWaferType.Round_Flat;
                lfWaferSizeX_µm = (int)((Wafer as FlatWafer).Diameter);
                lfWaferSizeY_µm = (int)((Wafer as FlatWafer).Diameter);

            }
            else if (Wafer is RectangularWafer)
            {
                hmwafertype = HeightMapResults.eWaferType.Square;
                lfWaferSizeX_µm = (Wafer as RectangularWafer).Width;
                lfWaferSizeY_µm = (Wafer as RectangularWafer).Height;
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

            Scheduler.StartSingleTask("ProcessAHM", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessAHM();

                        ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.HeightMeasurement_AHM, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeAHM();
                        RegisterResultInDatabase(ResultTypeFile.HeightMeasurement_AHM, ResultState.Error);
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.HeightMeasurement_AHM, ResultState.Error);
                    string msg = "AHM generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    PurgeAHM();
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessAHM()
        {
            //-------------------------------------------------------------
            // Create a matrix to transfrom from microns to viewer pixels
            //-------------------------------------------------------------
            const int viewerImageSize = 4198; // Fixed, the viewer displays a 4000x4000 image with a 99 pixels margin each side
            float ahmPixelSize = Math.Max(Wafer.SurroundingRectangle.Width, Wafer.SurroundingRectangle.Height) / viewerImageSize;
            Point waferCenter = new Point(viewerImageSize / 2, viewerImageSize / 2);

            RectangularMatrix rmatrix = new RectangularMatrix();
            rmatrix.Init(waferCenter, new SizeF(ahmPixelSize, ahmPixelSize));
            viewerMatrix = rmatrix;


            List<ModuleBase> AncestorDieLoadermodule = FindAncestors(mod => mod is DieDataLoaderBase);
            if (AncestorDieLoadermodule.Count == 0)
            {
                throw new ApplicationException("No Die loader module has been set prior to this module");
            }

            DieDataLoaderBase DirectAncestor = AncestorDieLoadermodule[0] as DieDataLoaderBase;
            DieLayer dielayer = ((DirectAncestor.Layer) as DieLayer);


            // on peux faire mieux les données devrait être en micron et non en pixel ! 
            // on fait ça pour la retro compatibilité vu que l'on touche pas au viewer
            float lfPixelSizeX_µm = 0.0f;
            float lfPixelSizeY_µm = 0.0f;
            if (dielayer.Matrix is RectangularMatrix)
            {
                RectangularMatrix mat = dielayer.Matrix as RectangularMatrix;
                lfPixelSizeX_µm = mat.PixelWidth;
                lfPixelSizeY_µm = mat.PixelHeight;
            }
            float lfDieOriginX_µm = (float)dielayer.DieOriginX_µm;
            float lfDieOriginY_µm = (float)dielayer.DieOriginY_µm;
            float lfDiePitchX_µm = (float)dielayer.DiePitchX_µm;
            float lfDiePitchY_µm = (float)dielayer.DiePitchY_µm;

            //-------------------------------------------------------------
            // Write AHM file
            //-------------------------------------------------------------
            log("Creating ahm file " + _AHMFilename);


            HeightMapResults ahmResults = new HeightMapResults();

            ahmResults.SetWaferParameters(hmwafertype, lfWaferSizeX_µm, lfWaferSizeY_µm, lfPixelSizeX_µm, lfPixelSizeY_µm);
            ahmResults.SetDieGridParameters(lfDieOriginX_µm, lfDieOriginY_µm, lfDiePitchX_µm, lfDiePitchY_µm);
            ahmResults.SetHeightMapParameters(paramUseHeightMap, (float)paramMinHeight.Value, (float)paramMaxHeight.Value, (float)paramStepHeight.Value);
            ahmResults.SetCoplanarityParameters(paramUseCoplanarityMap, (float)paramMinCoplanarity.Value, (float)paramMaxCoplanarity.Value, (float)paramStepCoplanarity.Value);
            ahmResults.SetSubstrateCoplanarityParameters(paramUseSubstrateCoplanarity, (float)paramMinSubstrateCoplanarity.Value, (float)paramMaxSubstrateCoplanarity.Value, (float)paramStepSubstrateCoplanarity.Value);

            foreach (Cluster3DDieHM cluster in _clusterList)
            {
                ahmResults.AddDieHMResult(cluster._dieHMresult);
                Interlocked.Increment(ref nbObjectsOut);
            }

            string sMsgError;
            if (!ahmResults.WriteInFile(_AHMFilename, out sMsgError))
            {
                throw new Exception(sMsgError);
            }
            else
            {
                logDebug("ahm generated: " + _AHMFilename);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeAHM()
        {
            // Purge de la liste interne de clusters
            //......................................
            foreach (Cluster3DDieHM cluster in _clusterList)
                cluster.DelRef();
            _clusterList.Clear();
        }
    }
}
