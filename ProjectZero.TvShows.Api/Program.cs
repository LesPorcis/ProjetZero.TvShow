using ProjectZero.TvShows.Application;
using ProjectZero.TvShows.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Composition root : enregistrement des services applicatifs et d'infrastructure.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Alimente la génération build-time du openapi.json.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
