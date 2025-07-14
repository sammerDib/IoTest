using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Data;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class EMERecipeVM : ObservableObject, IRecipeInfo, IPmRecipe, IEMERecipe
    {
        #region Fields
        private bool _isModified = false;
        private ActorType _actorType;
        private string _comment;
        private DateTime _created;
        private bool _isShared;
        private bool _isTemplate;
        private Guid _key;
        private PathString _name;
        private int? _stepId;
        private int? _userId;
        private int? _creatorChamberId;
        private int _version;
        private string _content;
        private string _fileVersion;
        private List<Acquisition> _acquisitions;
        private ExecutionSettings _execution;
        private Step _step;
        private bool _isSaveResultsEnabled;
        #endregion
        public EMERecipeVM()
        {
            this.PropertyChanged += EMERecipeVM_PropertyChanged;
        }

        private void EMERecipeVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsModified))
                IsModified = true;
        }            

        #region IRecipeInfo Implementation
        public ActorType ActorType { get => _actorType; set { if (_actorType != value) { _actorType = value; OnPropertyChanged(); } } }
        public string Comment { get => _comment; set { if (_comment != value) { _comment = value; OnPropertyChanged(); } } }
        public DateTime Created { get => _created; set { if (_created != value) { _created = value; OnPropertyChanged(); } } }
        public bool IsShared { get => _isShared; set { if (_isShared != value) { _isShared = value; OnPropertyChanged(); } } }
        public bool IsTemplate { get => _isTemplate; set { if (_isTemplate != value) { _isTemplate = value; OnPropertyChanged(); } } }
        public Guid Key { get => _key; set { if (_key != value) { _key = value; OnPropertyChanged(); } } }
        public string Name { get => _name; set { if (_name != value) { _name = new PathString(value).RemoveInvalidFilePathCharacters("_", false); OnPropertyChanged(); } } }
        public int? StepId { get => _stepId; set { if (_stepId != value) { _stepId = value; OnPropertyChanged(); } } }
        public int? UserId { get => _userId; set { if (_userId != value) { _userId = value; OnPropertyChanged(); } } }
        public int? CreatorChamberId { get => _creatorChamberId; set { if (_creatorChamberId != value) { _creatorChamberId = value; OnPropertyChanged(); } } }

        #endregion IRecipeInfo Implementation

        #region IPmRecipe Implementation
        public int Version { get => _version; set { if (_version != value) { _version = value; OnPropertyChanged(); } } }
        public string Content { get => _content; set { if (_content != value) { _content = value; OnPropertyChanged(); } } }

        #endregion IPmRecipe Implementation

        #region IEMERecipe Implementation
        public string FileVersion { get => _fileVersion; set { if (_fileVersion != value) { _fileVersion = value; OnPropertyChanged(); } } }        
        public List<Acquisition> Acquisitions { get => _acquisitions; set { if (_acquisitions != value) { _acquisitions = value; OnPropertyChanged(); } } }
        public ExecutionSettings Execution { get => _execution; set { if (_execution != value) { _execution = value; OnPropertyChanged(); } } }
        public Step Step { get => _step; set { if (_step != value) { _step = value; OnPropertyChanged(); } } }
        public bool IsSaveResultsEnabled { get => _isSaveResultsEnabled; set { if (_isSaveResultsEnabled != value) { _isSaveResultsEnabled = value; OnPropertyChanged(); } } }        
        #endregion

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }
        public WaferDimensionalCharacteristic WaferDimentionalCharacteristic => Step?.Product?.WaferCategory.DimentionalCharacteristic;
    }
}
