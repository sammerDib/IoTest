using System;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.AsoEditor
{
    ///////////////////////////////////////////////////////////////////////
    // View Model pour chaque DefectClass / DefectCategory (chaque ligne de la DataGrid).
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class DefectViewModel : ObservableRecipient
    {
        private AsoDefectClass asoDefectClass; // Le modèle sous-jacent
        private AsoDefectVidCategory asoDefectCategory; // Le modèle sous-jacent
        private AsoEditorParameter Parameter { get; set; }

        //=================================================================
        // Constructor
        //=================================================================
        public DefectViewModel(AsoEditorParameter parameter, AsoDefectClass defectClass)
        {
            Parameter = parameter;
            asoDefectClass = defectClass;
        }

        public DefectViewModel(AsoEditorParameter parameter, AsoDefectVidCategory defectCategory)
        {
            Parameter = parameter;
            asoDefectCategory = defectCategory;
        }

        //=================================================================
        // Propriétés Bindables
        //=================================================================
        public string DefectLabel
        {
            get { return asoDefectClass.DefectLabel; }
        }

        public int VidNumber
        {
            get { return asoDefectCategory.VID; }
        }

        public string DefectCategory
        {
            get
            {
                if (asoDefectCategory != null)
                    return asoDefectCategory.DefectCategory;
                else
                    return asoDefectClass.DefectCategory;
            }
            set
            {
                if (value == DefectCategory)
                    return;
                if (asoDefectCategory != null)
                    asoDefectCategory.DefectCategory = value;
                else
                    asoDefectClass.DefectCategory = value;
                OnPropertyChanged();
                Parameter.ReportChange();
            }
        }

        public string Color
        {
            get
            {
                if (asoDefectCategory != null)
                    return asoDefectCategory.Color;
                else
                    return asoDefectClass.Color;
            }
            set
            {
                if (value == Color)
                    return;
                if (asoDefectCategory != null)
                    asoDefectCategory.Color = FindClosestKnwonColor(value);
                else
                    asoDefectClass.Color = FindClosestKnwonColor(value);
                OnPropertyChanged();
                Parameter.ReportChange();
            }
        }

        public bool SaveThumbnails
        {
            get
            {
                if (asoDefectCategory != null)
                    return asoDefectCategory.SaveThumbnails;
                else
                    return asoDefectClass.SaveThumbnails;
            }
            set
            {
                if (value == SaveThumbnails)
                    return;
                if (asoDefectCategory != null)
                    asoDefectCategory.SaveThumbnails = value;
                else
                    asoDefectClass.SaveThumbnails = value;
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
