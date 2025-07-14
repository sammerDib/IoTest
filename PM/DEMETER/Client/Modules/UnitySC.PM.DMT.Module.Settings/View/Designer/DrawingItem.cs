using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.DMT.Modules.Settings.View.Designer
{
    public class DrawingItem : ObservableObject
    {
        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible; set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(); } }
        }


    }
}
