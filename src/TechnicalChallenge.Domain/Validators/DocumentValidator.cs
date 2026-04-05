namespace TechnicalChallenge.Domain.Validators;

public static class DocumentValidator
{
    public static bool IsValid(string? document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return false;
        if (string.IsNullOrEmpty(document)) return false;

        //remove sujeira do documento (pontos, traços)
        document = document.Replace(".", "").Replace("-", "").Replace("/", "");

        if (document.Length == 11) return IsCpf(document);
        if (document.Length == 14) return IsCnpj(document);

        return false;
    }

    private static bool IsCpf(string document)
    {
        //ignora se for tudo número igual (ex: 111.111.111-11)
        if (new string(document[0], document.Length) == document) return false;

        //calcula o primeiro dígito verificador
        int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf = document.Substring(0, 9);
        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

        int remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        string digit = remainder.ToString();
        tempCpf = tempCpf + digit;
        sum = 0;

        //segundo dígito verificador
        int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit = digit + remainder.ToString();

        return document.EndsWith(digit);
    }

    private static bool IsCnpj(string document)
    {
        //ignora se for tudo número igual
        if (new string(document[0], document.Length) == document) return false;

        //primeiro dígito de conferência do cnpj
        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCnpj = document.Substring(0, 12);
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

        int remainder = (sum % 11);
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        string digit = remainder.ToString();
        tempCnpj = tempCnpj + digit;
        sum = 0;

        //segundo dígito de conferência do cnpj
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

        remainder = (sum % 11);
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit = digit + remainder.ToString();

        return document.EndsWith(digit);
    }

    public static string CleanDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            return string.Empty;

        return new string(document.Where(char.IsDigit).ToArray()).Trim();
    }
}
