global using GeneralPurposeApplication.Server;
using GeneralPurposeApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;
using GeneralPurposeApplication.Server.Data.GraphQL;
using Microsoft.AspNetCore.SignalR;

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
    options.AddPolicy(name: "AngularPolicy",
        cfg => {
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
            cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
        }));

builder.Services.AddSignalR();

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

builder.Services.AddScoped<JwtHandler>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         RequireExpirationTime = true,
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
         ValidAudience = builder.Configuration["JwtSettings:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]!))
     };
 });

builder.Services.AddCors(options =>
    options.AddPolicy(name: "AngularPolicy",
        cfg => {
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
            cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
        }));

builder.Services.AddGraphQLServer()
   .AddAuthorization()
   .AddQueryType<Query>()
   .AddMutationType<Mutation>()
   .AddFiltering()
   .AddSorting();

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


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AngularPolicy");

app.UseHealthChecks(new PathString("/api/health"), new CustomHealthCheckOptions());

app.MapControllers();

app.MapGraphQL("/api/graphql");

app.MapMethods("/api/heartbeat", new[] { "HEAD" },
   () => Results.Ok());

app.MapHub<HealthCheckHub>("/api/health-hub");

app.MapGet("/api/broadcast/update2", async (IHubContext<HealthCheckHub> hub) =>
{
    await hub.Clients.All.SendAsync("Update", "test");
    return Results.Text("Update message sent.");
});

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
    //app.MapFallbackToFile("index.html");
     else
    {
        app.UseExceptionHandler("/Error");
        app.MapGet("/Error", () => Results.Problem());
        app.UseHsts();
    }
}

app.Run();
