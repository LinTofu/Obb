using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Obb.Data;
using Obb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ObbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ObbContext")));

builder.Services.AddScoped<IObbMethod, ObbDBMethod>();
builder.Services.AddScoped<IObbLoginService, ObbLoginService>();
builder.Services.AddScoped<IObbBookService, ObbBookService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/ObbBorrow/Index";
                options.Cookie.Name = ".AspNetCore.Cookies";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.SlidingExpiration = true;

            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ObbLogin}/{action=Index}/{id?}");

app.Run();
