using expenseTrackerPOC.Data;
using expenseTrackerPOC.Models;
using expenseTrackerPOC.Models.Configurations;
using expenseTrackerPOC.Services.Auth;
using expenseTrackerPOC.Services.Auth.Interfaces;
using expenseTrackerPOC.Services.Core;
using expenseTrackerPOC.Services.Core.Interfaces;
using expenseTrackerPOC.Services.Profile;
using expenseTrackerPOC.Services.Profile.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var emailConnectionString = builder.Configuration["EmailSettings:ConnectionString"];
builder.Services.AddSingleton(emailConnectionString);

//This configures HOW to validate tokens that come IN with requests
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        // "When a request comes in with a JWT token, validate it using these rules"
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,//Verify Token Signature :Ensures token wasn't tampered with
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Authentication must be registered before the app starts, Makes the JWT authentication service available throughout the app
// The UseAuthentication() middleware automatically:
// Extracts token from request header
// Validates token using the configuration from program.cs
// Extracts claims and makes them available via HttpContext.User
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAngularApp");
app.Run();


//options.Events = new JwtBearerEvents
//{
//OnAuthenticationFailed = context =>
//{
//if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//{
//context.Response.Headers.Add("Token-Expired", "true");
//}
//return Task.CompletedTask;
//}
//};
//});