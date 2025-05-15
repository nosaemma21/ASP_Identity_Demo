using Identity.Api.Data;
using Identity.Api.Entities;
using Identity.Api.Extensions;
using Identity.Api.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder
    .Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Adding jwt functionality
builder.Services.AddJwtConfig(builder.Configuration);

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin))
    .AddPolicy("AdminAndUser", policy => policy.RequireRole(AppRoles.User, AppRoles.Admin));

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

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

//todo: Add registration, signin, email confirmation and jwt generation
