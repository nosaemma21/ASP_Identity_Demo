using Identity.Api.Entities;
using Identity.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();

//Adding jwt functionality
builder.Services.AddJwtConfig(builder.Configuration);

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin))
    .AddPolicy("AdminAndUser", policy => policy.RequireRole(AppRoles.User, AppRoles.Admin));

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var app = builder.Build();

//Adding roles
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
{
    await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
}
if (!await roleManager.RoleExistsAsync(AppRoles.User))
{
    await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
