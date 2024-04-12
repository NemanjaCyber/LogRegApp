using LogRegApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LogRegApp.Controllers;
using LogRegApp.Interfaces;
using LogRegApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Ovdje smo dodali builder.Configuration.AddJsonFile("appsettings.json");
//kako bi se konfiguracija učitala iz datoteke appsettings.json.
//Zatim smo dodali konfiguraciju kao singleton uslugu
//builder.Services.AddSingleton<IConfiguration>(config);.
//Sada _config treba biti dostupan u cijelom opsegu aplikacije.
builder.Configuration.AddJsonFile("appsettings.json");
var _config = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(_config);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LogRegApp"));
});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
