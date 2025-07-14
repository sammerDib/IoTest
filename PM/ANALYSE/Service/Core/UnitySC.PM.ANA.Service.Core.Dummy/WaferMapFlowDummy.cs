using System.Threading;

using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class WaferMapFlowDummy : WaferMapFlow
    {
        public WaferMapFlowDummy(WaferMapInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.RotationAngle = 0.3.Degrees();
            Result.DieGridTopLeft = new XYPosition(new WaferReferential(), -150, 150);
            Result.DieDimensions = Input.DieDimensions;

            // 1000 * 1000 Grid

            //Result.DieSizeWidth = 0.25.Millimeters();
            //Result.DieSizeHeight = 0.25.Millimeters();
            //Result.DieStreetWidth = Length.FromMilimeters(0.05);
            //Result.DieStreetHeight = Length.FromMilimeters(0.05);

            //var nbColumns = 1000;
            //var nbRows = 1000;

            //bool[][] diesPresence = new bool[nbColumns][];

            //for (int i = 0; i < nbColumns; i++)
            //{
            //    diesPresence[i] = new bool[nbRows];
            //}

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 200; j < nbRows; j++)
            //    {
            //        diesPresence[i][j] = true;
            //    }
            //}

            //Result.DiesPresence = diesPresence;

            //Result.DieReference = new DieIndex(133, 144);

            // 5 * 5 Grid

            //Result.DieSizeWidth = Length.FromMilimeters(20);
            //Result.DieSizeHeight = Length.FromMilimeters(20);
            //Result.DieStreetWidth = 1.Millimeters();
            //Result.DieStreetHeight = 1.Millimeters();

            //var nbColumns = 5;
            //var nbRows = 5;

            //bool[][] diesPresence = new bool[nbColumns][];

            //for (int i = 0; i < nbColumns; i++)
            //{
            //    diesPresence[i] = new bool[nbRows];
            //}

            //for (int i = 0; i < nbColumns; i++)
            //{
            //    for (int j = 0; j < nbRows; j++)
            //    {
            //        diesPresence[i][j] = true;
            //    }
            //}
            //Result.DieReference = new DieIndex(0, 0);

            // 100 * 100 Grid

            //Result.DieSizeWidth = Length.FromMilimeters(2.5);
            //Result.DieSizeHeight = Length.FromMilimeters(2.5);
            //Result.DieStreetWidth = 0.5.Millimeters();
            //Result.DieStreetHeight = 0.5.Millimeters();

            //var nbColumns = 100;
            //var nbRows = 100;

            //bool[][] diesPresence = new bool[nbColumns][];

            //for (int i = 0; i < nbColumns; i++)
            //{
            //    diesPresence[i] = new bool[nbRows];
            //}

            //for (int i = 0; i < nbColumns; i++)
            //{
            //    for (int j = 0; j < nbRows; j++)
            //    {
            //        diesPresence[i][j] = true;
            //    }
            //}

            //Result.DieReference = new DieIndex(60, 70);

            //Result.DiesPresence = diesPresence;
            Result.DieDimensions = new DieDimensionalCharacteristic(
                                       dieWidth: 14.5.Millimeters(),
                                       dieHeight: 14.5.Millimeters(),
                                       streetWidth: 0.5.Millimeters(),
                                       streetHeight: 0.5.Millimeters(),
                                       dieAngle: 0.Degrees());

            int nbColumns = 20;
            int nbRows = 20;
            var diesPresence = new Matrix<bool>(nbColumns, nbRows);

            for (int c = 0; c < nbColumns; c++)
            {
                for (int r = 0; r < nbRows; r++)
                {
                    diesPresence.SetValue(c, r, true);
                }
            }

            diesPresence.SetValue(1, 9, false);


            diesPresence.SetValue(0, 0, false);
            diesPresence.SetValue(0, 1, false);
            diesPresence.SetValue(0, 2, false);
            diesPresence.SetValue(0, 3, false);
            diesPresence.SetValue(0, 4, false);
            diesPresence.SetValue(0, 5, false);
            diesPresence.SetValue(0, 6, false);
            diesPresence.SetValue(0, 7, false);
            diesPresence.SetValue(0, 8, false);

            diesPresence.SetValue(1, 0, false);
            diesPresence.SetValue(1, 1, false);
            diesPresence.SetValue(1, 2, false);
            diesPresence.SetValue(1, 3, false);
            diesPresence.SetValue(1, 4, false);
            diesPresence.SetValue(1, 5, false);

            diesPresence.SetValue(2, 0, false);
            diesPresence.SetValue(2, 1, false);
            diesPresence.SetValue(2, 2, false);
            diesPresence.SetValue(2, 3, false);

            diesPresence.SetValue(3, 0, false);
            diesPresence.SetValue(3, 1, false);
            diesPresence.SetValue(3, 2, false);

            diesPresence.SetValue(4, 0, false);
            diesPresence.SetValue(4, 1, false);

            diesPresence.SetValue(5, 0, false);
            diesPresence.SetValue(5, 1, false);

            diesPresence.SetValue(6, 0, false);

            diesPresence.SetValue(7, 0, false);

            diesPresence.SetValue(8, 0, false);

            diesPresence.SetValue(0, 19, false);
            diesPresence.SetValue(0, 18, false);
            diesPresence.SetValue(0, 17, false);
            diesPresence.SetValue(0, 16, false);
            diesPresence.SetValue(0, 15, false);
            diesPresence.SetValue(0, 14, false);
            diesPresence.SetValue(0, 13, false);
            diesPresence.SetValue(0, 12, false);
            diesPresence.SetValue(0, 11, false);

            diesPresence.SetValue(1, 19, false);
            diesPresence.SetValue(1, 18, false);
            diesPresence.SetValue(1, 17, false);
            diesPresence.SetValue(1, 16, false);
            diesPresence.SetValue(1, 15, false);
            diesPresence.SetValue(1, 14, false);

            diesPresence.SetValue(2, 19, false);
            diesPresence.SetValue(2, 18, false);
            diesPresence.SetValue(2, 17, false);
            diesPresence.SetValue(2, 16, false);

            diesPresence.SetValue(3, 19, false);
            diesPresence.SetValue(3, 18, false);
            diesPresence.SetValue(3, 17, false);

            diesPresence.SetValue(4, 19, false);
            diesPresence.SetValue(4, 18, false);

            diesPresence.SetValue(5, 19, false);
            diesPresence.SetValue(5, 18, false);

            diesPresence.SetValue(6, 19, false);

            diesPresence.SetValue(7, 19, false);

            diesPresence.SetValue(8, 19, false);

            diesPresence.SetValue(19, 0, false);
            diesPresence.SetValue(19, 1, false);
            diesPresence.SetValue(19, 2, false);
            diesPresence.SetValue(19, 3, false);
            diesPresence.SetValue(19, 4, false);
            diesPresence.SetValue(19, 5, false);
            diesPresence.SetValue(19, 6, false);
            diesPresence.SetValue(19, 7, false);
            diesPresence.SetValue(19, 8, false);

            diesPresence.SetValue(18, 0, false);
            diesPresence.SetValue(18, 1, false);
            diesPresence.SetValue(18, 2, false);
            diesPresence.SetValue(18, 3, false);
            diesPresence.SetValue(18, 4, false);
            diesPresence.SetValue(18, 5, false);

            diesPresence.SetValue(17, 0, false);
            diesPresence.SetValue(17, 1, false);
            diesPresence.SetValue(17, 2, false);
            diesPresence.SetValue(17, 3, false);

            diesPresence.SetValue(16, 0, false);
            diesPresence.SetValue(16, 1, false);
            diesPresence.SetValue(16, 2, false);

            diesPresence.SetValue(15, 0, false);
            diesPresence.SetValue(15, 1, false);

            diesPresence.SetValue(14, 0, false);
            diesPresence.SetValue(14, 1, false);

            diesPresence.SetValue(13, 0, false);

            diesPresence.SetValue(12, 0, false);

            diesPresence.SetValue(11, 0, false);

            diesPresence.SetValue(19, 19, false);
            diesPresence.SetValue(19, 18, false);
            diesPresence.SetValue(19, 17, false);
            diesPresence.SetValue(19, 16, false);
            diesPresence.SetValue(19, 15, false);
            diesPresence.SetValue(19, 14, false);
            diesPresence.SetValue(19, 13, false);
            diesPresence.SetValue(19, 12, false);
            diesPresence.SetValue(19, 11, false);

            diesPresence.SetValue(18, 19, false);
            diesPresence.SetValue(18, 18, false);
            diesPresence.SetValue(18, 17, false);
            diesPresence.SetValue(18, 16, false);
            diesPresence.SetValue(18, 15, false);
            diesPresence.SetValue(18, 14, false);

            diesPresence.SetValue(17, 19, false);
            diesPresence.SetValue(17, 18, false);
            diesPresence.SetValue(17, 17, false);
            diesPresence.SetValue(17, 16, false);

            diesPresence.SetValue(16, 19, false);
            diesPresence.SetValue(16, 18, false);
            diesPresence.SetValue(16, 17, false);

            diesPresence.SetValue(15, 19, false);
            diesPresence.SetValue(15, 18, false);

            diesPresence.SetValue(14, 19, false);
            diesPresence.SetValue(14, 18, false);

            diesPresence.SetValue(13, 19, false);

            diesPresence.SetValue(12, 19, false);

            diesPresence.SetValue(11, 19, false);

            Result.DieReference = new DieIndex(0, 0);

            Result.DiesPresence = diesPresence;

            Thread.Sleep(1000);
        }
    }
}
