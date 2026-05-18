using System.ComponentModel.DataAnnotations;

namespace Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class DateOfBirthAttribute : ValidationAttribute
{
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 120;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // [Required] handles null values; skip here to avoid duplicate messages.
        if (value is null)
            return ValidationResult.Success;

        DateOnly date;
        switch (value)
        {
            case DateOnly d:
                date = d;
                break;
            case DateTime dt:
                date = DateOnly.FromDateTime(dt);
                break;
            default:
                return new ValidationResult("Date of birth has an invalid format. Expected yyyy-MM-dd.",
                    new[] { validationContext.MemberName ?? "DateOfBirth" });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (date > today)
            return new ValidationResult("Date of birth cannot be in the future.",
                new[] { validationContext.MemberName ?? "DateOfBirth" });

        var age = today.Year - date.Year;
        if (date > today.AddYears(-age))
            age--;

        if (age < MinAge)
            return new ValidationResult($"You must be at least {MinAge} years old to register.",
                new[] { validationContext.MemberName ?? "DateOfBirth" });

        if (age > MaxAge)
            return new ValidationResult($"Date of birth indicates an age above {MaxAge} years, which is not allowed.",
                new[] { validationContext.MemberName ?? "DateOfBirth" });

        return ValidationResult.Success;
    }
}
