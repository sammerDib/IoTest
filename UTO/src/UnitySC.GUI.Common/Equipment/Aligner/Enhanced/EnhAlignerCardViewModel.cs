using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.Equipment.Aligner.Enhanced
{
    public class EnhAlignerCardViewModel : AlignerCardViewModel
    {
        #region Constructor

        static EnhAlignerCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(EnhAlignerCardViewModel), typeof(EnhAlignerCard));
        }

        public EnhAlignerCardViewModel(UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner)
            : base(aligner)
        {
            AngleSources = new List<string>() { "Manual" };
        }

        #endregion

        #region Properties

        private List<UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule> _processModules { get; set; }
        public List<string> AngleSources { get; }

        private string _selectedAngleSources;
        public string SelectedAngleSources
        {
            get => _selectedAngleSources;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedAngleSources, value))
                {
                    OnPropertyChanged(nameof(IsManualSourceSelected));
                }
            }
        }

        public bool IsManualSourceSelected => SelectedAngleSources == AngleSources.First();

        #endregion

        #region override

        private SafeDelegateCommandAsync _alignCommand;

        public new SafeDelegateCommandAsync AlignCommand
            => _alignCommand ??= new SafeDelegateCommandAsync(
                AlignCommandExecute,
                AlignCommandCanExecute);

        private Task AlignCommandExecute()
        {
            var angleSetPoint = Angle.FromDegrees(CurrentAngleAsDouble);

            if (SelectedAngleSources != AngleSources.First()
                && _processModules.FirstOrDefault(d => d.Name == SelectedAngleSources) is {} pm)
            {
                angleSetPoint = Angle.FromDegrees(pm.GetAlignmentAngle());
            }

            return Aligner.AlignAsync(angleSetPoint, AlignType);
        }

        private bool AlignCommandCanExecute()
        {
            if (Aligner == null)
            {
                return false;
            }

            var context = Aligner.NewCommandContext(nameof(Aligner.Align))
                .AddArgument("target", Angle.FromDegrees(0))
                .AddArgument("alignType", AlignType);
            return Aligner.CanExecute(context);
        }

        #endregion

        #region public

        public void Setup(List<UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule.DriveableProcessModule> processModules)
        {
            _processModules = processModules;

            foreach (var processModule in _processModules)
            {
                AngleSources.Add(processModule.Name);
            }

            SelectedAngleSources = _processModules.Count > 0 ? AngleSources[1] : AngleSources.First();
        }

        #endregion
    }
}
