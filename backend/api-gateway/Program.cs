using Microsoft.OpenApi.Models;
using VolunteerManagement.Model;
using VolunteerManagement.API.Endpoints;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Immutable;
using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using VolunteerManagement.Messaging;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();

builder.Services.AddOpenApi(
options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            {
                "BearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Authorization header using the Bearer scheme"
                }
            }
        };
        return Task.CompletedTask;
    });
}
);
builder.Services.AddValidation();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<RabbitMQProducer>();

builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    }
)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration.GetSection("Authentication:Schemes:Bearer:Authority").Value;
    options.Audience = builder.Configuration.GetSection("Authentication:Schemes:Bearer:Audience").Value;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "auth_cookie";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
}
)
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("Authentication:Google:ClientId").Value ?? "";
    options.ClientSecret = builder.Configuration.GetSection("Authentication:Google:ClientSecret").Value ?? "";
});


builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build())
    .AddPolicy("AdminOnly", policy => {
        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAssertion(context => {
            var adminEmails = builder.Configuration.GetSection("Authentication:AdminEmails").Get<List<string>>();
            if (adminEmails == null || adminEmails.Count == 0)
                return false;
            
            var emailClaim = context.User.Claims.FirstOrDefault(c => 
                c.Type.Contains("emailaddress"));
            
            return emailClaim != null && adminEmails.Contains(emailClaim.Value);
        }
    );
    })
    .AddPolicy("Public", policy => policy.RequireAssertion(_ => true));

builder.Services.AddSignalR();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference(options => options
    .AddPreferredSecuritySchemes("BearerAuth"))
    .AllowAnonymous();
}

app.MapVolunteersEndpoints();
app.MapLocationsEndpoints();
app.MapInterviewTemplatesEndpoints();
app.MapRecruitmentCampaignsEndpoints();
app.MapRecruitmentFormTemplatesEndpoints();
app.MapVolunteerDisponibilitiesEndpoints();
app.MapInterviewsEndpoints();


// get the values of the enums as lists
app.MapGet("/api/type/gender", (HttpContext context) =>
{
    return Enum.GetValues(typeof(Gender)).Cast<Gender>();
})
.AllowAnonymous();
app.MapGet("/api/type/study_type", () =>
{
    return Enum.GetValues(typeof(StudyType)).Cast<StudyType>();
})
.AllowAnonymous();
app.MapGet("/api/type/study_language", () =>
{
    return Enum.GetValues(typeof(StudyLanguage)).Cast<StudyLanguage>();
})
.AllowAnonymous();
app.MapGet("/api/type/shirt_size", () =>
{
    return Enum.GetValues(typeof(ShirtSize)).Cast<ShirtSize>();
})
.AllowAnonymous();
app.MapGet("/api/type/recruiting_status", () =>
{
    return Enum.GetValues(typeof(RecruitingStatus)).Cast<RecruitingStatus>();
})
.AllowAnonymous();
app.MapGet("/api/type/volunteer_status", () =>
{
    return Enum.GetValues(typeof(VolunteerStatus)).Cast<VolunteerStatus>();
})
.AllowAnonymous();
app.MapGet("/api/type/department", () =>
{
    return Enum.GetValues(typeof(Department)).Cast<Department>();
})
.AllowAnonymous();
app.MapGet("/api/type/diet", () =>
{
    return Enum.GetValues(typeof(Diet)).Cast<Diet>();
})
.AllowAnonymous();

app.MapGet("/login", (HttpContext context) =>
{
    return Results.Challenge(new AuthenticationProperties { RedirectUri = "/" }, [GoogleDefaults.AuthenticationScheme]);
}).AllowAnonymous();

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Map("/userinfo", (HttpContext context) =>
{
    var user = context.User;
    if (user?.Identity?.IsAuthenticated ?? false)
    {
        var claims = user.Claims.ToDictionary(c => c.Type.Split("/").Last(), c => c.Value);
        return Results.Json(claims);
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous();



app.Run();