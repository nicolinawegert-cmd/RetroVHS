using System.ComponentModel.DataAnnotations;

namespace RetroVHS.Shared.Validation;

/// <summary>
/// Validerar att ett år är inom intervallet [minYear, nuvarande år].
/// Används för utgivningsår på filmer — framtida år ska inte tillåtas.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MaxCurrentYearAttribute : ValidationAttribute
{
    private readonly int _minYear;

    public MaxCurrentYearAttribute(int minYear = 1888)
    {
        _minYear = minYear;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int year)
        {
            int currentYear = DateTime.UtcNow.Year;
            if (year >= _minYear && year <= currentYear)
                return ValidationResult.Success;

            return new ValidationResult(
                $"Utgivningsåret måste vara mellan {_minYear} och {currentYear}.",
                new[] { validationContext.MemberName! });
        }

        return ValidationResult.Success;
    }
}
