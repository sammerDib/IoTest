using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace GlobaltopoModule.ViewModel
{
    /// <summary>
    /// ViewModel pour l'affichage de la vue GlobalTopo
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class GTInputPrmViewModel : ObservableRecipient
    {
        private GTParameters _parameter;
        public ObservableCollection<AreaViewModel> ExcludeAreas { get; private set; }

        public GTInputPrmViewModel(GTParameters parameter)
        {
            _parameter = parameter;
            ExcludeAreas = new ObservableCollection<AreaViewModel>(_parameter.InputPrmClass.ExcludeAreasList.Select(x => new AreaViewModel(x, () => { SynchroVmToM(); })).ToList());
        }

        public float X1_mm
        {
            get => _parameter.InputPrmClass.X1_mm;
            set
            {
                if (value != _parameter.InputPrmClass.X1_mm)
                {
                    _parameter.InputPrmClass.X1_mm = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }
        public float X2_mm
        {
            get => _parameter.InputPrmClass.X2_mm;
            set
            {
                if (value != _parameter.InputPrmClass.X2_mm)
                {
                    _parameter.InputPrmClass.X2_mm = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public float Y1_mm
        {
            get => _parameter.InputPrmClass.Y1_mm;
            set
            {
                if (value != _parameter.InputPrmClass.Y1_mm)
                {
                    _parameter.InputPrmClass.Y1_mm = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public float Y2_mm
        {
            get => _parameter.InputPrmClass.Y2_mm;
            set
            {
                if (value != _parameter.InputPrmClass.Y2_mm)
                {
                    _parameter.InputPrmClass.Y2_mm = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public float EdgeExcusion_mm
        {
            get => _parameter.InputPrmClass.EdgeExcusion_mm;
            set
            {
                if (value != _parameter.InputPrmClass.EdgeExcusion_mm)
                {
                    _parameter.InputPrmClass.EdgeExcusion_mm = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public int NbSamples
        {
            get => _parameter.InputPrmClass.NbSamples;
            set
            {
                if (value != _parameter.InputPrmClass.NbSamples)
                {
                    _parameter.InputPrmClass.NbSamples = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }
        public float RadiusCenterBow
        {
            get => _parameter.InputPrmClass.RadiusCenterBow;
            set
            {
                if (value != _parameter.InputPrmClass.RadiusCenterBow)
                {
                    _parameter.InputPrmClass.RadiusCenterBow = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public lpType LowPassKernelType
        {
            get => _parameter.InputPrmClass.LowPassKernelType;
            set
            {
                if (value != _parameter.InputPrmClass.LowPassKernelType)
                {
                    _parameter.InputPrmClass.LowPassKernelType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsKernelSizeVisible));
                    OnPropertyChanged(nameof(IsGaussianSigmaVisible));
                    _parameter.ReportChange();
                }
            }
        }

        private List<lpType> _lpTypes;
        public List<lpType> LpTypes
        {
            get
            {
                if (_lpTypes == null)
                {
                    _lpTypes = Enum.GetValues(typeof(lpType)).Cast<lpType>().ToList();
                }
                return _lpTypes;
            }
        }
        public bool IsKernelSizeVisible
        {
            get => LowPassKernelType == lpType.Gaussian || LowPassKernelType == lpType.Uniform;
        }

        public int LowPassKernelSize
        {
            get => _parameter.InputPrmClass.LowPassKernelSize;
            set
            {
                if (value != _parameter.InputPrmClass.LowPassKernelSize)
                {
                    _parameter.InputPrmClass.LowPassKernelSize = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        public bool IsGaussianSigmaVisible
        {
            get => LowPassKernelType == lpType.Gaussian;
        }

        public float LowPassGaussianSigma
        {
            get => _parameter.InputPrmClass.LowPassGaussianSigma;
            set
            {
                if (value != _parameter.InputPrmClass.LowPassGaussianSigma)
                {
                    _parameter.InputPrmClass.LowPassGaussianSigma = value;
                    OnPropertyChanged();
                    _parameter.ReportChange();
                }
            }
        }

        private AreaViewModel _selectedArea;
        public AreaViewModel SelectedArea
        {
            get => _selectedArea;
            set
            {
                if (value != _selectedArea)
                {
                    _selectedArea = value;
                    RemoveCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged();
                }
            }
        }

        public void SynchroVmToM()
        {
            _parameter.InputPrmClass.ExcludeAreasList = ExcludeAreas.Select(x => x.Rectangle).ToList();
        }

        #region Command

        private AutoRelayCommand _addCommand = null;
        public AutoRelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new AutoRelayCommand(
              () =>
              {
                  ExcludeAreas.Add(new AreaViewModel(new RectangleF(), () => { SynchroVmToM(); }));
                  SelectedArea = ExcludeAreas.Last<AreaViewModel>();
                  SynchroVmToM();
                  _parameter.ReportChange();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _removeCommand = null;
        public AutoRelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new AutoRelayCommand(
              () =>
              {
                  ExcludeAreas.Remove(SelectedArea);
                  SelectedArea = null;
                  SynchroVmToM();
                  _parameter.ReportChange();
              },
              () => { return SelectedArea != null; }));
            }
        }

        #endregion
    }
}
