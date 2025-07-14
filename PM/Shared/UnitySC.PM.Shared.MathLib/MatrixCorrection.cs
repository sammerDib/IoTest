using System;

using UnitySC.PM.Shared.MathLib.Geometry;

namespace UnitySC.PM.Shared.MathLib
{
    [Serializable]
    public class MatrixCorrection
    {
        private double _s;
        private double _theta;
        private double _tX;
        private double _tY;

        private Matrix _refMat;
        private Matrix _invRefMat;

        public double S
        {
            get { return _s; }
            set { _s = value; buildMatrix(); }
        }

        public double Theta
        {
            get { return _theta; }
            set { _theta = value; buildMatrix(); }
        }

        public double Tx
        {
            get { return _tX; }
            set { _tX = value; buildMatrix(); }
        }

        public double Ty
        {
            get { return _tY; }
            set { _tY = value; buildMatrix(); }
        }

        public Matrix Matrix { get { return _refMat; } }
        public Matrix InvMatrix { get { return _invRefMat; } }

        public MatrixCorrection()
        {
            _s = 1;
            _theta = 0;
            _tX = 0;
            _tY = 0;
            buildMatrix();
        }

        public MatrixCorrection(MatrixCorrection correctionToCopy)
        {
            _s = correctionToCopy.S;
            _theta = correctionToCopy.Theta;
            _tX = correctionToCopy.Tx;
            _tY = correctionToCopy.Ty;
            buildMatrix();
        }

        public MatrixCorrection(double fS, double fTheta, double fTx, double fTy)
        {
            _s = fS;
            _theta = fTheta;
            _tX = fTx;
            _tY = fTy;
            buildMatrix();
        }

        public MatrixCorrection(Matrix srcMatrix)
        {
            double a = srcMatrix.MatrixValues[0, 0];
            double b = srcMatrix.MatrixValues[0, 1];
            double c = srcMatrix.MatrixValues[1, 0];
            double d = srcMatrix.MatrixValues[1, 1];
            _tX = srcMatrix.MatrixValues[0, 2];
            _tY = srcMatrix.MatrixValues[1, 2];
            _s = Math.Sign(a) * Math.Sqrt((a * a) + (b * b));
            _theta = Math.Atan2(c, d);
            buildMatrix();
        }

        private void buildMatrix()
        {
            _refMat = new Matrix(3, 3);
            // Matrice de changement de repère
            _refMat.MatrixValues[0, 0] = S * Math.Cos(Theta);
            _refMat.MatrixValues[1, 0] = S * Math.Sin(Theta);
            _refMat.MatrixValues[2, 0] = 0;
            _refMat.MatrixValues[0, 1] = -S * Math.Sin(Theta);
            _refMat.MatrixValues[1, 1] = S * Math.Cos(Theta);
            _refMat.MatrixValues[2, 1] = 0;
            _refMat.MatrixValues[0, 2] = Tx;
            _refMat.MatrixValues[1, 2] = Ty;
            _refMat.MatrixValues[2, 2] = 1;

            _invRefMat = new Matrix(3, 3);
            // Matrice inverse
            _invRefMat.MatrixValues[0, 0] = Math.Cos(Theta) / S;
            _invRefMat.MatrixValues[1, 0] = -Math.Sin(Theta) / S;
            _invRefMat.MatrixValues[2, 0] = 0;
            _invRefMat.MatrixValues[0, 1] = Math.Sin(Theta) / S;
            _invRefMat.MatrixValues[1, 1] = Math.Cos(Theta) / S;
            _invRefMat.MatrixValues[2, 1] = 0;
            _invRefMat.MatrixValues[0, 2] = -(Math.Cos(Theta) * Tx + Math.Sin(Theta) * Ty) / S;
            _invRefMat.MatrixValues[1, 2] = (Math.Sin(Theta) * Tx - Math.Cos(Theta) * Ty) / S;
            _invRefMat.MatrixValues[2, 2] = 1;
        }

        public Point4D ApplyCorrection(Point4D posIni)
        {
            Point4D Pos = new Point4D(posIni);

            // Définition de la matrice d'origine
            Matrix matPointOriginaux = new Matrix(3, 1);
            matPointOriginaux.MatrixValues[0, 0] = posIni.X;
            matPointOriginaux.MatrixValues[1, 0] = posIni.Y;
            matPointOriginaux.MatrixValues[2, 0] = 1;

            Matrix matPosRecalcule = _refMat.DotProduct(matPointOriginaux);
            if (matPosRecalcule == null)
            {
                //FogEventLog.Log("Invalid multiplication result matrix", FogEventLog.Prio.Error);
                return posIni;
            }
            Pos.X = matPosRecalcule.MatrixValues[0, 0];
            Pos.Y = matPosRecalcule.MatrixValues[1, 0];

            return Pos;
        }

        public Point4D ApplyRevertCorrection(Point4D posIni)
        {
            Point4D Pos = new Point4D(posIni);

            // Définition de la matrice d'origine
            Matrix matPointOriginaux = new Matrix(3, 1);
            matPointOriginaux.MatrixValues[0, 0] = posIni.X;
            matPointOriginaux.MatrixValues[1, 0] = posIni.Y;
            matPointOriginaux.MatrixValues[2, 0] = 1;

            Matrix matPosRecalcule = _invRefMat.DotProduct(matPointOriginaux);
            if (matPosRecalcule == null)
            {
                //FogEventLog.Log("Invalid multiplication result matrix", FogEventLog.Prio.Error);
                return posIni;
            }
            Pos.X = matPosRecalcule.MatrixValues[0, 0];
            Pos.Y = matPosRecalcule.MatrixValues[1, 0];
            return Pos;
        }

        public static Point4D ApplyCorrectionFromValues(Point4D posIni, double fS, double fTheta, double fTx, double fTy)
        {
            MatrixCorrection Corr = new MatrixCorrection(fS, fTheta, fTx, fTy);
            return Corr.ApplyCorrection(posIni);
        }

        public static Point4D ApplyRevertCorrectionFromValues(Point4D posIni, double fS, double fTheta, double fTx, double fTy)
        {
            MatrixCorrection Corr = new MatrixCorrection(fS, fTheta, fTx, fTy);
            return Corr.ApplyRevertCorrection(posIni);
        }
    }
}