using RecruitmentService.DatabaseUtils;
using RecruitmentService.Messaging;
using RecruitmentService.Services;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VolunteerManagementDatabase"))
    .UseExceptionProcessor());

builder.Services.AddScoped<CandidateService>();
builder.Services.AddScoped<RecruitmentService.Services.RecruitmentService>();
builder.Services.AddScoped<InterviewTemplateService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<RecruitmentFormTemplateService>();
builder.Services.AddScoped<VolunteerDisponibilityService>();
builder.Services.AddScoped<InterviewService>();
builder.Services.AddScoped<VolunteerService>();
builder.Services.AddScoped<KafkaProducer>();
builder.Services.AddHostedService<RabbitMQListener>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

app.Run();
