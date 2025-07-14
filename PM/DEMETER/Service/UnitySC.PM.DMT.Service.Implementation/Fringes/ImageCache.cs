using System.Collections.Generic;
using System.Windows.Media;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Implementation.Fringes
{
    /// <summary>
    /// Une classe pour stocker les images déjà générées
    /// </summary>
    internal class ImageCache
    {
        /// <summary>
        /// Cache pour stocker les images de franges déjà chargées
        /// </summary>
        public Dictionary<Fringe, List<USPImageMil>> Fringes = new Dictionary<Fringe, List<USPImageMil>>();

        public Dictionary<Fringe, Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>>> FringesDict =
            new Dictionary<Fringe, Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>>>();

        /// <summary>
        /// Cache pour stocker les images de franges déjà chargées
        /// </summary>
        public Dictionary<Color, USPImageMil> ColoredImages = new Dictionary<Color, USPImageMil>();
        
        /// <summary>
        /// L'image de Global Topo avec le point blanc au milieu
        /// </summary>
        public USPImageMil GlobalTopoPointImage;

        /// <summary>
        /// Libère toutes les resources
        /// </summary>
        public void Clear()
        {
            foreach (var imglist in Fringes.Values)
            {
                foreach (USPImageMil procimg in imglist)
                    procimg.Dispose();
            }
            Fringes.Clear();

            foreach (var directionPeriodImageListDict in FringesDict.Values)
            {
                foreach (var periodImageListDict in directionPeriodImageListDict.Values)
                {
                    foreach (var imageList in periodImageListDict.Values)
                    {
                        imageList.ForEach(image => image.Dispose());
                        imageList.Clear();
                    }
                    periodImageListDict.Clear();
                }
                directionPeriodImageListDict.Clear();
            }
            FringesDict.Clear();

            foreach (USPImageMil procimg in ColoredImages.Values)
                procimg.Dispose();
            ColoredImages.Clear();

            if (GlobalTopoPointImage != null)
            {
                GlobalTopoPointImage.Dispose();
                GlobalTopoPointImage = null;
            }
        }
    }
}
