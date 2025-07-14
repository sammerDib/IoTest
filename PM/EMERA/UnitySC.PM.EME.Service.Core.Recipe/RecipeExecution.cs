using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Optional;
using Optional.Unsafe;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Wheel;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class RecipeExecution
    {
        private readonly ILogger<RecipeOrchestrator> _logger;
        private readonly IEmeraCamera _camera;
        private readonly WheelBase _filterWheel;
        private readonly bool _flowsAreSimulated;
        private readonly PhotoLumAxes _motionAxes;
        private readonly Dictionary<string, EMELightBase> _lights;

        private Dictionary<EMEFilter, double> _zFocusPositions = new Dictionary<EMEFilter, double>();
        private CancellationTokenSource _tokenSource;
        private Mapper _mapper;

        public RecipeExecution(ILogger<RecipeOrchestrator> logger, IEMEServiceConfigurationManager configurationManager,
            EmeHardwareManager hardwareManager, IEmeraCamera camera)
        {
            _logger = logger;
            _mapper = ClassLocator.Default.GetInstance<Mapper>();
            _camera = camera;
            _filterWheel = hardwareManager.Wheel;
            _lights = hardwareManager.EMELights;
            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            _flowsAreSimulated = configurationManager.FlowsAreSimulated;
        }

        public void PrepareRecipe(RecipeAdapter recipe)
        {
            _zFocusPositions.Clear();
            if (recipe.RunOptions.RunAutoFocus)
            {
                _zFocusPositions = ZFocusPositions(recipe);
            }
        }

        private Dictionary<EMEFilter, double> ZFocusPositions(RecipeAdapter recipe)
        {
            var center = new XYZPosition(new WaferReferential(), 0, 0, 0);
            MoveStage(center);

            var zFocusPositions = new Dictionary<EMEFilter, double>();
            foreach (var filter in recipe.Acquisitions.Select(x => x.Filter).ToList())
            {
                if (zFocusPositions.ContainsKey(filter.Type))
                {
                    continue;
                }
                var zFocusResult = FindZFocus(filter);
                if (!zFocusResult.HasValue)
                {
                    zFocusPositions.Add(filter.Type, recipe.GetAcquisitionConfiguration().DefaultZFocus.Millimeters);
                    continue;
                }
                zFocusPositions.Add(filter.Type, zFocusResult.ValueOrFailure());
            }
            return zFocusPositions;
        }

        public Option<double> FindZFocus(Filter filter)
        {
            MoveFilterWheel(filter.Position);

            var getZFocusInput = new GetZFocusInput { TargetDistanceSensor = filter.DistanceOnFocus };
            var flow = _flowsAreSimulated ? new GetZFocusFlowDummy(getZFocusInput) : new GetZFocusFlow(getZFocusInput);
            var flowResult = flow.Execute();

            return flowResult.Status.State == FlowState.Success ? Option.Some(flowResult.Z) : Option.None<double>();
        }

        public void PrepareAcquisition(AcquisitionSettings acquisition, RecipeAdapter recipe)
        {
            _logger.Information($"Setting camera exposure time to {acquisition.ExposureTime}ms");
            _camera.SetCameraExposureTime(acquisition.ExposureTime);

            double filterPosition = acquisition.Filter.Position;
            _logger.Information($"Moving to filter {acquisition.Filter} at position {filterPosition}");
            MoveFilterWheel(filterPosition);

            if (recipe.RunOptions.RunAutoFocus)
            {
                MoveToFocus(acquisition.Filter);
            }

            _logger.Information("Turning off unused lights");
            foreach (var light in _lights.Values.Distinct())
            {
                light.SwitchOn(false);
                Thread.Sleep(1000);
            }

            _logger.Information(
                $"Turning on the {_lights[acquisition.Light.DeviceID].Name} light");
            _lights[acquisition.Light.DeviceID].SwitchOn(true);
            var lightPower = 8.00;
            switch (acquisition.Light.Type)
            {
                case Interface.Light.EMELightType.DirectionalDarkField90Degree:
                case Interface.Light.EMELightType.DirectionalDarkField0Degree:
                    lightPower = recipe.GetAcquisitionConfiguration().DDFLightPower;
                    break;
                case Interface.Light.EMELightType.UltraViolet270nm:
                case Interface.Light.EMELightType.UltraViolet310nm:
                case Interface.Light.EMELightType.UltraViolet365nm:
                    lightPower = recipe.GetAcquisitionConfiguration().UVLightPower;
                    break;
                default:
                    break;
            }
            _logger.Information($"Setting power of {_lights[acquisition.Light.DeviceID].Name} to {lightPower}");
            _lights[acquisition.Light.DeviceID].SetPower(lightPower);
        }

        private void MoveToFocus(Filter filter)
        {
            double zTarget = _zFocusPositions[filter.Type];
            var position = _motionAxes.GetPosition().ToXYZPosition();
            var newStagePosition = new XYZPosition(position.Referential, position.X, position.Y, zTarget);
            _logger.Information($"Moving to AutoFocus at Z={zTarget}");
            MoveStage(newStagePosition);
        }

        public ServiceImage CaptureImage(double x, double y)
        {
            var position = new XYPosition(new WaferReferential(), x, y);
            _logger.Information($"Moving to X={position.X}, Y={position.Y} ({position.Referential})");
            MoveStage(position);

            return _camera.SingleAcquisition();
        }

        public void PostRecipe()
        {
            _logger.Information("Turning off lights");
            _lights.Values.ToList().ForEach(light => light.SwitchOn(false));
        }

        private void MoveFilterWheel(double position)
        {
            _filterWheel.Move(position);
            _filterWheel.WaitMotionEnd(3000);
        }

        private void MoveStage(PositionBase position)
        {
            _motionAxes.GoToPosition(position);
            _motionAxes.WaitMotionEnd(30000);
        }

        public EMERecipe Convert_RecipeToEMERecipe(DataAccess.Dto.Recipe dbRecipe)
        {
            if (dbRecipe == null)
                return null;

            var recipe = _mapper.AutoMap.Map<EMERecipe>(dbRecipe);
            return recipe;
        }

        public DataAccess.Dto.Recipe Convert_EMERecipeToRecipe(EMERecipe emeRecipe)
        {
            if (emeRecipe == null)
                return null;

            var recipe = _mapper.AutoMap.Map<DataAccess.Dto.Recipe>(emeRecipe);
            return recipe;
        }
    }
}
