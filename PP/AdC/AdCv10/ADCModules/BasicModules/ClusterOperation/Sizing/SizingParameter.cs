using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;

namespace BasicModules.Sizing
{
    internal class SizingParameter : ClassificationParameterBase
    {
        public CustomExceptionDictionary<string, SizingClass> SizingClasses { get; private set; } = new CustomExceptionDictionary<string, SizingClass>(exceptionKeyName: "Defect Class");

        //=================================================================
        // Constructeur
        //=================================================================
        public SizingParameter(ModuleBase module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "SizingClass")
                {
                    SizingClass sizingClass = new SizingClass();
                    sizingClass.DefectLabel = node.GetAttributeValue("Value");
                    sizingClass.Measure = node.GetAttributeValue("Measure").ToEnum<eSizingType>();
                    sizingClass.TuningMultiplier = Int32.Parse(node.GetAttributeValue("TuningMultiplier"));
                    sizingClass.TuningOffset = Int32.Parse(node.GetAttributeValue("TuningOffset"));

                    SizingClasses.Add(sizingClass.DefectLabel, sizingClass);
                }
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            foreach (SizingClass sizingClass in SizingClasses.Values)
            {
                XmlElement node = SaveParameter(xmlNode, "SizingClass", sizingClass.DefectLabel);
                node.AddAttribute("Measure", sizingClass.Measure);
                node.AddAttribute("TuningMultiplier", sizingClass.TuningMultiplier);
                node.AddAttribute("TuningOffset", sizingClass.TuningOffset);
            }
            return null;
        }

        //=================================================================
        // Synchornization de la liste des défauts avec la classif
        //=================================================================
        public void SynchronizeWithClassification()
        {
            HashSet<string> defectClasses = FindAvailableDefectLabels();

            // Suppression des classes non utilisées
            //......................................
            var sizing_class_labels = SizingClasses.Keys.ToList();
            foreach (string label in sizing_class_labels)
            {
                bool exists = defectClasses.Contains(label);
                if (!exists)
                    SizingClasses.Remove(label);
            }

            // Ajout des nouvelles classes
            //............................
            foreach (string label in defectClasses)
            {
                SizingClass sizingClass;
                bool exists = SizingClasses.TryGetValue(label, out sizingClass);
                if (!exists)
                {
                    sizingClass = new SizingClass();
                    sizingClass.DefectLabel = label;
                    SizingClasses.Add(sizingClass.DefectLabel, sizingClass);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            HashSet<string> defectClasses = FindAvailableDefectLabels();

            // Classes ajoutées ?
            //...................
            int count = 0;
            string error = null;

            foreach (string label in defectClasses)
            {
                SizingClass sizingClass;
                bool exists = SizingClasses.TryGetValue(label, out sizingClass);
                if (!exists)
                {
                    if (count++ > 5)
                        return "New defect classes, review configuration";
                    else
                    {
                        if (count > 1)
                            error += Environment.NewLine;
                        error += "New defect class: " + label;
                    }
                }
            }
            if (error != null)
                return "Review configuration" + error;


            // Suppression des classes non utilisées
            //......................................
            var sizing_class_labels = SizingClasses.Keys.ToList();
            foreach (string label in sizing_class_labels)
            {
                bool exists = defectClasses.Contains(label);
                if (!exists)
                    return "Defect classes have changed, review configuration";
            }

            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as SizingParameter;

            return parameter != null &&
                   SizingClasses.DictionaryEqual(parameter.SizingClasses);
        }

        //=================================================================
        // IHM
        //=================================================================
        private SizingControl _parameterUI;
        private SizingViewModel _viewModel;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _viewModel = new SizingViewModel(this);
                    _parameterUI = new SizingControl(_viewModel);
                }

                return _parameterUI;
            }
        }


    }
}
