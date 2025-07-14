using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public static class DieIndexConverter
    {
        public static DieIndex ToDieReference(this DieIndex die, DieIndex dieReference)
        {
            return new DieIndex(ConvertColumnToDieReference(die.Column, dieReference), ConvertRowToDieReference(die.Row, dieReference));
        }

        public static DieIndex FromDieReference(this DieIndex die, DieIndex dieReference)
        {
            return new DieIndex(ConvertColumnFromDieReference(die.Column, dieReference), ConvertRowFromDieReference(die.Row, dieReference));
        }

        public static int ConvertRowFromDieReference(int row, DieIndex dieReference)
        {
            return ConvertRowFromDieReference(row, dieReference.Row);
        }

        public static int ConvertRowToDieReference(int row, DieIndex dieReference)
        {
            return ConvertRowToDieReference(row, dieReference.Row);
        }

        public static int ConvertColumnFromDieReference(int column, DieIndex dieReference)
        {
            return ConvertColumnFromDieReference(column, dieReference.Column);
        }

        public static int ConvertColumnToDieReference(int column, DieIndex dieReference)
        {
            return ConvertColumnToDieReference(column, dieReference.Column);
        }

        public static int ConvertRowFromDieReference(int row, int rowReferenceDie)
        {
            return WaferMap.ConvertRowFromDieReference(row, rowReferenceDie);
        }

        public static int ConvertRowToDieReference(int row, int rowReferenceDie)
        {
            return WaferMap.ConvertRowFromDieReference(row, rowReferenceDie);
        }

        public static int ConvertColumnFromDieReference(int column, int columnReferenceDie)
        {
            return WaferMap.ConvertColumnFromDieReference(column, columnReferenceDie);
        }

        public static int ConvertColumnToDieReference(int column, int columnReferenceDie)
        {
            return WaferMap.ConvertColumnToDieReference(column, columnReferenceDie);
        }
    }
}
