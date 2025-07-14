using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using BasicModules.Edition.MicroscopeReview.View;
using BasicModules.Edition.MicroscopeReview.ViewModel;

namespace BasicModules.Edition.MicroscopeReview
{
    public class MicroscopeReviewParameter : ClassificationParameterBase
    {
        public Dictionary<string, MicroscopeReviewClass> MicroscopeReviewClassses { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public MicroscopeReviewParameter(MicroscopeReviewModule module, string name) :
            base(module, name)
        {
            MicroscopeReviewClassses = new Dictionary<string, MicroscopeReviewClass>();
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            XmlNode node = ReadParameter("Class", xmlNodes);
            foreach (XmlNode childnode in node)
            {
                MicroscopeReviewClass category = Serializable.LoadFromXml<MicroscopeReviewClass>(childnode);
                MicroscopeReviewClassses.Add(category.DefectLabel, category);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement node = SaveParameter(xmlNode, "Class", this);

            foreach (MicroscopeReviewClass defectCategory in MicroscopeReviewClassses.Values)
                defectCategory.SerializeAsChildOf(node);

            return null;
        }


        // =================================================================
        //  IHM
        // =================================================================
        private MicroscopeReviewViewModel _parameterViewModel;
        private MicroscopeReviewControl _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new MicroscopeReviewViewModel(this);
                    _parameterUI = new MicroscopeReviewControl();
                    _parameterUI.DataContext = _parameterViewModel;
                }
                return _parameterUI;
            }
        }

        public void Synchronize()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> defectClassList = FindAvailableDefectLabels();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            foreach (string label in defectClassList)
            {
                MicroscopeReviewClass microscopeReviewClass;
                bool found = MicroscopeReviewClassses.TryGetValue(label, out microscopeReviewClass);
                if (!found)
                {
                    microscopeReviewClass = new MicroscopeReviewClass();
                    microscopeReviewClass.NbSamples = 1;
                    microscopeReviewClass.DefectLabel = label;
                    MicroscopeReviewClassses.Add(microscopeReviewClass.DefectLabel, microscopeReviewClass);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (MicroscopeReviewClass microscopeReviewClass in MicroscopeReviewClassses.Values.ToList())
            {
                bool found = defectClassList.Contains(microscopeReviewClass.DefectLabel);
                if (!found)
                    MicroscopeReviewClassses.Remove(microscopeReviewClass.DefectLabel);
            }
        }

        public override string Validate()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> defectClassList = FindAvailableDefectLabels();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            int count = 0;
            string error = null;

            foreach (string label in defectClassList)
            {
                MicroscopeReviewClass microscopeReviewClass;
                bool found = MicroscopeReviewClassses.TryGetValue(label, out microscopeReviewClass);
                if (!found)
                {
                    if (count++ < 5)
                    {
                        if (count > 1)
                            error += Environment.NewLine;
                        error += "New defect class:" + label;
                    }
                    else
                        return "New defect classes, review configuration";
                }
                else
                {
                    //  Todo : Validation. Utile ? 
                }
            }
            if (error != null)
                return "Review configuration," + error;

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (MicroscopeReviewClass microscopeReviewClasss in MicroscopeReviewClassses.Values.ToList())
            {
                bool found = defectClassList.Contains(microscopeReviewClasss.DefectLabel);
                if (!found)
                    return "Defect classes have changed, review configuration";
            }

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as MicroscopeReviewParameter;
            return parameter != null &&
                   MicroscopeReviewClassses.DictionaryEqual(parameter.MicroscopeReviewClassses);
        }
    }
}
