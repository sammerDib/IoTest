using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.PSI
{
    public class PSISteps
    {
        private readonly Dictionary<int, PSIStep> _steps;

        public PSISteps(Length initialPiezoPosition, int stepCount, Length step)
        {
            _steps = new Dictionary<int, PSIStep>();
            foreach (int stepNumber in Enumerable.Range(1, stepCount))
            {
                _steps.Add(stepNumber, new PSIStep()
                {
                    Number = stepNumber,
                    StartPosition = initialPiezoPosition + ((stepNumber - 1) * step),
                    EndPosition = initialPiezoPosition + (stepNumber * step),
                    Images = new List<USPImage>(),
                });
            }
        }

        public List<PSIStep> GetOrderedSteps()
        {
            return (from step in _steps orderby step.Key ascending select step.Value).ToList();
        }

        public PSIStep GetStep(int stepNumber)
        {
            bool success = _steps.TryGetValue(stepNumber, out var step);
            if (success)
            {
                return step;
            }
            else
            {
                throw new Exception($"Step with ID {stepNumber} not found.");
            }
        }

        public List<USPImage> GetAllImages()
        {
            return _steps.SelectMany(step => step.Value.Images).ToList();
        }

        public void SaveImages(string dirPath)
        {
            foreach (var step in GetOrderedSteps())
            {
                foreach (var image in step.Images)
                {
                    string imageFilename = Path.Combine(dirPath, $"psiImage_step{step.Number}_{step.Images.IndexOf(image) + 1}.png");
                    ImageReport.SaveImage(image, imageFilename);
                }
            }
        }
    }
}
