# Data Validation

Data validation is used to ensure data integrity at runtime.
Just declare your fields and/or properties contraints as attibutes and call the static method `DataValidator.TryValidateObject()` to validate your instance.
This static method returns a `List<ValidationResult>` object that may contains details on the errors.
If the list is empty, consider your instance valid, i.e. satisfying all data contraints.


Here is a quick example:
```csharp
public class Simple
{
    [Required, StringLength(6)]
    public string StringProperty { get; set; }

    [Range(1, 10)]
    public double DoubleProperty { get; set; }
}

var simpleInstance = new Simple()
{
    StringProperty = "aStringThatHasMuchMoreThanSixCharacters",
    DoubleProperty = 4.2,
}

var errors = DataValidator.TryValidateObject(simpleInstance);
Console.WriteLine(errors.Count);
Console.WriteLine(errors.First().ErrorMessage == "The field StringProperty must be a string with a maximum length of 6.");

/*
Executing this boilerplate code results in the following output:
1
true
*/
```

For an exhaustive list of existing data validation attibutes, please see the [official documentation](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netframework-4.7.1).
Note that [custom attributes](#Custom-attributes) can also be defined.

## Standard attributes

Here is some attibutes that could be interesting:
- [Required](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute?view=netframework-4.7.1)
- [StringLength](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute?view=netframework-4.7.1)
- [Range](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute?view=netframework-4.7.1)

## Custom attributes

It should extends [ValidationAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validationattribute?view=netframework-4.7.1) class.
The `ListValidatorAttribute` class is an example of how to define a custom data validation on lists.

## Improvements

- Error messages could be localized
