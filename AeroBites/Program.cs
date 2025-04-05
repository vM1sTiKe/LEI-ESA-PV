using AeroBites.Data;
using AeroBites.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using AeroBites;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<AeroBitesContext>(builder.Configuration.GetConnectionString("AeroBitesContext"), options => options.EnableRetryOnFailure());

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AddressService>();

builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Index";
    options.AccessDeniedPath = "/";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

app.MapControllerRoute(name: "/Admin/", pattern: "{controller=Admin}/{action=Restaurants}");

app.CreateDbIfNotExists();

app.Run();