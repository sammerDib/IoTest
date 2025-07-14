using System;
using System.Windows.Controls;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    /// <summary>
    /// Interaction logic for LayerSelectionView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class LayerSelectionView : UserControl
    {
        private LayerSelectionParameter Parameter;

        //=================================================================
        // Constructeur
        //=================================================================
        internal LayerSelectionView(LayerSelectionParameter parameter)
        {
            InitializeComponent();
            DataContext = Parameter = parameter;
        }

        //=================================================================
        // 
        //=================================================================
        private void UserControl_Loaded(object sender, EventArgs e)
        {
            Parameter.Synchronize();
        }

    }
}
