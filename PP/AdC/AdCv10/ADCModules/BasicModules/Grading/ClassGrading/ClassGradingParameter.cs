using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Xml;

using AdcTools;

using BasicModules.Grading.ClassGrading.View;
using BasicModules.Grading.ClassGrading.ViewModel;

namespace BasicModules.Grading.ClassGrading
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ClassGradingParameter : ClassificationParameterBase
    {
        public ObservableCollection<ClassGradingRule> ClassGradingRules { get; private set; }
        public HashSet<string> DefectClassList { get; private set; }
        private UserControl _parameterUI;
        private ClassGradingViewModel _classGradingVM;

        public ClassGradingParameter(ClassGradingModule module, string name) :
            base(module, name)
        {
            ClassGradingRules = new ObservableCollection<ClassGradingRule>();
        }

        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _classGradingVM = new ClassGradingViewModel(this);
                    _parameterUI = new ClassGradingControl();
                    _parameterUI.DataContext = _classGradingVM;
                }
                return _parameterUI;
            }
        }

        public override bool HasSameValue(object obj)
        {
            var @class = obj as ClassGradingParameter;
            return @class != null &&
                   ClassGradingRules.SequenceEqual(@class.ClassGradingRules);
        }

        public override void Load(XmlNodeList parameterNodes)
        {
            XmlNode node = ReadParameter("Rule", parameterNodes);
            foreach (XmlNode childnode in node)
            {
                ClassGradingRule rule = Serializable.LoadFromXml<ClassGradingRule>(childnode);
                ClassGradingRules.Add(rule);
            }
        }

        public override XmlElement Save(XmlNode parametersNode)
        {
            XmlElement node = SaveParameter(parametersNode, "Rule", this);

            foreach (ClassGradingRule rule in ClassGradingRules)
                rule.SerializeAsChildOf(node);

            return null;
        }


        public void Synchronize()
        {
            DefectClassList = FindAvailableDefectLabels();

            foreach (var rule in ClassGradingRules)
            {
                if (!DefectClassList.Contains(rule.DefectClass))
                    rule.DefectClass = null;
            }
        }

        public override string Validate()
        {
            Synchronize();
            if (ClassGradingRules.Any(x => string.IsNullOrEmpty(x.DefectClass)))
                return "Defect class is missing";
            return null;
        }
    }
}
