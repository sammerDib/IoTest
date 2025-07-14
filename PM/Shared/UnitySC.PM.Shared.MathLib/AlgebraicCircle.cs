using System;
using System.Collections.Generic;

namespace UnitySC.PM.Shared.MathLib
{
    /// <summary>
    /// Performs AlgebraicCircle calculation
    /// </summary>
    /// <remarks>
    /// returns Equation of the circle
    /// </remarks>
    public class AlgebraicCircle
    {
        //public double[] equation = new double[3];  //équation du cercle
        public double dRadius;

        public double dX;
        public double dY;

        public AlgebraicCircle()
        {
            dRadius = 0.0;
            dX = 0.0;
            dY = 0.0;
        }

        public bool AlgebraicCircle_Calculation(List<UnitySC.PM.Shared.MathLib.Geometry.Point> Vertex)
        //( const double* x, const double* y, int pitch, int count, double& xc, double& yc, double& r)
        {
            dRadius = 0.0;
            dX = 0.0;
            dY = 0.0;

            if (Vertex == null) return false; if (Vertex.Count == 0) return false;

            double mx = 0; double my = 0;
            for (int i = 0; i < Vertex.Count; i++)
            {
                mx += Vertex[i].X;
                my += Vertex[i].Y;
            }
            mx /= Vertex.Count; my /= Vertex.Count;

            double Suv = 0;
            double Suu = 0; double Svv = 0;
            double Suuv = 0; double Suvv = 0;
            double Suuu = 0; double Svvv = 0;
            for (int i = 0; i < Vertex.Count; i++)
            {
                double u = Vertex[i].X - mx;
                double v = Vertex[i].Y - my;
                double uu; double vv;
                Suv += u * v;
                Suu += (uu = u * u); Svv += (vv = v * v);
                Suuv += uu * v; Suvv += u * vv;
                Suuu += uu * u; Svvv += vv * v;
            }

            Solve2x2Matrix(out dX, out dY, Suu, Suv, Suv, Svv, 0.5 * (Suuu + Suvv), 0.5 * (Suuv + Svvv));
            dRadius = Math.Sqrt(dX * dX + dY * dY + (Suu + Svv) / Vertex.Count);
            dX += mx; dY += my;
            return true;
        }

        private static void Solve2x2Matrix(out double outx, out double outy, double m11, double m12, double m21, double m22, double y1, double y2)
        {
            double d = m11 * m22 - m12 * m21;
            // precondition facultative si on accepte de propager des INFs dans le resultat quand les points sont strictement alignés
            if (d == 0)
            {
                outy = outx = 0;
                return;
            }
            double id = 1.0 / d;
            outx = id * (m22 * y1 - m12 * y2);
            outy = id * (-m21 * y1 + m11 * y2);
            return;
        }
    }
}