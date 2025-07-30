using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyApp.Data;
using MyApp.Filtres;
using MyApp.Models;
using Npgsql;
using NpgsqlTypes;

var builder = WebApplication.CreateBuilder(args);


var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(dbUrl))
    throw new Exception("DATABASE_URL is not set");

var uri = new Uri(dbUrl);
var userInfo = uri.UserInfo.Split(':');


var port = uri.Port > 0 ? uri.Port : 5432; 

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = port, 
    Username = userInfo[0],
    Password = userInfo[1],
    Database = uri.AbsolutePath.Trim('/'),
    SslMode = SslMode.Require,
    TrustServerCertificate = true
}.ToString();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options => 
    {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; 
    options.LogoutPath = "/Account/Logout";
});

builder.Services.AddScoped<CheckBlockedAttribute>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<CheckBlockedAttribute>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");



app.Run();
