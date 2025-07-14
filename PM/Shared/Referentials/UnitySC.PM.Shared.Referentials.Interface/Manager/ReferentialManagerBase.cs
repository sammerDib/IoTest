using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    public abstract class ReferentialManagerBase<T> : IReferentialManager where T : PositionBase, IEquatable<T>
    {
        protected readonly ILogger<ReferentialManagerBase<T>> _logger;
        protected List<IReferentialConverter<T>> _referentialConverters;
        public abstract void DeleteSettings(ReferentialTag referentialTag);
        public abstract void SetSettings(ReferentialSettingsBase settings);
        public abstract ReferentialSettingsBase GetSettings(ReferentialTag referentialTag);                        

        public ReferentialManagerBase()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<ReferentialManagerBase<T>>>();
        }
        /// <summary>
        /// Convert a position into a position in a new referential
        /// </summary>
        /// <param name="positionToConvert">Position to convert</param>
        /// <param name="referentialTo">Destination referential</param>
        /// <returns></returns>
        public virtual PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            var currentPosition = positionToConvert as T;

            ReferentialTag tagFrom = positionToConvert.Referential.Tag;

            if (tagFrom == referentialTo)
            {
                return positionToConvert;
            }

            var referentialsPath = GetShortestPath(tagFrom, referentialTo);
            _logger.Verbose($"Start all referential conversion from {tagFrom} to {referentialTo}");

            try
            {
                foreach (ReferentialTag currentTag in referentialsPath)
                {
                    var from = currentPosition.Referential.Tag;
                    var to = currentTag;
                    var converter = GetConverter(from, to);
                    _logger.Verbose($"Apply converter between {from} and {to}");
                    currentPosition = converter.Convert(currentPosition);

                    if (currentPosition.Referential.Tag != currentTag)
                    {
                        throw new Exception($"Bad output referential in converter between {from} and {to}");
                    }
                }

                return currentPosition;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during referential conversion");
                throw new Exception("Error during referential conversion", ex);
            }
        }
        public virtual void DisableReferentialConverter(ReferentialTag from, ReferentialTag to)
        {
            GetConverter(from, to).IsEnabled = false;
        }
        public virtual void EnableReferentialConverter(ReferentialTag from, ReferentialTag to)
        {
            GetConverter(from, to).IsEnabled = true;
        }
        protected List<ReferentialTag> GetShortestPath(ReferentialTag sourceTag, ReferentialTag destinationTag)
        {
            var paths = new List<List<ReferentialTag>>();
            PathRecusiveSearch(sourceTag, destinationTag, new List<ReferentialTag>(), new List<ReferentialTag>() { }, paths);

            if (!paths.Any())
            {
                _logger.Error($"No converter path exist between {sourceTag} and {destinationTag}");
                throw new InvalidOperationException($"No converter path exist between {sourceTag} and {destinationTag}");
            }
            else
            {
                return paths.OrderBy(x => x.Count).First();
            }
        }
        private void PathRecusiveSearch(ReferentialTag sourceTag, ReferentialTag destinationTag, List<ReferentialTag> visitedTags, List<ReferentialTag> localPaths, List<List<ReferentialTag>> results)
        {
            // New results founds
            if (sourceTag == destinationTag)
            {
                results.Add(localPaths.ToList());
                return;
            }

            visitedTags.Add(sourceTag);

            // Recur for all adjacent referential
            foreach (ReferentialTag i in GetAjacentReferentials(sourceTag))
            {
                if (!visitedTags.Contains(i))
                {
                    localPaths.Add(i);
                    PathRecusiveSearch(i, destinationTag, visitedTags,
                                      localPaths, results);

                    localPaths.Remove(i);
                }
            }

            visitedTags.Remove(sourceTag);
        }
        private List<ReferentialTag> GetAjacentReferentials(ReferentialTag tag)
        {
            List<ReferentialTag> result = new List<ReferentialTag>();
            switch (tag)
            {
                case ReferentialTag.Motor:
                    result.Add(ReferentialTag.Stage);
                    break;

                case ReferentialTag.Stage:
                    result.Add(ReferentialTag.Motor);
                    result.Add(ReferentialTag.Wafer);
                    break;

                case ReferentialTag.Wafer:
                    result.Add(ReferentialTag.Stage);
                    result.Add(ReferentialTag.Die);
                    break;

                case ReferentialTag.Die:
                    result.Add(ReferentialTag.Wafer);
                    break;

                default:
                    break;
            }
            return result;
        }
        public IReferentialConverter<T> GetConverter(ReferentialTag from, ReferentialTag to)
        {
            foreach (var referential in _referentialConverters)
            {
                if (referential.Accept(from, to))
                {
                    return referential;
                }
            }
            return default;
        }
    }
}
