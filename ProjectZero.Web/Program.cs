using ProjectZero.Web.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddModules();
builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

// Exception handling sits at the top of the pipeline: UseExceptionHandler catches unhandled
// exceptions and delegates to the registered IExceptionHandler(s); UseStatusCodePages gives a
// body to error responses that come without an exception (routing 404, auth 401...).
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
