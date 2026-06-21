using ProjectZero.Web.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddModules();
builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
