global using GeneralPurposeApplication.Server;
using GeneralPurposeApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCities.Server.Data.Models;
using Microsoft.AspNetCore.Identity;
using GeneralPurposeApplication.Server.Data.Models;


var builder = WebApplication.CreateBuilder(args);

// Adds Serilog support
builder.Host.UseSerilog((ctx, lc) => lc
   .ReadFrom.Configuration(ctx.Configuration)
   .WriteTo.MSSqlServer(connectionString:
               ctx.Configuration.GetConnectionString("DefaultConnection"),
           restrictedToMinimumLevel: LogEventLevel.Information,
           sinkOptions: new MSSqlServerSinkOptions
           {
               TableName = "LogEvents",
               AutoCreateSqlTable = true
           }
           )
   .WriteTo.Console()
   );

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddCheck("ICMP_01", new ICMPHealthCheck("www.ryadel.com", 100))
    .AddCheck("ICMP_02", new ICMPHealthCheck("www.google.com", 100))
    .AddCheck("ICMP_03", new ICMPHealthCheck($"www.{Guid.NewGuid():N}.com", 100));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add ApplicationDbContext and SQL Server support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add ASP.NET Core Identity support
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
   .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.UseSerilogRequestLogging();

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

// 🔥 Apply the CORS middleware
app.UseCors();

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
