using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

await app.UseOcelot();

app.UseAuthorization();

app.MapControllers();

app.Run();