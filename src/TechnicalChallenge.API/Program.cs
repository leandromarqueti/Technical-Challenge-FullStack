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

//cors aberto pra dev - ajustar em prod
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

//garante que o banco sqlite seja criado na primeira vez
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TechnicalChallenge.Infrastructure.Persistence.AppDbContext>();
    context.Database.EnsureCreated();
}

//pipeline
app.UseMiddleware<CultureMiddleware>();
app.UseSwaggerConfiguration();
app.UseCors("AllowAll");
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
