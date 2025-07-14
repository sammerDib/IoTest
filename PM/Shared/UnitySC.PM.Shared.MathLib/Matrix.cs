using System;
using System.Collections.Generic;

namespace UnitySC.PM.Shared.MathLib
{
    /// <summary>
    /// Performs Matrix Calculations
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class Matrix// : IDisposable
    {
        public int NbLines;  //nb. lignes
        public int NbColums;  //nb. colonnes
        public double[,] MatrixValues;

        public Matrix(int nbLines, int nbColums)
        {
            NbLines = nbLines;
            NbColums = nbColums;
            MatrixValues = new double[NbLines, NbColums];
        }

        public Matrix(int nbLines, int nbColums, double[,] p)
        {
            NbLines = nbLines;
            NbColums = nbColums;
            MatrixValues = new double[NbLines, NbColums];
            p.CopyTo(MatrixValues, 0);
        }

        public Matrix()
        {
        }

        public Matrix Multiply(double c)
        {
            Matrix B = new Matrix(NbLines, NbColums);

            int i, j;
            for (i = 0; i < NbLines; i++)
            {
                for (j = 0; j < NbColums; j++)
                {
                    B.MatrixValues[i, j] = (double)((double)c * (double)MatrixValues[i, j]);
                }
            }

            return B;
        }

        public Matrix Transpose()
        {
            Matrix At = new Matrix(NbColums, NbLines);

            int i, j;
            for (i = 0; i < At.NbLines; i++)
            {
                for (j = 0; j < At.NbColums; j++)
                {
                    At.MatrixValues[i, j] = (double)MatrixValues[j, i];
                }
            }

            return At;
        }

        public void SetAll(double value)
        {
            int i, j;
            for (i = 0; i < NbLines; i++)
            {
                for (j = 0; j < NbColums; j++)
                {
                    MatrixValues[i, j] = (double)value;
                }
            }
        }

        public void SetIdentity()
        {
            int i, j;
            for (i = 0; i < NbLines; i++)
            {
                for (j = 0; j < NbColums; j++)
                {
                    if (i == j)
                    {
                        MatrixValues[i, j] = 1.0;
                    }
                    else
                    {
                        MatrixValues[i, j] = 0.0;
                    }
                }
            }
        }

        public void SetDiagonal(double value)
        {
            int i, j;
            for (i = 0; i < NbLines; i++)
            {
                for (j = 0; j < NbColums; j++)
                {
                    if (i == j)
                    {
                        MatrixValues[i, j] = (double)value;
                    }
                    else
                    {
                        MatrixValues[i, j] = 0.0;
                    }
                }
            }
        }

        public List<double> GetListDouble()
        {
            List<double> list = new List<double>((NbColums * NbLines));

            int i, j;
            for (j = 0; j < NbColums; j++)// Colonnes
            {
                for (i = 0; i < NbLines; i++) // Lignes
                {
                    list.Add(MatrixValues[i, j]);
                }
            }

            return list;
        }

        public double[] GetDoubleTab()
        {
            double[] list = new double[NbColums * NbLines];

            int i, j;
            for (j = 0; j < NbColums; j++)// Colonnes
            {
                for (i = 0; i < NbLines; i++) // Lignes
                {
                    list[i * NbColums + j] = (double)(MatrixValues[i, j]);
                }
            }

            return list;
        }

        public Matrix DotProduct(Matrix matrixB)     //renvoie le produit matriciel (this . B)
        {
            if (NbColums != matrixB.NbLines)
                return null;

            int i, j, k;

            Matrix C = new Matrix(NbLines, matrixB.NbColums);

            for (i = 0; i < C.NbLines; i++)
            {
                for (j = 0; j < C.NbColums; j++)
                {
                    for (k = 0; k < NbColums; k++)
                    {
                        C.MatrixValues[i, j] += (double)((double)MatrixValues[i, k] * (double)matrixB.MatrixValues[k, j]);
                    }
                }
            }

            return C;
        }

        public Matrix SubMatrix(int ib, int jb)  //renvoie la matrice A laquelle a été supprimé la ib ligne et la jb colonne
        {
            Matrix B = new Matrix(NbLines - 1, NbColums - 1);
            int i, j;
            int indi = 0, indj = 0;
            for (i = 0; i < NbLines; i++)
            {
                for (j = 0; j < NbColums; j++)
                {
                    if (i != ib && j != jb)
                    {
                        B.MatrixValues[indi, indj] = MatrixValues[i, j];
                        if (indj < B.NbLines - 1)
                        {
                            indj++;
                        }
                        else
                        {
                            indj = 0;
                            indi++;
                        }
                    }
                }
            }

            return B;
        }

        public double Determinant()
        {
            double det = 0;

            if (NbLines == 1)
            {
                return MatrixValues[0, 0];
            }
            else if (NbLines == 2)
            {
                return (double)(MatrixValues[0, 0] * (double)MatrixValues[1, 1] - (double)MatrixValues[0, 1] * (double)MatrixValues[1, 0]);
            }
            else
            {
                int j;
                for (j = 0; j < NbColums; j++)
                {
                    Matrix B = SubMatrix(0, j);

                    if (j % 2 == 0)
                    {
                        det += (double)((double)MatrixValues[0, j] * (double)B.Determinant());
                    }
                    else
                    {
                        det += (double)((-1.0) * (double)MatrixValues[0, j] * (double)B.Determinant());
                    }
                }
            }

            return det;
        }

        public Matrix CoMatrix()
        {
            Matrix B = new Matrix(NbLines, NbColums);

            int i, j;
            for (i = 0; i < B.NbLines; i++)
            {
                for (j = 0; j < B.NbColums; j++)
                {
                    Matrix S = SubMatrix(i, j);
                    if ((i + j) % 2 == 0)
                    {
                        B.MatrixValues[i, j] = ((double)S.Determinant());
                    }
                    else
                    {
                        B.MatrixValues[i, j] = (double)(-1.0 * S.Determinant());
                    }
                }
            }
            return B;
        }

        public Matrix Inverse()
        {
            double det = Determinant();

            if (det == 0.0)
                return null;

            Matrix coA = CoMatrix();
            Matrix coAt = coA.Transpose();

            Matrix invA = coAt.Multiply((double)(1.0 / det));

            return invA;
        }

        public Matrix Add(Matrix matrixB)
        {
            Matrix C = new Matrix(NbLines, NbColums);

            if ((NbLines != matrixB.NbLines) || (NbColums != matrixB.NbColums))
                return null;

            for (int j = 0; j < NbColums; j++)// Colonnes
            {
                for (int i = 0; i < NbLines; i++) // Lignes
                {
                    C.MatrixValues[i, j] = MatrixValues[i, j] + matrixB.MatrixValues[i, j];
                }
            }
            return C;
        }

        public Matrix Sub(Matrix matrixB)
        {
            Matrix C = new Matrix(NbLines, NbColums);

            if ((NbLines != matrixB.NbLines) || (NbColums != matrixB.NbColums))
                return null;

            for (int j = 0; j < NbColums; j++)// Colonnes
            {
                for (int i = 0; i < NbLines; i++) // Lignes
                {
                    C.MatrixValues[i, j] = MatrixValues[i, j] - matrixB.MatrixValues[i, j];
                }
            }
            return C;
        }

        public Matrix ElemtSquare()
        {
            Matrix C = new Matrix(NbLines, NbColums);

            for (int j = 0; j < NbColums; j++)// Colonnes
            {
                for (int i = 0; i < NbLines; i++) // Lignes
                {
                    C.MatrixValues[i, j] = MatrixValues[i, j] * MatrixValues[i, j];
                }
            }
            return C;
        }

        public Matrix ElemtExp()
        {
            Matrix C = new Matrix(NbLines, NbColums);

            for (int j = 0; j < NbColums; j++)// Colonnes
            {
                for (int i = 0; i < NbLines; i++) // Lignes
                {
                    C.MatrixValues[i, j] = Math.Exp(MatrixValues[i, j]);
                }
            }
            return C;
        }

        public double Norm()
        {
            double MaxLsum = double.MinValue;
            double MaxCsum = double.MinValue;

            for (int j = 0; j < NbColums; j++)// Colonnes
            {
                double Sum = 0;
                for (int i = 0; i < NbLines; i++) // Lignes
                {
                    Sum += MatrixValues[i, j];
                }
                MaxLsum = Math.Max(MaxLsum, Sum);
            }
            for (int i = 0; i < NbLines; i++) // Lignes
            {
                double Sum = 0;
                for (int j = 0; j < NbColums; j++)// Colonnes
                {
                    Sum += MatrixValues[i, j];
                }
                MaxCsum = Math.Max(MaxCsum, Sum);
            }

            return Math.Max(MaxCsum, MaxLsum);
        }

        // Dispose
        /*       public void Dispose()
              {
                  /*Dispose(true);
                  // This object will be cleaned up by the Dispose method.
                  // Therefore, you should call GC.SupressFinalize to
                  // take this object off the finalization queue
                  // and prevent finalization code for this object
                  // from executing a second time.
                  GC.SuppressFinalize(this);
              }

             // Dispose(bool disposing) executes in two distinct scenarios.
              // If disposing equals true, the method has been called directly
              // or indirectly by a user's code. Managed and unmanaged resources
              // can be disposed.
              // If disposing equals false, the method has been called by the
              // runtime from inside the finalizer and you should not reference
              // other objects. Only unmanaged resources can be disposed.
              private void Dispose(bool disposing)
              {
                  // Check to see if Dispose has already been called.
                  if (!disposed)
                  {
                      // If disposing equals true, dispose all managed
                      // and unmanaged resources.
                      if (disposing)
                      {
                          // Dispose managed resources.
                          component.Dispose();
                      }

                      // Call the appropriate methods to clean up
                      // unmanaged resources here.
                      // If disposing is false,
                      // only the following code is executed.
                      CloseHandle(handle);
                      handle = IntPtr.Zero;

                      // Note disposing has been done.
                      disposed = true;
                  }
              }

              // Use interop to call the method necessary
              // to clean up the unmanaged resource.
              [System.Runtime.InteropServices.DllImport("Kernel32")]
              private extern static Boolean CloseHandle(IntPtr handle);*/
    }
}