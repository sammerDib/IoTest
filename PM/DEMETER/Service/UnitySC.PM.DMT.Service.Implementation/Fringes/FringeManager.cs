using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Implementation.Fringes
{
    public class FringeManager : IFringeManager
    {
        private const string FringesConfigurationFileName = "Fringes.xml";

        private readonly AlgorithmManager _algorithmManager;

        /// <summary>
        ///     Cache pour stocker les images de franges déjà chargées, un par écran.
        /// </summary>
        private readonly Dictionary<Side, ImageCache> _cache = new Dictionary<Side, ImageCache>();

        private readonly DMTHardwareManager _hardwareManager;

        /// <summary>
        ///     Le logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly MeasuresConfiguration _measuresConfiguration;

        public FringeManager(ILogger<FringeManager> logger, AlgorithmManager algorithmManager,
            MeasuresConfiguration measuresConfiguration, DMTHardwareManager hardwareManager)
        {
            _logger = logger;
            _algorithmManager = algorithmManager;
            _measuresConfiguration = measuresConfiguration;
            _hardwareManager = hardwareManager;
        }

        /// <summary>
        ///     Liste des franges disponibles
        /// </summary>
        public List<Fringe> Fringes { get; } = new List<Fringe>();

        /// <summary>
        ///     Couleurs disponibles pour les franges /  la réflectométrie
        /// </summary>
        public List<Color> Colors { get; set; }

        /// <summary>
        ///     La frange à utliser quand on fait de la Global Topo
        /// </summary>
        public Fringe FringeForGlobalTopo { get; set; }

        /// <summary>
        ///     Lecture de la configuration depuis le disque
        /// </summary>
        public void Init()
        {
            // Lecture du fichier
            //...................
            PathString path =
                Path.Combine(
                    ClassLocator.Default.GetInstance<IDMTServiceConfigurationManager>().ConfigurationFolderPath,
                    FringesConfigurationFileName);
            _logger.Information("Loading fringe list from " + path);

            Colors = _measuresConfiguration.GetConfiguration<BrightFieldMeasureConfiguration>().Colors;

            //ValidataWhiteToFringeRatio();

            // Création automatique des franges standard à partir du disque
            //.............................................................
            var deflectometryMeasureConfiguration = _measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>();
            Fringes.AddRange(deflectometryMeasureConfiguration.PeriodList.SelectMany(period =>
                deflectometryMeasureConfiguration.NumberOfImagesPerDirectionList.Select(number =>
                    new Fringe { FringeType = FringeType.Standard, Period = period, NbImagesPerDirection = number })));

            // Création du cache des images
            //.............................
            if (_hardwareManager.Screens.Count == 0)
                throw new ApplicationException("No screen found, hardware not initialized");
            foreach (var side in _hardwareManager.ScreensBySide.Keys)
                _cache.Add(side, new ImageCache());
        }

        /// <summary>
        ///     Libère toutes les resources, en particulier le cache d'image
        /// </summary>
        public void Shutdown()
        {
            foreach (var side in _cache.Keys)
                _cache[side].Clear();
            _cache.Clear();
        }

        /// <summary>
        ///     Retourne le nombre d'images de la "frange", normalement un nombre pair.
        /// </summary>
        public int GetNbImages(Fringe fringe)
        {
            switch (fringe.FringeType)
            {
                case FringeType.Standard:
                    return 2 * fringe.NbImagesPerDirection;

                case FringeType.Multi:
                    return 2 * fringe.NbImagesPerDirection * fringe.Periods.Count;

                default:
                    throw new ApplicationException("unknown FringeType:" + fringe.FringeType);
            }
        }

        public Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> GetFringeImageDict(Side side,
            Fringe fringe)
        {
            var fringeKey = fringe;
            if (fringe.FringeType != FringeType.Multi)
            {
                fringeKey = Fringes.Find(f => f.HasSameValue(fringe));
            }

            if (fringeKey == null)
            {
                throw new ApplicationException("unknown fringe" + fringe);
            }

            var fringeImagesDict = FindFringesDictInCache(side, fringeKey);
            if (fringeImagesDict != null)
            {
                return fringeImagesDict;
            }

            switch (fringe.FringeType)
            {
                case FringeType.Standard:
                    fringeImagesDict = GenerateFringeDict(side, fringe.Period, fringe.NbImagesPerDirection);
                    break;

                case FringeType.Multi:
                    fringeImagesDict = GenerateMultiFringeDict(side, fringe.Periods, fringe.NbImagesPerDirection);
                    break;

                default:
                    throw new ApplicationException("unknown FringeType:" + fringe.FringeType);
            }

            _cache[side].FringesDict[fringeKey] = fringeImagesDict;

            return fringeImagesDict;
        }

        /// <summary>
        ///     Retourne les images correspondant à une Fringe.
        ///     On maintient un cache des fringes déjà utilisées
        /// </summary>
        public List<USPImageMil> GetFringeImages(Side side, Fringe fringe)
        {
            return GetFringeImageDict(side, fringe)
                .SelectMany(directionPeriodDictPair => directionPeriodDictPair.Value)
                .SelectMany(periodImageListPair => periodImageListPair.Value).ToList();
        }

        private Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> FindFringesDictInCache(Side side,
            Fringe fringeKey)
        {
            return _cache[side].FringesDict.FirstOrDefault(f => f.Key.HasSameValue(fringeKey)).Value;
        }

        /// <summary>
        ///     Retourne l'image du point blanc pour la global topo (anciennement appelée image cross)
        ///     TODO: paramétrer les coordonnées du point blanc!
        /// </summary>
        public USPImageMil GetGlobalTopoPointImage(Side side)
        {
            var cache = _cache[side];
            var screen = _hardwareManager.ScreensBySide[side];

            if (_cache[side].GlobalTopoPointImage == null)
            {
                // Allocation
                //...........
                int sizeX = screen.Width;
                int sizeY = screen.Height;
                cache.GlobalTopoPointImage =
                    new USPImageMil(sizeX, sizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                cache.GlobalTopoPointImage.Clear();

                // Dessin
                //.......
                var cSharpImage = cache.GlobalTopoPointImage.GetCSharpImage();
                int size = _measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>().GlobalTopoPointSize;
                int x0 = (screen.Width - size) / 2;
                int y0 = (screen.Height - size) / 2;

                for (int y = y0; y < y0 + size; y++)
                for (int x = x0; x < x0 + size; x++)
                    cSharpImage.uint8(x, y) = 255;
            }

            return cache.GlobalTopoPointImage;
        }

        private Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> GenerateMultiFringeDict(Side side,
            List<int> periods, int nbImagesPerDirection)
        {
            return periods.SelectMany(period => GenerateFringeDict(side, period, nbImagesPerDirection))
                .GroupBy(kvPair => kvPair.Key, kvPair => kvPair.Value)
                .ToDictionary(directionDictGroup => directionDictGroup.Key,
                    directionDictGroup =>
                        directionDictGroup.ToDictionary(dict => dict.Keys.First(), dict => dict.Values.First()));
        }

        public Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> GenerateFringeDict(Side side,
            int period, int nbImagesPerDirection)
        {
            int w = _hardwareManager.ScreensBySide[side].Width;
            int h = _hardwareManager.ScreensBySide[side].Height;

            return Enumerable.Range(0, nbImagesPerDirection)
                .SelectMany(imageNumber =>
                {
                    return new List<KeyValuePair<FringesDisplacement, USPImageMil>>
                    {
                        new KeyValuePair<FringesDisplacement, USPImageMil>(FringesDisplacement.X,
                            GenerateVerticalFringe(w, h, period, nbImagesPerDirection, imageNumber)),
                        new KeyValuePair<FringesDisplacement, USPImageMil>(FringesDisplacement.Y,
                            GenerateHorizontalFringe(w, h, period, nbImagesPerDirection, imageNumber))
                    };
                })
                .GroupBy(kvPair => kvPair.Key, kvPair => kvPair.Value).ToDictionary(keyListPair => keyListPair.Key,
                    keyListPair => new Dictionary<int, List<USPImageMil>>(1) { { period, keyListPair.ToList() } });
        }

        /// <summary>
        ///     Génère une image de frange verticale
        /// </summary>
        private USPImageMil GenerateVerticalFringe(int width, int height, int period,
            int nbImagesPerDirection, int index)
        {
            return GenerateFringeImage(width, height, period, nbImagesPerDirection, index, false);
        }

        /// <summary>
        ///     Génère une image de frange horizontale
        /// </summary>
        private USPImageMil GenerateHorizontalFringe(int width, int height, int period,
            int nbImagesPerDirection, int index)
        {
            return GenerateFringeImage(width, height, period, nbImagesPerDirection, index);
        }

        private USPImageMil GenerateFringeImage(int width, int height, int period, int nbImagesPerDirection, int index,
            bool horizontal = true)
        {
            double shift = (double)index / nbImagesPerDirection;
            double maxPixelValue = _measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>().FringesMaxValue;
            byte[,] data = new byte[height, width];

            int firstIterationBound = horizontal ? height : width;
            int secondIterationBound = horizontal ? width : height;

            int center = firstIterationBound / 2 - 1;

            for (int i = 0; i < firstIterationBound; i++)
            {
                double f = horizontal ? (center - i) / (double)period : (i - center) / (double)period;
                f = (f + shift) * 2 * Math.PI;
                f = maxPixelValue / 2 * (1 + Math.Cos(f));
                for (int j = 0; j < secondIterationBound; j++)
                {
                    if (horizontal)
                    {
                        data[i, j] = (byte)f;
                    }
                    else
                    {
                        data[j, i] = (byte)f;
                    }
                }
            }

            return new USPImageMil(data, data.GetLength(1), data.GetLength(0), 8 + MIL.M_UNSIGNED,
                MIL.M_IMAGE + MIL.M_PROC);
        }

        private Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> LoadCustomImageDict(
            List<string> imagePaths)
        {
            return new Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>>();
        }

        #region WhiteToFringeRatio

        /*public void ValidataWhiteToFringeRatio()
        {
            var table = ((DeflectometryMeasureConfiguration)_measuresConfiguration.GetMeasureConfiguration(MeasureType.DeflectometryMeasure)).WhiteToFringeRatios;
            if (table == null || table.Count == 0)
                return;

            int i;
            for (i = 1; i < table.Count; i++)
            {
                if (table[i].PeriodPix <= table[i - 1].PeriodPix)
                    throw new ApplicationException("Invalid WhiteToFringeRatio table at index: " + i);
            }
        }

        public double GetWhiteToFringeRatio(int period)
        {
            // Vérifications sanitaires
            //.........................
            var table = Config.WhiteToFringeRatios;
            if (table == null || table.Count == 0)
                return 1;

            if (period < table[0].PeriodPix || table.Last().PeriodPix < period)
                throw new ApplicationException("Fringe period is outside of WhiteToFringeRatio table: " + period);

            // Recherche dans la table
            //........................
            int i;
            for (i = 0; i < table.Count; i++)
            {
                if (table[i].PeriodPix >= period)
                    break;
            }

            // Calcul du ratio
            //................
            if (table[i].PeriodPix == period)
            {
                return table[i].ExpoCoefficient;
            }

            double c1 = table[i - 1].ExpoCoefficient;
            double c2 = table[i].ExpoCoefficient;
            double d = (double)(period - table[i - 1].PeriodPix) / (table[i].PeriodPix - table[i - 1].PeriodPix);
            double ratio = c1 + (c2 - c1) * d;
            return ratio;
        }*/

        #endregion WhiteToFringeRatio
    }
}
