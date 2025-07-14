using System;
using System.Collections.Generic;
using System.Xml;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Axes
{
    public class CyclingTest : FunctionalTest
    {
        public override void Run()
        {
            Console.WriteLine("Test_Cycling - Starting");
            LoadTrajectoryFromFile("trajectory.xml", out var trajectory);
            var axisConfig = HardwareManager.Axes.AxesConfiguration.AxisConfigs.Find(config => config.AxisID == "X");
            int maxSpeedConfig = 9;
            if (axisConfig is ACSAxisConfig acsAxisConfig)
                maxSpeedConfig = acsAxisConfig.MaxSpeedService;

            var startTime = DateTime.Now;
            const int timeout = 60 * 1000; // ms
            while (DateTime.Now.Subtract(startTime).TotalSeconds < timeout)
            {
                foreach (var point in trajectory)
                {
                    var newPosition = new XYPosition(new StageReferential(), point.X, point.Y);
                    var rdmAxisSpeed = (AxisSpeed)GenerateRandomInt(0, maxSpeedConfig);
                    switch (point.Mode)
                    {
                        case Point.MovingMode.LinearMove:
                            HardwareManager.Axes.LinearMotion(newPosition, rdmAxisSpeed);
                            break;

                        case Point.MovingMode.SetPosMove:
                            HardwareManager.Axes.GotoPosition(newPosition, rdmAxisSpeed);
                            break;
                    }
                }
                break;
            }
        }

        private static void LoadTrajectoryFromFile(string fileNameToLoad, out List<Point> trajectoryToFill)
        {
            trajectoryToFill = new List<Point>();
            var doc = new XmlDocument();
            doc.Load(fileNameToLoad);
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("/Scenario/ScenarioItemList/ScenarioItem"))
            {
                int rdmMovingMode = GenerateRandomInt(0, 2);
                double xCoord = Convert.ToDouble(node.Attributes["XPos"]?.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                double yCoord = Convert.ToDouble(node.Attributes["YPos"]?.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                trajectoryToFill.Add(new Point(xCoord, yCoord, 200, (Point.MovingMode)rdmMovingMode));
            }
        }

        private static int GenerateRandomInt(int min, int max)
        {
            var randObj = new Random();
            return randObj.Next(min, max);
        }
    }

    internal class Point
    {
        public enum MovingMode
        {
            LinearMove = 0,
            SetPosMove = 1
        };

        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; }
        public MovingMode Mode { get; set; }

        public Point(double x, double y, double speed, MovingMode mode)
        {
            X = x;
            Y = y;
            Speed = speed;
            Mode = mode;
        }
    }
}
