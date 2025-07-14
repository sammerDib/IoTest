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
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.HLS.Client.CommonUI
{
    /// <summary>
    /// Interaction logic for RecipeSummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.HeLioS)]
    public partial class RecipeSummary : UserControl, IRecipeSummaryUc
    {
        public RecipeSummary()
        {
            InitializeComponent();
        }

        public ActorType ActorType => ActorType.HeLioS;

        public void Init(bool isStandalone)
        {
            // Todo rti
            //throw new NotImplementedException();
        }

        public void LoadRecipe(Guid key)
        {
            // Todo rti
            //throw new NotImplementedException();
        }

        public void Refresh()
        {
            // Todo rti
            //throw new NotImplementedException();
        }
    }
}
