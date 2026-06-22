using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.API.Models.Validation
{
    public class AllowedUniversityEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string email || string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.Success;
            }

            if (email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Email must use the @fpt.edu.vn domain.");
        }
    }
}
