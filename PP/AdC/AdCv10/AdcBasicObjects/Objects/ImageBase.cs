using System;
using System.ComponentModel;
using System.Drawing;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // Interface
    ///////////////////////////////////////////////////////////////////////
    public interface IImage : IDisposable
    {
        ProcessingImage CurrentProcessingImage { get; }
        LayerBase Layer { get; }

        void AddRef();
        void DelRef();
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour les images ADC, toujours associé à une couche
    /// (layer) de données.
    /// 
    /// Contient en réalité deux images:
    /// - ProcessingImage: l'image qui a été modifiée par les modules de
    /// traitement d'images.
    /// - OriginalProcessingImage: l'image d'orgine avant traitement.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class ImageBase : ObjectBase, IImage
    {
        /// <summary>
        /// En pixels.
        /// Position de l'image (die ou mosaïque) dans l'image wafer globale. 
        /// Pour une image pleine plaque, c'est la taille de l'image pleine plaque. 
        /// </summary>
        public Rectangle imageRect;

        /// <summary> Index unique de l'image dans sa layer. </summary>
        public int ImageIndex;

        //=================================================================
        // Constructeur
        //=================================================================
        public ImageBase(LayerBase layer)
        {
            Layer = layer;
        }

        //=================================================================
        // Implémentation de IImage
        // On stocke 2 images (orignale et résultat), mais on founit trois
        // propriétés:
        // - OriginalProcessingImage: pour récupérer l'image fournie par l'acquisition
        // - ResultProcessingImage: pour avoir l'image après traitement.
        // - CurrentProcessingImage: si on veut modifier l'image par des traitements.
        // Ce n'est pas une image stockée en tant que telle, mais juste un accesseur.
        //=================================================================
        /// <summary> Couche de données à laquelle appartient l'image </summary>
        [Browsable(false)] public LayerBase Layer { get; protected set; }

        /// <summary> Indique si l'image de travail est l'image original ou l'image résultat </summary>
        public bool CurrentIsOriginal = false;

        /// <summary> 
        /// Image de travail.
        /// NB: 
        /// En général, CurrentProcessingImage est équivalent à l'image résultat.
        /// Mais ce n'est pas toujours le cas: au chargement ou après la
        /// clusterisation, les traitement doivent repartir de l'image
        /// originale. 
        /// Les modules DataLoader et Clusterization positionnent CurrentIsOriginal
        /// pour indiquer que CurrentProcessingImage doit cloner l'image
        /// originale dans l'image résultat.
        /// </summary>
        [Browsable(false)]
        public ProcessingImage CurrentProcessingImage
        {
            get
            {
                if (CurrentIsOriginal)
                {
                    // Clone de Original dans Result
                    using (MilImage milCloneImage = (MilImage)OriginalProcessingImage.GetMilImage().DeepClone()) ResultProcessingImage.SetMilImage(milCloneImage);
                    CurrentIsOriginal = false;
                }

                return _resultProcessingImage;
            }
        }

        /// <summary> Image qui a été modifiée par les modules de traitement d'images. </summary>
        [Browsable(false)]
        public ProcessingImage ResultProcessingImage
        {
            get { return _resultProcessingImage; }
            set { _resultProcessingImage = value; }
        }
        [Browsable(false)] private ProcessingImage _resultProcessingImage = new ProcessingImage();

        /// <summary> Image d'orgine avant traitement, telle que fournie par l'acquisition. </summary>
        [Browsable(false)]
        public ProcessingImage OriginalProcessingImage
        {
            get { return _originalProcessingImage; }
            set { _originalProcessingImage = value; }
        }

        private ProcessingImage _originalProcessingImage = new ProcessingImage();

        //=================================================================
        // Proprietées Browsable
        //=================================================================
        [Category("ImageSize"), Browsable(true)]
        public int Width { get { return _originalProcessingImage.Width; } }
        [Category("ImageSize"), Browsable(true)]
        public int Height { get { return _originalProcessingImage.Height; } }

        ///<summary> Alias d'imageRect </summary>
        [Category("ImageSize"), Browsable(true)]
        public Rectangle Rectangle { get { return imageRect; } }

        //=================================================================
        // 
        //=================================================================
        private PathString _filename;
        public string Filename
        {
            get { return _filename ?? null; }
            set
            {
                _filename = new PathString(value);
                Name = _filename.Basename;
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_resultProcessingImage != null)
            {
                _resultProcessingImage.Dispose();
                _resultProcessingImage = null;
            }

            if (_originalProcessingImage != null)
            {
                _originalProcessingImage.Dispose();
                _originalProcessingImage = null;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected override void CloneTo(DisposableObject obj)
        {
            ImageBase clone = (ImageBase)obj;
            clone._resultProcessingImage = (ProcessingImage)ResultProcessingImage.DeepClone();
            clone._originalProcessingImage = (ProcessingImage)OriginalProcessingImage.DeepClone();
        }

    }
}
