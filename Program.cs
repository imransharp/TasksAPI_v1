using TasksAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;



using TasksAPI.Data;
using Microsoft.EntityFrameworkCore;
using TasksAPI.Models;
using TasksAPI.DTOs;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;



var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TasksAPI", Version = "v1" });

    // Enable JWT auth
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token with **Bearer** prefix, e.g., `Bearer your_token_here`"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = new JwtSettings();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/secure", [Microsoft.AspNetCore.Authorization.Authorize]() =>
{
    return Results.Ok("You are authorized and logged in.");
});


app.MapPost("/register", async ([FromBody] RegisterRequest request, AppDbContext db) =>
{
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (existingUser != null)
    {
        return Results.BadRequest("Email already registered.");
    }

    using var hmac = new HMACSHA256();
    var salt = Convert.ToBase64String(hmac.Key);
    var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

    var user = new User
    {
        FullName = request.FullName,
        Email = request.Email,
        PasswordHash = passwordHash,
        PasswordSalt = salt
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok("User registered successfully.");
});


app.MapPost("/login", async ([FromBody] RegisterRequest request, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (user == null)
        return Results.Unauthorized();

    var hmac = new HMACSHA256(Convert.FromBase64String(user.PasswordSalt));
    var incomingHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

    if (incomingHash != user.PasswordHash)
        return Results.Unauthorized();

    var jwtSettings = new JwtSettings();
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email)
        }),
        Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
        Issuer = jwtSettings.Issuer,
        Audience = jwtSettings.Audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { token = tokenString });
});



app.MapGet("/", () => "Tasks API is running!");

app.Run();
