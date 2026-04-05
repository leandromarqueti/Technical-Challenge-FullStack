using TechnicalChallenge.API.Extensions;
using TechnicalChallenge.API.Middlewares;
using TechnicalChallenge.Application;
using TechnicalChallenge.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//serviços do sistema
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

//autenticação jwt
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();

//configura o json pros enums e controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddSwaggerConfiguration();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

//configuração do cors para o frontend
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

//sobe o banco e as migrações na primeira vez
app.Services.InitializeDatabase();

//carrega os middlewares (cultura, swagger, cors)
app.UseMiddleware<CultureMiddleware>();
app.UseSwaggerConfiguration();
app.UseCors("ProductionPolicy");

//middlewares básicos do aspnet
app.UseHsts();
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
