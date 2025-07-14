using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UC;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{

    public class RecipeRunLiveViewUcVM : ObservableRecipient
    {

        private IRecipeRunLiveViewUc _recipeRunLiveUc;

        public IRecipeRunLiveViewUc RecipeRunLiveUc
        {
            get => _recipeRunLiveUc; set { if (_recipeRunLiveUc != value) { _recipeRunLiveUc = value; OnPropertyChanged(); } }
        }

        private string _actorName ;

        public string  ActorName
        {
            get => _actorName; set { if (_actorName != value) { _actorName = value; OnPropertyChanged(); } }
        }

        internal void Display()
        {
            RecipeRunLiveUc.Display();
        }

        internal void Hide()
        {
            RecipeRunLiveUc.Hide();
        }
    }


    public class RecipeRunLiveViewModel : ObservableRecipient
    {
        public RecipeRunLiveViewModel() : base()
        {
            
        }
   
        public void Init(RecipeInfo recipeInfo)
        {
        }

        private RecipeRunLiveViewUcVM _currentPMRecipeRunLiveViewUC;


        public RecipeRunLiveViewUcVM CurrentPMRecipeRunLiveViewUC
        {
            get => _currentPMRecipeRunLiveViewUC;
            set
            {
                if (_currentPMRecipeRunLiveViewUC != value)
                {
                    _currentPMRecipeRunLiveViewUC = value;

                    OnPropertyChanged();
                }
            }
        }

        private List<ActorType> _actors;

        public List<ActorType> Actors
        {
            get => _actors;
            set
            {
                if (_actors != value)
                {
                    _actors = value;
                    UpdateRecipeRunLiveViews();
                    OnPropertyChanged();
                }
            }
        }

        private ActorType _actor;

        public ActorType Actor
        {
            get => _actor;
            set
            {
                if (_actor != value)
                {
                    _actor = value;
                    UpdateRecipeRunLiveView();
                    OnPropertyChanged();
                }
            }
        }


        private NotifierVM _notifierVM;

        public NotifierVM NotifierVM
        {
            get
            {
                if (_notifierVM == null)
                    _notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                return _notifierVM;
            }
        }

        private void UpdateRecipeRunLiveViews()
        {

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PMRecipeRunLiveViews.Clear();
                foreach (var actor in _actors)
                {
                    var recipeRunLiveView = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeRunLiveView(actor);
                    if (recipeRunLiveView != null)
                    {
                        recipeRunLiveView.Init(true);
                        PMRecipeRunLiveViews.Add(new RecipeRunLiveViewUcVM() { ActorName = actor.ToString(), RecipeRunLiveUc = recipeRunLiveView });
                    }

                }
                if (PMRecipeRunLiveViews.Count > 0)
                    CurrentPMRecipeRunLiveViewUC = PMRecipeRunLiveViews[0];
                else
                    CurrentPMRecipeRunLiveViewUC = null;
            }));

        }

        private void UpdateRecipeRunLiveView()
        {

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                    var recipeRunLiveView = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeRunLiveView(Actor);
                    if (recipeRunLiveView != null)
                    {
                        recipeRunLiveView.Init(true);
                        CurrentPMRecipeRunLiveViewUC = new RecipeRunLiveViewUcVM() { ActorName = Actor.ToString(), RecipeRunLiveUc = recipeRunLiveView };
                    }
                    else
                        CurrentPMRecipeRunLiveViewUC = null;
            }));

        }

        public void Display()
        {
            if (CurrentPMRecipeRunLiveViewUC != null)
            {
                CurrentPMRecipeRunLiveViewUC.Display();
            }
            
        }

        public void Hide()
        {
            if (CurrentPMRecipeRunLiveViewUC != null)
            {
                CurrentPMRecipeRunLiveViewUC.Hide();
            }
        }

        private ObservableCollection<RecipeRunLiveViewUcVM> _pmRecipeRunLiveViews;

        public ObservableCollection<RecipeRunLiveViewUcVM> PMRecipeRunLiveViews
        {
            get
            {
                if (_pmRecipeRunLiveViews is null)
                {
                    _pmRecipeRunLiveViews = new ObservableCollection<RecipeRunLiveViewUcVM>();
                }

                return _pmRecipeRunLiveViews;
            }
            set
            {
                if (_pmRecipeRunLiveViews != value)
                {
                    _pmRecipeRunLiveViews = value;

                    OnPropertyChanged();
                }
            }
        }

        
    }
}
