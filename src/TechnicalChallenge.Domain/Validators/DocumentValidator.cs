namespace TechnicalChallenge.Domain.Validators;

public static class DocumentValidator
{
    public static bool IsValid(string? document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return false;

        var cleanDoc = CleanDocument(document);

        return cleanDoc.Length switch
        {
            11 => IsValidCpf(cleanDoc),
            14 => IsValidCnpj(cleanDoc),
            _ => false
        };
    }

    public static bool IsValidCpf(string cpf)
    {
        var cleanCpf = CleanDocument(cpf);

        if (cleanCpf.Length != 11)
            return false;

        //Rejeita sequências repetidas (ex: 111.111.111-11)
        if (cleanCpf.Distinct().Count() == 1)
            return false;

        var digits = cleanCpf.Select(c => int.Parse(c.ToString())).ToArray();

        //Cálculo do primeiro dígito verificador
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (digits[9] != firstDigit)
            return false;

        //Cálculo do segundo dígito verificador
        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return digits[10] == secondDigit;
    }

    public static bool IsValidCnpj(string cnpj)
    {
        var cleanCnpj = CleanDocument(cnpj);

        if (cleanCnpj.Length != 14)
            return false;

        if (cleanCnpj.Distinct().Count() == 1)
            return false;

        var digits = cleanCnpj.Select(c => int.Parse(c.ToString())).ToArray();

        //Primeiro dígito verificador
        int[] weights1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var sum = 0;
        for (var i = 0; i < 12; i++)
            sum += digits[i] * weights1[i];

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (digits[12] != firstDigit)
            return false;

        //Segundo dígito verificador
        int[] weights2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;
        for (var i = 0; i < 13; i++)
            sum += digits[i] * weights2[i];

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return digits[13] == secondDigit;
    }

    public static string CleanDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return string.Empty;

        return new string(document.Where(char.IsDigit).ToArray()).Trim();
    }
}
