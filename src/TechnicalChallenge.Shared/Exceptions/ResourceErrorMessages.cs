using System.Globalization;
using System.Resources;

namespace TechnicalChallenge.Shared.Exceptions;

public static class ResourceErrorMessages
{
    private static readonly ResourceManager ResourceManager =
        new ResourceManager("TechnicalChallenge.Shared.Exceptions.ResourceErrorMessages", typeof(ResourceErrorMessages).Assembly);

    public static string DOCUMENT_INVALID => GetString("DOCUMENT_INVALID");
    public static string NAME_EMPTY => GetString("NAME_EMPTY");
    public static string PERSON_NOT_FOUND => GetString("PERSON_NOT_FOUND");
    public static string CATEGORY_NOT_FOUND => GetString("CATEGORY_NOT_FOUND");
    public static string TRANSACTION_NOT_FOUND => GetString("TRANSACTION_NOT_FOUND");
    public static string DOCUMENT_ALREADY_EXISTS => GetString("DOCUMENT_ALREADY_EXISTS");
    public static string CANNOT_DELETE_CATEGORY_WITH_TRANSACTIONS => GetString("CANNOT_DELETE_CATEGORY_WITH_TRANSACTIONS");
    public static string CANNOT_DELETE_PERSON_WITH_TRANSACTIONS => GetString("CANNOT_DELETE_PERSON_WITH_TRANSACTIONS");
    public static string DB_ERROR => GetString("DB_ERROR");
    public static string SUCCESS => GetString("SUCCESS");

    private static string GetString(string name)
    {
        return ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
    }
}
