using notification_service.Hubs;
using notification_service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add the Kafka consumer as a hosted service
builder.Services.AddHostedService<KafkaNotificationConsumer>();

var app = builder.Build();

// Serve static files for the test client
app.UseStaticFiles();

// Map controllers
app.MapControllers();

// Map the SignalR hub
app.MapHub<NotificationHub>("/notifications");

app.MapGet("/", () => "Notification Service is running!");

app.Run();
