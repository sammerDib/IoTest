using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Recipe
{
    public class ANARecipeVM : ObservableObject, IRecipeInfo, IPmRecipe, IANARecipe
    {
        #region Fields

        private bool _isModified = false;
        private bool _isRunExecuted = false;
        private string _comment;
        private bool _isShared;
        private DateTime _created;
        private bool _isTemplate;
        private Guid _key;
        private string _name;
        private int? _stepId;
        private int? _userId;
        private int? _creatorChamberId;
        private int _version;
        private string _content;
        private string _fileVersion;
        private List<MeasureSettingsBase> _measuresSettings;
        private List<MeasurePoint> _points;
        private List<DieIndex> _dies;
        private Step _step;
        private ActorType _actorType;
        private AlignmentSettings _alignment;
        private bool _isWaferMapSkipped;
        private bool _isAlignmentMarksSkipped;
        private WaferMapSettings _waferMap;
        private AlignmentMarksSettings _alignmentMarks;
        private bool _isWaferLessModified;
        private ExecutionSettings _execution;

        #endregion Fields

        public ANARecipeVM()
        {
            this.PropertyChanged += ANARecipeVM_PropertyChanged;
        }

        private void ANARecipeVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
        public string Name { get => _name; set { if (_name != value) { _name = new PathString(value).RemoveInvalidFilePathCharacters("_",false); OnPropertyChanged(); } } }
        public int? StepId { get => _stepId; set { if (_stepId != value) { _stepId = value; OnPropertyChanged(); } } }
        public int? UserId { get => _userId; set { if (_userId != value) { _userId = value; OnPropertyChanged(); } } }
        public int? CreatorChamberId { get => _creatorChamberId; set { if (_creatorChamberId != value) { _creatorChamberId = value; OnPropertyChanged(); } } }

        #endregion IRecipeInfo Implementation

        #region IPmRecipe Implementation

        public int Version { get => _version; set { if (_version != value) { _version = value; OnPropertyChanged(); } } }
        public string Content { get => _content; set { if (_content != value) { _content = value; OnPropertyChanged(); } } }

        #endregion IPmRecipe Implementation

        #region IANARecipe Implementation

        public string FileVersion { get => _fileVersion; set { if (_fileVersion != value) { _fileVersion = value; OnPropertyChanged(); } } }
        public List<MeasureSettingsBase> Measures { get => _measuresSettings; set { if (_measuresSettings != value) { _measuresSettings = value; OnPropertyChanged(); } } }
        public List<MeasurePoint> Points { get => _points; set { if (_points != value) { _points = value; OnPropertyChanged(); } } }
        public List<DieIndex> Dies { get => _dies; set { if (_dies != value) { _dies = value; OnPropertyChanged(); } } }
        public Step Step { get => _step; set { if (_step != value) { _step = value; OnPropertyChanged(); } } }

        public AlignmentSettings Alignment { get => _alignment; set { if (_alignment != value) { _alignment = value; OnPropertyChanged(); } } }

        public bool IsWaferMapSkipped { get => _isWaferMapSkipped; set { if (_isWaferMapSkipped != value) { _isWaferMapSkipped = value; OnPropertyChanged(); } } }
        public WaferMapSettings WaferMap { get => _waferMap; set { if (_waferMap != value) { _waferMap = value; OnPropertyChanged(); } } }

        public bool IsAlignmentMarksSkipped { get => _isAlignmentMarksSkipped; set { if (_isAlignmentMarksSkipped != value) { _isAlignmentMarksSkipped = value; OnPropertyChanged(); } } }

        public AlignmentMarksSettings AlignmentMarks { get => _alignmentMarks; set { if (_alignmentMarks != value) { _alignmentMarks = value; OnPropertyChanged(); } } }

        public ExecutionSettings Execution { get => _execution; set { if (_execution != value) { _execution = value; OnPropertyChanged(); } } }

        public bool IsWaferLessModified { get => _isWaferLessModified; set { if (_isWaferLessModified != value) { _isWaferLessModified = value; OnPropertyChanged(); } } }

        #endregion IANARecipe Implementation

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }

        public bool IsRunExecuted
        {
            get => _isRunExecuted; set { if (_isRunExecuted != value) { _isRunExecuted = value; OnPropertyChanged(); } }
        }


        public WaferDimensionalCharacteristic WaferDimentionalCharacteristic => Step?.Product?.WaferCategory.DimentionalCharacteristic;

 
        public MeasureSettingsBase GetMeasureFromName(string measureName)
        {
            return Measures.Find(m => m.Name == measureName);
        }

  
    }
}
