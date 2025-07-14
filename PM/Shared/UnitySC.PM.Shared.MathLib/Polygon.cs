using System.Collections.Generic;

namespace UnitySC.PM.Shared.MathLib
{
    /// <summary>
    /// Performs Polygons Calculation
    /// </summary>
    /// <remarks>
    /// </remarks>
	public class Polygon
    {
        public static bool IsInside(UnitySC.PM.Shared.MathLib.Geometry.Point point, List<UnitySC.PM.Shared.MathLib.Geometry.Point> Polygon)
        {
            int i = 0;
            int j = 0;
            bool c = false;

            for (j = Polygon.Count - 1; i < Polygon.Count; j = i++)
            {
                if ((((Polygon[i].Y <= point.Y) && (point.Y < Polygon[j].Y)) ||
                    ((Polygon[j].Y <= point.Y) && (point.Y < Polygon[i].Y))) &&
                    (point.X < ((Polygon[j].X - Polygon[i].X) * (point.Y - Polygon[i].Y) / (Polygon[j].Y - Polygon[i].Y) + Polygon[i].X)))
                {
                    c = !c;
                }
            }

            return c;
        }

        // isLeft(): teste si un point est à gauche|sur|à droite d'une ligne infinie.
        //    Input:  3 points a, b et c
        //    Return: >0 si c est à gauche de la ligne passant par a et b
        //            =0 si c est sur la ligne
        //            <0 si c est à droite
        private static bool isLeft(UnitySC.PM.Shared.MathLib.Geometry.Point a, UnitySC.PM.Shared.MathLib.Geometry.Point b, UnitySC.PM.Shared.MathLib.Geometry.Point P)
        {
            return (((b.X - a.X) * (P.Y - a.Y) - (P.X - a.X) * (b.Y - a.Y)) >= 0.0);
        }

        // inTriangle(): Teste si un point est inclu au triangle
        //    Input:  4 points a, b, c et P
        //    Return: true si P est inclu au triangle a,b,c
        //            false si P est strictement exclu au triangle a,b,c
        private static bool inTriangle(UnitySC.PM.Shared.MathLib.Geometry.Point a, UnitySC.PM.Shared.MathLib.Geometry.Point b, UnitySC.PM.Shared.MathLib.Geometry.Point c, UnitySC.PM.Shared.MathLib.Geometry.Point P)
        {
            bool abP = isLeft(a, b, P);
            bool bcP = isLeft(b, c, P);
            bool caP = isLeft(c, a, P);
            bool cab = isLeft(c, a, b);
            bool abc = isLeft(a, b, c);
            bool cbP = isLeft(c, b, P);
            bool cba = isLeft(c, b, a);

            //return (abP && bcP && caP) || (!abP && !bcP && !caP);

            //(IsLeft(C,A,H)*IsLeft(C,A,B)>0 and IsLeft(A,B,H)*IsLeft(A,B,C)>0 and IsLeft(C,B,H)*IsLeft(C,B,A)>0)
            return ((caP & cab) && (abP & abc) && (cbP & cba));
        }

        public static bool IsInsideTriangle(UnitySC.PM.Shared.MathLib.Geometry.Point point, List<UnitySC.PM.Shared.MathLib.Geometry.Point> Triangle)
        {
            if (Triangle.Count != 3)
            {
                // Bad preconditions
                return false;
            }

            return inTriangle(Triangle[0], Triangle[1], Triangle[2], point);
        }
    }
}