using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Service.Interface.Calibration;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public static class DieIndexSelector
    {
        /// <summary>
        /// Run a condition check for each of the following die selection patterns:
        /// "100%", "75%", "50%", "25%" and "10%".
        /// </summary>
        public static bool SelectionCheckCondition(int iterableColumn, int iterableRow, double percentageDies, WaferMapResult waferMap, int virtualSquareRows = 0, int virtualSquareColumns = 0)
        {
            try
            {
                switch (percentageDies)
                {
                    case 1:
                        return (waferMap.DiesPresence.GetValue(iterableRow, iterableColumn));

                    case 0.75:
                        return ((iterableColumn % 2 == 0 && iterableRow % 2 == 0)
                            || (iterableColumn % 2 != 0 && iterableRow % 2 == 0)
                            || (iterableColumn % 2 == 0 && iterableRow % 2 != 0))
                            && (waferMap.DiesPresence.GetValue(iterableRow, iterableColumn));

                    case 0.50:
                        return !((iterableColumn % 2 == 0 && iterableRow % 2 == 0)
                            || (iterableColumn % 2 != 0 && iterableRow % 2 != 0))
                            && (waferMap.DiesPresence.GetValue(iterableRow, iterableColumn));

                    case 0.25:
                        return (iterableColumn % 2 != 0 && iterableRow % 2 == 0)
                            && (waferMap.DiesPresence.GetValue(iterableRow, iterableColumn));

                    default:
                        return (iterableColumn % virtualSquareColumns == 0 && iterableRow % virtualSquareRows == 0)
                            && (waferMap.DiesPresence.GetValue(iterableRow + GetMiddleElement(virtualSquareRows), iterableColumn + GetMiddleElement(virtualSquareColumns)));
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Later on, we might want to have selection squares of 4x4, 5x5, 6x6, 7x7:
        /// this function ensures that it would find the middle element despite the dimension of the selection square (< 9x9).
        /// However, do note that a minor adjustement might be needed, for when we want to amp up our selection square up to 9x9 or beyond.
        /// Though, it is highly unlikely we'd want to go for such large selection squares, because it would then defeat the purpose of an even distribution of dies without having them be too dispersed.
        /// </summary>
        public static int GetMiddleElement(int virtualSquareRowsColumns = 0)
        {
            return (int)Math.Ceiling((double)virtualSquareRowsColumns / 3);
        }

        /// <summary>
        /// Handle the following die selection patterns:
        /// "100%", "75%", "50%", "25%" and "10%".
        /// </summary>
        public static List<DieIndex> SelectPercentageOfDiesOnGrid(WaferMapResult waferMap, double percentageDies)
        {
            // Parameters for the 10% selection
            double numberOfZones = (waferMap.NbColumns * waferMap.NbRows) * percentageDies;
            int virtualGridRowColumn = (int)Math.Ceiling(Math.Sqrt(numberOfZones)); // multiplying this value by itself would yield the number of selecctions (e.g. 49)
            int virtualSquareColumns = (int)Math.Ceiling(((double)waferMap.NbColumns / virtualGridRowColumn)); // Cx
            int virtualSquareRows = (int)Math.Ceiling(((double)waferMap.NbRows / virtualGridRowColumn)); // Cy

            var selectedDies = new List<DieIndex>();

            for (int iterableColumn = 0; iterableColumn < waferMap.NbColumns; iterableColumn++)
            {
                for (int iterableRow = 0; iterableRow < waferMap.NbRows; iterableRow++)
                {
                    if (SelectionCheckCondition(iterableColumn, iterableRow, percentageDies, waferMap, virtualSquareRows, virtualSquareColumns))
                    {
                        if (percentageDies < 0.20)
                        {
                            selectedDies.Add(new DieIndex(iterableColumn + GetMiddleElement(virtualSquareColumns), iterableRow + GetMiddleElement(virtualSquareRows)));
                        }
                        else
                        {
                            selectedDies.Add(new DieIndex(iterableColumn, iterableRow));
                        }
                    }
                }
            }
            return selectedDies;
        }

        /// <summary>
        /// Add dies from <paramref name="newDies"/> into <paramref name="toFill"/> without adding
        /// ones already present in it.
        /// </summary>
        public static void FillDiesNoDuplicates(IList<DieIndex> toFill, IEnumerable<DieIndex> newDies)
        {
            foreach (DieIndex die in newDies.Where(die => !toFill.Contains(die)))
            {
                toFill.Add(die);
            }
        }

        /// <summary>
        /// Selects all die indices indcluded in the wafer where the row and column are a multiple of <paramref name="everyNbDie"/>
        /// The selected dies form a homogeneous grid.
        /// </summary>
        /// <param name="waferMap">`Wafer map information</param>
        /// <param name="everyNbDie">Modulus for the grid</param>
        public static List<DieIndex> SelectDiesOnGrid(WaferMapResult waferMap, int everyNbDie, XYCalibDirection scanDir)
        {
            var selectedDies = new List<DieIndex>();
            switch (scanDir)
            {
                case XYCalibDirection.RightLeftThenBottomTop: //  Right -> Left and Bottom -> Top -aka - End Columns  Then Ends Rows 
                    for (int j = waferMap.NbRows - 1; j >= 0; j -= everyNbDie)
                    {
                        for (int i = waferMap.NbColumns - 1; i >= 0; i -= everyNbDie)
                        {
                            if (waferMap.DiesPresence.GetValue(j, i)) { selectedDies.Add(new DieIndex(i, j)); }
                        }
                    }
                    break;

                case XYCalibDirection.BottomTopThenRightLeft: //  Bottom -> Top and  Right -> Left -aka - End Rows Then Ends Columns 
                    for (int i = waferMap.NbColumns - 1; i >= 0; i -= everyNbDie)
                    {
                        for (int j = waferMap.NbRows - 1; j >= 0; j -= everyNbDie)
                        {
                            if (waferMap.DiesPresence.GetValue(j, i)) { selectedDies.Add(new DieIndex(i, j)); }
                        }
                    }
                    break;

                case XYCalibDirection.LeftRightThenTopBottom: // Left -> Right and Top -> Bottom -aka - Columns Then Rows 
                    for (int j = 0; j < waferMap.NbRows; j += everyNbDie)
                    {
                        for (int i = 0; i < waferMap.NbColumns; i += everyNbDie)
                        {
                            if (waferMap.DiesPresence.GetValue(j, i)) { selectedDies.Add(new DieIndex(i, j)); }
                        }
                    }
                    break;

                case XYCalibDirection.TopBottomThenLeftRight:   // Top -> Bottom and Left -> Right -aka - Rows Then Columns
                default:
                    for (int i = 0; i < waferMap.NbColumns; i += everyNbDie)
                    {
                        for (int j = 0; j < waferMap.NbRows; j += everyNbDie)
                        {
                            if (waferMap.DiesPresence.GetValue(j, i)) { selectedDies.Add(new DieIndex(i, j)); }
                        }
                    }
                    break;
            }
            return selectedDies;
        }

        public static List<DieIndex> SelectDiesOnCentralCross(WaferMapResult waferMap, int periodicityFromCenter, int nbDiesPerBranch)
        {
            var selectedDies = new List<DieIndex>();

            int centerRow = waferMap.NbRows / 2;
            int centerCol = waferMap.NbColumns / 2;

            // Truncate nbDiesPerBranch in case of crazy input to avoid looping uselessly
            nbDiesPerBranch = Math.Min(Math.Max(centerRow, centerCol) + 1, nbDiesPerBranch);

            // Horizontal dies
            for (int i = 0; i < 2 * nbDiesPerBranch + 1; i++)
            {
                int col = centerCol + (i - nbDiesPerBranch) * periodicityFromCenter;

                if (col < 0 || col >= waferMap.NbColumns)
                    continue;

                if (waferMap.DiesPresence.GetValue(centerRow, col))
                    selectedDies.Add(new DieIndex(col, centerRow));
            }

            //Vertical dies
            for (int i = 0; i < 2 * nbDiesPerBranch + 1; i++)
            {
                int row = centerRow + (i - nbDiesPerBranch) * periodicityFromCenter;

                if (row < 0 || row >= waferMap.NbRows)
                    continue;

                // Don't add the center twice
                if (row == centerRow)
                    continue;

                if (waferMap.DiesPresence.GetValue(row, centerCol))
                    selectedDies.Add(new DieIndex(centerCol, row));
            }
            return selectedDies;
        }
    }
}
