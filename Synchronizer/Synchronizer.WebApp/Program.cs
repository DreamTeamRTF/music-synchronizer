using Microsoft.AspNetCore.Authentication.Cookies;
using Synchronizer.Core;
using Synchronizer.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var applicationConfig = new SynchronizerConfig
{
    DbConnection = builder.Configuration.GetConnectionString("Postgres")!,
    MigrationsAssemly = typeof(Program).Assembly.ToString()
};
builder.Services.AddSynchronizerCore(applicationConfig);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/Login");
        options.LogoutPath = new PathString("/Account/Logout");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Services.MigrateTables();

app.Run();