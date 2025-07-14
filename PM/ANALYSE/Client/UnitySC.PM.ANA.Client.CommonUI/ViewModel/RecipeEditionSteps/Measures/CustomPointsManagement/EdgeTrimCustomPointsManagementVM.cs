using System;
using System.Collections.Generic;
using System.Windows;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class EdgeTrimCustomPointsManagementVM : CustomPointsManagementVM
    {
      
        public EdgeTrimCustomPointsManagementVM(MeasurePointsVM measurePoints) : base(measurePoints)
        {
        }

        private Angle _startAngle = 0.Degrees();

        public Angle StartAngle
        {
            get => _startAngle;
            set => SetProperty(ref _startAngle, value);
        }

        private int _numberOfPoints = 10;

        public int NumberOfPoints
        {
            get => _numberOfPoints;
            set => SetProperty(ref _numberOfPoints, value);
        }

        private Length _distanceFromBorder = 100.Micrometers();
        public Length DistanceFromBorder
        {
            get => _distanceFromBorder;
            set => SetProperty(ref _distanceFromBorder, value);
        }

        private AutoRelayCommand _generatePointsCommand;

        public AutoRelayCommand GeneratePointsCommand
        {
            get
            {
                return _generatePointsCommand ?? (_generatePointsCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (MeasurePoints.Points.Count>0)
                        {
                            var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The current measure points will be replaced by the new generated ones. Do you want to proceed ?", "Undo calibration", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                            if (res == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        List<XYZTopZBottomPosition> points = new List<XYZTopZBottomPosition>();

                        var distanceFromCenter = MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory.DimentionalCharacteristic.Diameter / 2 - DistanceFromBorder;

                        var currentPos = MeasurePoints.GetCurrentPositionOnRelevantReferential();
                        for (int i = 0; i < NumberOfPoints ; i++)
                        {
                            var angleInRadians = -Math.PI/2 + StartAngle.Radians + i * 2 * Math.PI / NumberOfPoints;
                            var X = Math.Cos(angleInRadians) * distanceFromCenter.Millimeters;
                            var Y = Math.Sin(angleInRadians) * distanceFromCenter.Millimeters;
                            var point = new XYZTopZBottomPosition(new WaferReferential(), X, Y, currentPos.ZTop , currentPos.ZBottom);
                            points.Add(point);
                        }

                        MeasurePoints.AddMeasurePoints(points, true,false);
                    },
                    () => { return true; }
                ));
            }
        }
        

    }
}
