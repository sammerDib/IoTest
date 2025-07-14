using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UnitySC.PM.Shared.MathLib
{
    public class Q_Hull
    {
        [DllImport("qhull_d6.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int FOG_Triangulate(double[] dpoints, int numPoints, int[] Triangles, int[] NumTriangles/*, int[] Envelope, int[] NumEnvelope*/);

        [DllImport("qhull_d6.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int FOG_GetDelaunayTriangle(double[] dpoint);

        [DllImport("qhull_d6.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FOG_FreeQHull();

        [DllImport("qhull_d6.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int FOG_InitFindBestTriangle(double[] dpoints, int numPoints);

        // Constructeur
        private Q_Hull()
        {
        }

        public static List<Geometry.Triangle> Triangulate(List<UnitySC.PM.Shared.MathLib.Geometry.Point> Vertex)
        {
            List<Geometry.Point<int>> Env = new List<Geometry.Point<int>>();
            return Triangulate(Vertex, out Env);
        }

        public static List<Geometry.Triangle> Triangulate(List<UnitySC.PM.Shared.MathLib.Geometry.Point> Vertex, out List<Geometry.Point<int>> ConvexHull)
        {
            // Preconditions
            int numPoints = Vertex.Count;
            if (numPoints < 3)
                throw new ArgumentException("Need at least three vertices for triangulation");

            // assignation
            ConvexHull = new List<UnitySC.PM.Shared.MathLib.Geometry.Point<int>>();

            List<double> dpoints = new List<double>();
            int[] Triangles = new int[2 * 3 * numPoints];// 3 points par triangle, maximum 3 * plus de triangles que de points
            int[] NumTriangles = new int[1];
            int[] Envelope = new int[2 * numPoints];
            int[] NumEnvelope = new int[1];

            // Copie des points
            try
            {
                //dpoints = (double*)memAlloc(numPoints, sizeof(double));

                for (int i = 0; i < numPoints; i++)
                {
                    dpoints.Add(Vertex[i].X);
                    dpoints.Add(Vertex[i].Y);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Exception in triangulate function! " + Ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (FOG_Triangulate(dpoints.ToArray(), numPoints, Triangles, NumTriangles/*, Envelope, NumEnvelope*/) == 0)
            {
                if (NumTriangles[0] >= 2 * numPoints)
                {
                    // Erreur: probleme certain d'allocation
                    return null;
                }

                // Copie des triangles
                List<Geometry.Triangle> TrianglesList = new List<Geometry.Triangle>();

                for (int i = 0; i < NumTriangles[0]; i++)
                {
                    Geometry.Triangle Tri = new Geometry.Triangle(Triangles[i * 3], Triangles[i * 3 + 1], Triangles[i * 3 + 2]);
                    TrianglesList.Add(Tri);
                }

                // Copie de l'enveloppe convexe
                if (ConvexHull != null && Envelope.Length > 0)
                {
                    Geometry.Point<int> pt;
                    for (int i = 0; i < NumEnvelope[0]; i++)
                    {
                        pt = new Geometry.Point<int>(Vertex[Envelope[i]].X, Vertex[Envelope[i]].Y, Envelope[i]);
                        ConvexHull.Add(pt);
                        //if (i > 0) ConvexHull.Add(pt);
                    }
                    //pt = new Geometry.Point<int>(Vertex[Envelope[0]].X, Vertex[Envelope[0]].Y, Envelope[0]);
                    //ConvexHull.Add(pt);
                }

                return TrianglesList;
            }
            else
            {
                return null;
            }
        }

        public static int InitFindBestTriangle(List<UnitySC.PM.Shared.MathLib.Geometry.Point> Vertex)
        {
            // Preconditions
            int numPoints = Vertex.Count;
            if (numPoints < 3)
                throw new ArgumentException("Need at least three vertices");

            List<double> dpoints = new List<double>();

            // Copie des points
            try
            {
                for (int i = 0; i < numPoints; i++)
                {
                    dpoints.Add(Vertex[i].X);
                    dpoints.Add(Vertex[i].Y);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Exception in Find Best Triangle function! " + Ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            return FOG_InitFindBestTriangle(dpoints.ToArray(), numPoints);
        }

        public static void CloseFindBestTriangle()
        {
            FOG_FreeQHull();
        }

        public static int FindBestTriangle(UnitySC.PM.Shared.MathLib.Geometry.Point point)
        {
            double[] pt = new double[2];
            pt[0] = point.X;
            pt[1] = point.Y;
            return FOG_GetDelaunayTriangle(pt);
        }

        public static void FreeQHullMemory()
        {
            // Libérer la mémoire
            FOG_FreeQHull();
        }

        //_______________ Fontions non QHull

        // Fonction qui permet de connaitre l'intersection d'une ligne avec une constante
        private static double LineVsConstInter(double Constante, double[] Point1, double[] Point2, bool Yconst)
        {
            double intersec = double.NaN;

            // Calcul de la droite Y = a.x + b
            double a = 0.0;
            if (Point2[0] != Point1[0])
                a = (Point2[1] - Point1[1]) / (Point2[0] - Point1[0]);
            else
            {
                if (Point1[1] <= Constante && Constante < Point2[1])
                {
                    intersec = Point1[0];
                }
                else if (Point2[1] <= Constante && Constante < Point1[1])
                {
                    intersec = Point1[0];
                }
                return intersec;
            }
            double b = Point1[1] - a * Point1[0];

            // Ligne constante sur Y
            if (Yconst)
            {
                // Si la constante se situe entre les deux points, on calcule
                if (Point1[1] <= Constante && Constante < Point2[1])
                {
                    intersec = (Constante - b) / a;
                }
                else if (Point2[1] <= Constante && Constante < Point1[1])
                {
                    intersec = (Constante - b) / a;
                }
            }
            else
            {
                // Ligne sur x
                if (Point1[0] <= Constante && Constante < Point2[0])
                {
                    intersec = Constante * a + b;
                }
                else if (Point2[0] <= Constante && Constante < Point1[0])
                {
                    intersec = Constante * a + b;
                }
            }

            return intersec;
        }

        /// <summary>
        ///     Cette fonction retourne pour chaque triangle contenant des point, une liste de points
        ///     Une des deux listes X ou Y doit contenir un seul element (ligne)
        /// </summary>
        /// <param name="PointsX"></param>
        /// <param name="PointsY"></param>
        /// <param name="TrianglesInput"></param>
        /// <param name="Vertices"></param>
        /// <param name="SearchOnX"> True si on recherche sur une ligne à Y fixe (PointsY contiendra un seul element)</param>
        /// <returns>Liste des triangles avec les points qu'ils contiennent</returns>
        public static List<List<int>> SearchPointsOnTriangles(out List<int> TriSub, List<float> PointsX, List<float> PointsY, List<Geometry.Triangle> TrianglesInput, List<UnitySC.PM.Shared.MathLib.Geometry.Point> Vertices, bool SearchOnX)
        {
            TriSub = new List<int>();

            if (TrianglesInput == null)
                return null;

            int NumPoints = 0;
            if (SearchOnX)
            {
                NumPoints = PointsX.Count;
            }
            else
            {
                NumPoints = PointsY.Count;
            }

            List<List<int>> ReturnTriOwn = new List<List<int>>();
            //for (int i = 0; i < NumPoints; i++) ReturnTriOwn.Add(-1);

            // On subdivise les triangles
            if (SearchOnX)
            {
                // Recherche sur X
                double Y = (double)PointsY[0];
                for (int i = 0; i < TrianglesInput.Count; i++)
                {
                    double MaxY = Math.Max(Math.Max(Vertices[TrianglesInput[i].p1].Y, Vertices[TrianglesInput[i].p2].Y), Vertices[TrianglesInput[i].p3].Y);
                    double MinY = Math.Min(Math.Min(Vertices[TrianglesInput[i].p1].Y, Vertices[TrianglesInput[i].p2].Y), Vertices[TrianglesInput[i].p3].Y);
                    if (MinY <= Y && Y < MaxY)
                    {
                        TriSub.Add(i);
                    }
                }
            }
            else
            {
                // Recherche sur Y
                double X = PointsX[0];
                for (int i = 0; i < TrianglesInput.Count; i++)
                {
                    double MaxX = Math.Max(Math.Max(Vertices[TrianglesInput[i].p1].X, Vertices[TrianglesInput[i].p2].X), Vertices[TrianglesInput[i].p3].X);
                    double MinX = Math.Min(Math.Min(Vertices[TrianglesInput[i].p1].X, Vertices[TrianglesInput[i].p2].X), Vertices[TrianglesInput[i].p3].X);
                    if (MinX <= X && X < MaxX)
                    {
                        TriSub.Add(i);
                    }
                }
            }
            int iRestPoints = NumPoints;
            // Pour chaque triangle, je calcule les limites en X ou Y et je trouve les points contenus dedans, puis on enlève les points de la liste
            double Constant = (double)PointsY[0];
            if (!SearchOnX)
            {
                Constant = (double)PointsX[0];
            }

            for (int i = 0; i < TriSub.Count; i++)
            {
                // J'ajoute un triangle
                ReturnTriOwn.Add(new List<int>());

                // Calcul des intersections
                double[] dIntersec = new double[3];
                dIntersec[0] = LineVsConstInter(Constant, Vertices[TrianglesInput[TriSub[i]].p1].ToArray(), Vertices[TrianglesInput[TriSub[i]].p2].ToArray(), SearchOnX);
                dIntersec[1] = LineVsConstInter(Constant, Vertices[TrianglesInput[TriSub[i]].p2].ToArray(), Vertices[TrianglesInput[TriSub[i]].p3].ToArray(), SearchOnX);
                dIntersec[2] = LineVsConstInter(Constant, Vertices[TrianglesInput[TriSub[i]].p3].ToArray(), Vertices[TrianglesInput[TriSub[i]].p1].ToArray(), SearchOnX);

                // Calcul des limites du triangle
                double Max = double.MinValue;
                double Min = double.MaxValue;
                int iNumValid = 0;
                for (int o = 0; o < 3; o++)
                {
                    if (!double.IsNaN(dIntersec[o]))
                    {
                        iNumValid++;
                        Max = Math.Max(Max, dIntersec[o]);
                        Min = Math.Min(Min, dIntersec[o]);
                    }
                }
                if (iNumValid == 2 && SearchOnX)
                {
                    // Calcul de tous les points présents dans le triangle
                    for (int pt = 0; pt < PointsX.Count; pt++)
                    {
                        if (Min < PointsX[pt] && PointsX[pt] < Max)
                        {
                            // On renseigne l'appartenance du point
                            ReturnTriOwn[ReturnTriOwn.Count - 1].Add(pt);
                            //ReturnTriOwn[pt] = TriSub[i];

                            iRestPoints--;
                        }
                    }
                }
                else if (iNumValid == 2 && !SearchOnX)
                {
                    for (int pt = 0; pt < PointsY.Count; pt++)
                    {
                        if (Min < PointsY[pt] && PointsY[pt] < Max)
                        {
                            ReturnTriOwn[ReturnTriOwn.Count - 1].Add(pt);
                            iRestPoints--;
                        }
                    }
                }
            }

            // s'il reste des points qui ne vont avec aucun triangle, on les ajoute dans un triangle nul
            /*if (iRestPoints > 0)
            {
                ReturnTriOwn.Add(new List<int>());
                for (int i = 0; i < iRestPoints; i++)
                {
                    ReturnTriOwn[ReturnTriOwn.Count - 1].Add(-1);
                }

                TriSub.Add(-1);
            }*/

            return ReturnTriOwn;
        }
    }
}