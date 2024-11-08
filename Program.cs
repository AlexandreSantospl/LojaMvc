using Microsoft.AspNetCore.Authentication.JwtBearer; // Adicione esta linha
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using Data;
using SetorDeCompras.Services;
using SetorDeCompras.Models;
using SetorDeCompras.Repository.AuthRepo;
using SetorDeCompras.Repository.UserRepo;
using SetorDeCompras.Repository.ProdutosRepo;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do servi�o JWT
builder.Services.AddScoped<JwtService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new JwtService(
        configuration["Jwt:Key"],
        configuration["Jwt:Issuer"],
        configuration["Jwt:Audience"]
    );
});

// Configura��o do banco de dados
var connectionString = "Host=ep-curly-brook-a5bwiouv.us-east-2.aws.neon.tech;Username=neondb_owner;Password=ea1LuzE9yviC;Database=neondb;SslMode=Require";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configura��es de Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Configura��o do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "http://localhost:5000", 
        ValidAudience = "http://localhost:5000", 
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sua_chave_secreta_aqui")),
        ValidateIssuerSigningKey = true
    };
});
//Registrar os repositorios
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProdutosRepository>();

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

// Habilita autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
