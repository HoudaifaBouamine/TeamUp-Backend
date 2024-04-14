using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CommandLine;

namespace Features.Projects;

// Tring to validate the team size 
public class TeamSizeValidationAttribute : ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        System.Console.WriteLine("\n\n -------------> wiiiiiiiiiiiiiiiiiiiiiiio\n\n");
        return new ValidationResult("Team size must be one of");
    }

    public override string FormatErrorMessage(string name)
    {
        return String.Format(CultureInfo.CurrentCulture, 
            ErrorMessageString, name);
    }


}