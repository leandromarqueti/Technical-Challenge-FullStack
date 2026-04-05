using System.Globalization;

namespace TechnicalChallenge.API.Middlewares;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var languageHeader = context.Request.Headers["Accept-Language"].ToString();
        var culture = "en-US"; //Default fallback

        if (!string.IsNullOrEmpty(languageHeader))
        {
            //O header Accept-Language costuma vir assim: "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7"
            var languages = languageHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (languages.Length > 0)
            {
                //Pega a primeira linguagem principal (ex: pt-BR)
                var preferredLanguage = languages[0].Split(';')[0].Trim();

                //Lida com fallback caso venha só "pt" sem localização
                if (preferredLanguage.Equals("pt", StringComparison.OrdinalIgnoreCase))
                    preferredLanguage = "pt-BR";
                else if (preferredLanguage.Equals("es", StringComparison.OrdinalIgnoreCase))
                    preferredLanguage = "es-ES";
                else if (preferredLanguage.Equals("en", StringComparison.OrdinalIgnoreCase))
                    preferredLanguage = "en-US";

                culture = preferredLanguage;
            }
        }

        try
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }
        catch (CultureNotFoundException)
        {
            //Se enviar um idioma não mapeado, volta para o padrão (inglês)
            var defaultCulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = defaultCulture;
            CultureInfo.CurrentUICulture = defaultCulture;
        }

        await _next(context);
    }
}
