using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AddisEats.Api.Models;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EthiopianMobileAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var phone = value.ToString()?.Trim();
        var valid = phone is not null &&
            Regex.IsMatch(phone, @"^(?:0[79]\d{8}|\+251[79]\d{8})$", RegexOptions.CultureInvariant);

        return valid
            ? ValidationResult.Success
            : new ValidationResult(
                ErrorMessage ?? "Phone must be an Ethiopian mobile number.",
                context.MemberName is null ? null : [context.MemberName]);
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class EtbAmountAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not decimal amount)
        {
            return new ValidationResult(
                ErrorMessage ?? "Amount must be a decimal ETB value.",
                context.MemberName is null ? null : [context.MemberName]);
        }

        var bits = decimal.GetBits(amount);
        var scale = (bits[3] >> 16) & 0xff;
        if (amount <= 0)
        {
            return new ValidationResult(
                ErrorMessage ?? "ETB amount must be greater than zero.",
                context.MemberName is null ? null : [context.MemberName]);
        }

        if (scale > 2)
        {
            return new ValidationResult(
                ErrorMessage ?? "ETB amount must have no more than two decimal places.",
                context.MemberName is null ? null : [context.MemberName]);
        }

        return ValidationResult.Success;
    }
}
