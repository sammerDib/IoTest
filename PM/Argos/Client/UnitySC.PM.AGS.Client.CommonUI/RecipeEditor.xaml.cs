using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.AGS.Client.CommonUI
{
    /// <summary>
    /// Interaction logic for RecipeEditor.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.Argos)]
    public partial class RecipeEditor : UserControl, IRecipeEditorUc
    {
        public RecipeEditor()
        {
            InitializeComponent();
        }

        public ActorType ActorType => ActorType.Argos;

        public event EventHandler ExitEditor;

        public bool CanClose()
        {
            // Todo RTi
            //throw new NotImplementedException();
            return true;
        }

        public RecipeInfo CreateNewRecipe(string name, int stepId, int userId)
        {
            return new RecipeInfo()
            {
                Name = "TodoArgosTeam",
                ActorType = ActorType.Argos
            };
        }

        public void ExportRecipe(Guid key)
        {
            throw new NotImplementedException();
        }

        public RecipeInfo ImportRecipe(int stepId, int userId)
        {
            throw new NotImplementedException();
        }

        public void Init(bool isStandalone)
        {
            // Todo
            //throw new NotImplementedException();
        }

        public void LoadRecipe(Guid key)
        {
            // Todo
            //throw new NotImplementedException();
        }
    }
}
