global using GeneralPurposeApplication.Server;
using GeneralPurposeApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddCheck("ICMP_01", new ICMPHealthCheck("www.ryadel.com", 100))
    .AddCheck("ICMP_02", new ICMPHealthCheck("www.google.com", 100))
    .AddCheck("ICMP_03", new ICMPHealthCheck($"www.{Guid.NewGuid():N}.com", 100));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ApplicationDbContext and SQL Server support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
 );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHealthChecks(new PathString("/api/health"), new CustomHealthCheckOptions());
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
    {
        appBuilder.UseSpa(spa =>
        {
            spa.Options.SourcePath = "../GeneralPurposeApplication.Client";
            spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        });
    });
}

else
{
    app.MapFallbackToFile("index.html");
}

app.Run();
