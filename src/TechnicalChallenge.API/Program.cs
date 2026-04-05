using TechnicalChallenge.API.Extensions;
using TechnicalChallenge.API.Middlewares;
using TechnicalChallenge.Application;
using TechnicalChallenge.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//servicos principais
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

//autenticacao jwt
builder.Services.AddJwtAuthentication(builder.Configuration);

//controllers + enum como string no json
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddSwaggerConfiguration();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

//cors (restrito como em produção, permitindo a porta do Client do desafio)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://seusistema.com.br")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

//garante que o banco sqlite seja criado na primeira vez através da Infra
app.Services.InitializeDatabase();

//pipeline
app.UseMiddleware<CultureMiddleware>();
app.UseSwaggerConfiguration();
app.UseCors("ProductionPolicy");

//HSTS e redirecionamento HTTPS ativo
//Pode ser necessário aceitar os certificados locais do .NET caso acuse erro SSL
app.UseHsts();
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
