using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Dataflow.Operations.Interface;
using UnitySC.Dataflow.Operations.Interface.UTODF;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Operations.Implementation
{
    public class UTODFOperations : UTOBaseOperations, IUTODFOperations
    {
        private IRecipeOperations _recipeOperations;
        private IDFStatusVariableOperations _svOperations;
        public IRecipeOperations RecipeOperations { get => _recipeOperations; }
        public IDFStatusVariableOperations SVOperations { get => _svOperations; }

        public UTODFOperations()
        {
            _recipeOperations = ClassLocator.Default.GetInstance<IRecipeOperations>();
            _svOperations = ClassLocator.Default.GetInstance<IDFStatusVariableOperations>();
            
        }

    }
}
