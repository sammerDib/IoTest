using System.Windows;
using System.Windows.Input;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Views
{
    public partial class GeneralInformationsView
    {
        public GeneralInformationsView()
        {
            InitializeComponent();
        }

        #region Property

        #region SubstrateCount

        public static readonly DependencyProperty SubstrateCountProperty =
            DependencyProperty.Register(
                nameof(SubstrateCount),
                typeof(int),
                typeof(GeneralInformationsView),
                new PropertyMetadata(0, null));

        public int SubstrateCount
        {
            get => (int)GetValue(SubstrateCountProperty);
            set => SetValue(SubstrateCountProperty, value);
        }

        #endregion

        #region CurrentActivityStep

        public static readonly DependencyProperty CurrentActivityStepProperty =
            DependencyProperty.Register(
                nameof(CurrentActivityStep),
                typeof(string),
                typeof(GeneralInformationsView),
                new PropertyMetadata("-", null));

        public string CurrentActivityStep
        {
            get => (string)GetValue(CurrentActivityStepProperty);
            set => SetValue(CurrentActivityStepProperty, value);
        }

        #endregion

        #region EquipmentName

        public static readonly DependencyProperty EquipmentNameProperty =
            DependencyProperty.Register(
                nameof(EquipmentName),
                typeof(string),
                typeof(GeneralInformationsView),
                new PropertyMetadata("-", null));

        public string EquipmentName
        {
            get => (string)GetValue(EquipmentNameProperty);
            set => SetValue(EquipmentNameProperty, value);
        }

        #endregion

        #region Throughput

        public static readonly DependencyProperty ThroughputProperty = DependencyProperty.Register(
            nameof(Throughput),
            typeof(double),
            typeof(GeneralInformationsView),
            new PropertyMetadata(0.0, null));

        public double Throughput
        {
            get => (double)GetValue(ThroughputProperty);
            set { SetValue(ThroughputProperty, value); }
        }

        public static readonly DependencyProperty ThroughputVisibilityProperty =
            DependencyProperty.Register(
                nameof(ThroughputVisibility),
                typeof(Visibility),
                typeof(GeneralInformationsView),
                new PropertyMetadata(default(Visibility)));

        public Visibility ThroughputVisibility
        {
            get => (Visibility)GetValue(ThroughputVisibilityProperty);
            set => SetValue(ThroughputVisibilityProperty, value);
        }
        #endregion

        #endregion

        #region Reset Command

        public static readonly DependencyProperty ResetCommandVisibilityProperty =
            DependencyProperty.Register(
                nameof(ResetCommandVisibility),
                typeof(Visibility),
                typeof(GeneralInformationsView),
                new PropertyMetadata(default(Visibility)));

        public Visibility ResetCommandVisibility
        {
            get => (Visibility)GetValue(ResetCommandVisibilityProperty);
            set => SetValue(ResetCommandVisibilityProperty, value);
        }

        public static readonly DependencyProperty ResetCommandProperty =
            DependencyProperty.Register(
                nameof(ResetCommand),
                typeof(ICommand),
                typeof(GeneralInformationsView),
                new PropertyMetadata(default(ICommand)));

        public ICommand ResetCommand
        {
            get => (ICommand)GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        #endregion

    }
}
