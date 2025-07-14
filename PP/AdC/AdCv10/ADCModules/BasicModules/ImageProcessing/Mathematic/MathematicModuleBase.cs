using System;
using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.Mathematic
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// L'intérêt de module est de regroupper les images d'un même die/tesselle 
    /// en provenance de plusieurs parents.
    /// Lorsque toutes les images d'un die/tesselle ont été reçues, on appelle la 
    /// classes qui n'a plus qu'à les combiner.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class MathematicModuleBase : ImageModuleBase
    {
        //=================================================================
        // Méthode abstraite
        //=================================================================
        protected abstract void PerformMathematic(ImageBase[] images);

        //=================================================================
        // Données internes
        //=================================================================
        private struct Index
        {
            public int X, Y;
        }

        /// <summary>
        /// Structure contenant les images d'une même die/tesselle en provenant de layer differentes.
        /// </summary>
        protected class MultiImage
        {
            public int ImageCount;
            public ImageBase[] Images;
        }

        /// <summary>
        /// Stockage des images par index de die/tesselle
        /// </summary>
        private Dictionary<Index, MultiImage> MultiImages;
        private object mutex = new object();

        //=================================================================
        // Constructeur
        //=================================================================
        public MathematicModuleBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            MultiImages = new Dictionary<Index, MultiImage>();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Index index = GetImageIndex(obj);
            bool complete = StoreImage(index, parent, obj);
            if (complete)
            {
                MultiImage multiImage = MultiImages[index];
                try
                {
                    ProcessMultiImage(multiImage);
                }
                finally
                {
                    Purge(multiImage);
                    MultiImages.Remove(index);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected virtual void ProcessMultiImage(MultiImage multiImage)
        {
            CheckImageSize(multiImage);
            PerformMathematic(multiImage.Images);
            ProcessChildren(multiImage.Images[0]);
        }

        //=================================================================
        /// <summary> Vérifie que toutes les images ont la même taille </summary>
        //=================================================================
        protected void CheckImageSize(MultiImage multiImage)
        {
            int width = multiImage.Images[0].Width;
            int height = multiImage.Images[0].Height;

            for (int i = 1; i < multiImage.Images.Length; i++)
            {
                ImageBase img = multiImage.Images[i];
                if (img.Width != width || img.Height != height)
                    throw new ApplicationException("invalid image size: " + img.Width + "x" + img.Height + " expected: " + width + "x" + height);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (State != eModuleState.Aborting && MultiImages.Count != 0)
                Recipe.SetError(this, "some images have not been processed");

            Purge();
            base.OnStopping(oldState);
        }

        //=================================================================
        // 
        //=================================================================
        private Index GetImageIndex(ObjectBase obj)
        {
            Index index = new Index();

            if (obj is FullImage)
            {
                index.X = index.Y = 0;
            }
            else if (obj is DieImage)
            {
                DieImage image = (DieImage)obj;
                index.X = image.DieIndexX;
                index.Y = image.DieIndexY;
            }
            else if (obj is MosaicImage)
            {
                MosaicImage image = (MosaicImage)obj;
                index.X = image.Column;
                index.Y = image.Line;
            }
            else
            {
                throw new ApplicationException("unknown image type: " + obj?.GetType());
            }

            return index;
        }

        //=================================================================
        /// <summary>
        /// Stocke l'image en attendant que les autres images du même die/tesselle soient reçues.
        /// Retourne vrai si toutes les images d'un die/tesselle sont présentes.
        /// </summary>
        //=================================================================
        private bool StoreImage(Index index, ModuleBase parent, ObjectBase obj)
        {
            lock (mutex)
            {
                // Crée ou retrouve la MultiImage correspondante
                //..............................................
                MultiImage multiImage;
                bool found = MultiImages.TryGetValue(index, out multiImage);
                if (!found)
                {
                    multiImage = new MultiImage();
                    multiImage.Images = new ImageBase[Parents.Count];
                    MultiImages.Add(index, multiImage);
                }

                // Ajoute l'image à la MultiImage
                //...............................
                int pos = Parents.FindIndex(p => p == parent);
                multiImage.Images[pos] = (ImageBase)obj;
                obj.AddRef();
                multiImage.ImageCount++;

                // As-t-on toutes les images de la MultiImage ?
                //.............................................
                bool complete = (multiImage.ImageCount == Parents.Count);
                return complete;
            }
        }

        //=================================================================
        //
        //=================================================================
        private void Purge()
        {
            foreach (MultiImage m in MultiImages.Values)
                Purge(m);
            MultiImages = null;
        }

        private void Purge(MultiImage m)
        {
            foreach (ImageBase img in m.Images)
            {
                if (img != null)
                    img.DelRef();
            }
        }

    }
}
