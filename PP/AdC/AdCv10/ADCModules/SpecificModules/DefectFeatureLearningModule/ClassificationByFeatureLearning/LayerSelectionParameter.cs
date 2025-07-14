using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class LayerSelectionParameter : ClassificationParameterBase
    {
        public ObservableCollection<LayerSelector> DefectClasses { get; } = new ObservableCollection<LayerSelector>();
        public ObservableCollection<Characteristic> AvailableCharacteristics { get; } = new ObservableCollection<Characteristic>();

        private bool _withCMC;
        [XmlIgnore]
        public bool WithCMC
        {
            get => _withCMC; set { if (_withCMC != value) { _withCMC = value; OnPropertyChanged(); } }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public LayerSelectionParameter(ClassificationByFeatureLearningModule module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            DefectClasses.Clear();
            XmlNode defectClassNode = ReadParameter(Name, parameterNodes);
            foreach (XmlNode node in defectClassNode.ChildNodes)
            {
                LayerSelector defectClass = Serializable.LoadFromXml<LayerSelector>(node);
                DefectClasses.Add(defectClass);
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            XmlElement elem = SaveParameter(xmlNode, Name, this);

            foreach (LayerSelector defectClass in DefectClasses)
                defectClass.SerializeAsChildOf(elem);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private LayerSelectionView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                    _parameterUI = new LayerSelectionView(this);
                return _parameterUI;
            }
        }

        ///=================================================================<summary>
        /// Vérifie que toutes les classes de défault sont bien gérées par
        /// l'object Parameter.
        ///</summary>=================================================================
        public bool Synchronize()
        {
            bool changed = false;

            WithCMC = Module.FindAncestors(mod => mod.Factory.ModuleName == "CMC").Any();

            HashSet<string> availableDefectLabelList = FindAvailableDefectLabels();
            IEnumerable<Characteristic> newCharacteristics = FindAvailableCharacteristics().Where(x => x.Type == typeof(double));

            // Suppression des characteristics inutiles
            for (int i = AvailableCharacteristics.Count - 1; i >= 0; i--)
            {
                if (!newCharacteristics.Contains(AvailableCharacteristics[i]))
                    AvailableCharacteristics.RemoveAt(i);
            }

            // Ajout des characteristics manquantes
            foreach (Characteristic characteristic in newCharacteristics)
            {
                if (!AvailableCharacteristics.Contains(characteristic))
                    AvailableCharacteristics.Add(characteristic);
            }

            //-------------------------------------------------------------
            // Suppression des classes inutiles
            //-------------------------------------------------------------
            for (int i = DefectClasses.Count - 1; i >= 0; i--)
            {
                LayerSelector defectClass = DefectClasses[i];
                bool needed = availableDefectLabelList.Any(l => l == defectClass.DefectLabel);
                if (!needed)
                {
                    DefectClasses.RemoveAt(i);
                    changed = true;
                }
            }

            //-------------------------------------------------------------
            // Création des classes manquantes
            //-------------------------------------------------------------
            foreach (string label in availableDefectLabelList)
            {
                LayerSelector selector = DefectClasses.Find(x => x.DefectLabel == label);
                if (selector != null)
                {
                    // Vérifie que la caractéristique sélectionnée existe
                    //...................................................
                    if (!AvailableCharacteristics.Contains(selector.Characteristic))
                        selector.Characteristic = AvailableCharacteristics.LastOrDefault();
                }
                else
                {
                    // Crée une classe de défaut
                    //..........................
                    selector = new LayerSelector();
                    selector.DefectLabel = label;
                    selector.Characteristic = AvailableCharacteristics.LastOrDefault();
                    DefectClasses.Add(selector);

                    changed = true;
                }
            }

            if (changed)
                ReportChange();

            Validate();

            return true;
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            //-------------------------------------------------------------
            // Est-ce que toutes les classes de défaut sont gérées ?
            //-------------------------------------------------------------
            HashSet<string> availableDefectLabelList = FindAvailableDefectLabels();
            HashSet<Characteristic> availableCharacteristics = FindAvailableCharacteristics();

            string error = null;
            int errcount = 0;
            foreach (string label in availableDefectLabelList)
            {
                // Vérifie que la classe de défaut existe
                //.......................................
                LayerSelector selector = DefectClasses.Find(x => x.DefectLabel == label);
                if (selector == null)
                {
                    if (errcount++ > 5)
                    {
                        return "Defect classes have changed, review the classification";
                    }
                    else
                    {
                        if (errcount > 1)
                            error += Environment.NewLine;
                        error += "New defect class, review the classification: " + selector.DefectLabel;
                    }
                }

                // Vérifie que la caractéristique sélectionnée existe
                //...................................................
                else
                {
                    if (!availableCharacteristics.Contains(selector.Characteristic))
                    {
                        if (errcount++ > 5)
                        {
                            return "Defect classes have changed, review the classification";
                        }
                        else
                        {
                            if (errcount > 1)
                                error += Environment.NewLine;
                            error += "Invalid characteristic, review the classification: " + selector.DefectLabel;
                        }
                    }
                }
            }

            if (error != null)
                return error;

            //-------------------------------------------------------------
            // Est-ce qu'il y a des classes inutiles ?
            //-------------------------------------------------------------
            if (DefectClasses.Count != availableDefectLabelList.Count)
                return "Defect classes have changed, review the classification";

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        //=================================================================
        //
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var parameter = obj as LayerSelectionParameter;
            return parameter != null &&
                   DefectClasses.ValuesEqual(parameter.DefectClasses);
        }

        //=================================================================
        //
        //=================================================================
        public override HashSet<string> FindAvailableDefectLabels()
        {
            Classification classification = ((ClassificationByFeatureLearningModule)Module).Classification;
            if (classification == null)
            {
                return new HashSet<string>();
            }
            else
            {
                var list = ((ClassificationByFeatureLearningModule)Module).Classification?.Defects.Select(d => d.ClassName);
                HashSet<string> defectLabels = new HashSet<string>(list);
                return defectLabels;
            }
        }

    }
}
