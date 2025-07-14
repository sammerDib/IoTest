/**
   * \brief La classe cSpiral est une évolution de la classe Spiral.
   * ->amélioration de la fonction GetElement()
   * ->ajout d'une fonction calculant le nombre de point à calculer en fonction d'une surface et d'un step
   * \author Q-VIGUIER
   * \date 04/06/2012
   *
   */

using System;

namespace UnitySC.PM.Shared.MathLib
{
    public class cSpiral
    {
        public enum SENSE : uint
        {
            RIGHT = 0,
            LEFT = 1,
            UP = 2,
            DOWN = 3
        }

        private

            int i_x = 0; private int i_y = 0; private int i_lh = 0; private int i_lb = 0; private int i_cd = 0; private int i_cg = 0;

        private SENSE sens = SENSE.RIGHT;

        private int i_CurrentX = 0; private int i_CurrentY = 0;

        private double dSpiralStep = 1.0;

        public cSpiral()
        {
            i_x = 0;
            i_y = 0;
            i_lh = 0; //ligne haut
            i_lb = 0; //ligne bas
            i_cd = 0; //colonne droite
            i_cg = 0; //colonne gauche
            sens = SENSE.RIGHT;
            i_CurrentX = 0;
            i_CurrentY = 0;
            dSpiralStep = 1.0f;
        }

        ~cSpiral()
        {
        }

        /**
       * \fn public double evaluateStep(double d_area, int i_nbStep)
       *  \brief Fonction de calcul du pitch.
       *
       *  \param d_area aire à évaluer
       *  \param i_nbStep nombre de steps dans l'aire
       * \return taille des steps à effectuer
       */

        public double evaluateStepSize(double d_area, int i_nbStep)
        {
            float MaxX = 0;
            float minX = 0;
            float MaxY = 0;
            float minY = 0;
            if (d_area > 0 && i_nbStep > 1)
            {
                evaluateSpiraleLimit(i_nbStep, out minX, out MaxX, out minY, out MaxY);
                double dResult = Math.Sqrt(d_area / ((MaxY - minY) * (MaxX - minX)));

                // Si dResult = infinity, c'est qu on q divise par 0, la spirale n est pas une spirale mais une ligne
                if (double.IsInfinity(dResult))
                {
                    double dCriticSize = Math.Sqrt(d_area);
                    if (double.IsInfinity(dCriticSize))
                    {
                        dCriticSize = 0.0;
                    }
                    return dCriticSize;
                }

                // On reinitialise la spirale
                i_x = 0;
                i_y = 0;
                i_lh = 0; //ligne haut
                i_lb = 0; //ligne bas
                i_cd = 0; //colonne droite
                i_cg = 0; //colonne gauche
                sens = SENSE.RIGHT;
                i_CurrentX = 0;
                i_CurrentY = 0;
                dSpiralStep = 1.0f;

                return dResult;
            }

            return 0.0;
        }

        /**
        * \fn  public bool evaluateSpiraleLimit(int nbStep, out float minX, out float MaxX, out float minY, out float MaxY)
        *  \brief Fonction déterminant les positions relatives des points extérieures d'une spirale par rapport au nombre de point (ne tient pas compte du step)
        */

        public bool evaluateSpiraleLimit(int nbStep, out float minX, out float MaxX, out float minY, out float MaxY)
        {
            UnitySC.PM.Shared.MathLib.Geometry.Point Ptcursor = new UnitySC.PM.Shared.MathLib.Geometry.Point(0, 0);
            int i = 0;
            MaxX = 0;
            minX = 0;
            MaxY = 0;
            minY = 0;

            if (nbStep > 1)
            {
                for (i = 0; i < nbStep; i++)
                {
                    Ptcursor = GetElement(i);
                    if (Ptcursor.X > MaxX) MaxX = (float)Ptcursor.X;
                    if (Ptcursor.X < minX) minX = (float)Ptcursor.X;
                    if (Ptcursor.Y > MaxY) MaxY = (float)Ptcursor.Y;
                    if (Ptcursor.Y < minY) minY = (float)Ptcursor.Y;
                }
                return true;
            }
            return false;
        }

        /**
        * \fn public double evaluateArea(double d_area, int i_nbStep)
        *  \brief Fonction de de l'aire de la spirale.
        *  \param
        *  \param
        * \return l'aire de la spirale
        */

        public double evaluateArea(double d_step, int i_nbStep)
        {
            float MaxX = 0;
            float minX = 0;
            float MaxY = 0;
            float minY = 0;
            if (d_step > 0 && i_nbStep > 1)
            {
                evaluateSpiraleLimit(i_nbStep, out minX, out MaxX, out minY, out MaxY);
                //les positions maximales et minimales décrivent un rectangle dans lequel se trouve la spirale: l'aire en est déduite
                return ((MaxY - minY) * d_step) * ((MaxX - minX) * d_step);
                //return (double)(Math.Pow(d_step * (Math.Sqrt(i_nbStep) - 1), 2));
            }
            return 0.0;
        }

        /**
        * \fn public Point GetElement(int iIndex)
        *  \brief Fonction déterminant la position d'un point sur une spirale
        *  \param iIndex point concerné
        * \return un Point si il exite sinon renvoi null
        */

        public UnitySC.PM.Shared.MathLib.Geometry.Point GetElement(int iIndex)
        {
            if (iIndex < 0) return null;
            //if iIndex=0 return the original point
            if (iIndex == 0) return new UnitySC.PM.Shared.MathLib.Geometry.Point(0, 0);
            UnitySC.PM.Shared.MathLib.Geometry.Point finalPoint = new UnitySC.PM.Shared.MathLib.Geometry.Point(0, 0);
            int i_radiusCircle = 0;             //the radius of the circle containing the point
            int i_positionPoint = iIndex - 1;
            int i_positionCircle = iIndex - 1;  //position on the circle containing the point
            int i_orientation = 0;              //4 origin of cercle/3 South/2 East/1 North/0 West
            int i_nbPointSide = 0;              //number by side of the circle
            int i_positionSide = 0;             //position on the side (anticlockwise)
            i_x = 0;                            //offset x
            i_y = 0;                            //offset y

            //calculation of the radius
            do
            {
                i_radiusCircle++;
                i_positionPoint = i_positionPoint - i_radiusCircle * 8;
                if (i_positionPoint >= 0) i_positionCircle = i_positionPoint;
            } while (i_positionPoint >= 0);

            //calculation: number of point by side & position on the circle & orientation
            i_nbPointSide = 2 * i_radiusCircle + 1;
            i_positionCircle = (i_positionCircle + i_nbPointSide * i_nbPointSide) % (i_nbPointSide * i_nbPointSide - 1);
            i_positionSide = i_positionCircle % (i_nbPointSide - 1);
            if (i_nbPointSide == 1) return null;
            i_orientation = i_positionCircle / (i_nbPointSide - 1);

            //positionning
            switch (i_orientation)
            {
                case 0:
                case 4:
                    i_x = i_radiusCircle;
                    i_y = -i_radiusCircle + i_positionSide;
                    break;

                case 1:
                    i_y = i_radiusCircle;
                    i_x = i_radiusCircle - i_positionSide;
                    break;

                case 2:
                    i_x = -i_radiusCircle;
                    i_y = i_radiusCircle - i_positionSide;
                    break;

                case 3:
                    i_y = -i_radiusCircle;
                    i_x = -i_radiusCircle + i_positionSide;
                    break;
            }
            finalPoint.X = i_x;
            finalPoint.Y = i_y;
            return finalPoint;
        }

        public UnitySC.PM.Shared.MathLib.Geometry.Point GetNextElement()
        {
            UnitySC.PM.Shared.MathLib.Geometry.Point NextPoint = new UnitySC.PM.Shared.MathLib.Geometry.Point(0, 0);
            // définition de la nouvelle position

            if (sens == SENSE.RIGHT && i_x < i_cd + 1)
            {
                i_CurrentX = i_x;
                i_CurrentY = i_y;
                i_x += 1;
            }
            else if (sens == SENSE.DOWN && i_y > i_lb)
            {
                i_CurrentX = i_x;
                i_CurrentY = i_y;
                i_y -= 1;
            }
            else if (sens == SENSE.LEFT && i_x > i_cg)
            {
                i_CurrentX = i_x;
                i_CurrentY = i_y;
                i_x -= 1;
            }
            else if (sens == SENSE.UP && i_y < i_lh)
            {
                i_CurrentX = i_x;
                i_CurrentY = i_y;
                i_y += 1;
            }
            else
            {
                if (sens == SENSE.RIGHT)
                {
                    sens = SENSE.UP;
                    i_lh += 1;
                    i_CurrentX = i_x;
                    i_CurrentY = i_y;
                    i_y += 1;
                }
                else if (sens == SENSE.DOWN)
                {
                    sens = SENSE.RIGHT;
                    i_cd += 1;
                    i_CurrentX = i_x;
                    i_CurrentY = i_y;
                    i_x += 1;
                }
                else if (sens == SENSE.LEFT)
                {
                    sens = SENSE.DOWN;
                    i_lb -= 1;
                    i_CurrentX = i_x;
                    i_CurrentY = i_y;
                    i_y -= 1;
                }
                else if (sens == SENSE.UP)
                {
                    sens = SENSE.LEFT;
                    i_cg -= 1;
                    i_CurrentX = i_x;
                    i_CurrentY = i_y;
                    i_x -= 1;
                }
            }

            // On procéde à la mesure
            NextPoint.X = i_CurrentX * dSpiralStep;
            NextPoint.Y = i_CurrentY * dSpiralStep;

            return NextPoint;
        }
    }
}