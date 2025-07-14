using System.Collections.Generic;

namespace UnitySC.PM.Shared.MathLib
{
    /// <summary>
    /// Performs Median Plan Calculation
    /// </summary>
    /// <remarks>
    /// returns Equation of the Plan z = ax+by+c, with (X,Y)List and Z List Points
    /// </remarks>
	public class Plan
    {
        public double[] Equation = new double[3];  //équation du Plan z = ax+by+c

        public static Plan MedianPlan(List<UnitySC.PM.Shared.MathLib.Geometry.Point> vertex, List<double> z)
        {
            Plan plan = new Plan();

            if (vertex.Count < 3)
                return null;

            CxCore.CvMat cv_A = CxCore.cxcore.CvCreateMat(vertex.Count, 3, CxCore.cxtypes.CV_64FC1);
            CxCore.CvMat cv_mZ = CxCore.cxcore.CvCreateMat(vertex.Count, 1, CxCore.cxtypes.CV_64FC1);

            bool bLineX = true;
            bool bLineY = true;

            double lastX = vertex[0].X;
            double lastY = vertex[0].Y;
            int i;
            for (i = 0; i < vertex.Count; i++)
            {
                CxCore.cxtypes.cvmSet(ref cv_A, i, 0, vertex[i].X);
                CxCore.cxtypes.cvmSet(ref cv_A, i, 1, vertex[i].Y);
                CxCore.cxtypes.cvmSet(ref cv_A, i, 2, 1.0);

                if (bLineX && (lastX != vertex[i].X))
                {
                    bLineX = false;
                }
                if (bLineY && (lastY != vertex[i].Y))
                {
                    bLineY = false;
                }

                CxCore.cxtypes.cvmSet(ref cv_mZ, i, 0, z[i]);
            }

            if (bLineX)
            {
                CxCore.cxtypes.cvmSet(ref cv_A, 0, 0, CxCore.cxtypes.cvmGet(cv_A, 0, 0) + 10.0);
                CxCore.cxtypes.cvmSet(ref cv_A, vertex.Count - 1, 0, CxCore.cxtypes.cvmGet(cv_A, vertex.Count - 1, 0) - 10.0);
            }
            else if (bLineY)
            {
                CxCore.cxtypes.cvmSet(ref cv_A, 0, 1, CxCore.cxtypes.cvmGet(cv_A, 0, 0) + 10.0);
                CxCore.cxtypes.cvmSet(ref cv_A, vertex.Count - 1, 1, CxCore.cxtypes.cvmGet(cv_A, vertex.Count - 1, 0) - 10.0);
            }

            //******************************* Solve ******************************
            // Solve the linear system.
            // NOTE: the matrix is not square because of the regularization
            // The inversion is done by least-square (such as pseudo-inverse
            // matrix)
            CxCore.CvMat cv_Sol = CxCore.cxcore.CvCreateMat(3, 1, CxCore.cxtypes.CV_64FC1);
            CxCore.cxcore.CvSolve(ref cv_A, ref cv_mZ, ref cv_Sol, CxCore.cxcore.CV_SVD_MODIFY_A);
            //*********************************************************************

            for (i = 0; i < 3; i++)
            {
                plan.Equation[i] = CxCore.cxtypes.cvmGet(cv_Sol, i, 0);
            }

            CxCore.cxcore.CvReleaseMat(ref cv_A);
            CxCore.cxcore.CvReleaseMat(ref cv_mZ);
            CxCore.cxcore.CvReleaseMat(ref cv_Sol);

            return plan;
        }
    }
}