using Hanssens.Net;
using innovate_work_admin_app.Extensions;
using innovate_work_admin_app.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
string baseRoot = builder?.Configuration.GetSection("BaseAddress").Value;

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<LocalStorage>();

builder.Services.AddHttpClient(baseRoot, "authentication");
builder.Services.AddHttpClient(baseRoot, "clients");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = builder.Configuration["Authentication:LoginPath"];
});

builder.Services.AddAuthorization();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtTokenCheckMiddleware>();

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Client}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
