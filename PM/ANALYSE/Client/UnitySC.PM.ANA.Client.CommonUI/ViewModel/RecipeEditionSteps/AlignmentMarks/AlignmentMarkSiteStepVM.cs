using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentMarkSiteStepVM : StepBaseVM
    {
        public ObservableCollection<AlignmentMarkStepVM> AlignmentMarks { get; set; }

        public RecipeAlignmentMarksVM RecipeAlignmentMarks { get; set; }

        public AlignmentMarkSiteStepVM(RecipeAlignmentMarksVM recipeAlignmentMarks)
        {
            RecipeAlignmentMarks = recipeAlignmentMarks;
            AlignmentMarks = new ObservableCollection<AlignmentMarkStepVM>();
            AddNewAlignmentMark(true, false);
        }

        internal void Reset()
        {
            StepState = StepStates.NotDone;
       
            foreach (var alignmentMark in AlignmentMarks.ToList()) 
            {
                if (alignmentMark.IsMain)
                    alignmentMark.Reset();
                else
                    AlignmentMarks.Remove(alignmentMark);
            }

            RecipeAlignmentMarks.UpdateCameraPoints();
            UpdateStepState();
        }

        public void AddNewAlignmentMark(bool isMain = false, bool startEdition = true)
        {
            var newAlignmentMark = new AlignmentMarkStepVM(this, isMain);
            AddAlignmentMark( newAlignmentMark, startEdition);
        }
        
        public void AddAlignmentMark( AlignmentMarkStepVM newAlignmentMark, bool startEdition=false)
        {
            newAlignmentMark.PropertyChanged += AlignmentMark_PropertyChanged;
            AlignmentMarks.Add(newAlignmentMark);
            if (startEdition)
            {
                newAlignmentMark.StepState = StepStates.InProgress;
                newAlignmentMark.IsEditing = true;
            }
            UpdateStepState();
        }

        
        private void AlignmentMark_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlignmentMarkStepVM.StepState))
            {
                UpdateStepState();
            }

            if (e.PropertyName == nameof(AlignmentMarkStepVM.IsEditing))
            {
                RecipeAlignmentMarks.UpdateCameraPoints();
            }
        }

        public void RemoveAlignmentMark(AlignmentMarkStepVM alignmentMarkToRemove)
        {
            AlignmentMarks.Remove(alignmentMarkToRemove);
            alignmentMarkToRemove.PropertyChanged -= AlignmentMark_PropertyChanged;
            UpdateStepState();

        }

        public void RemoveAllAlignmentMarks() 
        { 
            //foreach (var alignmentMark in AlignmentMarks.ToList()) 
            //{
            //    RemoveAlignmentMark(alignmentMark);
            //}

            Reset();
        }

        public void UpdateStepState()
        {
            UpdateAllIsLast();
            if (AlignmentMarks.Any(am => am.StepState == StepStates.Error))
            {
                StepState = StepStates.Error;
                ErrorMessage = AlignmentMarks.FirstOrDefault(am => am.StepState == StepStates.Error)?.ErrorMessage;
                return;
            }

            if (AlignmentMarks.All(am => am.StepState == StepStates.Done))
            {
                StepState = StepStates.Done;
                return;
            }

            if (AlignmentMarks.Any(am => am.StepState == StepStates.InProgress))
            {
                StepState = StepStates.InProgress;
                return;
            }
            

            StepState = StepStates.NotDone;
        }

        public void UpdateAllIsLast()
        { 
                 // Update the IsLast Status
            foreach (var alignmentMark in AlignmentMarks)
            {
                alignmentMark.UpdateIsLast();
            }
        }
  
    }
}
