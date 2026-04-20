using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobApplicationTracker.API.Middleware;
using JobApplicationTracker.Application.Configuration;
using JobApplicationTracker.Application.JobApplications;
using JobApplicationTracker.Application.Common.Models;
using JobApplicationTracker.Persistence.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt settings are not configured.");

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateJobApplicationRequest>();
builder.Services.AddOpenApi();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .Select(x => new
            {
                Field = x.Key,
                Messages = x.Value!.Errors.Select(error => error.ErrorMessage)
            });

        return new BadRequestObjectResult(ApiResponse<object>.Fail(errors));
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(ApiResponse<object>.Ok(new { status = "Healthy" }))).AllowAnonymous();
app.MapControllers();

app.Run();
