using System;
using System.Collections.Generic;
using System.Drawing;


namespace AdcTools
{
    public class MergeGrid<T> : DeepGrid<T> where T : class
    {
        public int MergeCount { get; private set; }
        public int mergeNeighbourDistance = 0;    // distance mini entre deux voisins pour le merge.
                                                  // distance=0 => les voisins doivent se toucher.

        // Delegates
        //..........
        public delegate bool MergeDelegate(T t1, T t2); // Retourne vrai si les items ont ete mergés, faux s'il sont incompatibles
        public delegate Rectangle GetRectangleDelegate(T t);
        protected MergeDelegate mergeDelegate;
        protected GetRectangleDelegate getRectangleDelegate;


        //=================================================================
        // 
        //=================================================================
        public MergeGrid(MergeDelegate mergeDelegate, GetRectangleDelegate getRectangleDelegate)
        {
            this.mergeDelegate = mergeDelegate;
            this.getRectangleDelegate = getRectangleDelegate;
        }

        //=================================================================
        // 
        //=================================================================
        public void Merge(T t)
        {
            using (Region scanRegion = new Region()) // Région déjà scannée
            using (Region currentRegion = new Region()) // Région englobante pour l'itération en cours
            {
                bool bItemModified; // Estc-e que l'item courant a été mergé avec d'autres ?
                scanRegion.MakeEmpty();

                do
                {
                    // On itère plusieurs fois jusqu'à ce que les clusters ne soient plus modifiés
                    bItemModified = false;

                    // Rectangle de voisinage  
                    Rectangle neighbouringRectangle = getRectangleDelegate(t);
                    neighbouringRectangle.Inflate(mergeNeighbourDistance, mergeNeighbourDistance);
                    neighbouringRectangle.Intersect(FullExtent);

                    // Région à considérer lors de cette itération
                    currentRegion.MakeEmpty();
                    currentRegion.Union(neighbouringRectangle);
                    currentRegion.Exclude(scanRegion);

                    // Région deja scannée
                    scanRegion.MakeEmpty();
                    scanRegion.Union(neighbouringRectangle);

                    // Essai de chaque item de la grille
                    //.....................................
                    IEnumerable<T> candidates = Get(currentRegion);
                    foreach (T candidate in candidates)
                    {
                        if (candidate == t)
                            throw new ApplicationException("merging an item with itself");

                        Rectangle candidateRectangle = getRectangleDelegate(candidate);
                        if (neighbouringRectangle.IntersectsWith(candidateRectangle))
                        {
                            bool bMerged = mergeDelegate(t, candidate);

                            if (bMerged)
                            {
                                bItemModified = true;
                                MergeCount++;

                                Remove(candidateRectangle, candidate);

                                // Mise a jour de la region deja scannee
                                Rectangle r = candidateRectangle;
                                r.Inflate(mergeNeighbourDistance, mergeNeighbourDistance);
                                r.Intersect(FullExtent);
                                scanRegion.Union(r);
                            }
                        }
                    }

                } while (bItemModified);

                Insert(getRectangleDelegate(t), t);
            }
        }

    }
}
