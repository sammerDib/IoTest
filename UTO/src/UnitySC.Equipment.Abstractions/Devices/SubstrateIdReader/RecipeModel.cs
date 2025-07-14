using System;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader
{
    public class RecipeModel
    {
        #region Constructors

        public RecipeModel(int number, string name, bool isFrame, bool isStored, double angle)
        {
            if (!(1 <= number && number <= 30))
            {
                throw new ArgumentOutOfRangeException(nameof(number), @"Recipe number must be in range [1..30].");
            }

            Number   = number;
            Name     = name ?? throw new ArgumentNullException(nameof(name));
            IsFrame  = isFrame;
            IsStored = isStored;
            Angle = angle;
        }

        #endregion Constructors

        #region Properties

        public bool IsFrame { get; }

        public bool IsStored { get; }

        public string Name { get; }

        public int Number { get; }

        public double Angle { get; }

        #endregion Properties
    }
}
