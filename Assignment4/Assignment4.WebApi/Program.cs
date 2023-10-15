using Assignment4.WebApi.Hubs;
using Assignment4.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DecryptionService>();
builder.Services.AddScoped<JwtService>();

// Skapar en CorsPolicy som bara tillåter anrop från webbklienten (https://localhost:7247)
// Alla headers och credentials tillåts, men endast GET Requests är tillåtna
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("https://localhost:7247")
            .AllowAnyHeader()
            .AllowCredentials()
            .WithMethods("GET"));
});

// Lägger till JWT som auktoriserings-metod i projektet.
// Kräver säker anslutning (HTTPS) och validerar parametrarna Issuer, Audience och Lifetime på token.
// Validerar även signaturen av tokenets utfärdare (SecretKey)
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetSection("TokenValidation").GetValue<string>("Issuer")!,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection("TokenValidation").GetValue<string>("Audience")!,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("TokenValidation").GetValue<string>("SecretKey")!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WritePermission", policy =>
        policy.RequireRole("sensor"));

    options.AddPolicy("ReadPermission", policy =>
        policy.RequireRole("user"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Använda policyn som skapats
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthorization();

// Mappa hubben
app.MapHub<TemperatureHub>("/temperatureHub");
app.MapControllers();

app.Run();
