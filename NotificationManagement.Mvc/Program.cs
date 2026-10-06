using Microsoft.EntityFrameworkCore;
using NotificationManagement.Business;
using NotificationManagement.Business.Settings;
using NotificationManagement.Data;
using NotificationManagement.Data.Context;
using NotificationManagement.Mvc.Middleware;


var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Application layers (each layer registers its own services)
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.Configure<SmsSettings>(
    builder.Configuration.GetSection("SmsSettings"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddBusinessLayer();

var app = builder.Build();

// Development convenience: apply pending EF migrations automatically at startup.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// Centralized exception handling (must be first so it wraps everything else)
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
