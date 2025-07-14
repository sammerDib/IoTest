using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.Classification
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ClassificationParameter : ClassificationParameterBase
    {
        public List<DefectClass> DefectClassList { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationParameter(ClassificationModule module, string name)
            : base(module, name)
        {
            DefectClassList = new List<DefectClass>();
        }
        // ajout pour faire marcher ma classe ClassificationWithDebugModule
        public ClassificationParameter(ClassificationWithDebugModule module, string name)
            : base(module, name)
        {
            DefectClassList = new List<DefectClass>();
        }
        //=================================================================
        // XML
        //=================================================================
        public override void Load(XmlNodeList xmlNodes)
        {
            foreach (XmlNode node in xmlNodes)
            {
                if (node.Attributes["Label"].Value == "DefectClass")
                {
                    DefectClass defectClass = new DefectClass();
                    defectClass.Load(node);
                    DefectClassList.Add(defectClass);
                }
            }
        }

        public override XmlElement Save(XmlNode xmlNode)
        {
            foreach (DefectClass defectClass in DefectClassList)
                defectClass.Save(xmlNode);

            return null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private ClassificationControl _parameterUI;
        private ClassificationViewModel _parameterViewModel;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterViewModel = new ClassificationViewModel(this);
                    _parameterUI = new ClassificationControl(_parameterViewModel);
                }

                return _parameterUI;
            }
        }

        //=================================================================
        //
        //=================================================================
        public override string Validate()
        {
            HashSet<Characteristic> availableCharacteristics = FindAvailableCharacteristics();

            if (DefectClassList.Count == 0)
                return "Empty classification";

            foreach (DefectClass defectClass in DefectClassList)
            {
                if (defectClass.compartorList.Count == 0)
                    return "No criteria for defect class: " + defectClass.label;

                foreach (ComparatorBase cmp in defectClass.compartorList)
                {
                    bool ok = availableCharacteristics.Contains(cmp.characteristic);
                    if (!ok)
                        return "Characteristics have changed, review the classification";
                }
            }

            return null;
        }

        //=================================================================
        //
        //=================================================================
        public void Synchronize()
        {
            HashSet<Characteristic> caracs = FindAvailableCharacteristics();

            bool changed = false;
            foreach (DefectClass defectClass in DefectClassList)
            {
                for (int i = defectClass.compartorList.Count - 1; i >= 0; i--)
                {
                    ComparatorBase cmp = defectClass.compartorList[i];
                    bool ok = caracs.Contains(cmp.characteristic);
                    if (!ok)
                    {
                        defectClass.compartorList.RemoveAt(i);
                        changed = false;
                    }
                }
            }

            if (changed)
                ReportChange();
        }

        public override bool HasSameValue(object obj)
        {
            var parameter = obj as ClassificationParameter;
            return parameter != null &&
                 DefectClassList.ValuesEqual(parameter.DefectClassList);
        }

    }
}
