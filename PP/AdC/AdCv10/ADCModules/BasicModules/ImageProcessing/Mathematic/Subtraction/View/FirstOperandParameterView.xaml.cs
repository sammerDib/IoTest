using System;
using System.Windows.Controls;

namespace BasicModules.Mathematic
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class FirstOperandParameterView : UserControl
    {
        public FirstOperandParameterView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, EventArgs e)
        {
            ((FirstOperandParameterViewModel)DataContext).Synchronize();
        }
    }
}
