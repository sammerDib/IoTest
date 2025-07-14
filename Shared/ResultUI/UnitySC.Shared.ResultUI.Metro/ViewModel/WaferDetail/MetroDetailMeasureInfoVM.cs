using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail
{
    /// <summary>
    /// This class is only used to specify the DataContext of the MetroMeasureInfoHeaderView view.
    /// </summary>
    public class DesignInstanceDetailMeasureInfo : MetroDetailMeasureInfoVM<MeasurePointResult>
    {
        
    }

    public abstract class MetroDetailMeasureInfoVM<T> : ObservableObject where T : MeasurePointResult
    {
        #region Properties

        public const string DifferenceWithTargetSymbole = "Deviation";

        private string _dieIndex;

        public string DieIndex
        {
            get => _dieIndex;
            set => SetProperty(ref _dieIndex, value);
        }

        public bool HasSelectedPoint => Point != null;

        private T _point;

        public T Point
        {
            get => _point;
            set
            {
                if (SetProperty(ref _point, value))
                {
                    OnPointChanged();
                }
            }
        }

        private MeasureDieResult _die;

        public MeasureDieResult Die
        {
            get => _die;
            set 
            {
                if (SetProperty(ref _die, value))
                {
                    OnDieChanged();
                }
            }
        }

        private int _digits;

        public int Digits
        {
            get => _digits;
            set 
            {
                if (SetProperty(ref _digits, value))
                {
                    OnPointChanged();
                }
            }
        }

        #endregion
        
        protected virtual void OnPointChanged()
        {
            OnPropertyChanged(nameof(HasSelectedPoint));
        }

        protected virtual void OnDieChanged()
        {
            DieIndex = Die != null ? $"[ {Die.ColumnIndex} ; {Die.RowIndex} ]" : null;
        }
    }
}
