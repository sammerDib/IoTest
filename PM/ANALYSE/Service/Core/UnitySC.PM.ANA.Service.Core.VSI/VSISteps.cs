using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.VSI
{
    public class VSISteps
    {
        private readonly Dictionary<int, VSIStep> _steps;

        public VSISteps(Length initialPiezoPosition, int stepCount, Length step)
        {
            _steps = new Dictionary<int, VSIStep>();
            foreach (int stepNumber in Enumerable.Range(1, stepCount))
            {
                _steps.Add(stepNumber, new VSIStep()
                {
                    Number = stepNumber,
                    StartPosition = initialPiezoPosition + ((stepNumber - 1) * step),
                    EndPosition = initialPiezoPosition + (stepNumber * step),
                });
            }
        }

        public List<VSIStep> GetOrderedSteps()
        {
            return (from step in _steps orderby step.Key ascending select step.Value).ToList();
        }

        public VSIStep GetStep(int stepNumber)
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

        public List<ServiceImage> GetAllImages()
        {
            return _steps.Select(step => step.Value.Image).ToList();
        }

        public void SaveImages(string dirPath)
        {
            foreach (var step in GetOrderedSteps())
            {
                var image = step.Image;
                string imageFilename = Path.Combine(dirPath, $"vsiImage_step{step.Number}.png");
                ImageReport.SaveImage(image, imageFilename);
            }
        }
    }
}
