using System;
using System.Collections.Generic;
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
using System.ComponentModel.Composition;
using UnitySC.Shared.Data.Enum;
using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.DataAccess.Service.Interface;

namespace UnitySC.PM.Shared.UI.DummyPM
{
    /// <summary>
    /// Interaction logic for DummySummary.xaml
    /// </summary>
    [Export(typeof(IRecipeSummaryUc))]
    [UCMetadata(ActorType = ActorType.Unknown)]
    public partial class DummySummary : UserControl, IRecipeSummaryUc
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        public DummySummary()
        {
            InitializeComponent();
        }

        public ActorType ActorType => ActorType.Unknown;

        public void Init(bool isStandalone)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
        }

        public void LoadRecipe(Guid key)
        {
        }

        public void Refresh()
        {
        }
    }
}
