namespace UnitySC.PM.Shared.MathLib
{
    public class Spiral
    {
        public enum SENS : uint
        {
            DROITE = 0,
            GAUCHE = 1,
            HAUT = 2,
            BAS = 3
        }

        private
            int x = 0; private int y = 0; private int lh = 0; private int lb = 0; private int cd = 0; private int cg = 0;

        private SENS sens = SENS.DROITE;
        private int CurrentX = 0; private int CurrentY = 0;
        private double dSpiralStep = 1.0f;

        public Spiral(/*double SpiralStep*/)
        {
            //dSpiralStep = SpiralStep;
            x = 0;
            y = 0;
            lh = 0; //ligne haut
            lb = 0; //ligne bas
            cd = 0; //colonne droite
            cg = 0; //colonne gauche
            sens = SENS.DROITE;
            CurrentX = 0;
            CurrentY = 0;
            dSpiralStep = 1.0f;
        }

        ~Spiral()
        {
        }

        public UnitySC.PM.Shared.MathLib.Geometry.Point GetElement(int iIndex)
        {
            x = 0;
            y = 0;
            lh = 0; //ligne haut
            lb = 0; //ligne bas
            cd = 0; //colonne droite
            cg = 0; //colonne gauche
            CurrentX = 0;
            CurrentY = 0;

            for (int i = 0; i <= iIndex; i++)
                GetNextElement();

            return GetNextElement();
        }

        public UnitySC.PM.Shared.MathLib.Geometry.Point GetNextElement()
        {
            UnitySC.PM.Shared.MathLib.Geometry.Point NextPoint = new UnitySC.PM.Shared.MathLib.Geometry.Point(0, 0);
            // définition de la nouvelle position

            if (sens == SENS.DROITE && x < cd + 1)
            {
                CurrentX = x;
                CurrentY = y;
                x += 1;
            }
            else if (sens == SENS.BAS && y > lb)
            {
                CurrentX = x;
                CurrentY = y;
                y -= 1;
            }
            else if (sens == SENS.GAUCHE && x > cg)
            {
                CurrentX = x;
                CurrentY = y;
                x -= 1;
            }
            else if (sens == SENS.HAUT && y < lh)
            {
                CurrentX = x;
                CurrentY = y;
                y += 1;
            }
            else
            {
                if (sens == SENS.DROITE)
                {
                    sens = SENS.HAUT;
                    lh += 1;
                    CurrentX = x;
                    CurrentY = y;
                    y += 1;
                }
                else if (sens == SENS.BAS)
                {
                    sens = SENS.DROITE;
                    cd += 1;
                    CurrentX = x;
                    CurrentY = y;
                    x += 1;
                }
                else if (sens == SENS.GAUCHE)
                {
                    sens = SENS.BAS;
                    lb -= 1;
                    CurrentX = x;
                    CurrentY = y;
                    y -= 1;
                }
                else if (sens == SENS.HAUT)
                {
                    sens = SENS.GAUCHE;
                    cg -= 1;
                    CurrentX = x;
                    CurrentY = y;
                    x -= 1;
                }
            }

            // On procéde à la mesure
            NextPoint.X = CurrentX * dSpiralStep;
            NextPoint.Y = CurrentY * dSpiralStep;

            return NextPoint;
        }
    }
}