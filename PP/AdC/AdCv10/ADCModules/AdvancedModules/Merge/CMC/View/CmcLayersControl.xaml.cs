using System;
using System.Windows.Controls;


namespace AdvancedModules.CmcNamespace
{
    /// <summary>
    /// Logique d'interaction pour CmcLayersControl.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class CmcLayersControl : UserControl
    {
        private CmcViewModel ViewModel { get { return (CmcViewModel)DataContext; } }

        //=================================================================
        // Constructeur
        //=================================================================
        internal CmcLayersControl(CmcViewModel datacontext)
        {
            DataContext = datacontext;
            InitializeComponent();
        }

        //=================================================================
        //
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            // Refresh the view Model
            ViewModel.Init();
        }

    }
}
