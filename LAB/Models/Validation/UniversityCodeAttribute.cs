using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LAB.API.Models.Validation
{
    public class UniversityCodeAttribute : ValidationAttribute
    {
        private static readonly Regex CodePattern = new(@"^[A-Z]{3}\d{3}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string code || string.IsNullOrWhiteSpace(code))
            {
                return ValidationResult.Success;
            }

            if (CodePattern.IsMatch(code))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Code must follow the format AAA999.");
        }
    }
}
