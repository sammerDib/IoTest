using System;
using System.Windows.Controls;
using System.Xml;

using ADCConfiguration.View.Recipe;

using ADCEngine;

namespace ADCConfiguration.ViewModel.Recipe
{
    /// <summary>
    /// Paramétre pour la comparaison de historique des fichiers
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class FileParameterHistoryViewModel : ParameterBase
    {
        private FileParameterHistory _parameterUI;

        public FileParameterHistoryViewModel(ModuleBase module, string label, int? version, string fileName) : base(module, label)
        {
            _version = version;
            _fileName = fileName;
            SelectedOption = fileName;
        }

        /// <summary>
        /// Version du fichier en BDD
        /// </summary>
        private int? _version;
        public int? Version
        {
            get => _version; set { if (_version != value) { _version = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Nom du fichier
        /// </summary>
        private string _fileName;
        public string FileName
        {
            get => _fileName; set { if (_fileName != value) { _fileName = value; OnPropertyChanged(); } }
        }

        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new FileParameterHistory();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        public override bool HasSameValue(object obj)
        {
            var fileParameter = obj as FileParameterHistoryViewModel;
            return fileParameter != null &&
                Name == fileParameter.Name &&
                SelectedOption == fileParameter.SelectedOption &&
                Version == fileParameter.Version;
        }

        public override void Load(XmlNodeList parameterNodes)
        {
            throw new NotImplementedException();
        }

        public override XmlElement Save(XmlNode parametersNode)
        {
            throw new NotImplementedException();
        }
    }
}
