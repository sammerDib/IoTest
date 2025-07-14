using System;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.KlarfEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class DefectColorViewModel : ObservableRecipient
    {
        private KlarfDefectColorCategory klaColorCategory; // Le modèle sous-jacent  
        private KlarfDefectColorParameters Parameter { get; set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public DefectColorViewModel(KlarfDefectColorCategory colorCategory)
        {
            klaColorCategory = colorCategory;
        }

        public DefectColorViewModel(KlarfDefectColorParameters parameter, KlarfDefectColorCategory defectCategory)
        {
            Parameter = parameter;
            klaColorCategory = defectCategory;
        }

        public string DefectLabel
        {
            get { return klaColorCategory.DefectLabel; }
        }

        public string Color
        {
            get
            {
                if (klaColorCategory != null)
                    return klaColorCategory.Color;
                else
                    return klaColorCategory.Color;
            }
            set
            {
                if (value == Color)
                    return;
                if (klaColorCategory != null)
                    klaColorCategory.Color = FindClosestKnwonColor(value);
                else
                    klaColorCategory.Color = FindClosestKnwonColor(value);
                OnPropertyChanged();
                Parameter.ReportChange();
            }
        }


        //=================================================================
        // 
        //=================================================================
        private string FindClosestKnwonColor(string ColorName)
        {
            Color col = System.Drawing.ColorTranslator.FromHtml(ColorName);
            KnownColor kc = FindClosestKnwonColor(col);
            return kc.ToString();
        }

        private KnownColor FindClosestKnwonColor(Color color)
        {
            double dist0 = double.PositiveInfinity;
            KnownColor kc0 = KnownColor.Red;

            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
            {
                Color c = System.Drawing.Color.FromKnownColor(kc);
                double dist = Math.Sqrt(Math.Pow(color.R - c.R, 2) + Math.Pow(color.G - c.G, 2) + Math.Pow(color.B - c.B, 2));
                if (dist < dist0)
                {
                    dist0 = dist;
                    kc0 = kc;
                }
            }

            return kc0;
        }
    }
}
