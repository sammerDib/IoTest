using System;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace GlobaltopoModule.ViewModel
{
    /// <summary>
    /// View model pour l'afficahge des areas
    /// </summary>
    public class AreaViewModel : ObservableRecipient
    {
        public RectangleF Rectangle;
        private Action _synchroVmToM;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="rectangleF"> Rectangle de descirption</param>
        /// <param name="synchroVmToM"> Action pour la synchro avec le model </param>
        public AreaViewModel(RectangleF rectangleF, Action synchroVmToM)
        {
            Rectangle = rectangleF;
            _synchroVmToM = synchroVmToM;
        }

        public float X
        {
            get => Rectangle.X;
            set { if (Rectangle.X != value) { Rectangle.X = value; OnPropertyChanged(); _synchroVmToM.Invoke(); } }
        }

        public float Y
        {
            get => Rectangle.Y;
            set { if (Rectangle.Y != value) { Rectangle.Y = value; OnPropertyChanged(); _synchroVmToM.Invoke(); } }
        }

        public float Width
        {
            get => Rectangle.Width;
            set { if (Rectangle.Width != value) { Rectangle.Width = value; OnPropertyChanged(); _synchroVmToM.Invoke(); } }
        }

        public float Height
        {
            get => Rectangle.Height;
            set { if (Rectangle.Height != value) { Rectangle.Height = value; OnPropertyChanged(); _synchroVmToM.Invoke(); } }
        }
    }
}
