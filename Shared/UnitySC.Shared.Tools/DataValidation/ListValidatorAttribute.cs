using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UnitySC.Shared.Tools.DataValidation
{
    public class ListValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isList = value.GetType().GetGenericTypeDefinition() == typeof(List<>);
            if (isList)
            {
                var errorMessages = new List<string>();
                foreach (object item in (IEnumerable)value)
                {
                    var validationResults = new List<ValidationResult>();
                    bool itemIsValid = Validator.TryValidateObject(item, new ValidationContext(item), validationResults, true);
                    if (!itemIsValid)
                    {
                        errorMessages.AddRange(validationResults.Select(r => r.ErrorMessage));
                    }
                }

                bool isValid = errorMessages.Count() == 0;
                if (!isValid)
                {
                    ErrorMessage = $"The property of type List<{value.GetType().GetGenericArguments()[0].Name}> is not valid ! {errorMessages.Count()} error(s) found: {string.Join(" ; ", errorMessages)}";
                }

                return isValid;
            }
            else
            {
                return false;
            }
        }
    }
}
