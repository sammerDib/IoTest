using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using ADCEngine;

using AdcTools;

using AdvancedModules.ClusterOperation.Sieve.View;
using AdvancedModules.ClusterOperation.Sieve.ViewModel;

using BasicModules;

namespace AdvancedModules.ClusterOperation.Sieve
{
    internal class SieveParameter : ClassificationParameterBase
    {
        #region Properties 
        public CustomExceptionDictionary<string, SieveClass> SieveClasses { get; private set; }

        private SieveViewModel _sieveViewModel;

        private SieveParameterView _parameterUI;

        #endregion

        #region Constructors
        public SieveParameter(ModuleBase module, string name) :
        base(module, name)
        {
            // TO IMPLEMENT 
            SieveClasses = new CustomExceptionDictionary<string, SieveClass>(exceptionKeyName: "DefectLabel");
        }

        #endregion

        #region Methods
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _sieveViewModel = new SieveViewModel(this);
                    _parameterUI = new SieveParameterView();
                    _parameterUI.DataContext = _sieveViewModel;
                }
                return _parameterUI;
            }

        }
        public override bool HasSameValue(object obj)
        {
            throw new NotImplementedException();
        }

        public override void Load(XmlNodeList pXmlNodes)
        {
            XmlNode nodes = ReadParameter("Class", pXmlNodes);
            foreach (XmlNode node in nodes)
            {
                SieveClass sieveClass = Serializable.LoadFromXml<SieveClass>(node);
                SieveClasses.Add(sieveClass.DefectLabel, sieveClass);
            }
        }

        public override XmlElement Save(XmlNode pXmlNode)
        {
            XmlElement node = SaveParameter(pXmlNode, "Class", this);
            foreach (SieveClass defectCategory in SieveClasses.Values)
                defectCategory.SerializeAsChildOf(node);

            return null;
        }

        public void Synchronize()
        {
            HashSet<string> defectClassList = FindAvailableDefectLabelsWithDummyDefect();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            ////-------------------------------------------------------------
            foreach (string label in defectClassList)
            {
                SieveClass sieveClass;
                bool found = SieveClasses.TryGetValue(label, out sieveClass);
                if (!found)
                {
                    sieveClass = new SieveClass();
                    sieveClass.DefectLabel = label;
                    SieveClasses.Add(sieveClass.DefectLabel, sieveClass);
                }
            }

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (SieveClass sieveClass in SieveClasses.Values.ToList())
            {
                bool found = defectClassList.Contains(sieveClass.DefectLabel);
                if (!found)
                    SieveClasses.Remove(sieveClass.DefectLabel);
            }
        }

        public override string Validate()
        {
            //-------------------------------------------------------------
            // Récupération des infos des autres modules
            //-------------------------------------------------------------
            HashSet<string> defectClassList = FindAvailableDefectLabelsWithDummyDefect();

            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées
            //-------------------------------------------------------------
            int count = 0;
            string error = null;

            foreach (string label in defectClassList)
            {
                SieveClass sieveClass;
                bool found = SieveClasses.TryGetValue(label, out sieveClass);
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
            }
            if (error != null)
                return "Review configuration," + error;

            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (SieveClass sieveClass in SieveClasses.Values.ToList())
            {
                bool found = defectClassList.Contains(sieveClass.DefectLabel);
                if (!found)
                    return "Defect classes have changed, review configuration";
            }

            //-------------------------------------------------------------
            // Sinon tout va bien
            //-------------------------------------------------------------
            return null;
        }

        #endregion



    }
}
