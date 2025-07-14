using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UnitySC.Shared.Tools.DataValidation
{
    public static class DataValidator
    {
        public static List<ValidationResult> TryValidateObject(object instance)
        {
            var validationResult = new List<ValidationResult>();
            Validator.TryValidateObject(instance, new ValidationContext(instance), validationResult, true);

            return validationResult;
        }
    }
}
