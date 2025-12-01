using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SGHSS.Api.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class PasswordAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage =
        "A senha deve ter pelo menos 8 caracteres, conter letra maiúscula, letra minúscula, número e caractere especial.";

    public int MinimumLength { get; }

    public PasswordAttribute(int minimumLength = 8)
        : base(DefaultErrorMessage)
    {
        MinimumLength = minimumLength;
    }

    public override bool IsValid(object? value)
    {
        string? senha = value as string;
        if (string.IsNullOrEmpty(senha))
        {
            return false;
        }

        if (senha.Length < MinimumLength)
        {
            return false;
        }

        Regex regex = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])");
        return regex.IsMatch(senha);
    }
}