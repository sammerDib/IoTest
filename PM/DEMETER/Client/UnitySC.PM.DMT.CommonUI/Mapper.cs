using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.CommonUI.Proxy
{
    public class Mapper
    {
        private IMapper _mapper;

        // Use for auto mapping
        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var hSupervisor = ClassLocator.Default.GetInstance<CameraSupervisor>();
                    var sSupervisor = ClassLocator.Default.GetInstance<ScreenSupervisor>();
                    var cSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
                    var aSupervisor = ClassLocator.Default.GetInstance<AlgorithmsSupervisor>();
                    var rSupervisor = ClassLocator.Default.GetInstance<RecipeSupervisor>();
                    var gSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
                    var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                    var mapper = ClassLocator.Default.GetInstance<Mapper>();
                    var mainRecipeEditionVM = ClassLocator.Default.GetInstance<MainRecipeEditionVM>();
                    var recipeManager = ClassLocator.Default.GetInstance<IRecipeManager>();

                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Fringe, FringeVM>().ReverseMap();
                        cfg.CreateMap<BackLightMeasure, BackLightVM>().ReverseMap();

                        // Custom for Model to ViewModel
                        cfg.CreateMap<ROI, RoiVM>()
                            .ConstructUsing(model => new RoiVM(hSupervisor))
                            .ReverseMap();
                        cfg.CreateMap<DMTRecipe, RecipeEditionVM>()
                            .ConstructUsing(model => new RecipeEditionVM(recipeManager, cSupervisor, rSupervisor, gSupervisor, dialogService, mapper))
                            .ReverseMap();
                        cfg.CreateMap<BrightFieldMeasure, BrightFieldVM>()
                            .ConstructUsing(model => new BrightFieldVM(hSupervisor, sSupervisor, cSupervisor, aSupervisor, dialogService, mapper, mainRecipeEditionVM))
                            .ReverseMap();
                        cfg.CreateMap<HighAngleDarkFieldMeasure, HighAngleDarkFieldVM>()
                            .ConstructUsing(model => new HighAngleDarkFieldVM(hSupervisor, sSupervisor, cSupervisor, aSupervisor, dialogService, mapper, mainRecipeEditionVM))
                            .ReverseMap();
                        cfg.CreateMap<DeflectometryMeasure, DeflectometryVM>()
                            .ConstructUsing(model => new DeflectometryVM(hSupervisor, sSupervisor, cSupervisor, aSupervisor, rSupervisor, dialogService, mapper,
                            mainRecipeEditionVM, model.Side))
                            .ForMember(vm => vm.Outputs, m => m.MapFrom<OutputToVMResolver>());
                        cfg.CreateMap<MeasureBase, MeasureVM>()
                            .Include<DeflectometryMeasure, DeflectometryVM>()
                            .Include<BrightFieldMeasure, BrightFieldVM>()
                            .Include<HighAngleDarkFieldMeasure, HighAngleDarkFieldVM>()
                            .Include<BackLightMeasure, BackLightVM>();

                        // Custom for ViewModel to Model
                        cfg.CreateMap<DeflectometryVM, DeflectometryMeasure>()
                            .ForMember(vm => vm.Outputs, m => m.MapFrom<VMToOutputResolver>());
                        cfg.CreateMap<MeasureVM, MeasureBase>()
                            .Include<DeflectometryVM, DeflectometryMeasure>()
                            .Include<BrightFieldVM, BrightFieldMeasure>()
                            .Include<HighAngleDarkFieldVM, HighAngleDarkFieldMeasure>()
                            .Include<BackLightVM, BackLightMeasure>();
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }
    }

    internal class OutputToVMResolver : IValueResolver<DeflectometryMeasure, DeflectometryVM, Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>>>
    {
        public Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>> Resolve(DeflectometryMeasure source, DeflectometryVM destination, Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>> destMember, ResolutionContext context)
        {
            Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>> selectableItemVMs = new Dictionary<DeflectometryOutput, SelectableItemVM<DeflectometryOutput>>();

            foreach (DeflectometryOutput output in Enum.GetValues(typeof(DeflectometryOutput)))
            {
                SelectableItemVM<DeflectometryOutput> selectableItem = new SelectableItemVM<DeflectometryOutput>() { WrappedObject = output };
                if (source.Outputs.HasFlag(output))
                    selectableItem.IsSelected = true;
                if (source.AvailableOutputs.HasFlag(output))
                    selectableItem.IsAvailable = true;
                selectableItemVMs.Add(output, selectableItem);
            }

            return selectableItemVMs;
        }
    }

    internal class VMToOutputResolver : IValueResolver<DeflectometryVM, DeflectometryMeasure, DeflectometryOutput>
    {
        public DeflectometryOutput Resolve(DeflectometryVM source, DeflectometryMeasure destination, DeflectometryOutput destMember, ResolutionContext context)
        {
            DeflectometryOutput outputs = 0;
            foreach (var selectedItem in source.Outputs.Where(x => x.Value.IsSelected))
                outputs |= selectedItem.Value.WrappedObject;

            return outputs;
        }
    }
}
