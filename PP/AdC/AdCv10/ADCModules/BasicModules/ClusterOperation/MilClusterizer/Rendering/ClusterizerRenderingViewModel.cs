using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using AdcBasicObjects;
using AdcBasicObjects.Rendering;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;


namespace BasicModules.MilClusterizer
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClusterizerRenderingViewModel : ImageRenderingViewModel
    {
        public MilClusterizerModule MilClusterizer => (MilClusterizerModule)Module;

        public ClusterizerRenderingViewModel(ModuleBase module) :
            base(module)
        {
        }

        public ObservableCollection<Cluster> Clusters { get; } = new ObservableCollection<Cluster>();

        private double _xmin;
        public double XMin
        {
            get => _xmin; set { if (_xmin != value) { _xmin = value; OnPropertyChanged(); } }
        }


        private double _xmax;
        public double XMax
        {
            get => _xmax; set { if (_xmax != value) { _xmax = value; OnPropertyChanged(); } }
        }


        private double _ymin;
        public double YMin
        {
            get => _ymin; set { if (_ymin != value) { _ymin = value; OnPropertyChanged(); } }
        }


        private double _ymax;
        public double YMax
        {
            get => _ymax; set { if (_ymax != value) { _ymax = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Affiche les clusters en plus de l'image
        /// </summary>
        /// <param name="index"></param>
        protected override void DisplayImage(int index)
        {
            // On remplace base.DisplayImage(index);
            ImageBase image = (ImageBase)RenderingObjects[ImageIndex];
            MilImage milImage = image.OriginalProcessingImage.GetMilImage();

            CurrentBitmap = milImage.ConvertToWpfBitmapSource();
            FileName = !string.IsNullOrEmpty(image.Filename) ? Path.GetFileName(image.Filename) : string.Empty;

            XMin = image.imageRect.X;
            YMin = image.imageRect.Y;
            XMax = XMin + image.imageRect.Width;
            YMax = YMin + image.imageRect.Height;

            // Et on ajoute les clusters
            Clusters.Clear();
            var clusters = MilClusterizer.RenderingClusters.Where(cl => cl.pixelRect.IntersectsWith(image.imageRect));
            Clusters.AddRange(clusters);
        }

    }
}
