using System.Runtime.Serialization;
using System.Windows;

namespace UnitySC.Shared.Image
{
    [DataContract]
    public class ServiceImageWithStatistics : ServiceImage
    {
        /// <summary>
        /// Le facteur de réduction qui a été appliqué à l'image
        /// </summary>
        [DataMember]
        public double Scale;

        /// <summary>
        /// Id de l'image, s'incrémente quand on a grabbé une nouvelle image
        /// </summary>
        [DataMember]
        public long ImageId;

        /// <summary>
        /// La zone acquise car on peut demander à ne transferer qu'une partie de l'image
        /// </summary>
        [DataMember]
        public Int32Rect AcquisitionRoi;

        /// <summary>
        /// Largeur hauteur de l'image complète. NB: c'est différent de la largeur/hauteur de la roi à cause du scale
        /// </summary>
        [DataMember]
        public int OriginalWidth, OriginalHeight;

        /// <summary>
        /// La ROI sur laquelle ont été calculées les stats
        /// </summary>
        [DataMember]
        public Int32Rect StatisticRoi;

        /// <summary>
        /// Statistiques sur l'image (optionnelles)
        /// </summary>
        [DataMember]
        public double Min, Max, Mean, StandardDeviation;

        /// <summary>
        /// Histogramme calculé sur la ROI
        /// </summary>
        [DataMember]
        public long[] Histogram;

        /// <summary>
        /// Projection de l'image sur l'axe X
        /// </summary>
        [DataMember]
        public long[] Profile;
    }
}
