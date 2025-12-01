using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SGHSS.Api.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class CpfAttribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "CPF inválido.";

    public CpfAttribute()
        : base(DefaultErrorMessage)
    {
    }

    public override bool IsValid(object? value)
    {
        string? cpf = value as string;
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        string digits = Regex.Replace(cpf, @"[^\d]", string.Empty);

        if (digits.Length != 11)
        {
            return false;
        }

        string[] invalids = new string[]
        {
            "00000000000","11111111111","22222222222","33333333333","44444444444",
            "55555555555","66666666666","77777777777","88888888888","99999999999"
        };
        if (invalids.Contains(digits))
        {
            return false;
        }

        int sum1 = 0;
        for (int i = 0; i < 9; i++)
        {
            sum1 += (digits[i] - '0') * (10 - i);
        }
        int remainder1 = sum1 % 11;
        int checkDigit1 = remainder1 < 2 ? 0 : 11 - remainder1;
        if (checkDigit1 != (digits[9] - '0'))
        {
            return false;
        }

        int sum2 = 0;
        for (int i = 0; i < 10; i++)
        {
            sum2 += (digits[i] - '0') * (11 - i);
        }
        int remainder2 = sum2 % 11;
        int checkDigit2 = remainder2 < 2 ? 0 : 11 - remainder2;
        if (checkDigit2 != (digits[10] - '0'))
        {
            return false;
        }

        return true;
    }
}
