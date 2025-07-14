using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.DiesSelection
{
    public class DieSorter : IComparer
    {
        private bool _isSortByRow;
        public DieSorter(SortField isSortByRow)
        {
            _isSortByRow = (isSortByRow == SortField.Row);
        }

        public int Compare(object x, object y)
        {
            var custX = (DieIndexWithSelectionVM)x;
            var custY = (DieIndexWithSelectionVM)y;

            if (_isSortByRow)
                return custY.Index.Row.CompareTo(custX.Index.Row);

         return custX.Index.Column.CompareTo(custY.Index.Column);
        }
    }
}
