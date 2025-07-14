using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;


namespace AdcBasicObjects
{
    public abstract class ComparatorViewModelBase : ObservableRecipient
    {
        //=================================================================
        // Fonctions abstraites
        //=================================================================
        protected ComparatorBase _comparator;
        public virtual ComparatorBase Comparator { get { return _comparator; } set { _comparator = value; } }

        // Test si le comparateur est utile ou non
        public abstract bool IsNull { get; set; }

        // Crée une view assocée au ViewModel
        public abstract Window GetUI();

        //=================================================================
        // Gestion de la liste des Editeurs
        //=================================================================

        // Méthode "factory"
        public delegate ComparatorViewModelBase EditorFactory();

        // La liste des type supportés et des comparateurs associés
        public static Dictionary<Type, EditorFactory> editors = new Dictionary<Type, EditorFactory>();

        //=================================================================
        // Init de la liste des éediteurs.
        // NB: d'autres éditeurs peuvent être ajoutés en dehors de cette méthode
        //=================================================================
        static ComparatorViewModelBase()
        {
            editors.Add(typeof(bool), BooleanComparatorViewModel.GetViewModel);
            editors.Add(typeof(double), RangeComparatorViewModel.GetViewModel);
            editors.Add(typeof(RectangleF), RectangleComparatorViewModel.GetViewModel);
            editors.Add(typeof(string), StringComparatorViewModel.GetViewModel);
        }

        //=================================================================
        // Crée un ViewModel associé à un comparateur
        //=================================================================
        public static ComparatorViewModelBase GetEditorViewModel(Type type)
        {
            EditorFactory factory = editors[type];
            ComparatorViewModelBase vm = factory();
            return vm;
        }


    }
}
