using Microsoft.EntityFrameworkCore;
using Npgsql;
using Data;
using SetorDeCompras.Services;
using SetorDeCompras.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=ep-curly-brook-a5bwiouv.us-east-2.aws.neon.tech;Username=neondb_owner;Password=ea1LuzE9yviC;Database=neondb;SslMode=Require";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
