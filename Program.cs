using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data;
using SmartPark.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext (samo enkrat)
var connectionString = builder.Configuration.GetConnectionString("SmartParkContext")
    ?? throw new InvalidOperationException("Connection string 'SmartParkContext' not found.");

builder.Services.AddDbContext<SmartParkContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlite(connectionString);
    else
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
});

// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SmartParkContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
});

builder.Services.AddScoped<SmartPark.Services.IPaymentGateway, SmartPark.Services.DemoPaymentGateway>();
builder.Services.AddHostedService<ParkingSyncHostedService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed / Import (izvede se ob zagonu aplikacije)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<SmartParkContext>();

    if (db.Database.IsSqlite())
        await db.Database.EnsureCreatedAsync();
    else
        await db.Database.MigrateAsync();

    // admin + vloge (+ opcijsko demo parkirišče, če je v SeedAsync še prisotno)
    await DbInitializer.SeedAsync(services, app.Environment);

    // Demo podatki so na voljo takoj; uradni uvoz teče v ozadju.
    await DbInitializer.EnsureDemoParkingAsync(services);
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();