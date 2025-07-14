using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using UnitySC.Shared.Tools.DataValidation;

namespace UnitySC.Shared.Tools.Test.DataValidation
{
    public class Simple
    {
        [Required, StringLength(6)]
        public string StringProperty { get; set; }

        [Range(1, 10)]
        public double DoubleProperty { get; set; }
    }

    public class Complex
    {
        [Required, ListValidator]
        public List<Simple> ListProperty { get; set; }
    }
}
