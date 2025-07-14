using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures
{
    /// <summary>
    /// Interaction logic for MountainsEditView.xaml
    /// </summary>
    public partial class MountainsWPFControl : UserControl
    {
        private MountainsWinFormsControl _mountainsControl;
        private string _initDocPath;
        public MountainsWPFControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (_mountainsControl == null)
            {
                try
                {
                    // Create the ActiveX control.
                    _mountainsControl = new MountainsWinFormsControl();
                    MountainsConfiguration mountainsConfig = ClassLocator.Default.GetInstance<MountainsConfiguration>();
                    bool isHostedByPM = mountainsConfig.IsHostedByPM;
                    // Assign the ActiveX control as the host control's child.
                    if (isHostedByPM)
                    {
                        formHost.Child = _mountainsControl;
                        _mountainsControl.InitDoc(_initDocPath);
                        tbError.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _mountainsControl = null;
                        ClassLocator.Default.GetInstance<ILogger>().Error("Error during loading activeX. Check license");
                        tbError.Visibility = Visibility.Visible;
                    }

                }
                catch (Exception ex)
                {
                    _mountainsControl = null;
                    ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Error during loading activeX. Check license");
                    tbError.Visibility = Visibility.Visible;
                }

            }
            busy.IsBusy = false;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_mountainsControl != null)
            {
                _mountainsControl.ClearContent();
            }
        }

        public void InitDoc(string filePath = null)
        {
            _initDocPath = filePath;
            if (_mountainsControl != null)
            {
                _mountainsControl.InitDoc(_initDocPath);
            }
        }

        public void SubstituteStudiable(string filePath)
        {
            if (_mountainsControl != null)
            {
                _mountainsControl.SubstituteStudiable(filePath);
            }
        }

        public void ClearContent()
        {
            if (_mountainsControl != null)
            {
                _mountainsControl.ClearContent();
            }
        }

        public void Save(string filePath)
        {
            if (_mountainsControl != null)
            {
                _mountainsControl.Save(filePath);
            }
        }

        public List<ExternalProcessingResultItem> GetResultsDefinedInCurrentTemplate()
        {
            if (_mountainsControl != null)
            {
                return _mountainsControl.GetResultsDefinedInCurrentTemplate();
            }
            return null;
        }
    }
}
