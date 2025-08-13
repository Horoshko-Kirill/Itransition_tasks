using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseWork.Models;
using CourseWork.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CourseWorkDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; 
})
    .AddEntityFrameworkStores<CourseWorkDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

app.UseRouting();         

app.UseAuthentication();   
app.UseAuthorization();


app.MapGet("/", () => "Hello World!");

app.Run();
