using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public struct RecipeSortedPoints
    {
        public DieIndex DieIndex;
        public int PointId;
        public string MeasureName;
    }

    public class ANARecipeMeasurePointsSorter
    {
        private readonly IReferentialManager _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
        private readonly ILogger _logger = ClassLocator.Default.GetInstance<ILogger<ANARecipeMeasurePointsSorter>>();
        private ANARecipe _recipe;
        private List<DieIndex> _dies;
        private List<RecipeMeasure> _recipeMeasuresToSort;
        private Dictionary<int, MeasurePoint> _measurePoints;
        private List<MeasurePoints> _groupedDuplicatePoints;
        private List<RecipeSortedPoints> _recipeSortedPoints;
        private bool _routePerDie = true;
        private bool _optimizeDiesRoute = true;
        private bool _optimizePointsRoute = true;

        #region Common Recipe Measure Sort

        public List<RecipeSortedPoints> SortRecipeMeasurePoints(ANARecipe recipe, List<RecipeMeasure> recipeMeasuresToSort,
            bool optimizeDiesRoute = false,
            bool optimizePointsRoute = false,
            bool routePerDie = true)
        {
            _recipeSortedPoints = new List<RecipeSortedPoints>();
            _recipeMeasuresToSort = new List<RecipeMeasure>(recipeMeasuresToSort);
            _recipe = recipe;
            _dies = recipe.Dies != null ? new List<DieIndex>(_recipe.Dies) : null;
            _routePerDie = routePerDie;
            _optimizeDiesRoute = optimizeDiesRoute;
            _optimizePointsRoute = optimizePointsRoute;
            _measurePoints = new Dictionary<int, MeasurePoint>(_recipe.Points.Count);
            foreach (var p in _recipe.Points)
            {
                _measurePoints.Add(p.Id, p);
            }

            OptimizeRoutes();

            foreach (var ClampAndUnclampMeasures in SplitClampAndUnclampMeasures(_recipeMeasuresToSort))
            {
                foreach (var cantZAxisMoveAndCanZAxisMoveMeasures in SplitCantZAxisMoveAndCanZAxisMoveMeasures(ClampAndUnclampMeasures))
                {
                    switch (recipe.Execution.Strategy)
                    {
                        case MeasurementStrategy.PerMeasurementType:
                            if (_routePerDie)
                            {
                                RouteByMeasuresPerDie(cantZAxisMoveAndCanZAxisMoveMeasures);
                            }
                            else
                            {
                                RouteByDiesPerMeasure(cantZAxisMoveAndCanZAxisMoveMeasures);
                            }
                            break;

                        case MeasurementStrategy.PerPoint:
                            if (_routePerDie)
                            {
                                RouteByPointsPerDie(cantZAxisMoveAndCanZAxisMoveMeasures);
                            }
                            else
                            {
                                RouteByDiesPerPoint(cantZAxisMoveAndCanZAxisMoveMeasures);
                            }
                            break;

                            // Uncomment and implem when MeasurementStrategy.Optimized is availabe
                            /*case MeasurementStrategy.Optimized:
                                break;*/
                    }
                }
            }
            return _recipeSortedPoints;
        }

        private IEnumerable<RecipeMeasure>[] SplitClampAndUnclampMeasures(IEnumerable<RecipeMeasure> recipeMeasures)
        {
            return new IEnumerable<RecipeMeasure>[2] {
                        recipeMeasures.Where(measure => !measure.Measure.WaferUnclampedMeasure),
                        recipeMeasures.Where(measure => measure.Measure.WaferUnclampedMeasure)
                    };
        }

        private IEnumerable<IEnumerable<RecipeMeasure>> SplitCantZAxisMoveAndCanZAxisMoveMeasures(IEnumerable<RecipeMeasure> recipeMeasures)
        {
            var cantZAxisMoveAndCanZAxisMoveMeasures = new IEnumerable<RecipeMeasure>[2] {
                        recipeMeasures.Where(measure => !measure.Measure.CanZAxisMove(measure.Settings)),
                        recipeMeasures.Where(measure => measure.Measure.CanZAxisMove(measure.Settings))
                    };
            var splittedMeasures = SplitCantZMoveAxisMeasuresAccordingZPositions(cantZAxisMoveAndCanZAxisMoveMeasures);

            splittedMeasures.Add(cantZAxisMoveAndCanZAxisMoveMeasures[1]);

            return splittedMeasures;
        }

        private List<IEnumerable<RecipeMeasure>> SplitCantZMoveAxisMeasuresAccordingZPositions(IEnumerable<RecipeMeasure>[] cantZAxisMoveAndCanZAxisMoveMeasures)
        {
            var splittedMeasures = new List<IEnumerable<RecipeMeasure>>();
            foreach (var currentMeasure in cantZAxisMoveAndCanZAxisMoveMeasures[0])
            {
                if (splittedMeasures.Count == 0)
                {
                    splittedMeasures.Add(new List<RecipeMeasure>() { currentMeasure });
                }
                else
                {
                    _ = _measurePoints.TryGetValue(currentMeasure.MeasurePointIds.First(), out var currentMeasureFirstPoint);
                    var currentMeasureZTop = currentMeasureFirstPoint.Position.ZTop;
                    var currentMeasureZbottom = currentMeasureFirstPoint.Position.ZBottom;

                    int i = 0;
                    for (i = 0; i < splittedMeasures.Count; i++)
                    {
                        var sortedMeasure = splittedMeasures[i];

                        _ = _measurePoints.TryGetValue(sortedMeasure.First().MeasurePointIds.First(), out var sortedMeasureFirstPoint);
                        var sortedMeasureZTop = sortedMeasureFirstPoint.Position.ZTop;
                        var sortedMeasureZbottom = sortedMeasureFirstPoint.Position.ZBottom;
                        if (currentMeasureZTop == sortedMeasureZTop && currentMeasureZbottom == sortedMeasureZbottom)
                        {
                            splittedMeasures[i] = splittedMeasures[i].Append(currentMeasure);
                            break;
                        }
                    }

                    if (i == splittedMeasures.Count)
                    {
                        splittedMeasures.Add(new List<RecipeMeasure>() { currentMeasure });
                    }
                }
            }

            return splittedMeasures;
        }

        private void SplitMeasureAccordingReferential(IEnumerable<RecipeMeasure> splitRecipeMeasures,
            out List<RecipeMeasure> measureInDieReferential,
            out List<RecipeMeasure> measureInWaferReferential)
        {
            measureInDieReferential = new List<RecipeMeasure>();
            measureInWaferReferential = new List<RecipeMeasure>();
            foreach (var measure in splitRecipeMeasures)
            {
                if (IsMeasureInWaferReferential(measure))
                {
                    measureInWaferReferential.Add(measure);
                }
                else
                {
                    measureInDieReferential.Add(measure);
                }
            }
        }

        private bool IsMeasureInWaferReferential(RecipeMeasure measure)
        {
            var measurePointId = measure.MeasurePointIds.First();

            _ = _measurePoints.TryGetValue(measurePointId, out var point);
            return !point.IsDiePosition;
        }

        private void AddSortedPoint(DieIndex die, RecipeMeasure recipeMeasure, int pointIndex)
        {
            var sortedPoint = new RecipeSortedPoints()
            {
                DieIndex = die,
                PointId = pointIndex,
                MeasureName = recipeMeasure.Settings.Name
            };
            _recipeSortedPoints.Add(sortedPoint);
        }

        #endregion Common Recipe Measure Sort

        #region PerMeasurementType

        private void RouteByMeasuresPerDie(IEnumerable<RecipeMeasure> cantZAxisMoveAndCanZAxisMoveMeasures)
        {
            SplitMeasureAccordingReferential(cantZAxisMoveAndCanZAxisMoveMeasures, out var measureInDieReferential,
                                    out var measureInWaferReferential);

            AddRecipeMeasuresInDieReferential(measureInDieReferential);
            AddRecipeMeasuresInWaferReferentialFromRecipeMeasure(measureInWaferReferential);
        }

        private void RouteByDiesPerMeasure(IEnumerable<RecipeMeasure> cantZAxisMoveAndCanZAxisMoveMeasures)
        {
            SplitMeasureAccordingReferential(cantZAxisMoveAndCanZAxisMoveMeasures, out var measureInDieReferential,
                                    out var measureInWaferReferential);

            foreach (var measure in measureInDieReferential)
            {
                AddRecipeMeasuresInDieReferential(new List<RecipeMeasure>() { measure });
            }

            AddRecipeMeasuresInWaferReferentialFromRecipeMeasure(measureInWaferReferential);
        }

        private void AddRecipeMeasuresInDieReferential(List<RecipeMeasure> measureInDieReferential)
        {
            if (_dies != null)
            {
                foreach (var die in _dies)
                {
                    foreach (var recipeMeasure in measureInDieReferential)
                    {
                        foreach (var pointIndex in recipeMeasure.MeasurePointIds)
                        {
                            AddSortedPoint(die, recipeMeasure, pointIndex);
                        }
                    }
                }
            }
        }

        private void AddRecipeMeasuresInWaferReferentialFromRecipeMeasure(List<RecipeMeasure> measureInWaferReferential)
        {
            foreach (var recipeMeasure in measureInWaferReferential)
            {
                foreach (var measurePointId in recipeMeasure.MeasurePointIds)
                {
                    AddSortedPoint(null, recipeMeasure, measurePointId);
                }
            }
        }

        #endregion PerMeasurementType

        #region PerPoint

        private void RouteByPointsPerDie(IEnumerable<RecipeMeasure> cantZAxisMoveAndCanZAxisMoveMeasures)
        {
            SplitMeasureAccordingReferential(cantZAxisMoveAndCanZAxisMoveMeasures, out var measureInDieReferential,
                                    out var measureInWaferReferential);

            if (_dies != null && measureInDieReferential.Any())
            {
                foreach (var die in _dies)
                {
                    foreach (var duplicatesPoints in _groupedDuplicatePoints)
                    {
                        foreach (var point in duplicatesPoints.MeasurePointsList)
                        {
                            if (point.IsDiePosition)
                            {
                                AddRecipeMeasureInDieReferentialFromDieAndPoint(measureInDieReferential, die, point);
                            }
                        }
                    }
                }
            }

            if (measureInWaferReferential.Any())
            {
                foreach (var duplicatesPoints in _groupedDuplicatePoints)
                {
                    AddRecipeMeasuresInWaferReferentialFromRecipePoints(measureInWaferReferential, duplicatesPoints.MeasurePointsList);
                }
            }
        }

        private void RouteByDiesPerPoint(IEnumerable<RecipeMeasure> cantZAxisMoveAndCanZAxisMoveMeasures)
        {
            SplitMeasureAccordingReferential(cantZAxisMoveAndCanZAxisMoveMeasures, out var measureInDieReferential,
                                    out var measureInWaferReferential);

            if (_dies != null && measureInDieReferential.Any())
            {
                foreach (var duplicatesPoints in _groupedDuplicatePoints)
                {
                    foreach (var point in duplicatesPoints.MeasurePointsList)
                    {
                        if (point.IsDiePosition)
                        {
                            foreach (var die in _dies)
                            {
                                AddRecipeMeasureInDieReferentialFromDieAndPoint(measureInDieReferential, die, point);
                            }
                        }
                    }
                }
            }

            if (measureInWaferReferential.Any())
            {
                foreach (var duplicatesPoints in _groupedDuplicatePoints)
                {
                    AddRecipeMeasuresInWaferReferentialFromRecipePoints(measureInWaferReferential, duplicatesPoints.MeasurePointsList);
                }
            }
        }

        private void AddRecipeMeasuresInWaferReferentialFromRecipePoints(List<RecipeMeasure> measureInWaferReferential, List<MeasurePoint> points)
        {
            if (!measureInWaferReferential.IsEmpty())
            {
                foreach (var point in points)
                {
                    foreach (var recipeMeasure in measureInWaferReferential)
                    {
                        int pointIndex = point.Id;
                        if (!recipeMeasure.MeasurePointIds.Contains(pointIndex))
                            continue;

                        AddSortedPoint(null, recipeMeasure, pointIndex);
                    }
                }
            }
        }

        private void AddRecipeMeasureInDieReferentialFromDieAndPoint(List<RecipeMeasure> measureInDieReferential, DieIndex die, MeasurePoint point)
        {
            foreach (var recipeMeasure in measureInDieReferential.Where(recipeMeasure => recipeMeasure.MeasurePointIds.Contains(point.Id)))
            {
                AddSortedPoint(die, recipeMeasure, point.Id);
            }
        }

        #endregion PerPoint

        #region Optimize Routes

        private void OptimizeRoutes()
        {
            if (_recipe.WaferHasDies && _optimizeDiesRoute)
            {
                var initialTotalDistance = ComputeTotalDistance(_recipe.Dies);

                _dies = FindShortestDiesRoute(_recipe.Dies);
                var totalDistance = ComputeTotalDistance(_dies);

                _logger.Debug($"Initial approximative distance = {initialTotalDistance}");
                _logger.Debug($"Optimize approximative distance = {totalDistance}");
            }

            // Uncomment when MeasurementStrategy.Optimized is availabe
            //if (_recipe.Execution.Strategy == MeasurementStrategy.PerPoint || _recipe.Execution.Strategy == MeasurementStrategy.Optimized)
            if (_recipe.Execution.Strategy == MeasurementStrategy.PerPoint)
            {
                _groupedDuplicatePoints = GroupDuplicatePoints();
                if (_optimizePointsRoute)
                {
                    _groupedDuplicatePoints = SortGroupOfDuplicatePoints(_groupedDuplicatePoints);
                }
            }
            else if (_recipe.Execution.Strategy == MeasurementStrategy.PerMeasurementType && _optimizePointsRoute)
            {
                SortMeasurePointForEachMeasure(_recipeMeasuresToSort);
            }
        }

        private void SortMeasurePointForEachMeasure(List<RecipeMeasure> measureInDieReferential)
        {
            List<MeasurePoints> measurePoints = new List<MeasurePoints>();

            foreach (var recipeMeasure in measureInDieReferential)
            {
                foreach (var pointIndex in recipeMeasure.MeasurePointIds)
                {
                    _ = _measurePoints.TryGetValue(pointIndex, out var recipePoint);

                    var measurePoint = new MeasurePoints()
                    {
                        Position = recipePoint.Position.ToXYPosition(null),
                        MeasurePointsList = new List<MeasurePoint>() { recipePoint }
                    };
                    measurePoints.Add(measurePoint);
                }

                var sortedMeasurePoints = Tsp2Opt.MakeShortestLoopPath(measurePoints);
                measurePoints.Clear();
                recipeMeasure.MeasurePointIds.Clear();
                foreach (var sortedPoint in sortedMeasurePoints)
                {
                    recipeMeasure.MeasurePointIds.Add(sortedPoint.MeasurePointsList[0].Id);
                }
            }
        }

        private static List<MeasurePoints> SortGroupOfDuplicatePoints(List<MeasurePoints> groupedDuplicatePoints)
        {
            return Tsp2Opt.MakeShortestLoopPath(groupedDuplicatePoints);
        }

        private List<MeasurePoints> GroupDuplicatePoints()
        {
            var uniqueMeasurePoints = new List<MeasurePoints>();
            var recipePointCopy = new List<MeasurePoint>(_recipe.Points);
            for (int i = 0; i < _recipe.Points.Count; i++)
            {
                var duplicatePoints = new List<MeasurePoint>();
                for (int j = recipePointCopy.Count - 1; j >= 0; j--)
                {
                    var measurePointI = _recipe.Points[i];
                    var measurePointJ = recipePointCopy[j];
                    if (measurePointI != null && measurePointJ != null)
                    {
                        var isDuplicate = true;
                        isDuplicate &= measurePointI.IsDiePosition == measurePointJ.IsDiePosition;
                        isDuplicate &= measurePointI.Position.Equals(measurePointJ.Position);
                        if (isDuplicate)
                        {
                            duplicatePoints.Add(measurePointJ);
                            recipePointCopy.RemoveAt(j);
                        }
                    }
                }
                if (duplicatePoints.Count > 0)
                {
                    duplicatePoints.Reverse();
                    var measurePoint = new MeasurePoints()
                    {
                        Position = duplicatePoints[0].Position.ToXYPosition(null),
                        MeasurePointsList = duplicatePoints
                    };
                    uniqueMeasurePoints.Add(measurePoint);
                }
            }

            return uniqueMeasurePoints;
        }

        #endregion Optimize Routes

        #region Sort dies

        private List<DieIndex> FindShortestDiesRoute(List<DieIndex> dies)
        {
            // Add current position as start point
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var currentPos = hardwareManager.Axes.GetPos().ToXYZTopZBottomPosition();

            var points = CreatePointsList(dies, currentPos);
            var sortedPoints = Tsp2Opt.MakeShortestLoopPath(points);

            // Remove the first element of the list, which is not a die but the current position
            sortedPoints.RemoveAt(0);

            return RecreateDiesList(sortedPoints);
        }

        private List<MeasurePoints> CreatePointsList(List<DieIndex> dies, XYZTopZBottomPosition currentPosition)
        {
            var points = new List<MeasurePoints>();
            var currentPos = new MeasurePoint()
            {
                Id = -1,
                IsDiePosition = false,
                PatternRec = null,
                Position = new PointPosition(currentPosition.X, currentPosition.Y)
            };

            points.Add(new MeasurePoints()
            {
                Position = currentPos.Position.ToXYPosition(null),
                MeasurePointsList = new List<MeasurePoint>() { currentPos }
            });

            for (int i = 0; i < dies.Count; i++)
            {
                var DefalutPointOnDie = _referentialManager.ConvertTo(
                    new XYZTopZBottomPosition(
                        new DieReferential(dies[i].Column, dies[i].Row),
                        0, 0, double.NaN, double.NaN),
                        ReferentialTag.Wafer).ToXYPosition();

                var point = new MeasurePoint()
                {
                    Id = i,
                    IsDiePosition = false,
                    PatternRec = null,
                    Position = new PointPosition(DefalutPointOnDie.X, DefalutPointOnDie.Y)
                };

                points.Add(new MeasurePoints()
                {
                    Position = point.Position.ToXYPosition(null),
                    MeasurePointsList = new List<MeasurePoint>() { point }
                });
            }

            return points;
        }

        private List<DieIndex> RecreateDiesList(List<MeasurePoints> points)
        {
            var diesSort = new List<DieIndex>();
            foreach (var p in points)
            {
                var pointInWaferRef = new XYZTopZBottomPosition(new WaferReferential(),
                    p.Position.X,
                    p.Position.Y,
                    double.NaN, double.NaN);

                var pointDieRef = _referentialManager.ConvertTo(pointInWaferRef, ReferentialTag.Die).Referential as DieReferential;
                var dieIndex = new DieIndex(pointDieRef.DieColumn, pointDieRef.DieLine);
                diesSort.Add(dieIndex);
            }
            return diesSort;
        }

        private Length ComputeTotalDistance(List<DieIndex> dies)
        {
            var totalDistance = 0.Millimeters();
            for (int i = 0; i < dies.Count - 1; i++)
            {
                var distance = ComputeDistanceBetweenTwoDies(dies[i], dies[i + 1]);
                totalDistance += distance.Millimeters();
            }

            return totalDistance;
        }

        private double ComputeDistanceBetweenTwoDies(DieIndex die1, DieIndex die2)
        {
            var die1Referential = new DieReferential(die1.Column, die1.Row);

            var defaultPointOnDie1 = _referentialManager.ConvertTo(
                new XYZTopZBottomPosition(die1Referential, 0, 0, double.NaN, double.NaN),
                ReferentialTag.Motor).ToXYPosition();

            var die2Referential = new DieReferential(die2.Column, die2.Row);
            var defaultPointOnDie2 = _referentialManager.ConvertTo(
                new XYZTopZBottomPosition(die2Referential, 0, 0, double.NaN, double.NaN),
            ReferentialTag.Motor).ToXYPosition();

            var distance = Math.Sqrt(
                (defaultPointOnDie2.X - defaultPointOnDie1.X) * (defaultPointOnDie2.X - defaultPointOnDie1.X)
                + (defaultPointOnDie2.Y - defaultPointOnDie1.Y) * (defaultPointOnDie2.Y - defaultPointOnDie1.Y));

            return distance;
        }

        #endregion Sort dies
    }
}
